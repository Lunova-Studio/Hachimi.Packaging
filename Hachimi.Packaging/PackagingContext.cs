namespace Hachimi.Packaging;

public sealed class PackagingContext {
    
    /// <summary>
    /// 打包的源目录，如 <c>bin/Release/win-x64/publish</c>
    /// </summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// 输出包路径，如 <c>publish/{AppName}.zip</c>
    /// </summary>
    public string OutputPath { get; set; } = string.Empty;
}