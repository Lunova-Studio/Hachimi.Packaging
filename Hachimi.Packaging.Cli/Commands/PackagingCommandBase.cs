using System.CommandLine;
using Hachimi.Packaging.Extensions;
using Hachimi.Packaging.Interfaces;
using Microsoft.Extensions.Logging;

namespace Hachimi.Packaging.Cli.Commands;

public abstract class PackagingCommandBase<TPackager, TSettings> 
    where TPackager : IPackager<TSettings>
    where TSettings : PackageSettings {
    protected abstract string Name { get; }
    
    protected abstract string Description { get; }

    protected ILogger Logger { get; }
    
    protected PackagingCommandBase(ILogger logger) => Logger = logger;
    
    protected abstract TPackager CreatePackager();
    
    protected abstract void ConfigureOptions(Command command);
    
    protected abstract TSettings ConfigureSettings(TSettings settings, ParseResult result);
    
    protected virtual PackagingContext ConfigureContext(PackagingContext context, ParseResult result) => context
        .SetOutput(result.GetValue(Options.OutputOption))
        .SetSource(result.GetValue(Options.SourceOption));
    
    public Command Build() {
        var command = new Command(Name, Description) {
            Options.SourceOption,
            Options.OutputOption
        };

        ConfigureOptions(command);
        command.SetAction(async result => {
            var packager = CreatePackager();
            
            await packager.PackAsync(
                context => ConfigureContext(context, result),
                settings => ConfigureSettings(settings, result)
            );
        });
        
        return command;
    }
}