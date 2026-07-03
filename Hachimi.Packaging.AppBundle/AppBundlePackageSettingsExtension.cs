using Hachimi.Packaging.Extensions;

namespace Hachimi.Packaging.AppBundle;

public static class AppBundlePackageSettingsExtension {
    public static T SetVersion<T>(this T settings, string version) where T : AppBundlePackageSettings =>
        settings.Modify(x => x.Version = version);
    
    public static T SetIcon<T>(this T settings, string iconPath) where T : AppBundlePackageSettings =>
        settings.Modify(x => x.IconPath = iconPath);
    
    public static T SetIdentifier<T>(this T settings, string identifier) where T : AppBundlePackageSettings =>
        settings.Modify(x => x.Identifier = identifier);
    
    public static T SetDisplayName<T>(this T settings, string displayName) where T : AppBundlePackageSettings =>
        settings.Modify(x => x.DisplayName = displayName);
        
    public static T SetPrincipalClass<T>(this T settings, string principalClass) where T : AppBundlePackageSettings =>
        settings.Modify(x => x.PrincipalClass = principalClass);
        
    public static T SetHighResolutionCapable<T>(this T settings, bool highResolutionCapable) where T : AppBundlePackageSettings =>
        settings.Modify(x => x.IsHighResolutionCapable = highResolutionCapable);
}