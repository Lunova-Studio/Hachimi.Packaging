namespace Hachimi.Packaging.Interfaces;

public interface IBundleBuilder<in TSettings> where TSettings : PackageSettings {
    Task<string> BuildAsync(PackagingContext context, TSettings settings);
}