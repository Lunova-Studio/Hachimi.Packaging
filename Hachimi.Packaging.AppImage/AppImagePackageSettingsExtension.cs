using System.Runtime.InteropServices;
using Hachimi.Packaging.Extensions;

namespace Hachimi.Packaging.AppImage;

public static class AppImagePackageSettingsExtension {
    public static T SetDisplayName<T>(this T settings, string displayName) where T : AppImagePackageSettings =>
        settings.Modify(x => x.DisplayName = displayName);

    public static T SetArchitecture<T>(this T settings, Architecture architecture) where T : AppImagePackageSettings =>
        settings.Modify(x => x.Architecture = architecture);

    public static T SetDescription<T>(this T settings, string description) where T : AppImagePackageSettings =>
        settings.Modify(x => x.Description = description);
    
    public static T SetIcon<T>(this T settings, string path) where T : AppImagePackageSettings =>
        settings.Modify(x => x.IconPath = path);
        
    public static T SetIsTerminal<T>(this T settings, bool isTerminal) where T : AppImagePackageSettings =>
        settings.Modify(x => x.IsTerminal = isTerminal);
        
    public static T SetCategories<T>(this T settings, params string[] categories) where T : AppImagePackageSettings =>
        settings.Modify(x => x.Categories = categories);
        
    public static T SetKeywords<T>(this T settings, params string[] keywords) where T : AppImagePackageSettings =>
        settings.Modify(x => x.Keywords = keywords);
}