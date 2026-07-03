using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Text;
using Hachimi.Packaging.Interfaces;

namespace Hachimi.Packaging.AppImage;

public sealed class AppImageBundleBuilder : IBundleBuilder<AppImagePackageSettings> {
    //TODO: metainfo
    public async Task<string> BuildAsync(PackagingContext context, AppImagePackageSettings settings) { 
        var (layoutDir, binDir, shareDir) = CreateAppDir();
        
        CopyExec(context,  binDir);
        CopyIcon(settings, layoutDir);

        await CreateAppRunAsync(settings, layoutDir);
        await CreateDesktopFileAsync(settings, layoutDir);
        
        return layoutDir;
    }

    private static (string, string, string) CreateAppDir() {
        var baseDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        
        var layoutDir = Path.Combine(baseDir, "squashfs-root");
        var usrDir = Path.Combine(layoutDir, "usr");
        var binDir = Path.Combine(usrDir, "bin");
        var shareDir = Path.Combine(usrDir, "share");
        
        Directory.CreateDirectory(layoutDir);
        Directory.CreateDirectory(usrDir);
        Directory.CreateDirectory(binDir);
        // Directory.CreateDirectory(shareDir);

        return (layoutDir, binDir, shareDir);
    }

    private static void CopyIcon(AppImagePackageSettings settings, string layoutDir) {
        if (string.IsNullOrEmpty(settings.IconPath))
            return;
        
        var iconInfo = new FileInfo(settings.IconPath);
        if (!iconInfo.Exists)
            return;

        iconInfo.CopyTo(Path.Combine(layoutDir, $"{settings.AppName}{iconInfo.Extension}"), true);
    }

    private static void CopyExec(PackagingContext context, string binDir) {
        var sourceDirInfo = new DirectoryInfo(context.Source);
        var files = sourceDirInfo.EnumerateFiles("*", SearchOption.AllDirectories)
            .Where(x => x.Extension is not ".pdb")
            .ToImmutableArray();

        foreach (var file in files) {
            var relativePath = Path.GetRelativePath(context.Source, file.FullName);
            var destPath = Path.Combine(binDir, relativePath);
            if (Path.GetDirectoryName(destPath) is { Length: > 0 } destDir)
                Directory.CreateDirectory(destDir);
            
            file.CopyTo(destPath, true);
        }
    }
    
    private static async Task CreateAppRunAsync(AppImagePackageSettings settings, string layoutDir) {
        var path = Path.Combine(layoutDir, "AppRun");
        var text = $"""
                     #!/bin/bash
                     exec "$APPDIR/usr/bin/{settings.AppName}" "$@"
                     """;
        
        await File.WriteAllTextAsync(path, text, new UTF8Encoding(false));
        if(!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
            File.SetUnixFileMode(path, UnixFileMode.UserExecute | UnixFileMode.GroupExecute | UnixFileMode.OtherExecute);
        
    }

    private static Task CreateDesktopFileAsync(AppImagePackageSettings settings, string layoutDir) {
        var execPath = $"/usr/bin/{settings.AppName}";
        var text = $"""
                     [Desktop Entry]
                     Type=Application
                     StartupNotify=true
                     Name={settings.DisplayName}
                     Exec="{execPath}"
                     Icon={settings.AppName}
                     StartupWMClass={settings.AppName}
                     Comment={settings.Description}
                     Terminal={settings.IsTerminal.ToString().ToLower()}
                     {Map(settings.Keywords, "Keywords=")}
                     {Map(settings.Categories, "Categories=")}
                     """;
        
        return File.WriteAllTextAsync(Path.Combine(layoutDir, $"{settings.AppName}.desktop"), text, new UTF8Encoding(false));
    }

    private static string Map<T>(IReadOnlyCollection<T> enumerable, string head = "") =>
        enumerable?.Count > 0
            ? $"{head}{string.Concat(enumerable.Select(x => $"{x};"))}" 
            : string.Empty;
}