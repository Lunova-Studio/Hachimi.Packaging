using System.CommandLine;
using Hachimi.Packaging.Extensions;
using Hachimi.Packaging.Portable;
using Microsoft.Extensions.Logging;

namespace Hachimi.Packaging.Cli.Commands;

public sealed class PortableCommand(ILogger logger) : PackagingCommandBase<PortablePackager, PortablePackageSettings>(logger) {
    protected override string Name => "portable";
    protected override string Description => "Portable software package compatible with Windows and Linux.";

    private readonly Option<string> _runtimeOption = 
        Options.String(
            "--runtime",
            "The .NET runtime identifier for the build (e.g., linux-x64, win-x64)",
            "-r",
            true);
    
    private readonly Option<string> _appNameOption = 
        Options.String(
            "--app-name",
            "The actual executable file name of the application (e.g., myapp.exe, myapp)",
            "-a");

    protected override PortablePackager CreatePackager() => new(Logger);

    protected override void ConfigureOptions(Command command) {
        command.Add(_runtimeOption);
        command.Add(_appNameOption);
    }
    
    protected override PortablePackageSettings ConfigureSettings(PortablePackageSettings settings, ParseResult result) => settings
        .SetAppName(result.GetValue(_appNameOption))
        .SetRuntime(result.GetValue(_runtimeOption));
}