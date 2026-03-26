#!/usr/bin/env python3
"""
Britannica Markdown Ingestion Dispatcher
=========================================
Routes a raw file to the appropriate wrapper, validates the result,
and writes a cleaned Markdown note ready for Britannica to ingest.

Usage:
    python ingest.py <file_path> [--out <output_dir>] [--dry-run]

Output:
    A .md file in --out (default: notes/cleaned/ relative to this script).

Exit codes:
    0 = note written and validated (grade A or B)
    1 = note written but weak (grade C or D)
    2 = unsupported file type or hard failure
"""

import argparse
import sys
import os
from pathlib import Path
from datetime import datetime

SCRIPT_DIR = Path(__file__).parent
DEFAULT_OUT = SCRIPT_DIR / "notes" / "cleaned"

SUPPORTED = {
    ".docx": "wrap_docx",
    ".pdf":  "wrap_pdf",
    ".xlsx": "wrap_xlsx",
    ".xlsm": "wrap_xlsx",
    ".xls":  "wrap_xlsx",
    ".pptx": "wrap_pptx",
}


def main():
    parser = argparse.ArgumentParser(description="Britannica file ingestion dispatcher")
    parser.add_argument("file", help="Path to the source file")
    parser.add_argument("--out", default=str(DEFAULT_OUT), help="Output directory for cleaned notes")
    parser.add_argument("--dry-run", action="store_true", help="Extract and validate but do not write output")
    args = parser.parse_args()

    source = Path(args.file).resolve()
    if not source.exists():
        print(f"[ERROR] File not found: {source}", file=sys.stderr)
        sys.exit(2)

    ext = source.suffix.lower()
    if ext not in SUPPORTED:
        print(f"[ERROR] Unsupported file type: {ext}", file=sys.stderr)
        print(f"  Supported: {', '.join(SUPPORTED.keys())}", file=sys.stderr)
        sys.exit(2)

    # Lazy import the right wrapper
    wrapper_name = SUPPORTED[ext]
    try:
        wrapper_mod = __import__(wrapper_name)
    except ImportError as e:
        print(f"[ERROR] Could not load wrapper {wrapper_name}: {e}", file=sys.stderr)
        sys.exit(2)

    print(f"[ingest] Source : {source.name}")
    print(f"[ingest] Type   : {ext}")
    print(f"[ingest] Wrapper: {wrapper_name}")

    note = wrapper_mod.extract(source)

    from validate import validate_note
    grade, issues = validate_note(note)
    note["extraction_quality"] = grade

    md = render_note(note, source, ext)

    # Validation report
    print(f"[ingest] Grade  : {grade}")
    if issues:
        for iss in issues:
            print(f"  [!] {iss}")

    if args.dry_run:
        print("[ingest] Dry-run — output not written.")
        print("\n--- Note preview ---")
        print(md[:1200])
        sys.exit(0 if grade in ("A", "B") else 1)

    out_dir = Path(args.out)
    out_dir.mkdir(parents=True, exist_ok=True)
    stem = source.stem.replace(" ", "_")
    ts = datetime.now().strftime("%Y%m%d_%H%M%S")
    out_path = out_dir / f"{stem}_{ts}.md"
    out_path.write_text(md, encoding="utf-8")
    print(f"[ingest] Written: {out_path}")

    sys.exit(0 if grade in ("A", "B") else 1)


def render_note(note: dict, source: Path, ext: str) -> str:
    """
    Renders the extracted dict into the Britannica cleaned-note structure.
    """
    now = datetime.now().strftime("%Y-%m-%d %H:%M")
    title = note.get("title") or source.stem
    summary = note.get("summary") or "(no summary extracted)"
    key_facts = note.get("key_facts") or []
    op_relevance = note.get("operational_relevance") or "(not determined)"
    gaps = note.get("gaps") or []
    method = note.get("method") or "unknown"
    quality = note.get("extraction_quality", "C")

    facts_md = "\n".join(f"- {f}" for f in key_facts) if key_facts else "- (none extracted)"
    gaps_md  = "\n".join(f"- {g}" for g in gaps) if gaps else "- none noted"

    return f"""# {title}

| Field | Value |
|---|---|
| **Source** | `{source}` |
| **File Type** | {ext} |
| **Ingested** | {now} |
| **Extraction Quality** | {quality} |
| **Method** | {method} |

---

## Summary

{summary}

---

## Key Facts

{facts_md}

---

## Operational Relevance

{op_relevance}

---

## Gaps / Uncertainties

{gaps_md}
"""


if __name__ == "__main__":
    main()
