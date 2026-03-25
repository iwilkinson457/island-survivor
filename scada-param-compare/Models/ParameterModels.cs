namespace ScadaParamCompare.Models;

/// <summary>A single parameter row from a paste.</summary>
public record ParameterRow(
    string TagName,
    string Value,
    string Unit,
    string Description,
    int SourceLine         // 1-based for traceability
);

/// <summary>One item in the comparison output.</summary>
public enum DiffKind { Added, Removed, Changed, Unchanged }

public record DiffRow(
    DiffKind Kind,
    string TagName,
    string? OldValue,
    string? NewValue,
    string? OldUnit,
    string? NewUnit,
    string? OldDescription,
    string? NewDescription
)
{
    public bool ValueChanged    => OldValue       != NewValue;
    public bool UnitChanged     => OldUnit        != NewUnit;
    public bool DescChanged     => OldDescription != NewDescription;
}

/// <summary>OPC tag validation result.</summary>
public record TagIssue(string TagName, string Message);

/// <summary>Full result of a compare operation.</summary>
public record CompareResult(
    List<DiffRow> Added,
    List<DiffRow> Removed,
    List<DiffRow> Changed,
    List<DiffRow> Unchanged,
    List<TagIssue> TagIssues,
    int ParsedLeft,
    int ParsedRight,
    List<string> ParseWarnings
)
{
    public int TotalChanges => Added.Count + Removed.Count + Changed.Count;
    public bool IsClean     => TotalChanges == 0 && TagIssues.Count == 0;
}
