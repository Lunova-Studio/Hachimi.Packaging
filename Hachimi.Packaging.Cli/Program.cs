using Hachimi.Packaging.AppImage;
using Hachimi.Packaging.Cli;
using Hachimi.Packaging.Extensions;
using NyaFs.Filesystem.SquashFs;
using NyaFs.Filesystem.SquashFs.Types;

LogMessages.Create();

var testPath = @"E:\Workspace\ICode\C#\nuke_test\artifacts";

#region MyRegion

var packager = new AppImagePackager(LogMessages.Logger);

await packager.PackAsync(x => {
    x.Source = Path.Combine(testPath, "linux-x64");
    x.OutputPath = Path.Combine(testPath, "ttt.AppImage");
    return x;
}, s => s
    .SetArchitecture("x86_64")
    .SetDisplayName("Nuke Test")
    .SetDescription("TTT")
    .SetAppName("nuke_test_avalonia")
    .SetIcon(@"E:\Workspace\ICode\C#\nuke_test\nuke_test_avalonia\Assets\icon.png"));

#endregion