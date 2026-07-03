using System.Runtime.InteropServices;

namespace Hachimi.Packaging.AppImage.Helpers;

internal class RuntimeHelper {
    private const int RuntimeDownloadTimeoutMinutes = 5;
    
    private static readonly HttpClient HttpClient = new() {
        Timeout = TimeSpan.FromMinutes(RuntimeDownloadTimeoutMinutes)
    };
    
    internal static async Task<byte[]> DownloadRuntimeAsync(Architecture architecture) {
        var url = GetRuntimeUrl(architecture);

        var data = await HttpClient.GetByteArrayAsync(url);

        return data.Length == 0 
            ? throw new InvalidOperationException("Downloaded runtime is empty.") 
            : data;
    }

    private static string GetRuntimeUrl(Architecture architecture) {
        const string baseUrl = "https://github.com/AppImage/AppImageKit/releases/download/continuous/runtime-";
        return architecture switch {
            Architecture.X86 or Architecture.X64 => $"{baseUrl}x86_64",
            Architecture.Arm => $"{baseUrl}armhf",
            Architecture.Arm64 => $"{baseUrl}aarch64",
            _ => throw new ArgumentException($"Unsupported architecture: {architecture}", nameof(architecture))
        };
    }
}