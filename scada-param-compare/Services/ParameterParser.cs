using ScadaParamCompare.Models;
using System.Text.RegularExpressions;

namespace ScadaParamCompare.Services;

public static class ParameterParser
{
    // ─── Parse ───────────────────────────────────────────────────────────────────

    /// <summary>
    /// Parse pasted text into parameter rows.
    /// Accepts CSV or TSV, with or without a header row.
    /// Expected columns (order-insensitive if header present):
    ///   Tag / TagName / Name, Value, Unit, Description / Desc
    /// If no header is detected, assumes: Col0=Tag, Col1=Value, Col2=Unit, Col3=Description
    /// </summary>
    public static (List<ParameterRow> Rows, List<string> Warnings) Parse(string text)
    {
        var rows    = new List<ParameterRow>();
        var warnings = new List<string>();

        if (string.IsNullOrWhiteSpace(text))
            return (rows, warnings);

        var lines = text.ReplaceLineEndings("\n")
                        .Split('\n', StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length == 0)
            return (rows, warnings);

        // detect delimiter
        var delim = DetectDelimiter(lines[0]);

        // check for header
        var firstCells = SplitLine(lines[0], delim);
        int idxTag  = -1, idxVal = -1, idxUnit = -1, idxDesc = -1;
        int startLine = 0;

        if (LooksLikeHeader(firstCells))
        {
            (idxTag, idxVal, idxUnit, idxDesc) = MapColumns(firstCells);
            startLine = 1;
        }
        else
        {
            // positional fallback
            idxTag  = 0;
            idxVal  = firstCells.Count > 1 ? 1 : -1;
            idxUnit = firstCells.Count > 2 ? 2 : -1;
            idxDesc = firstCells.Count > 3 ? 3 : -1;
        }

        if (idxTag < 0)
        {
            warnings.Add("Could not find a Tag/Name column. Check your paste format.");
            return (rows, warnings);
        }

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = startLine; i < lines.Length; i++)
        {
            var cells = SplitLine(lines[i], delim);
            if (cells.Count == 0 || cells.All(c => c.Length == 0)) continue;

            var tag  = Cell(cells, idxTag);
            if (string.IsNullOrWhiteSpace(tag)) continue;

            var val  = Cell(cells, idxVal);
            var unit = Cell(cells, idxUnit);
            var desc = Cell(cells, idxDesc);

            if (!seen.Add(tag))
                warnings.Add($"Line {i + 1}: duplicate tag name '{tag}' — only first occurrence used.");
            else
                rows.Add(new ParameterRow(tag, val, unit, desc, i + 1));
        }

        return (rows, warnings);
    }

    // ─── Validate OPC tags ────────────────────────────────────────────────────────

    private static readonly Regex ReservedChars = new(@"[#\[\]{}\|\\^`~<>?,;!@$%&*]", RegexOptions.Compiled);
    private static readonly Regex ValidTagStart  = new(@"^[A-Za-z_]",                  RegexOptions.Compiled);

    /// <summary>Basic OPC UA / classic OPC tag name validation.</summary>
    public static List<TagIssue> ValidateTags(IEnumerable<ParameterRow> rows)
    {
        var issues = new List<TagIssue>();
        foreach (var row in rows)
        {
            var t = row.TagName;
            if (t.Length > 256)
                issues.Add(new TagIssue(t, $"Tag name exceeds 256 characters ({t.Length})."));
            if (!ValidTagStart.IsMatch(t))
                issues.Add(new TagIssue(t, "Tag name must start with a letter or underscore."));
            if (ReservedChars.IsMatch(t))
                issues.Add(new TagIssue(t, $"Tag name contains reserved character(s): {string.Join("", ReservedChars.Matches(t).Select(m => m.Value).Distinct())}."));
            if (t.Contains("  "))
                issues.Add(new TagIssue(t, "Tag name contains consecutive spaces."));
        }
        return issues;
    }

    // ─── Diff ─────────────────────────────────────────────────────────────────────

    /// <summary>Compare two parameter sets. Matching is by tag name (case-insensitive). Values are normalised before comparison.</summary>
    public static CompareResult Compare(
        List<ParameterRow> left,
        List<ParameterRow> right,
        bool validateTags = true)
    {
        var leftMap  = left.ToDictionary(r => r.TagName, StringComparer.OrdinalIgnoreCase);
        var rightMap = right.ToDictionary(r => r.TagName, StringComparer.OrdinalIgnoreCase);

        var added    = new List<DiffRow>();
        var removed  = new List<DiffRow>();
        var changed  = new List<DiffRow>();
        var unchanged = new List<DiffRow>();

        // items in right but not left = Added
        foreach (var (key, r) in rightMap)
        {
            if (!leftMap.ContainsKey(key))
                added.Add(new DiffRow(DiffKind.Added, r.TagName, null, r.Value, null, r.Unit, null, r.Description));
        }

        // items in left but not right = Removed; items in both = Changed or Unchanged
        foreach (var (key, l) in leftMap)
        {
            if (!rightMap.TryGetValue(key, out var r))
            {
                removed.Add(new DiffRow(DiffKind.Removed, l.TagName, l.Value, null, l.Unit, null, l.Description, null));
            }
            else
            {
                var valChanged  = !NormaliseValue(l.Value).Equals(NormaliseValue(r.Value), StringComparison.OrdinalIgnoreCase);
                var unitChanged = !NormaliseStr(l.Unit).Equals(NormaliseStr(r.Unit), StringComparison.OrdinalIgnoreCase);
                var descChanged = !NormaliseStr(l.Description).Equals(NormaliseStr(r.Description), StringComparison.OrdinalIgnoreCase);

                if (valChanged || unitChanged || descChanged)
                    changed.Add(new DiffRow(DiffKind.Changed, l.TagName, l.Value, r.Value, l.Unit, r.Unit, l.Description, r.Description));
                else
                    unchanged.Add(new DiffRow(DiffKind.Unchanged, l.TagName, l.Value, r.Value, l.Unit, r.Unit, l.Description, r.Description));
            }
        }

        // sort all lists by tag name
        added.Sort((a, b) => string.Compare(a.TagName, b.TagName, StringComparison.OrdinalIgnoreCase));
        removed.Sort((a, b) => string.Compare(a.TagName, b.TagName, StringComparison.OrdinalIgnoreCase));
        changed.Sort((a, b) => string.Compare(a.TagName, b.TagName, StringComparison.OrdinalIgnoreCase));

        var issues = validateTags
            ? ValidateTags(right)
            : new List<TagIssue>();

        var warnings = new List<string>();

        return new CompareResult(added, removed, changed, unchanged, issues, left.Count, right.Count, warnings);
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────────

    private static char DetectDelimiter(string line)
    {
        var tabs   = line.Count(c => c == '\t');
        var commas = line.Count(c => c == ',');
        return tabs >= commas ? '\t' : ',';
    }

    private static List<string> SplitLine(string line, char delim)
    {
        // minimal CSV split — handles quoted fields
        if (delim == '\t')
            return line.Split('\t').Select(c => c.Trim()).ToList();

        var result = new List<string>();
        var current = new System.Text.StringBuilder();
        bool inQuote = false;
        foreach (var ch in line)
        {
            if (ch == '"') { inQuote = !inQuote; continue; }
            if (ch == delim && !inQuote) { result.Add(current.ToString().Trim()); current.Clear(); continue; }
            current.Append(ch);
        }
        result.Add(current.ToString().Trim());
        return result;
    }

    private static bool LooksLikeHeader(List<string> cells) =>
        cells.Any(c => c.StartsWith("Tag", StringComparison.OrdinalIgnoreCase)
                    || c.Equals("Name",  StringComparison.OrdinalIgnoreCase)
                    || c.Equals("Value", StringComparison.OrdinalIgnoreCase)
                    || c.Equals("Unit",  StringComparison.OrdinalIgnoreCase)
                    || c.StartsWith("Desc", StringComparison.OrdinalIgnoreCase));

    private static (int tag, int val, int unit, int desc) MapColumns(List<string> headers)
    {
        int tag = -1, val = -1, unit = -1, desc = -1;
        for (var i = 0; i < headers.Count; i++)
        {
            var h = headers[i];
            if (h.StartsWith("Tag", StringComparison.OrdinalIgnoreCase) || h.Equals("Name", StringComparison.OrdinalIgnoreCase))
                tag = i;
            else if (h.Equals("Value", StringComparison.OrdinalIgnoreCase))
                val = i;
            else if (h.Equals("Unit", StringComparison.OrdinalIgnoreCase) || h.Equals("Units", StringComparison.OrdinalIgnoreCase))
                unit = i;
            else if (h.StartsWith("Desc", StringComparison.OrdinalIgnoreCase))
                desc = i;
        }
        // fallback: if no tag found, use column 0
        if (tag < 0) tag = 0;
        return (tag, val, unit, desc);
    }

    private static string Cell(List<string> cells, int idx) =>
        idx >= 0 && idx < cells.Count ? cells[idx].Trim() : "";

    /// <summary>Normalise numeric strings so "1.0", "1", "1.000" all compare equal.</summary>
    private static string NormaliseValue(string v)
    {
        v = v.Trim();
        if (decimal.TryParse(v, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d))
            return d.ToString("G29"); // remove trailing zeros
        return v;
    }

    private static string NormaliseStr(string v) => v.Trim();
}
