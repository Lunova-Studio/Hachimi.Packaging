namespace Hachimi.Packaging.Extensions;

public static class PackageSettingsExtension {
    public static T Modify<T>(this T settings, Action<T> action) {
        action(settings);
        return settings;
    }
    
    public static T SetAppName<T>(this T settings, string appName) where T : PackageSettings =>
        settings.Modify(x => x.AppName = appName);

    public static T SetRuntime<T>(this T settings, string runtime) where T : PackageSettings =>
        settings.Modify(x => x.Runtime = runtime);
}