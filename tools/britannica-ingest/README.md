# Britannica Ingestion Tools

Converts raw files into cleaned Markdown notes, ready for Britannica to ingest.

## What this does

Britannica should never parse raw `.docx`, `.pdf`, `.xlsx`, or `.pptx` files directly.
These scripts are the **front door** — they extract content, structure it, grade the quality,
and write a validated Markdown note to `notes/cleaned/`.

---

## Quick start (PowerShell)

```powershell
cd C:\Users\Ian\.openclaw\workspace\tools\britannica-ingest

# Ingest a Word document
.\ingest.ps1 "C:\Docs\SomeProcedure.docx"

# Ingest a PDF
.\ingest.ps1 "C:\Docs\MaintenanceReport.pdf"

# Ingest an Excel workbook
.\ingest.ps1 "C:\Docs\Parameters.xlsx"

# Ingest a PowerPoint
.\ingest.ps1 "C:\Docs\ProjectBrief.pptx"

# Preview without writing (dry run)
.\ingest.ps1 "C:\Docs\Something.docx" -DryRun

# Custom output directory
.\ingest.ps1 "C:\Docs\Something.docx" -Out "D:\Britannica\notes\cleaned"
```

Or call Python directly:

```powershell
python ingest.py "C:\Docs\SomeProcedure.docx"
python ingest.py "C:\Docs\SomeProcedure.docx" --dry-run
python ingest.py "C:\Docs\SomeProcedure.docx" --out "D:\notes\cleaned"
```

---

## Supported file types

| Extension | Wrapper | Library |
|---|---|---|
| `.docx` | `wrap_docx.py` | python-docx |
| `.pdf` | `wrap_pdf.py` | pdfminer.six |
| `.xlsx` / `.xlsm` | `wrap_xlsx.py` | openpyxl |
| `.xls` | `wrap_xlsx.py` | xlrd (not installed — see below) |
| `.pptx` | `wrap_pptx.py` | python-pptx |

All libraries except xlrd are already installed in the project Python environment.

---

## Output format

Each note is written to `notes/cleaned/<filename>_<timestamp>.md` and follows the Britannica structure:

- **Title** — inferred from document content or filename
- **Source** — full path to the source file
- **File Type** — extension
- **Ingested** — timestamp
- **Extraction Quality** — A / B / C / D (see below)
- **Method** — what extractor was used
- **Summary** — first substantive content
- **Key Facts** — up to 8–10 bullet points
- **Operational Relevance** — keyword-detected relevance signals
- **Gaps / Uncertainties** — known issues with this extraction

---

## Quality grades

| Grade | Meaning |
|---|---|
| **A** | Trusted — rich content, real summary, multiple facts |
| **B** | Usable — mostly complete, minor gaps |
| **C** | Weak — partial extraction, sparse content |
| **D** | Failed — no usable content |

**Exit codes:** `0` = A/B, `1` = C/D, `2` = hard failure.

Britannica must lower confidence and state gaps explicitly when working from a C or D note.

---

## Known limitations

| Scenario | Expected result |
|---|---|
| Scanned/image PDF | Grade D — pdfminer extracts no text from image layers |
| Encrypted/password-protected files | Grade D or hard error |
| DOCX with embedded objects / drawings only | Grade C — only paragraph text extracted |
| Large XLSX (>50 rows, >3 sheets) | Truncated — gaps noted in output |
| `.xls` (legacy Excel) | Requires `pip install xlrd` — currently unsupported |
| PPTX with image-only slides | Grade C/D — no text from images |
| Multi-column PDF layouts | May produce jumbled text — garbled text warning added |

---

## File structure

```
tools/britannica-ingest/
├── ingest.py          # Main dispatcher (Python entry point)
├── ingest.ps1         # PowerShell wrapper (recommended for Wilko)
├── validate.py        # Note validator and grader
├── wrap_docx.py       # DOCX extractor
├── wrap_pdf.py        # PDF extractor
├── wrap_xlsx.py       # XLSX/XLSM/XLS extractor
├── wrap_pptx.py       # PPTX extractor
├── notes/
│   └── cleaned/       # Output: validated Markdown notes (auto-created)
└── README.md
```

---

## Adding a new file type

1. Create `wrap_<ext>.py` in this directory.
2. Implement `def extract(source: Path) -> dict` — return a dict with:
   - `title`, `summary`, `key_facts` (list), `operational_relevance`, `gaps` (list), `method`
   - `error` (str) on failure
3. Add the extension to the `SUPPORTED` dict in `ingest.py`.

---

## Dependencies

Already installed (standard Python 3.12 environment):
- `python-docx` — DOCX
- `pdfminer.six` — PDF
- `openpyxl` — XLSX/XLSM
- `python-pptx` — PPTX

Optional (not currently installed):
- `xlrd` — legacy `.xls` support (`pip install xlrd`)
