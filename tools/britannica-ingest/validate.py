"""
Britannica Note Validator
==========================
Grades an extracted note dict before it is written to disk.

Grade meanings:
  A = trusted — rich content, real summary, multiple facts
  B = usable  — mostly complete, minor gaps
  C = weak    — partial extraction, sparse content
  D = failed  — no usable content

Returns: (grade: str, issues: list[str])
"""

from typing import Tuple, List


_BINARY_MARKERS = [b"PK\x03\x04", b"%PDF", b"\xd0\xcf\x11\xe0"]


def validate_note(note: dict) -> Tuple[str, List[str]]:
    issues = []

    summary = (note.get("summary") or "").strip()
    key_facts = note.get("key_facts") or []
    error = note.get("error")

    # Hard fail
    if error:
        issues.append(f"Extraction error: {error}")
        return "D", issues

    if not summary:
        issues.append("No summary extracted")
    elif len(summary) < 40:
        issues.append("Summary is very short (< 40 chars)")

    if not key_facts:
        issues.append("No key facts extracted")
    elif len(key_facts) < 2:
        issues.append("Fewer than 2 key facts extracted")

    # Detect raw binary or XML internals leaking into summary
    for marker in _BINARY_MARKERS:
        if marker in summary.encode("utf-8", errors="ignore"):
            issues.append("Raw binary content detected in summary — extraction likely failed")
            return "D", issues

    # XML/ZIP artifact detection
    xml_leak_signals = ["<?xml", "<w:body", "<Relationships", "xl/worksheets", "word/document"]
    for sig in xml_leak_signals:
        if sig in summary:
            issues.append(f"XML/ZIP internals detected in summary ({sig!r}) — extraction failed")
            return "D", issues

    # Grade
    has_summary = bool(summary) and len(summary) >= 40
    has_facts = len(key_facts) >= 2

    if has_summary and has_facts and len(issues) == 0:
        return "A", issues
    elif has_summary and len(issues) <= 1:
        return "B", issues
    elif summary or key_facts:
        return "C", issues
    else:
        return "D", issues
