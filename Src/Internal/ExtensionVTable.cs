using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FbsDumper.SDK.Internal;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct ExtensionVTable
{
    public delegate* unmanaged<byte*> GetName;
    public delegate* unmanaged<bool> HasGenerator;
    public delegate* unmanaged<byte*, void> OnTableBuilt;
    public delegate* unmanaged<byte*, void> OnSchemaBuilt;
    public delegate* unmanaged<byte*, void> OnEnumBuilt;
    public delegate* unmanaged<byte*, void> InitGenerator;
    public delegate* unmanaged<byte*, byte**, int*, void> GeneratorGetTableDecl;
    public delegate* unmanaged<byte*, byte*, byte*, byte**, int*, void> GeneratorGetFieldDecl;
    public delegate* unmanaged<byte*, byte*, byte*, byte**, int*, void> GeneratorResolveFieldType;
    public delegate* unmanaged<byte*, void> FreeBuffer;
}

public static class ExtensionAbi
{
    internal const string EntryPoint = "FbsDumperGetExtension";
}

internal static unsafe class ExtensionVTableHelpers
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    internal static byte[] Encode(object value) =>
        Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, JsonOpts) + "\0");

    internal static T? Decode<T>(byte* ptr)
    {
        var len = StrLen(ptr);
        return JsonSerializer.Deserialize<T>(new ReadOnlySpan<byte>(ptr, len), JsonOpts);
    }

    internal static string? DecodeString(byte* ptr) =>
        ptr == null ? null : Marshal.PtrToStringUTF8((nint)ptr);

    internal static int StrLen(byte* ptr)
    {
        var len = 0;
        while (ptr[len] != 0) len++;
        return len;
    }

    internal static byte* AllocUtf8(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value + "\0");
        var ptr = (byte*)NativeMemory.Alloc((nuint)bytes.Length);
        bytes.AsSpan().CopyTo(new Span<byte>(ptr, bytes.Length));
        return ptr;
    }
}
