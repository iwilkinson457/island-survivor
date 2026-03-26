# Britannica Extraction Pipeline

## Purpose
Define how raw files become knowledge-ready Markdown before Britannica touches them.

## Folder model
- `FilesForInjest\` = raw files
- `notes\cleaned\` = final cleaned Markdown notes
- `notes\raw\` = manifests and logs
- `scripts\` = wrappers and validators

## File-type routing
- DOCX -> external DOCX-to-Markdown extractor
- PDF -> approved PDF wrapper/extractor
- XLSX/XLSM/XLS -> external workbook-to-Markdown extractor
- PPTX -> external slide-to-Markdown extractor
- Images -> OCR/vision-assisted summary when needed
- Protected or unsupported formats -> Markdown stub marked for manual review

## Required markdown structure
Each output note should contain:
- Title
- Source
- File Type
- Ingested
- Extraction Quality
- Summary
- Key Facts
- Operational Relevance
- Gaps / Uncertainties
- Method

## Quality grades
- A = trusted
- B = usable
- C = weak
- D = failed

## Validation rules
Only pass a note to Britannica if:
1. the file exists on disk
2. the content is readable Markdown
3. it includes a real summary, not just headings
4. it contains meaningful facts where the source permits it
5. it contains no raw binary, XML package internals, ZIP members, or raw PDF streams

## Operational rule
Britannica should retrieve from cleaned notes, not from raw files.
If a note is C or D quality, Britannica must lower confidence and state the gaps explicitly.
