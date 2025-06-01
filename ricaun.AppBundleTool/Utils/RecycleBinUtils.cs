using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace ricaun.AppBundleTool.Utils
{
    /// <summary>
    /// Provides utility methods for sending files and directories to the Recycle Bin.
    /// </summary>
    public static class RecycleBinUtils
    {
        /// <summary>
        /// Sends the specified file to the Recycle Bin if it exists.
        /// </summary>
        /// <param name="path">The full path of the file to recycle.</param>
        /// <returns>
        /// <c>true</c> if the file existed and was sent to the Recycle Bin; otherwise, <c>false</c>.
        /// </returns>
        public static bool FileToRecycleBin(string path)
        {
            if (File.Exists(path))
            {
                FileSystem.DeleteFile(path,
                    UIOption.OnlyErrorDialogs,
                    RecycleOption.SendToRecycleBin);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Sends the specified directory to the Recycle Bin if it exists.
        /// </summary>
        /// <param name="path">The full path of the directory to recycle.</param>
        /// <returns>
        /// <c>true</c> if the directory existed and was sent to the Recycle Bin; otherwise, <c>false</c>.
        /// </returns>
        public static bool DirectoryToRecycleBin(string path)
        {
            if (Directory.Exists(path))
            {
                FileSystem.DeleteDirectory(path,
                    UIOption.OnlyErrorDialogs,
                    RecycleOption.SendToRecycleBin);

                return true;
            }
            return false;
        }
    }
}