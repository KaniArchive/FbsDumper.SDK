using System.Runtime.InteropServices;
using System.Text;
using FbsDumper.SDK.Internal;

namespace FbsDumper.SDK;

public abstract unsafe class ExtensionBase : IExtension
{
    private static ExtensionBase? _instance;
    private static GeneratorBase? _generator;

    private static readonly ExtensionVTable VTable = new()
    {
        GetName = &VTableGetName,
        HasGenerator = &VTableHasGenerator,
        OnTableBuilt = &VTableOnTableBuilt,
        OnSchemaBuilt = &VTableOnSchemaBuilt,
        OnEnumBuilt = &VTableOnEnumBuilt,
        InitGenerator = &VTableInitGenerator,
        GeneratorGetTableDecl = &VTableGeneratorGetTableDecl,
        GeneratorGetFieldDecl = &VTableGeneratorGetFieldDecl,
        GeneratorResolveFieldType = &VTableGeneratorResolveFieldType,
        FreeBuffer = &VTableFreeBuffer,
    };

    private static readonly GCHandle NameHandle =
        GCHandle.Alloc(Array.Empty<byte>(), GCHandleType.Pinned);

    public abstract string Name { get; }
    public virtual bool HasCustomGenerator => false;

    public virtual void OnTableBuilt(TableInfo table, TypeMetadata type) { }
    public virtual void OnSchemaBuilt(SchemaInfo schema) { }
    public virtual void OnEnumBuilt(EnumInfo enumInfo, TypeMetadata type) { }
    public virtual GeneratorBase? CreateGenerator(GenerationOptions opts) => null;

    public virtual void Register(ExtensionContext context)
    {
        context.OnTableBuilt = OnTableBuilt;
        context.OnSchemaBuilt = OnSchemaBuilt;
        context.OnEnumBuilt = OnEnumBuilt;
        context.CreateGenerator = CreateGenerator;
    }

    [UnmanagedCallersOnly(EntryPoint = ExtensionAbi.EntryPoint)]
    public static ExtensionVTable* FbsDumperGetExtension()
    {
        fixed (ExtensionVTable* p = &VTable) return p;
    }

    protected static void SetInstance(ExtensionBase instance) =>
        _instance = instance;

    [UnmanagedCallersOnly]
    private static byte* VTableGetName()
    {
        if (_instance == null) return (byte*)0;
        var bytes = Encoding.UTF8.GetBytes(_instance.Name + "\0");
        NameHandle.Target = bytes;
        return (byte*)NameHandle.AddrOfPinnedObject();
    }

    [UnmanagedCallersOnly]
    private static bool VTableHasGenerator() =>
        _instance?.HasCustomGenerator ?? false;

    [UnmanagedCallersOnly]
    private static void VTableOnTableBuilt(byte* json)
    {
        if (_instance == null) return;
        var payload = ExtensionVTableHelpers.Decode<TableInfoPayload>(json);
        if (payload == null) return;
        var table = payload.ToTableInfo();
        _instance.OnTableBuilt(table, payload.ToTypeMetadata());
        payload.SyncMetadataFrom(table);
    }

    [UnmanagedCallersOnly]
    private static void VTableOnSchemaBuilt(byte* json)
    {
        if (_instance == null) return;
        var payload = ExtensionVTableHelpers.Decode<SchemaInfoPayload>(json);
        if (payload == null) return;
        _instance.OnSchemaBuilt(payload.ToSchemaInfo());
    }

    [UnmanagedCallersOnly]
    private static void VTableOnEnumBuilt(byte* json)
    {
        if (_instance == null) return;
        var payload = ExtensionVTableHelpers.Decode<EnumInfoPayload>(json);
        if (payload == null) return;
        _instance.OnEnumBuilt(payload.ToEnumInfo(), payload.ToTypeMetadata());
    }

    [UnmanagedCallersOnly]
    private static void VTableInitGenerator(byte* optsJson)
    {
        if (_instance == null) return;
        var payload = ExtensionVTableHelpers.Decode<GenerationOptionsPayload>(optsJson);
        if (payload == null) return;
        _generator = _instance.CreateGenerator(payload.ToGenerationOptions());
    }

    [UnmanagedCallersOnly]
    private static void VTableGeneratorGetTableDecl(byte* tableJson, byte** outStr, int* outLen)
    {
        *outStr = null;
        if (_generator == null) return;
        var table = ExtensionVTableHelpers.Decode<TableInfoPayload>(tableJson)?.ToTableInfo();
        if (table == null) return;
        var result = _generator.GetTableDecl(table);
        *outStr = ExtensionVTableHelpers.AllocUtf8(result);
        *outLen = Encoding.UTF8.GetByteCount(result);
    }

    [UnmanagedCallersOnly]
    private static void VTableGeneratorGetFieldDecl(byte* fieldJson, byte* tableJson, byte* namePtr, byte* typePtr, byte** outStr, int* outLen)
    {
        *outStr = null;
        if (_generator == null) return;
        var field = ExtensionVTableHelpers.Decode<FieldInfoPayload>(fieldJson)?.ToFieldInfo();
        var table = ExtensionVTableHelpers.Decode<TableInfoPayload>(tableJson)?.ToTableInfo();
        if (field == null || table == null) return;
        var name = ExtensionVTableHelpers.DecodeString(namePtr) ?? field.Name;
        var type = ExtensionVTableHelpers.DecodeString(typePtr) ?? field.Type.Name;
        var result = _generator.GetFieldDecl(field, table, name, type);
        *outStr = ExtensionVTableHelpers.AllocUtf8(result);
        *outLen = Encoding.UTF8.GetByteCount(result);
    }

    [UnmanagedCallersOnly]
    private static void VTableGeneratorResolveFieldType(byte* typeNamePtr, byte* fieldJson, byte* tableJson, byte** outStr, int* outLen)
    {
        *outStr = null;
        if (_generator == null) return;
        var typeName = ExtensionVTableHelpers.DecodeString(typeNamePtr);
        var field = ExtensionVTableHelpers.Decode<FieldInfoPayload>(fieldJson)?.ToFieldInfo();
        var table = ExtensionVTableHelpers.Decode<TableInfoPayload>(tableJson)?.ToTableInfo();
        if (typeName == null || field == null || table == null) return;
        var result = _generator.ResolveFieldType(typeName, field, table);
        if (result == null) return;
        *outStr = ExtensionVTableHelpers.AllocUtf8(result);
        *outLen = Encoding.UTF8.GetByteCount(result);
    }

    [UnmanagedCallersOnly]
    private static void VTableFreeBuffer(byte* ptr)
    {
        if (ptr != null) NativeMemory.Free(ptr);
    }
}
