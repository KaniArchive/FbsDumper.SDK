namespace FbsDumper.SDK;

public interface IExtension
{
    string Name { get; }
    void Register(ExtensionContext context);
}