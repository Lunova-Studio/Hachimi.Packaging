using System.CommandLine;

namespace Hachimi.Packaging.Cli;

internal static class Options {
    public static readonly Option<string> SourceOption = 
        String(
            "--source", 
            "The source directory containing the published application files (e.g., ./publish)", 
            "-s", 
            true);

    public static readonly Option<string> OutputOption =
        String(
            "--output",
            "The output package file path (e.g., package.zip or /output/package.zip)",
            "-o",
            true);
    
    public static Option<T> Create<T>(string name, string description, bool isRequired = false) {
        return new Option<T>(name) {
            Required = isRequired,
            Description = description
        };
    }
    
    public static Option<T> Create<T>(string name, string description, string alias, bool isRequired = false) {
        return new Option<T>(name, alias) {
            Required = isRequired,
            Description = description
        };
    }
        
    public static Option<string> String(string name, string description, bool isRequired = false) {
        return new Option<string>(name) {
            Required = isRequired,
            Description = description
        };
    }
    
    public static Option<string> String(string name, string description, string alias, bool isRequired = false) {
        return new Option<string>(name, alias) {
            Required = isRequired,
            Description = description
        };
    }
}