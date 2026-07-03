using System.CommandLine;
using Hachimi.Packaging.Cli.Commands;
using Microsoft.Extensions.Logging;
using ZLogger;

var factory = LoggerFactory.Create(builder => {
    builder.AddZLoggerConsole(x => {
        x.UsePlainTextFormatter(c => {
            c.SetPrefixFormatter($"[{0}] [{1:short}]: ", (in template, in info) => template.Format(info.Timestamp, info.LogLevel));
            c.SetExceptionFormatter((writer, ex) => Utf8StringInterpolation.Utf8String.Format(writer, $"{ex.Message}"));
        });
    });
});
        
var logger = factory.CreateLogger("Program");

var command = new RootCommand("Hachimi.Packaging") {
    new Command("hachimi", "miaomiaomiao?") {
        new PortableCommand(logger).Build(),
        new AppImageCommand(logger).Build(),
        new AppBundleCommand(logger).Build()
    }
};

return await command.Parse(args).InvokeAsync();