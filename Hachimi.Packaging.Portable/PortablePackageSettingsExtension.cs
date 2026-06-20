using Hachimi.Packaging.Extensions;

namespace Hachimi.Packaging.Portable;

public static class PortablePackageSettingsExtension {
    public static T SetAppName<T>(this T settings, string appName) where T : PortablePackageSettings =>
        settings.Modify(x => x.AppName = appName);
    
    public static T SetRuntime<T>(this T settings, string runTime) where T : PortablePackageSettings =>
        settings.Modify(x => x.Runtime = runTime);
}