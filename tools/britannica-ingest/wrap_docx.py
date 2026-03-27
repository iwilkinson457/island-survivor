"""
DOCX -> Markdown wrapper for Britannica ingestion.

Uses python-docx (already installed).
Extracts: title, headings, paragraphs, tables.
Preserves heading hierarchy in the output.
"""

from pathlib import Path
import re


_STRUCTURAL_HEADING_NAMES = {
    "summary", "table of contents", "contents", "introduction",
    "overview", "abstract", "document control", "revision history",
    "scope", "purpose", "references", "appendix",
}

_TOC_LINE_RE = re.compile(r"\t\d+\s*$|\s{3,}\d+\s*$")


def _is_toc_line(text: str) -> bool:
    """Returns True if a line looks like a table-of-contents entry."""
    return bool(_TOC_LINE_RE.search(text))


def extract(source: Path) -> dict:
    try:
        from docx import Document
    except ImportError:
        return {"error": "python-docx not installed (pip install python-docx)"}

    try:
        doc = Document(str(source))
    except Exception as e:
        return {"error": f"Failed to open DOCX: {e}"}

    # Try core properties for a clean title first
    try:
        core_title = (doc.core_properties.title or "").strip()
    except Exception:
        core_title = ""

    paragraphs_md = []
    first_real_heading = None   # first heading that isn't a structural/TOC stub
    body_text_parts = []        # all text (used for body_md)
    prose_parts = []            # non-TOC, non-heading prose (used for summary)
    table_summaries = []

    for block in doc.element.body:
        tag = block.tag.split("}")[-1] if "}" in block.tag else block.tag

        if tag == "p":
            from docx.text.paragraph import Paragraph
            para = Paragraph(block, doc)
            style = para.style.name if para.style else ""
            text = para.text.strip()
            if not text:
                continue

            is_toc = _is_toc_line(text)

            if style.startswith("Heading 1") or style == "Title":
                if (not first_real_heading
                        and text.lower() not in _STRUCTURAL_HEADING_NAMES
                        and not is_toc):
                    first_real_heading = text
                paragraphs_md.append(f"## {text}")
                body_text_parts.append(text)
                # Don't add headings to prose_parts — they're not summaries
            elif style.startswith("Heading 2"):
                paragraphs_md.append(f"### {text}")
                body_text_parts.append(text)
            elif style.startswith("Heading 3"):
                paragraphs_md.append(f"#### {text}")
                body_text_parts.append(text)
            elif style.startswith("Heading"):
                paragraphs_md.append(f"##### {text}")
                body_text_parts.append(text)
            elif style in ("List Paragraph", "List Bullet", "List Number"):
                paragraphs_md.append(f"- {text}")
                body_text_parts.append(text)
                if not is_toc and len(text) > 15:
                    prose_parts.append(text)
            else:
                paragraphs_md.append(text)
                body_text_parts.append(text)
                if not is_toc and len(text) > 15:
                    prose_parts.append(text)

        elif tag == "tbl":
            from docx.table import Table
            table = Table(block, doc)
            rows = []
            for i, row in enumerate(table.rows):
                cells = [c.text.strip() for c in row.cells]
                rows.append(cells)

            if rows:
                header = rows[0]
                md_rows = ["| " + " | ".join(header) + " |",
                           "| " + " | ".join(["---"] * len(header)) + " |"]
                for row in rows[1:]:
                    row = (row + [""] * len(header))[:len(header)]
                    md_rows.append("| " + " | ".join(row) + " |")
                tbl_md = "\n".join(md_rows)
                paragraphs_md.append(tbl_md)
                for row in rows[1:]:
                    body_text_parts.extend([c for c in row if c])
                table_summaries.append(f"Table with {len(rows)-1} data rows and columns: {', '.join(header[:6])}")

    full_body = "\n\n".join(paragraphs_md)
    body_words = " ".join(body_text_parts)

    # Summary: first 400 chars of real prose (no TOC lines, no headings)
    summary = " ".join(prose_parts[:5])[:500].strip()
    if not summary:
        # Fallback: raw body text, truncated
        summary = body_words[:300].strip()

    # Key facts: prose parts only (no TOC entries)
    key_facts = _extract_key_facts(prose_parts, table_summaries)

    op_relevance = _detect_operational_relevance(body_words)

    gaps = []
    if not doc.paragraphs:
        gaps.append("Document appears to have no paragraphs")
    if not full_body.strip():
        gaps.append("No text content extracted")
    if len(paragraphs_md) < 3:
        gaps.append("Very few content blocks -- document may be image-heavy or protected")
    if not prose_parts:
        gaps.append("No prose paragraphs found -- document may be heading/TOC heavy; substantive body text not reached")

    # Title preference: core properties > first real heading > filename
    title = core_title or first_real_heading or source.stem

    return {
        "title": title,
        "summary": summary,
        "key_facts": key_facts,
        "operational_relevance": op_relevance,
        "gaps": gaps,
        "method": "python-docx paragraph/table extraction",
        "body_md": full_body,
    }


def _extract_key_facts(prose_parts: list, table_summaries: list) -> list:
    facts = []
    seen = set()
    for text in prose_parts:
        text = text.strip()
        if len(text) < 20 or text in seen:
            continue
        if _is_toc_line(text):
            continue
        seen.add(text)
        facts.append(text[:200])
        if len(facts) >= 8:
            break
    for ts in table_summaries[:2]:
        facts.append(ts)
    return facts


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
    }
    matched = [v for k, v in signals.items() if k in lower]
    if matched:
        return "; ".join(matched[:3])
    return "General document — review for specific relevance"
