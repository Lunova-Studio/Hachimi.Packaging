namespace Hachimi.Packaging.Interfaces;

public delegate T Configure<T>(T options);

/// <summary>
/// 统一打包器接口
/// </summary>
/// <typeparam name="T">包信息设置，部分打包情况可忽略</typeparam>
public interface IPackager<T> where T : PackageSettings {
    Task PackAsync(Configure<PackagingContext> contextConfigure, Configure<T> settingsConfigure);
}