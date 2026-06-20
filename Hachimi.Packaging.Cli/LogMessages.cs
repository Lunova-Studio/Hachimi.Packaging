using Microsoft.Extensions.Logging;
using ZLogger;

namespace Hachimi.Packaging.Cli;

public static partial class LogMessages {
    public static ILogger Logger { get; private set; }

    public static void Create() { 
        var factory = LoggerFactory.Create(builder => {
            builder.AddZLoggerConsole(x => {
                x.UsePlainTextFormatter(c => {
                    c.SetPrefixFormatter($"[{0}] [{1:short}]: ", (in template, in info) => template.Format(info.Timestamp, info.LogLevel));
                    c.SetExceptionFormatter((writer, ex) => Utf8StringInterpolation.Utf8String.Format(writer, $"{ex.Message}"));
                });
            });
        });
        
        Logger = factory.CreateLogger("Program");
    }
    
    [ZLoggerMessage(LogLevel.Information, "Test")]
    public static partial void LogUser(this ILogger logger);
}