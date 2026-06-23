using Hachimi.Packaging.AppImage;
using Hachimi.Packaging.Cli;
using Hachimi.Packaging.Extensions;
using NyaFs.Filesystem.SquashFs;
using NyaFs.Filesystem.SquashFs.Types;

LogMessages.Create();

var testPath = @"E:\Workspace\ICode\C#\nuke_test\artifacts";

// var tp = Path.Combine(testPath, "squashfs-root o");
//
// PackWithNyaFs(tp, Path.Combine(testPath, "app.squashfs"));
//
//
// static void PackWithNyaFs(string appDirPath, string outputFile)
// {
//     var builder = new SquashFsBuilder(SqCompressionType.Gzip);
//     
//     // 添加根目录
//     builder.Directory("/", 0, 0, 0x755);
//     
//     // 收集所有目录和文件
//     var entries = Directory.GetFileSystemEntries(appDirPath, "*", SearchOption.AllDirectories);
//     
//     foreach (var entry in entries)
//     {
//         string relPath = Path.GetRelativePath(appDirPath, entry).Replace('\\', '/');
//         string destPath = "/" + relPath;
//         
//         if (Directory.Exists(entry))
//         {
//             builder.Directory(destPath, 0, 0, 0x755);
//             Console.WriteLine($"📁 {destPath}");
//         }
//         else if (File.Exists(entry))
//         {
//             try
//             {
//                 var content = File.ReadAllBytes(entry);
//                 int mode = Path.GetExtension(entry) == "" ? 0x755 : 0x644;
//                 builder.File(destPath, content, 0, 0, (uint)mode);
//                 Console.WriteLine($"📄 {destPath}");
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"⚠️ 跳过 {destPath}: {ex.Message}");
//             }
//         }
//     }
//     
//     Console.WriteLine("⏳ 生成镜像...");
//     var image = builder.GetFilesystemImage();
//     File.WriteAllBytes(outputFile, image);
//     Console.WriteLine($"✅ 完成: {outputFile} ({image.Length / 1024.0:F2} KB)");
// }

#region MyRegion

var packager = new AppImagePackager(LogMessages.Logger);

await packager.PackAsync(x => {
    x.Source = Path.Combine(testPath, "squashfs-root o");
    x.OutputPath = Path.Combine(testPath, "ttt.AppImage");
    return x;
}, s => s
    .SetArchitecture("x86_64")
    .SetDisplayName("Nuke Test")
    .SetDescription("TTT")
    .SetAppName("nuke_test_avalonia")
    .SetRuntime("linux-x64"));

Console.ReadKey();


#endregion