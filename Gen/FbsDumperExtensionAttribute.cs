namespace FbsDumper.SDK;

/// <summary>
/// Apply to your <see cref="ExtensionBase"/> subclass to enable automatic NativeAOT
/// trimmer root generation. The source generator will emit the necessary
/// <c>[DynamicDependency]</c> entries so the type survives ILLink and the NativeAOT linker.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class FbsDumperExtensionAttribute : Attribute { }
