namespace FbsDumper.SDK;

public abstract class GeneratorBase(GenerationOptions options)
{
    protected GenerationOptions Options { get; } = options;

    public virtual string GetTableDecl(TableInfo table) =>
        $"table {table.TableName}";

    public virtual string GetFieldDecl(FieldInfo field, string resolvedName, string resolvedType) =>
        $"\t{resolvedName}: {resolvedType}; // index 0x{field.Offset:X}";

    public virtual string? ResolveFieldType(string typeName, FieldInfo field, TableInfo table) => null;
}
