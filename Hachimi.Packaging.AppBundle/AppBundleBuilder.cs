using System.Collections.Immutable;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Hachimi.Packaging.Interfaces;

namespace Hachimi.Packaging.AppBundle;

public sealed class AppBundleBuilder : IBundleBuilder<AppBundlePackageSettings> {
    public async Task<string> BuildAsync(PackagingContext context, AppBundlePackageSettings settings) {
        var (layoutDir,contentsDir, macosDir, resourceDir) = CreateAppDir(settings);
        
        CopyExec(context, macosDir);
        CopyIcon(settings, resourceDir);
        
        await CreatePListFileAsync(settings, contentsDir);
        
        return layoutDir;
    }

    private static (string, string, string, string) CreateAppDir(AppBundlePackageSettings settings) {
        var baseDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        var layoutDir = Path.Combine(baseDir, $"{settings.DisplayName}.app");
        var contentsDir = Path.Combine(layoutDir, "Contents");
        var macosDir = Path.Combine(contentsDir, "MacOS");
        var resourceDir = Path.Combine(contentsDir, "Resources");

        Directory.CreateDirectory(layoutDir);
        Directory.CreateDirectory(contentsDir);
        Directory.CreateDirectory(macosDir);
        Directory.CreateDirectory(resourceDir);
        
        return (layoutDir, contentsDir, macosDir, resourceDir);
    }

    private static void CopyExec(PackagingContext context, string macosDir) {
        var sourceDirInfo = new DirectoryInfo(context.Source);
        var files = sourceDirInfo.EnumerateFiles("*", SearchOption.AllDirectories)
            .Where(x => x.Extension is not ".pdb")
            .ToImmutableArray();

        foreach (var file in files) {
            var relativePath = Path.GetRelativePath(context.Source, file.FullName);
            var destPath = Path.Combine(macosDir, relativePath);
            if (Path.GetDirectoryName(destPath) is { Length: > 0 } destDir)
                Directory.CreateDirectory(destDir);
            
            file.CopyTo(destPath, true);
        }
    }
    
    private static void CopyIcon(AppBundlePackageSettings settings, string resourcesDir) {
        if (string.IsNullOrEmpty(settings.IconPath))
            return;
        
        var iconInfo = new FileInfo(settings.IconPath);
        if (!iconInfo.Exists)
            return;

        iconInfo.CopyTo(Path.Combine(resourcesDir, iconInfo.Name), true);
    }

    private static async Task CreatePListFileAsync(AppBundlePackageSettings settings, string contentsDir) {
        var doc = new XDocument(
            new XDeclaration("1.0", "UTF-8", null),
            new XDocumentType("plist", "-//Apple//DTD PLIST 1.0//EN", "http://www.apple.com/DTDs/PropertyList-1.0.dtd", null),
            new XElement("plist",
                new XAttribute("version", "1.0"),
                new XElement("dict",
                    new XElement("key", "CFBundleName"),
                    new XElement("string", settings.AppName),
                    
                    new XElement("key", "CFBundleDisplayName"),
                    new XElement("string", settings.DisplayName),
                    
                    new XElement("key", "CFBundleIdentifier"),
                    new XElement("string", settings.Identifier),
                    
                    new XElement("key", "CFBundleVersion"),
                    new XElement("string", settings.Version),

                    new XElement("key", "CFBundleSignature"),
                    new XElement("string", "????"),

                    new XElement("key", "CFBundlePackageType"),
                    new XElement("string", "APPL"),
                    
                    new XElement("key", "CFBundleExecutable"),
                    new XElement("string", settings.AppName),
                    
                    new XElement("key", "CFBundleIconFile"),
                    new XElement("string", Path.GetFileName(settings.IconPath)),
                    
                    new XElement("key", "CFBundleShortVersionString"),
                    new XElement("string", settings.Version),
                    
                    new XElement("key", "NSPrincipalClass"),
                    new XElement("string", settings.PrincipalClass),
                    
                    new XElement("key", "NSHighResolutionCapable"),
                    new XElement(settings.IsHighResolutionCapable.ToString().ToLower())
                )
            )
        );

        var path = Path.Combine(contentsDir, "Info.plist");
        await using var writer = new XmlTextWriter(path, new UTF8Encoding(false));
        
        writer.Formatting = Formatting.Indented;
        doc.Save(writer);
    }
}