using Hachimi.Packaging.Interfaces;
using Microsoft.Extensions.Logging;

namespace Hachimi.Packaging.AppBundle;

/// <summary>
/// 应用程序包打包器
/// </summary>
public sealed partial class AppBundlePackager: IPackager<AppBundlePackageSettings> {
    private  readonly ILogger _logger;
    
    private string InfoPlistPath { get; set; }
    private string MacOSDirectory { get; set; }
    private string ContentsDirectory { get; set; }
    private string ResourcesDirectory { get; set; }

    public AppBundlePackager(ILogger logger) {
        _logger = logger;
    }
    
    public Task PackAsync(Configure<PackagingContext> contextConfigure, Configure<AppBundlePackageSettings> settingsConfigure) {
        var context = contextConfigure(new PackagingContext());
        var settings = settingsConfigure(new AppBundlePackageSettings());

        var appPath = context.OutputPath;
        ContentsDirectory = Path.Combine(appPath, "Contents");
        MacOSDirectory = Path.Combine(ContentsDirectory, "MacOS");
        InfoPlistPath = Path.Combine(ContentsDirectory, "Info.plistqi");
        ResourcesDirectory = Path.Combine(ContentsDirectory, "Resources");
        
        CreateAppBundle(appPath);

        throw new Exception();
    }

    private void CreateAppBundle(string appPath) {
        if (Directory.Exists(appPath))
            Directory.Delete(appPath, true);

        Directory.CreateDirectory(appPath!);
        Directory.CreateDirectory(ContentsDirectory);
        Directory.CreateDirectory(MacOSDirectory);
        Directory.CreateDirectory(ResourcesDirectory);
                                                               
        _logger.LogInformation("App bundle created successfully.");
    }

    private void CopyAppIcon(string iconPath) {
        var iconFileName = $".icns";
        var destPath = Path.Combine(ResourcesDirectory, iconFileName);
        
        File.Copy(iconPath, destPath, true);
        LogIconCopyToResourcesDirectory(iconFileName);
    }

    #region LogMessages

    [LoggerMessage(LogLevel.Information, "Icon copied to Resources directory: {fileName}")]
    partial void LogIconCopyToResourcesDirectory(string fileName);

    #endregion
}

public class AppBundlePackageSettings : PackageSettings {
    /// <summary>
    /// <c>.icns</c>
    /// </summary>
    public string AppIconPath { get; set; }
    
    public string Project { get; set; } 
}