using System.Buffers.Binary;
using System.Runtime.InteropServices;
using Hachimi.Packaging.AppImage.Helpers;
using Hachimi.Packaging.Interfaces;
using Microsoft.Extensions.Logging;
using NyaFs.Filesystem.SquashFs;
using NyaFs.Filesystem.SquashFs.Types;

namespace Hachimi.Packaging.AppImage;

public sealed partial class AppImagePackager : IPackager<AppImagePackageSettings> {
    private readonly ILogger _logger;
    private readonly AppImageBundleBuilder _builder;
    
    private const int BufferSize = 65536;

    public AppImagePackager(ILogger logger) {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _builder = new AppImageBundleBuilder();
    }

    public async Task PackAsync(
        Configurator<PackagingContext> contextConfigure,
        Configurator<AppImagePackageSettings> settingsConfigure) {
        _logger.LogInformation("Starting AppImage package build...");

        var context = contextConfigure(new PackagingContext());
        var settings = settingsConfigure(new AppImagePackageSettings());

        Validate(context, settings);
        await BuildAppImageAsync(context, settings, CancellationToken.None);

        _logger.LogInformation("AppImage package created successfully.");
    }

    private void SetFileExecutePermission(string filePath) {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        LogSettingExecutePermissionForFilename(Path.GetFileName(filePath));
        
        try {
            File.SetUnixFileMode(filePath,
                UnixFileMode.UserExecute | UnixFileMode.GroupExecute | UnixFileMode.OtherExecute);
        } catch (Exception ex) {
            _logger.LogWarning(ex, "Failed to set execute permission on {FilePath}", filePath);
        }
    }

    private void Validate(PackagingContext context, AppImagePackageSettings settings) {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(settings);

        if (string.IsNullOrWhiteSpace(context.Source))
            throw new ArgumentException("Source directory not specified.", nameof(context));

        if (!Directory.Exists(context.Source))
            throw new DirectoryNotFoundException($"Source directory not found: {context.Source}");

        if (string.IsNullOrWhiteSpace(context.OutputPath))
            throw new ArgumentException("Output path not specified.", nameof(context));

        if (string.IsNullOrWhiteSpace(settings.AppName))
            throw new ArgumentException("AppName not specified.", nameof(settings));

        LogPackageInfo(settings.AppName, settings.Architecture);
    }

    private async Task BuildAppImageAsync(PackagingContext context, AppImagePackageSettings settings, CancellationToken cancellationToken) {
        var tempSquashFSPath = Path.Combine(Path.GetTempPath(), $"appimage-{Guid.NewGuid():N}.squashfs");
        var tempDir = string.Empty;
        
        try {
            tempDir = await _builder.BuildAsync(context, settings);
            
            LogBuildStep(1, "Building SquashFS image...");
            
            var squashFSBuilder = new SquashFsBuilder(SqCompressionType.Gzip);
            squashFSBuilder.AddDirectory(tempDir);
            squashFSBuilder.AddSymLink($"{settings.AppName}{Path.GetExtension(settings.IconPath)}", "/.DirIcon");
            squashFSBuilder.Build(tempSquashFSPath);

            LogBuildStep(2, "Reading SquashFS data...");
            
            var squashFSData = await File.ReadAllBytesAsync(tempSquashFSPath, cancellationToken);
            LogSquashFSInfo(squashFSData.Length);

            LogBuildStep(3, "Downloading runtime...");
            
            cancellationToken.ThrowIfCancellationRequested();
            var runtimeData = await RuntimeHelper.DownloadRuntimeAsync(settings.Architecture);
            cancellationToken.ThrowIfCancellationRequested();

            LogBuildStep(4, "Writing AppImage file...");
            
            await WriteAppImageAsync(context.OutputPath, runtimeData, squashFSData, cancellationToken);
            
            SetFileExecutePermission(context.OutputPath);

            var fileInfo = new FileInfo(context.OutputPath);
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                fileInfo.UnixFileMode = UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
                                        UnixFileMode.GroupExecute | UnixFileMode.GroupRead |
                                        UnixFileMode.OtherRead | UnixFileMode.OtherExecute;
            
            LogBuildSuccess(context.OutputPath, fileInfo.Length);
        } catch (OperationCanceledException) {
            _logger.LogWarning("AppImage build cancelled");
            throw;
        } catch (Exception ex) {
            _logger.LogError(ex, "AppImage build failed");
            throw;
        } finally {
            if (File.Exists(tempSquashFSPath))
                File.Delete(tempSquashFSPath);
            
            new DirectoryInfo(tempDir).Parent?.Delete(true);
        }
    }

    private async Task WriteAppImageAsync(string outputPath, byte[] runtimeData, byte[] squashFSData, CancellationToken cancellationToken) {
        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        if (File.Exists(outputPath))
            File.Delete(outputPath);

        await using var fs = new FileStream(
            outputPath!,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: BufferSize,
            useAsync: true);

        await fs.WriteAsync(runtimeData, cancellationToken);
        LogWriteSegment("Runtime", 0, runtimeData.Length);

        await fs.WriteAsync(squashFSData, cancellationToken);
        LogWriteSegment("SquashFS", runtimeData.Length, squashFSData.Length);

        WriteAppImageDescriptor(fs, runtimeData.Length, squashFSData.LongLength);
        await fs.FlushAsync(cancellationToken);
    }

    private static void WriteAppImageDescriptor(FileStream stream, long payloadOffset, long payloadSize) {
        Span<byte> magic = [
            0x41, 0x49, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00
        ];

        Span<byte> offsetBytes = stackalloc byte[8];
        Span<byte> sizeBytes = stackalloc byte[8];

        BinaryPrimitives.WriteUInt64LittleEndian(offsetBytes, (ulong)payloadOffset);
        BinaryPrimitives.WriteUInt64LittleEndian(sizeBytes, (ulong)payloadSize);

        stream.Write(magic);
        stream.Write(offsetBytes);
        stream.Write(sizeBytes);
    }

    #region Logging
    
    [LoggerMessage(LogLevel.Information, "Package Info - Name: {AppName}, Arch: {Architecture}")]
    private partial void LogPackageInfo(string appName, Architecture architecture);

    [LoggerMessage(LogLevel.Information, "Step {Step}/4: {Message}")]
    private partial void LogBuildStep(int step, string message);

    [LoggerMessage(LogLevel.Information, "SquashFS Image - Size: {Size:N0} bytes")]
    private partial void LogSquashFSInfo(int size);

    [LoggerMessage(LogLevel.Information, "✓ AppImage Build Success - Path: {Path}, Size: {Size:N0} bytes")]
    private partial void LogBuildSuccess(string path, long size);

    [LoggerMessage(LogLevel.Debug, "Write {Segment} at offset {Offset}, size: {Size:N0} bytes")]
    private partial void LogWriteSegment(string segment, int offset, int size);
    
    [LoggerMessage(LogLevel.Information, "Setting execute permission for: {FileName}")]
    partial void LogSettingExecutePermissionForFilename(string fileName);
    
    #endregion
}
