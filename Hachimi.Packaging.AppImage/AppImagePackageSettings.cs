namespace Hachimi.Packaging.AppImage;

public class AppImagePackageSettings : PackageSettings {
    /// <summary>
    /// 应用显示名称
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 应用版本
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// 目标架构 (x86_64, i686, aarch64 等)
    /// </summary>
    public string Architecture { get; set; } = "x86_64";

    /// <summary>
    /// 应用描述
    /// </summary>
    public string? Description { get; set; }
}
