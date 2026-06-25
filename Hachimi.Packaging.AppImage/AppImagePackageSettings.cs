namespace Hachimi.Packaging.AppImage;

public class AppImagePackageSettings : PackageSettings {
    /// <summary>
    /// 应用显示名称
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
    
    /// <summary>
    /// 图标绝对路径
    /// </summary>
    public string IconPath { get; set; } = string.Empty;

    /// <summary>
    /// 目标架构 (x86_64, i686, aarch64 等)
    /// </summary>
    public string Architecture { get; set; } = "x86_64";

    /// <summary>
    /// 应用描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 是否在打开程序时显示终端
    /// </summary>
    public bool IsTerminal { get; set; }
    
    /// <summary>
    /// 应用分类
    /// </summary>
    public IReadOnlyCollection<string> Categories { get; set; }
    
    /// <summary>
    /// 搜索关键词（辅助用户在启动器搜索）
    /// </summary>
    public IReadOnlyCollection<string> Keywords { get; set; }
}
