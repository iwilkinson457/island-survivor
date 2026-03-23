using System.Text.RegularExpressions;
using MissionControl.Models;

namespace MissionControl.Services;

public class WorkLogService
{
    private static readonly string MemoryDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".openclaw", "workspace", "memory");

    private static readonly Regex DateFile = new(@"^(\d{4}-\d{2}-\d{2})\.md$", RegexOptions.Compiled);

    // ─── public API ─────────────────────────────────────────────────────────────

    /// <summary>Returns all work log days, newest first.</summary>
    public List<WorkLogDay> GetAll()
    {
        if (!Directory.Exists(MemoryDir)) return [];

        return Directory.GetFiles(MemoryDir, "*.md")
            .Select(f => (path: f, name: Path.GetFileName(f)))
            .Where(x => DateFile.IsMatch(x.name))
            .Select(x =>
            {
                var dateStr = DateFile.Match(x.name).Groups[1].Value;
                var date = DateOnly.Parse(dateStr);
                var raw = SafeRead(x.path);
                return Parse(date, raw);
            })
            .OrderByDescending(d => d.Date)
            .ToList();
    }

    public string MemoryDirectoryPath => MemoryDir;
    public bool MemoryDirectoryExists => Directory.Exists(MemoryDir);

    // ─── parser ─────────────────────────────────────────────────────────────────

    private static WorkLogDay Parse(DateOnly date, string raw)
    {
        var lines = raw.ReplaceLineEndings("\n").Split('\n').ToList();
        var sections = new List<WorkLogSection>();

        string currentHeading = "";
        var currentBullets = new List<string>();
        var currentBody = new List<string>();

        void FlushSection()
        {
            if (currentHeading.Length > 0 || currentBullets.Count > 0 || currentBody.Count > 0)
                sections.Add(new WorkLogSection(currentHeading, [.. currentBullets], [.. currentBody]));
            currentHeading = "";
            currentBullets = [];
            currentBody = [];
        }

        foreach (var rawLine in lines)
        {
            var line = rawLine.TrimEnd();

            // skip the top-level date heading (e.g. "# 2026-03-21")
            if (Regex.IsMatch(line, @"^#\s+\d{4}-\d{2}-\d{2}\s*$")) continue;

            if (line.StartsWith("## ") || line.StartsWith("### "))
            {
                FlushSection();
                currentHeading = line.TrimStart('#').Trim();
                continue;
            }

            if (line.StartsWith("# "))
            {
                FlushSection();
                currentHeading = line.TrimStart('#').Trim();
                continue;
            }

            if (string.IsNullOrWhiteSpace(line)) continue;

            // bullet: -, *, +
            var bulletMatch = Regex.Match(line, @"^[\-\*\+]\s+(.+)$");
            if (bulletMatch.Success)
            {
                currentBullets.Add(bulletMatch.Groups[1].Value.Trim());
                continue;
            }

            // numbered list
            var numberedMatch = Regex.Match(line, @"^\d+\.\s+(.+)$");
            if (numberedMatch.Success)
            {
                currentBullets.Add(numberedMatch.Groups[1].Value.Trim());
                continue;
            }

            currentBody.Add(line);
        }

        FlushSection();

        // if nothing was parsed into sections, treat whole raw content as one section
        if (sections.Count == 0 && raw.Trim().Length > 0)
            sections.Add(new WorkLogSection("", [], [raw.Trim()]));

        return new WorkLogDay(date, raw, sections);
    }

    // ─── helpers ────────────────────────────────────────────────────────────────

    private static string SafeRead(string path)
    {
        try { return File.ReadAllText(path); }
        catch { return ""; }
    }

    /// <summary>Inline-code spans and bold/italic to HTML. Very lightweight.</summary>
    public static string InlineHtml(string text)
    {
        // escape HTML first
        var s = System.Net.WebUtility.HtmlEncode(text);
        // `code`
        s = Regex.Replace(s, @"`([^`]+)`", "<code>$1</code>");
        // **bold**
        s = Regex.Replace(s, @"\*\*(.+?)\*\*", "<strong>$1</strong>");
        // *italic*
        s = Regex.Replace(s, @"\*(.+?)\*", "<em>$1</em>");
        return s;
    }
}
