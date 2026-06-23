namespace Hachimi.Packaging.AppImage.Helpers;

internal class RuntimeHelper {
    private const int RuntimeDownloadTimeoutMinutes = 5;
    
    private static readonly HttpClient HttpClient = new() {
        Timeout = TimeSpan.FromMinutes(RuntimeDownloadTimeoutMinutes)
    };
    
    internal static async Task<byte[]> DownloadRuntimeAsync(string architecture) {
        var url = GetRuntimeUrl(architecture);

        var data = await HttpClient.GetByteArrayAsync(url);

        return data.Length == 0 
            ? throw new InvalidOperationException("Downloaded runtime is empty.") 
            : data;
    }

    private static string GetRuntimeUrl(string architecture) {
        const string baseUrl = "https://github.com/AppImage/AppImageKit/releases/download/continuous/runtime-";
        return architecture.ToLowerInvariant() switch {
            "x86" or "x64" or "amd64" or "x86_64" => $"{baseUrl}x86_64",
            "arm32" or "armhf" => $"{baseUrl}armhf",
            "arm64" or "aarch64" => $"{baseUrl}aarch64",
            _ => throw new ArgumentException($"Unsupported architecture: {architecture}", nameof(architecture))
        };
    }
}