namespace Hachimi.Packaging.Extensions;

public static class PackagingContextExtension {
    public static PackagingContext SetSource(this PackagingContext settings, string source) =>
        settings.Modify(x => x.Source = source);
        
    public static PackagingContext SetOutput(this PackagingContext settings, string output) =>
        settings.Modify(x => x.OutputPath = output);
}