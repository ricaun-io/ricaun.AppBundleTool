using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ricaun.AppBundleTool.Utils
{
    /// <summary>
    /// Provides utility methods for downloading files and managing temporary folders.
    /// </summary>
    public static class BundleDownloadUtils
    {
        /// <summary>
        /// The name of the temporary folder used for downloads.
        /// </summary>
        public const string TempFolder = "ricaun.AppBundleTool";
        private const double CACHE_TOTAL_MINUTES = 1 / 60;

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
        /// <param name="cacheTotalMinutes">The number of minutes to cache the downloaded file before re-downloading.</param>
        /// <returns>The path to the downloaded file.</returns>
        public static async Task<string> DownloadAsync(string bundleUri, string authentication = null, double cacheTotalMinutes = CACHE_TOTAL_MINUTES)
        {
            var tempFolder = GetTempFolder();
            return await DownloadAsync(tempFolder, bundleUri, authentication, cacheTotalMinutes);
        }

        /// <summary>
        /// Downloads a file from the specified URI to the specified folder.
        /// </summary>
        /// <param name="tempFolder">The folder to download the file to.</param>
        /// <param name="bundleUri">The URI of the file to download.</param>
        /// <param name="authentication">Optional authentication token.</param>
        /// <param name="cacheTotalMinutes">The number of minutes to cache the downloaded file before re-downloading.</param>
        /// <returns>The path to the downloaded file.</returns>
        private static async Task<string> DownloadAsync(string tempFolder, string bundleUri, string authentication = null, double cacheTotalMinutes = CACHE_TOTAL_MINUTES)
        {
            if (!Uri.TryCreate(bundleUri, UriKind.RelativeOrAbsolute, out var uri))
            {
                throw new ArgumentException("Invalid bundle URI.", nameof(bundleUri));
            }
            
            if (!uri.IsAbsoluteUri) // Relative URI
            {
                var appBundleName = Path.GetFileName(uri.OriginalString);

                appBundleName = NameAndVersionBundleUtils.RemoveVersionBundle(appBundleName);

                if (Path.GetExtension(appBundleName) != ".zip")
                    throw new FileNotFoundException("The specified local bundle file does not have a .zip extension.", uri.OriginalString);

                var bundlePath = Path.Combine(tempFolder, appBundleName);
                var fullPath = Path.GetFullPath(uri.OriginalString);
                if (File.Exists(fullPath))
                {
                    File.Copy(fullPath, bundlePath, true);
                    return bundlePath;
                }
                throw new FileNotFoundException("The specified local bundle file does not exist.", fullPath);
            }
            else if (uri.IsFile) // File URI
            {
                var appBundleName = Path.GetFileName(uri.LocalPath);
                appBundleName = NameAndVersionBundleUtils.RemoveVersionBundle(appBundleName);
                if (Path.GetExtension(appBundleName) != ".zip")
                    throw new FileNotFoundException("The specified local bundle file does not have a .zip extension.", uri.OriginalString);
                var bundlePath = Path.Combine(tempFolder, appBundleName);
                var fullPath = uri.LocalPath;
                if (File.Exists(fullPath))
                {
                    File.Copy(fullPath, bundlePath, true);
                    return bundlePath;
                }
                throw new FileNotFoundException("The specified local bundle file does not exist.", fullPath);
            }
            else
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "AppBundleTool");

                if (string.IsNullOrWhiteSpace(authentication) == false)
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authentication}");

                var response = await client.GetAsync(bundleUri, HttpCompletionOption.ResponseHeadersRead);

                // Total size (might be null if server doesn't send Content-Length)
                var contentLengthHeader = response.Content.Headers.ContentLength;
                var contentLength = contentLengthHeader.HasValue ? contentLengthHeader.Value : -1L;

                var appBundleName = Path.GetFileName(uri.LocalPath);

                // Check Content-Disposition
                if (response.Content.Headers.ContentDisposition?.FileName is not null)
                {
                    appBundleName = response.Content.Headers.ContentDisposition.FileName.Trim('\"');
                }

                appBundleName = NameAndVersionBundleUtils.RemoveVersionBundle(appBundleName);

                if (Path.GetExtension(appBundleName) != ".zip")
                    throw new FileNotFoundException("The specified local bundle file does not have a .zip extension.", uri.OriginalString);

                var bundlePath = Path.Combine(tempFolder, appBundleName);

                //if (File.Exists(bundlePath))
                //{
                //    var lastTime = File.GetLastWriteTime(bundlePath);
                //    var now = DateTime.Now;
                //    var diff = now - lastTime;
                //    if (diff.TotalMinutes < cacheTotalMinutes)
                //    {
                //        return bundlePath;
                //    }
                //}

                await using var contentStream = await response.Content.ReadAsStreamAsync();
                await using var fileStream = new FileStream(bundlePath, FileMode.Create, FileAccess.Write, FileShare.None);

                var buffer = new byte[81920]; // 80 KB chunks
                long totalRead = 0;
                int read;
                DownloadProgress?.Invoke(totalRead, contentLength);
                do
                {
                    read = await contentStream.ReadAsync(buffer.AsMemory(0, buffer.Length));
                    if (read == 0) break;

                    await fileStream.WriteAsync(buffer.AsMemory(0, read));
                    totalRead += read;

                    DownloadProgress?.Invoke(totalRead, contentLength);
                } while (true);

                return bundlePath;
            }
        }

        /// <summary>
        /// An action that reports download progress. (total bytes downloaded, total bytes to download)
        /// </summary>
        public static Action<long, long> DownloadProgress { get; set; }
    }
}
