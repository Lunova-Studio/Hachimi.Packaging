using System.Collections.Immutable;
using System.IO.Compression;
using Hachimi.Packaging.Interfaces;
using Microsoft.Extensions.Logging;

namespace Hachimi.Packaging.AppBundle;

/// <summary>
/// MacOS 应用程序包打包器
/// </summary>
public sealed partial class AppBundlePackager: IPackager<AppBundlePackageSettings> {
    private readonly ILogger _logger;
    private readonly AppBundleBuilder _appBundleBuilder;

    public AppBundlePackager(ILogger logger) {
        _logger = logger;
        _appBundleBuilder = new AppBundleBuilder();
    }
    
    public async Task PackAsync(Configurator<PackagingContext> contextConfigure, Configurator<AppBundlePackageSettings> settingsConfigure) {
        var context = contextConfigure(new PackagingContext());
        var settings = settingsConfigure(new AppBundlePackageSettings());

        try {
            var appDir = await _appBundleBuilder.BuildAsync(context, settings);

            _logger.LogInformation("Created app directory {appDir}", appDir);
            await CreateZipPackageAsync(appDir, context);
        } catch (Exception e) {
            _logger.LogError(e, "Failed to create app bundle package.");
            throw;
        }
        
        _logger.LogInformation("Portable package created successfully.");
    }

    private async Task CreateZipPackageAsync(string appDir, PackagingContext context) {
        _logger.LogInformation("Creating zip archive...");
        
        if (File.Exists(context.OutputPath))
            File.Delete(context.OutputPath);
        
        await using var archive = await ZipFile.OpenAsync(context.OutputPath, ZipArchiveMode.Create);
        
        var appFolderName = Path.GetFileName(appDir);
        var files = Directory.EnumerateFiles(appDir, "*", SearchOption.AllDirectories)
            .ToImmutableArray();
                
        foreach (var file in files) {
            var relativePath = Path.GetRelativePath(appDir, file);
            var entryName = Path.Combine(appFolderName, relativePath)
                .Replace("\\", "/");
            
            await archive.CreateEntryFromFileAsync(file, entryName, CompressionLevel.SmallestSize);
        }
        
        LogZipArchiveCreatedWithFiles(files.Length);
    } 

    #region LogMessages
    
    [LoggerMessage(LogLevel.Information, "Zip archive created with {filesLength} files")]
    partial void LogZipArchiveCreatedWithFiles(int filesLength);
    
    #endregion
}