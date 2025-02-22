using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ricaun.AppBundleTool
{
    /// <summary>
    /// Provides utility methods for downloading files and managing temporary folders.
    /// </summary>
    public static class DownloadUtils
    {
        /// <summary>
        /// The name of the temporary folder used for downloads.
        /// </summary>
        public const string TempFolder = "ricaun.AppBundleTool";

        /// <summary>
        /// Gets the path to the temporary folder, creating it if it does not exist.
        /// </summary>
        /// <returns>The path to the temporary folder.</returns>
        public static string GetTempFolder()
        {
            var tempFolder = Path.Combine(Path.GetTempPath(), TempFolder);

            if (!Directory.Exists(tempFolder))
                Directory.CreateDirectory(tempFolder);

            return tempFolder;
        }

        /// <summary>
        /// Deletes the temporary folder and its contents.
        /// </summary>
        public static void DeleteTempFolder()
        {
            var tempFolder = GetTempFolder();
            if (Directory.Exists(tempFolder))
            {
                try
                {
                    Directory.Delete(tempFolder, true);
                }
                catch { }
            }
        }

        /// <summary>
        /// Downloads a file from the specified URI to the temporary folder.
        /// </summary>
        /// <param name="bundleUri">The URI of the file to download.</param>
        /// <param name="authentication">Optional authentication token.</param>
        /// <returns>The path to the downloaded file.</returns>
        public static async Task<string> DownloadAsync(string bundleUri, string authentication = null)
        {
            var tempFolder = GetTempFolder();
            return await DownloadAsync(tempFolder, bundleUri, authentication);
        }

        /// <summary>
        /// Downloads a file from the specified URI to the specified folder.
        /// </summary>
        /// <param name="tempFolder">The folder to download the file to.</param>
        /// <param name="bundleUri">The URI of the file to download.</param>
        /// <param name="authentication">Optional authentication token.</param>
        /// <returns>The path to the downloaded file.</returns>
        public static async Task<string> DownloadAsync(string tempFolder, string bundleUri, string authentication = null)
        {
            var uri = new Uri(bundleUri);
            var appBundleName = Path.GetFileName(uri.LocalPath);

            if (Path.GetExtension(appBundleName) != ".zip")
                appBundleName += ".zip";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "AppBundleTool");

            if (string.IsNullOrWhiteSpace(authentication) == false)
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authentication}");

            var response = await client.GetAsync(bundleUri);
            var bundlePath = Path.Combine(tempFolder, appBundleName);

            using var fileStream = new FileStream(bundlePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await response.Content.CopyToAsync(fileStream);

            return bundlePath;
        }
    }
}
