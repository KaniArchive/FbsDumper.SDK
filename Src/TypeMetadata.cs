namespace FbsDumper.SDK;

public sealed class TypeMetadata
{
    public IReadOnlyList<TypeFieldInfo> StaticFields { get; init; } = [];
    public IReadOnlyList<string> Interfaces { get; init; } = [];
    public IReadOnlyList<string> CustomAttributes { get; init; } = [];
}

public sealed class TypeFieldInfo
{
    public string Name { get; init; } = "";
    public string FieldType { get; init; } = "";
    public bool IsPublic { get; init; }
    public bool IsStatic { get; init; }
}