namespace FbsDumper.SDK;

public sealed class ExtensionContext
{
    public Action<TableInfo, TypeMetadata>? OnTableBuilt { get; set; }
    public Action<SchemaInfo>? OnSchemaBuilt { get; set; }
    public Action<EnumInfo, TypeMetadata>? OnEnumBuilt { get; set; }
    public Func<GenerationOptions, GeneratorBase?>? CreateGenerator { get; set; }
}