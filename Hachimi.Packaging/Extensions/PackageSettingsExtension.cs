namespace Hachimi.Packaging.Extensions;

public static class PackageSettingsExtension {
    public static T Modify<T>(this T settings, Action<T> action) where T : PackageSettings {
        action(settings);
        return settings;
    }
}