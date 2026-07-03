using System.Collections.Immutable;
using System.Diagnostics;
using System.Formats.Tar;
using System.IO.Compression;
using Hachimi.Packaging.Interfaces;
using Microsoft.Extensions.Logging;

namespace Hachimi.Packaging.Portable;

public sealed partial class PortablePackager : IPackager<PortablePackageSettings> {
    private readonly ILogger _logger;
    
    public PortablePackager(ILogger logger) {
        _logger = logger;
    }
    
    public async Task PackAsync(Configurator<PackagingContext> contextConfigure, Configurator<PortablePackageSettings> settingsConfigure) {
        _logger.LogInformation("Starting Portable package build...");

        var context = contextConfigure(new PackagingContext());
        var settings = settingsConfigure(new PortablePackageSettings());

        var runtime = settings.Runtime;
        
        ArgumentNullException.ThrowIfNull(runtime);
        LogCurrentRuntimeIsRuntime(runtime);

        if (runtime.Contains("osx"))
            throw new NotSupportedException("Portable is not supported.");

        if (runtime.Contains("win"))
            await PackWindowsAsync(context);
        else
            await PackLinuxAsync(context, settings);
        
        _logger.LogInformation("Portable package created successfully.");
    }

    #region Windows

    private async Task PackWindowsAsync(PackagingContext context) {
        _logger.LogInformation("Creating zip archive...");
        
        if (File.Exists(context.OutputPath))
            File.Delete(context.OutputPath);

        // Use SmallestSize for maximum compression
        await using var archive = await ZipFile.OpenAsync(context.OutputPath, ZipArchiveMode.Create);

        var files = Directory.EnumerateFiles(context.Source, "*", SearchOption.AllDirectories)
            .ToImmutableArray();
        
        foreach (var file in files) {
            var entryName = Path.GetRelativePath(context.Source, file)
                .Replace("\\", "/");

            LogAddingFile(entryName);
            await archive.CreateEntryFromFileAsync(file, entryName, CompressionLevel.SmallestSize);
        }

        LogZipArchiveCreatedWithFiles(files.Length);
    }

    #endregion
    
    #region Linux

    /// <summary>
    /// <c>.tar.gz</c>
    /// </summary>
    private async Task PackLinuxAsync(PackagingContext context, PackageSettings settings) {
        await SetExecutePermissionsAsync(context.Source, settings.AppName);
        
        _logger.LogInformation("Creating tar.gz archive...");
        
        if (File.Exists(context.OutputPath))
            File.Delete(context.OutputPath);
        
        var tempTarPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.tar");
        try {
            _logger.LogInformation("Creating tar archive...");

            await TarFile.CreateFromDirectoryAsync(context.Source, tempTarPath, false)
                .ConfigureAwait(false);

            _logger.LogInformation("Compressing with GZip...");

            await using var tarStream = File.OpenRead(tempTarPath);
            await using var outputStream = File.Create(context.OutputPath);
            await using var gzipStream = new GZipStream(outputStream, CompressionLevel.SmallestSize);
            await tarStream.CopyToAsync(gzipStream);

            LogTarGzArchiveCreatedSuccessfullySource(context.OutputPath);
        } catch (Exception e) {
            _logger.LogError(e, "Failed to create tar.gz archive");
            throw;
        } finally {
            if (File.Exists(tempTarPath))
                File.Delete(tempTarPath);
        }
    }

    /// <summary>
    /// 给予可执行文件权限
    /// </summary>
    private async Task SetExecutePermissionsAsync(string source, string appName) {
        _logger.LogInformation("Setting permissions for all files...");

        foreach (var file in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories)) {
            var fileName = Path.GetFileName(file);

            if (fileName == appName) {
                LogSettingExecutePermissionsFor(fileName);
                
                var exitCode = await ExecuteChmodAsync(file, "+x");
                if (exitCode != 0)
                    throw new InvalidOperationException($"Failed to set execute permissions on {file}");
            } else {
                LogSettingReadPermissionsFor(fileName);
                
                var exitCode = await ExecuteChmodAsync(file, "a+rx");
                if (exitCode != 0)
                    throw new InvalidOperationException($"Failed to set read permissions on {file}");
            }
        }
        
        _logger.LogInformation("Permissions set successfully for all files!");
    }
    
    private async Task<int> ExecuteChmodAsync(string filePath, string mode) {
        var processInfo = new ProcessStartInfo {
            FileName = "chmod",
            Arguments = $"{mode} \"{filePath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(processInfo) ?? throw new InvalidOperationException("Failed to start chmod process.");

        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (!string.IsNullOrEmpty(output))
            LogChmodInformation(output);

        if (!string.IsNullOrEmpty(error) && process.ExitCode != 0)
            LogChmodError(error);

        return process.ExitCode;
    }
    
    #endregion

    #region LogMessages
    
    [LoggerMessage(LogLevel.Error, "Chmod error: {error}")]
    partial void LogChmodError(string error);
    
    [LoggerMessage(LogLevel.Information, "chmod output: {output}")]
    partial void LogChmodInformation(string output);
        
    [LoggerMessage(LogLevel.Information, "Adding file: {fileName}")]
    partial void LogAddingFile(string fileName);
    
    [LoggerMessage(LogLevel.Information, "tar.gz archive created successfully: {output}")]
    partial void LogTarGzArchiveCreatedSuccessfullySource(string output);
    
    [LoggerMessage(LogLevel.Information, "Setting execute permissions for: {fileName}")]
    partial void LogSettingExecutePermissionsFor(string fileName);
        
    [LoggerMessage(LogLevel.Information, "Setting read permissions for: {fileName}")]
    partial void LogSettingReadPermissionsFor(string fileName);

    [LoggerMessage(LogLevel.Information, "Zip archive created with {filesLength} files")]
    partial void LogZipArchiveCreatedWithFiles(int filesLength);

    [LoggerMessage(LogLevel.Information, "Current runtime is {runtime}")]
    partial void LogCurrentRuntimeIsRuntime(string runtime);

    #endregion
}