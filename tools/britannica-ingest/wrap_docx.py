"""
DOCX -> Markdown wrapper for Britannica ingestion.

Uses python-docx (already installed).
Extracts: title, headings, paragraphs, tables.
Preserves heading hierarchy in the output.
"""

from pathlib import Path
import re


def extract(source: Path) -> dict:
    try:
        from docx import Document
    except ImportError:
        return {"error": "python-docx not installed (pip install python-docx)"}

    try:
        doc = Document(str(source))
    except Exception as e:
        return {"error": f"Failed to open DOCX: {e}"}

    paragraphs_md = []
    first_heading = None
    body_text_parts = []
    table_summaries = []

    for block in doc.element.body:
        tag = block.tag.split("}")[-1] if "}" in block.tag else block.tag

        if tag == "p":
            # Find the matching paragraph object
            from docx.text.paragraph import Paragraph
            para = Paragraph(block, doc)
            style = para.style.name if para.style else ""
            text = para.text.strip()
            if not text:
                continue

            if style.startswith("Heading 1") or style == "Title":
                if not first_heading:
                    first_heading = text
                paragraphs_md.append(f"## {text}")
                body_text_parts.append(text)
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
            else:
                paragraphs_md.append(text)
                body_text_parts.append(text)

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
                    # Pad/trim to header length
                    row = (row + [""] * len(header))[:len(header)]
                    md_rows.append("| " + " | ".join(row) + " |")
                tbl_md = "\n".join(md_rows)
                paragraphs_md.append(tbl_md)
                # Flatten table text for summary
                for row in rows[1:]:
                    body_text_parts.extend([c for c in row if c])
                table_summaries.append(f"Table with {len(rows)-1} data rows and columns: {', '.join(header[:6])}")

    full_body = "\n\n".join(paragraphs_md)
    body_words = " ".join(body_text_parts)

    # Build summary: first ~300 chars of non-heading body text
    summary_parts = [p for p in body_text_parts if len(p) > 20 and not p.startswith("#")]
    summary = " ".join(summary_parts[:4])[:500].strip()
    if not summary and body_words:
        summary = body_words[:300].strip()

    # Key facts: first 8 non-trivial sentences/bullets
    key_facts = _extract_key_facts(body_text_parts, table_summaries)

    # Operational relevance: look for keywords
    op_relevance = _detect_operational_relevance(body_words)

    # Gaps
    gaps = []
    if not doc.paragraphs:
        gaps.append("Document appears to have no paragraphs")
    if not full_body.strip():
        gaps.append("No text content extracted")
    if len(paragraphs_md) < 3:
        gaps.append("Very few content blocks — document may be image-heavy or protected")

    return {
        "title": first_heading or source.stem,
        "summary": summary,
        "key_facts": key_facts,
        "operational_relevance": op_relevance,
        "gaps": gaps,
        "method": "python-docx paragraph/table extraction",
        "body_md": full_body,
    }


def _extract_key_facts(text_parts: list, table_summaries: list) -> list:
    facts = []
    seen = set()
    for text in text_parts:
        text = text.strip()
        if len(text) < 15 or text in seen:
            continue
        seen.add(text)
        facts.append(text)
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
