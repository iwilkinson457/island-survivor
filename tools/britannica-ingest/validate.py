"""
Britannica Note Validator
==========================
Grades an extracted note dict before it is written to disk.

Grade meanings:
  A = trusted   -- rich content, real prose summary, multiple substantive facts
  B = usable    -- mostly complete, minor gaps or thin content
  C = weak      -- partial extraction, sparse or TOC/heading-heavy content
  D = failed    -- no usable content, binary leakage, or extraction error

Returns: (grade: str, issues: list[str])
"""

import re
from typing import Tuple, List

_BINARY_MARKERS = [b"PK\x03\x04", b"%PDF", b"\xd0\xcf\x11\xe0"]

# Signals that a "summary" is actually a table of contents or heading dump
_TOC_SIGNALS = [
    r"\t\d+$",          # text<tab>page-number  (TOC pattern)
    r"\s{3,}\d+$",      # text + lots of spaces + number
]
_TOC_RE = [re.compile(p, re.MULTILINE) for p in _TOC_SIGNALS]

# Phrases that indicate the title was grabbed from a structural heading
_STRUCTURAL_HEADINGS = {
    "summary", "table of contents", "contents", "introduction",
    "overview", "abstract", "document control", "revision history",
    "scope", "purpose", "references", "appendix",
}


def validate_note(note: dict) -> Tuple[str, List[str]]:
    issues = []

    summary  = (note.get("summary") or "").strip()
    key_facts = note.get("key_facts") or []
    title    = (note.get("title") or "").strip().lower()
    error    = note.get("error")

    # ── Hard fail ────────────────────────────────────────────────────────────
    if error:
        issues.append(f"Extraction error: {error}")
        return "D", issues

    # ── Binary / XML leakage ─────────────────────────────────────────────────
    for marker in _BINARY_MARKERS:
        if marker in summary.encode("utf-8", errors="ignore"):
            issues.append("Raw binary content detected in summary")
            return "D", issues

    xml_signals = ["<?xml", "<w:body", "<Relationships", "xl/worksheets", "word/document"]
    for sig in xml_signals:
        if sig in summary:
            issues.append(f"XML/ZIP internals in summary ({sig!r})")
            return "D", issues

    # ── Summary quality checks ───────────────────────────────────────────────
    if not summary:
        issues.append("No summary extracted")
    elif len(summary) < 40:
        issues.append("Summary is very short (< 40 chars)")
    else:
        # TOC / heading dump detection
        toc_lines = sum(1 for pat in _TOC_RE if pat.search(summary))
        if toc_lines:
            issues.append("Summary appears to contain TOC/page-number entries rather than prose")

        # Tab characters in summary suggest raw TOC leakage
        if "\t" in summary:
            issues.append("Summary contains tab characters -- likely raw TOC content, not prose")

        # Very short average sentence length suggests heading list not prose
        sentences = [s.strip() for s in re.split(r"[.!?]", summary) if len(s.strip()) > 5]
        if sentences:
            avg_len = sum(len(s) for s in sentences) / len(sentences)
            if avg_len < 20:
                issues.append(f"Summary sentences are very short (avg {avg_len:.0f} chars) -- may be heading/TOC dump")

    # ── Key facts quality checks ──────────────────────────────────────────────
    if not key_facts:
        issues.append("No key facts extracted")
    else:
        # Count facts that look like real content vs TOC stubs
        real_facts = [
            f for f in key_facts
            if len(f) >= 20
            and not any(pat.search(f) for pat in _TOC_RE)
            and "\t" not in f
        ]
        if len(real_facts) < 2:
            issues.append(f"Fewer than 2 substantive key facts (have {len(real_facts)} real vs {len(key_facts)} total)")
        elif len(real_facts) < len(key_facts) * 0.5:
            issues.append(f"More than half of key facts look like TOC entries ({len(key_facts) - len(real_facts)} of {len(key_facts)})")

    # ── Title quality check ───────────────────────────────────────────────────
    if title in _STRUCTURAL_HEADINGS:
        issues.append(f"Title appears to be a structural heading ('{title}') rather than a real document title")

    # ── Grade ─────────────────────────────────────────────────────────────────
    has_prose_summary = (
        bool(summary)
        and len(summary) >= 40
        and "\t" not in summary
        and not any(pat.search(summary) for pat in _TOC_RE)
    )
    real_fact_count = len([
        f for f in key_facts
        if len(f) >= 20 and not any(pat.search(f) for pat in _TOC_RE) and "\t" not in f
    ])

    if has_prose_summary and real_fact_count >= 3 and len(issues) == 0:
        return "A", issues
    elif has_prose_summary and real_fact_count >= 2 and len(issues) <= 1:
        return "B", issues
    elif summary or key_facts:
        return "C", issues
    else:
        return "D", issues
