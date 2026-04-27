namespace FbsDumper.SDK;

public sealed class SchemaInfo
{
    public IReadOnlyList<TableInfo> Tables { get; init; } = [];
    public IReadOnlyList<EnumInfo> Enums { get; init; } = [];
}

public sealed class TableInfo
{
    public string TableName { get; init; } = "";
    public string OriginalNamespace { get; init; } = "";
    public bool NoCreate { get; init; }
    public Dictionary<string, object> Metadata { get; } = [];
    public IReadOnlyList<FieldInfo> Fields { get; init; } = [];
}

public sealed class FieldInfo
{
    public string Name { get; init; } = "";
    public int Offset { get; init; }
    public bool IsArray { get; init; }
    public TypeInfo Type { get; init; } = new();
}

public sealed class TypeInfo
{
    public string Name { get; init; } = "";
    public string Namespace { get; init; } = "";
    public string FullName { get; init; } = "";
    public bool IsEnum { get; init; }
}

public sealed class EnumInfo
{
    public string EnumName { get; init; } = "";
    public string OriginalNamespace { get; init; } = "";
    public TypeInfo Type { get; init; } = new();
    public IReadOnlyList<EnumFieldInfo> Fields { get; init; } = [];
}

public sealed class EnumFieldInfo
{
    public string Name { get; init; } = "";
    public long Value { get; init; }
}
