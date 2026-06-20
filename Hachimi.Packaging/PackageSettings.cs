namespace Hachimi.Packaging;

public abstract class PackageSettings {
    /// <summary>
    /// 程序文件名，例如：<c>App.exe</c>
    /// </summary>
    public string AppName { get; set; }
    
    public string Runtime { get; set; }
}