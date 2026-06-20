namespace Hachimi.Packaging.Extensions;

public static class PackagingContextExtension {
    public static void WithSource(this PackagingContext context, string source) => 
        context.Source = source;
    
    public static void WithOutput(this PackagingContext context, string output) => 
        context.OutputPath = output;
}