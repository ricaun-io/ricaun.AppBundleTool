using System;
using System.IO;
using System.Threading.Tasks;

namespace ricaun.AppBundleTool.Utils
{
    /// <summary>
    /// Provides utility methods for downloading bundle files.
    /// </summary>
    public static class BundleDownloadUtils
    {
        /// <summary>
        /// The name of the temporary folder used for storing downloaded bundles.
        /// </summary>
        private const string TempFolder = "ricaun.AppBundleTool";

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
        /// Downloads a bundle file asynchronously from the specified URI.
        /// </summary>
        /// <param name="bundleUri">The URI of the bundle file to download. Must end with '.bundle.zip'.</param>
        /// <param name="authentication">The authentication token or credentials (optional).</param>
        /// <returns>A task that represents the asynchronous download operation. The task result contains the local file path of the downloaded bundle.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the bundle URI is not valid or does not end with '.bundle.zip'.</exception>
        public static async Task<string> DownloadAsync(string bundleUri, string authentication = null)
        {
            using (var bundle = new BundleUri(bundleUri))
            {
                try
                {
                    if (!bundle.IsValid())
                    {
                        throw new InvalidOperationException("The bundle URI is not valid, does not end with '.bundle.zip'");
                    }
                    return await bundle.DownloadAsync(GetTempFolder(), progress: DownloadProgress);
                }
                finally
                {
                    DownloadProgress = null;
                }
            }
        }
        /// <summary>
        /// Gets or sets an action that reports download progress.
        /// </summary>
        /// <value>
        /// An action with two parameters: the total bytes downloaded and the total bytes to download.
        /// </value>
        public static Action<long, long> DownloadProgress { get; set; }
    }
}
