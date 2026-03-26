"""
PDF -> Markdown wrapper for Britannica ingestion.

Uses pdfminer.six (already installed).
Extracts text page-by-page. Grades quality based on yield.

Limitations:
- Scanned/image-only PDFs yield nothing (grade D).
- Complex multi-column layouts may produce jumbled text (grade C).
- Tables are not reconstructed — treated as flowing text.
"""

from pathlib import Path
import re


def extract(source: Path) -> dict:
    try:
        from pdfminer.high_level import extract_text, extract_pages
        from pdfminer.layout import LTTextContainer, LTPage
    except ImportError:
        return {"error": "pdfminer.six not installed (pip install pdfminer.six)"}

    try:
        raw_text = extract_text(str(source))
    except Exception as e:
        return {"error": f"pdfminer failed to extract: {e}"}

    if not raw_text or not raw_text.strip():
        return {
            "title": source.stem,
            "summary": "",
            "key_facts": [],
            "operational_relevance": "(not determined — no text extracted)",
            "gaps": ["No text extracted — document may be image-only (scanned PDF) or encrypted"],
            "method": "pdfminer.six — no text yield",
            "body_md": "",
        }

    # Clean up extracted text
    lines = [ln.strip() for ln in raw_text.splitlines()]
    lines = [ln for ln in lines if ln]

    # Try to infer title from first non-trivial line
    title = _infer_title(lines) or source.stem

    # Build body markdown — group into paragraphs naively
    body_parts = _build_body(lines)
    body_md = "\n\n".join(body_parts)

    # Summary: first 400 chars of substantive content
    substantive = [p for p in body_parts if len(p) > 40]
    summary = " ".join(substantive[:3])[:500].strip()

    # Key facts
    key_facts = _extract_key_facts(body_parts)

    # Operational relevance
    op_relevance = _detect_operational_relevance(raw_text)

    # Gaps
    gaps = []
    word_count = len(raw_text.split())
    if word_count < 100:
        gaps.append(f"Low word count ({word_count}) — document may be mostly images or have restricted text layer")
    if _detect_garbled(raw_text):
        gaps.append("Text may be garbled — multi-column or complex layout detected")

    # Page count estimate
    try:
        pages = list(extract_pages(str(source)))
        page_count = len(pages)
        if page_count > 1:
            key_facts.append(f"Document has approximately {page_count} pages")
    except Exception:
        pass

    return {
        "title": title,
        "summary": summary,
        "key_facts": key_facts,
        "operational_relevance": op_relevance,
        "gaps": gaps,
        "method": "pdfminer.six text extraction",
        "body_md": body_md,
    }


def _infer_title(lines: list) -> str:
    for line in lines[:10]:
        # Skip page numbers, headers, footers
        if re.match(r"^[\d\s\-–|/]+$", line):
            continue
        if len(line) > 5 and len(line) < 120:
            return line
    return ""


def _build_body(lines: list) -> list:
    """Groups lines into paragraph-like blocks."""
    paragraphs = []
    current = []
    for line in lines:
        if not line:
            if current:
                paragraphs.append(" ".join(current))
                current = []
        else:
            current.append(line)
    if current:
        paragraphs.append(" ".join(current))
    # Merge very short fragments
    merged = []
    for p in paragraphs:
        if merged and len(merged[-1]) < 60 and len(p) < 60:
            merged[-1] = merged[-1] + " " + p
        else:
            merged.append(p)
    return merged


def _extract_key_facts(body_parts: list) -> list:
    facts = []
    seen = set()
    for p in body_parts:
        text = p.strip()
        if len(text) < 20 or text in seen:
            continue
        # Prefer lines that look like concrete statements
        seen.add(text)
        facts.append(text[:200])
        if len(facts) >= 8:
            break
    return facts


def _detect_garbled(text: str) -> bool:
    # Heuristic: excessive single-char tokens suggest column extraction issues
    words = text.split()
    if not words:
        return False
    single_char = sum(1 for w in words if len(w) == 1 and w.isalpha())
    return single_char / len(words) > 0.15


def _detect_operational_relevance(body: str) -> str:
    lower = body.lower()
    signals = {
        "procedure": "Contains procedural content",
        "maintenance": "Relates to maintenance",
        "safety": "Contains safety information",
        "alarm": "References alarm systems",
        "parameter": "Contains parameter/configuration data",
        "specification": "Technical specification document",
        "report": "Report document",
        "standard": "Standard or regulation reference",
        "training": "Training or instructional content",
        "drawing": "Contains engineering drawing references",
        "hazard": "Contains hazard / risk content",
    }
    matched = [v for k, v in signals.items() if k in lower]
    if matched:
        return "; ".join(matched[:3])
    return "General document — review for specific relevance"
