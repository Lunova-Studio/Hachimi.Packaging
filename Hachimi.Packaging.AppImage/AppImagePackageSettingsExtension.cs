using Hachimi.Packaging.Extensions;

namespace Hachimi.Packaging.AppImage;

public static class AppImagePackageSettingsExtension {
    public static T SetDisplayName<T>(this T settings, string displayName) where T : AppImagePackageSettings =>
        settings.Modify(x => x.DisplayName = displayName);

    public static T SetArchitecture<T>(this T settings, string architecture) where T : AppImagePackageSettings =>
        settings.Modify(x => x.Architecture = architecture);

    public static T SetDescription<T>(this T settings, string description) where T : AppImagePackageSettings =>
        settings.Modify(x => x.Description = description);
    
    public static T SetIcon<T>(this T settings, string path) where T : AppImagePackageSettings =>
        settings.Modify(x => x.IconPath = path);
}