using NyaFs.Filesystem.SquashFs;

namespace Hachimi.Packaging.AppImage;

public static class SquashFsExtension {
    private const uint UID = 0;
    private const uint GID = 0;
    private const uint DIR_MODE = 0x755;
    private const uint FILE_MODE = 0x644;
    private const uint EXEC_MODE = 0x755;
    
    internal static void AddDirectory(this SquashFsBuilder builder, string directory) {
        if (!Directory.Exists(directory))
            throw new DirectoryNotFoundException(directory);

        AddDirectoryRecursive(builder, directory, "/", UID, GID);
    }

    internal static void Build(this SquashFsBuilder builder, string outputFile) {
        var result = builder.GetFilesystemImage();
        File.WriteAllBytes(outputFile, result);
    }
    
    private static void AddDirectoryRecursive(SquashFsBuilder builder, string sourcePath, string destPath, uint uid, uint gid) {
        builder.Directory(destPath, uid, gid, DIR_MODE);

        foreach (var subDir in Directory.GetDirectories(sourcePath)) {
            var name = Path.GetFileName(subDir);
            AddDirectoryRecursive(builder, subDir, $"{destPath}/{name}", uid, gid);
        }

        foreach (var file in Directory.GetFiles(sourcePath)) {
            var name = Path.GetFileName(file);
            var content = File.ReadAllBytes(file);
            var mode = Path.GetExtension(file) == string.Empty 
                ? EXEC_MODE 
                : FILE_MODE;
            
            builder.File($"{destPath}/{name}", content, uid, gid, mode);
        }
    }
}