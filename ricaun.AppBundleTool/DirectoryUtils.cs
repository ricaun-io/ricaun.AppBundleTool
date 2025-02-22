using System;
using System.IO;

namespace ricaun.AppBundleTool
{
    internal class DirectoryUtils
    {
        internal static void CopyFilesRecursively(string sourcePathFolder, string destinationPathFolder, bool overwrite = true)
        {
            if (!Directory.Exists(sourcePathFolder))
            {
                throw new ArgumentException("Source path does not exist or could not be found.");
            }
            if (!Directory.Exists(destinationPathFolder))
            {
                Directory.CreateDirectory(destinationPathFolder);
            }
            var files = Directory.GetFiles(sourcePathFolder);
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var destFile = Path.Combine(destinationPathFolder, fileName);
                Console.WriteLine($"Copy: {destFile}");
                File.Copy(file, destFile, overwrite);
            }
            var folders = Directory.GetDirectories(sourcePathFolder);
            foreach (var folder in folders)
            {
                var folderName = Path.GetFileName(folder);
                var destFolder = Path.Combine(destinationPathFolder, folderName);
                CopyFilesRecursively(folder, destFolder, overwrite);
            }
        }
    }
}