namespace FbsDumper.Sdk.Internal;

internal sealed class TableInfoPayload
{
    public string TableName { get; set; } = "";
    public string OriginalNamespace { get; set; } = "";
    public bool NoCreate { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = [];
    public List<FieldInfoPayload> Fields { get; set; } = [];
    public List<TypeFieldInfoPayload> StaticFields { get; set; } = [];
    public List<string> Interfaces { get; set; } = [];
    public List<string> CustomAttributes { get; set; } = [];

    public TableInfo ToTableInfo()
    {
        var table = new TableInfo
        {
            TableName = TableName,
            OriginalNamespace = OriginalNamespace,
            NoCreate = NoCreate,
            Fields = Fields.Select(f => f.ToFieldInfo()).ToList(),
        };
        foreach (var kv in Metadata) table.Metadata[kv.Key] = kv.Value;
        return table;
    }

    public TypeMetadata ToTypeMetadata() => new()
    {
        StaticFields = StaticFields.Select(f => new TypeFieldInfo
        {
            Name = f.Name,
            FieldType = f.FieldType,
            IsPublic = f.IsPublic,
            IsStatic = f.IsStatic,
        }).ToList(),
        Interfaces = Interfaces,
        CustomAttributes = CustomAttributes,
    };

    public void SyncMetadataFrom(TableInfo table)
    {
        Metadata.Clear();
        foreach (var kv in table.Metadata) Metadata[kv.Key] = kv.Value;
    }
}

internal sealed class FieldInfoPayload
{
    public string Name { get; set; } = "";
    public int Offset { get; set; }
    public bool IsArray { get; set; }
    public TypeInfoPayload Type { get; set; } = new();

    public FieldInfo ToFieldInfo() => new()
    {
        Name = Name,
        Offset = Offset,
        IsArray = IsArray,
        Type = Type.ToTypeInfo(),
    };
}

internal sealed class TypeInfoPayload
{
    public string Name { get; set; } = "";
    public string Namespace { get; set; } = "";
    public string FullName { get; set; } = "";
    public bool IsEnum { get; set; }

    public TypeInfo ToTypeInfo() => new()
    {
        Name = Name,
        Namespace = Namespace,
        FullName = FullName,
        IsEnum = IsEnum,
    };
}

internal sealed class TypeFieldInfoPayload
{
    public string Name { get; set; } = "";
    public string FieldType { get; set; } = "";
    public bool IsPublic { get; set; }
    public bool IsStatic { get; set; }
}

internal sealed class SchemaInfoPayload
{
    public List<TableInfoPayload> Tables { get; set; } = [];
    public List<EnumInfoPayload> Enums { get; set; } = [];

    public SchemaInfo ToSchemaInfo() => new()
    {
        Tables = Tables.Select(t => t.ToTableInfo()).ToList(),
        Enums = Enums.Select(e => e.ToEnumInfo()).ToList(),
    };
}

internal sealed class EnumInfoPayload
{
    public string EnumName { get; set; } = "";
    public string OriginalNamespace { get; set; } = "";
    public TypeInfoPayload Type { get; set; } = new();
    public List<EnumFieldInfoPayload> Fields { get; set; } = [];
    public List<TypeFieldInfoPayload> StaticFields { get; set; } = [];
    public List<string> Interfaces { get; set; } = [];
    public List<string> CustomAttributes { get; set; } = [];

    public EnumInfo ToEnumInfo() => new()
    {
        EnumName = EnumName,
        OriginalNamespace = OriginalNamespace,
        Type = Type.ToTypeInfo(),
        Fields = Fields.Select(f => new EnumFieldInfo { Name = f.Name, Value = f.Value }).ToList(),
    };

    public TypeMetadata ToTypeMetadata() => new()
    {
        StaticFields = StaticFields.Select(f => new TypeFieldInfo
        {
            Name = f.Name,
            FieldType = f.FieldType,
            IsPublic = f.IsPublic,
            IsStatic = f.IsStatic,
        }).ToList(),
        Interfaces = Interfaces,
        CustomAttributes = CustomAttributes,
    };
}

internal sealed class EnumFieldInfoPayload
{
    public string Name { get; set; } = "";
    public long Value { get; set; }
}

internal sealed class GenerationOptionsPayload
{
    public string OutputPath { get; set; } = "";
    public string? CustomNamespace { get; set; }
    public string EnumOut { get; set; } = "Inline";
    public bool ForceSnakeCase { get; set; }
    public bool IsSplitMode { get; set; }
    public bool SkipDuplicates { get; set; }

    public GenerationOptions ToGenerationOptions() => new()
    {
        OutputPath = OutputPath,
        CustomNamespace = CustomNamespace,
        EnumOut = EnumOut switch
        {
            "Separate" => EnumOutMode.Separate,
            "Omit" => EnumOutMode.Omit,
            _ => EnumOutMode.Inline,
        },
        ForceSnakeCase = ForceSnakeCase,
        IsSplitMode = IsSplitMode,
        SkipDuplicates = SkipDuplicates,
    };
}
