namespace FbsDumper.SDK;

public sealed class GenerationOptions
{
    public string OutputPath { get; init; } = "";
    public string? CustomNamespace { get; init; }
    public EnumOutMode EnumOut { get; init; }
    public bool ForceSnakeCase { get; init; }
    public bool IsSplitMode { get; init; }
    public bool SkipDuplicates { get; init; }
}

public enum EnumOutMode
{
    Inline,
    Separate,
    Omit,
}
