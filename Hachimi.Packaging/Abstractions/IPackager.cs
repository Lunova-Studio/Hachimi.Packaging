using System.Threading;
using System.Threading.Tasks;

namespace Hachimi.Packaging;

/// <summary>
/// 通用打包器接口，定义了打包与还原的基本操作。
/// </summary>
public interface IPackager
{
    /// <summary>
    /// 执行打包操作。
    /// </summary>
    Task<PackageResult> PackAsync(PackagingOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// 执行还原/解包操作（可选实现）。
    /// </summary>
    Task<PackageResult> RestoreAsync(PackagingOptions options, CancellationToken cancellationToken = default);
}
