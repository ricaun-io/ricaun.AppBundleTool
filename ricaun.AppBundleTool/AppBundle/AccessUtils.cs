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
        /// Checks if the application has write access to the specified path.
        /// </summary>
        /// <param name="path">The path to check for write access.</param>
        /// <returns>True if the application can write to the specified path; otherwise, false.</returns>
        public static bool CheckWriteAccess(string path)
        {
            try
            {
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
    }
}
