using System.CommandLine;
using System.Runtime.InteropServices;
using Hachimi.Packaging.AppImage;
using Hachimi.Packaging.Extensions;
using Microsoft.Extensions.Logging;

namespace Hachimi.Packaging.Cli.Commands;

public sealed class AppImageCommand(ILogger logger) : PackagingCommandBase<AppImagePackager, AppImagePackageSettings>(logger) {
    protected override string Name => "appimage";
    protected override string Description => "Create an AppImage bundle from the source directory (e.g., myapp.AppImage)";

    private readonly Option<string> _appNameOption = 
        Options.String(
            "--app-name",
            "The actual executable file name of the application (e.g., myapp.exe, myapp)",
            "-a",
            true);

    private readonly Option<Architecture> _archOption = 
        Options.Create<Architecture>(
            "--arch",
            "The target system architecture",
            isRequired: true);

    private readonly Option<string> _displayNameOption = 
        Options.String(
            "--display-name",
            "The display name shown in the application launcher (e.g., MyApp)",
            "-dn");

    private readonly Option<string> _iconOption = 
        Options.String(
            "--icon",
            "The path to the icon file for the AppImage (e.g. /usr/share/icons/icon.png)",
            "-i");

    private readonly Option<string> _descriptionOption = 
        Options.String(
            "--description",
            "A brief description of the application (shown in the launcher)",
            "-d");

    private readonly Option<bool> _terminalOption = 
        Options.Create<bool>(
            "--terminal",
            "Whether the application should run in a terminal window (true/false)",
            "-t");

    private readonly Option<string[]> _categoriesOption = 
        Options.Create<string[]>(
            "--category",
            "Desktop categories for the application (e.g., Utility, Development), multiple allowed",
            "-c");

    private readonly Option<string[]> _keywordsOption = 
        Options.Create<string[]>(
            "--keyword",
            "Search keywords for the application (e.g., text, editor), multiple allowed",
            "-k");
    
    protected override AppImagePackager CreatePackager() => new(Logger);
    
    protected override void ConfigureOptions(Command command) {
        command.Add(_appNameOption);
        command.Add(_displayNameOption);
        command.Add(_archOption);
        command.Add(_iconOption);
        command.Add(_descriptionOption);
        command.Add(_categoriesOption);
        command.Add(_keywordsOption);
        command.Add(_terminalOption);
    }
    
    protected override AppImagePackageSettings ConfigureSettings(AppImagePackageSettings settings, ParseResult result) => settings
        .SetArchitecture(result.GetValue(_archOption))
        .SetDisplayName(result.GetValue(_displayNameOption))
        .SetIcon(result.GetValue(_iconOption))
        .SetDescription(result.GetValue(_descriptionOption))
        .SetAppName(result.GetValue(_appNameOption))
        .SetIsTerminal(result.GetValue(_terminalOption))
        .SetKeywords(result.GetValue(_keywordsOption))
        .SetCategories(result.GetValue(_categoriesOption));
}