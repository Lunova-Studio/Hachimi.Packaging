using System.CommandLine;
using Hachimi.Packaging.AppBundle;
using Hachimi.Packaging.Extensions;
using Microsoft.Extensions.Logging;

namespace Hachimi.Packaging.Cli.Commands;

public sealed class AppBundleCommand(ILogger logger) : PackagingCommandBase<AppBundlePackager, AppBundlePackageSettings>(logger) {
    protected override string Name => "app";
    protected override string Description => "Package the application as a macOS .app bundle";
    
    private readonly Option<string> _appNameOption = 
        Options.String(
            "--app-name",
            "The actual executable file name inside the .app bundle (e.g., myapp)",
            "-a",
            true);

    private readonly Option<string> _displayNameOption = 
        Options.String(
            "--display-name",
            "The display name of the application shown in Finder and the menu bar (e.g., MyApp)",
            "-dn");

    private readonly Option<string> _iconOption = 
        Options.String(
            "--icon",
            "The path to the .icns icon file for the macOS app (e.g., ./app.icns)");

    private readonly Option<string> _versionOption = 
        Options.String(
            "--version",
            "The application version string (e.g., 1.0.0, 2.3.1-beta)",
            "-v");

    private readonly Option<string> _identifierOption = 
        Options.String(
            "--identifier",
            "The bundle identifier in reverse-DNS format (e.g., com.example.myapp)",
            "-i");

    private readonly Option<string> _principalClassOption = 
        Options.String(
            "--principal-class",
            "The name of the principal class (usually NSApplication or a custom subclass)",
            "-pc");

    private readonly Option<bool> _highResolutionCapableOption = 
        Options.Create<bool>(
            "--high-resolution-capable",
            "Whether the app supports Retina/high-DPI displays (true/false)",
            "-hrc");
    
    protected override AppBundlePackager CreatePackager() => new(Logger);
    
    protected override void ConfigureOptions(Command command) {
        command.Add(_appNameOption);
        command.Add(_displayNameOption);
        command.Add(_iconOption);
        command.Add(_versionOption);
        command.Add(_identifierOption);
        command.Add(_principalClassOption);
        command.Add(_highResolutionCapableOption);
    }
    
    protected override AppBundlePackageSettings ConfigureSettings(AppBundlePackageSettings settings, ParseResult result) => settings
        .SetAppName(result.GetValue(_appNameOption))
        .SetDisplayName(result.GetValue(_displayNameOption))
        .SetIcon(result.GetValue(_iconOption))
        .SetVersion(result.GetValue(_versionOption))
        .SetIdentifier(result.GetValue(_identifierOption))
        .SetPrincipalClass(result.GetValue(_principalClassOption))
        .SetHighResolutionCapable(result.GetValue(_highResolutionCapableOption));
}