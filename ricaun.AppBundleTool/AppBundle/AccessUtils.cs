using System;
using System.IO;

namespace ricaun.AppBundleTool.AppBundle
{
    /// <summary>
    /// Provides utility methods for checking access permissions.
    /// </summary>
    public static class AccessUtils
    {
        /// <summary>
        /// Checks if the application has both read and write access to the specified file path.
        /// </summary>
        /// <param name="filePath">The path of the file to check for read and write access.</param>
        /// <returns>True if the application can read from and write to the specified file path; otherwise, false.</returns>
        public static bool CheckReadAndWriteAccess(string filePath)
        {
            return CheckFileReadAccess(filePath) && CheckWriteAccess(filePath);
        }

        /// <summary>
        /// Checks if the application has write access to the specified path.
        /// </summary>
        /// <param name="path">The path to check for write access.</param>
        /// <returns>True if the application can write to the specified path; otherwise, false.</returns>
        public static bool CheckWriteAccess(string path)
        {
            try
            {
                if (File.Exists(path))
                    path = Path.GetDirectoryName(path);
                else if (!Directory.Exists(path))
                    path = Path.GetDirectoryName(path);

                var fileName = Path.Combine(path, Path.GetRandomFileName());
                File.Create(fileName).Close();
                File.Delete(fileName);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the application has read access to the specified file.
        /// </summary>
        /// <param name="filePath">The path of the file to check for read access.</param>
        /// <returns>True if the application can read the specified file; otherwise, false.</returns>
        public static bool CheckFileReadAccess(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    // FileStream throws UnauthorizedAccessException if the file is not accessible for reading
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Open)) { }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
