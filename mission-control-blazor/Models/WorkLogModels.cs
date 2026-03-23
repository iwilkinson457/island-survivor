namespace MissionControl.Models;

public record WorkLogDay(
    DateOnly Date,
    string RawMarkdown,
    List<WorkLogSection> Sections
);

public record WorkLogSection(
    string Heading,        // empty string if no heading (top-level)
    List<string> Bullets,  // bullet list items (may be empty)
    List<string> BodyLines // non-bullet paragraph lines
);
