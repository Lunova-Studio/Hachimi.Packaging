namespace Hachimi.Packaging.AppBundle;

public class AppBundlePackageSettings : PackageSettings {
    /// <summary>
    /// <c>.icns</c>
    /// </summary>
    public string IconPath { get; set; }

    public string Version { get; set; } = "1.0.0";
    public string Identifier { get; set; } = "Identifier";
    public string DisplayName { get; set; }
    public string PrincipalClass { get; set; } = "NSApplication";

    public bool IsHighResolutionCapable { get; set; } = true;
}