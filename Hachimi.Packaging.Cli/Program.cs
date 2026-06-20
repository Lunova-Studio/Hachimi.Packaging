using System.Runtime.InteropServices;
using Hachimi.Packaging;
using Hachimi.Packaging.Cli;
using Hachimi.Packaging.Extensions;
using Hachimi.Packaging.Portable;

LogMessages.Create();

var testPath = @"E:\Workspace\ICode\C#\nuke_test\artifacts";
var packager = new PortablePackager(LogMessages.Logger);

await packager.PackAsync(x => {
    x.Source = Path.Combine(testPath, "win-x64");
    x.OutputPath = Path.Combine(testPath, "nuke_test_avalonia-win-x64-portable.zip");

    return x;
}, x => x
    .SetRuntime("win-x64"));