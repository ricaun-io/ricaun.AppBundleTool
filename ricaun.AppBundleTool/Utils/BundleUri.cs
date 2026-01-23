using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ricaun.AppBundleTool.Utils
{
    /// <summary>
    /// Represents a URI to a bundle file that can be downloaded from a remote location or accessed locally.
    /// </summary>
    public class BundleUri : IDisposable
    {
        /// <summary>
        /// Gets the URI of the bundle.
        /// </summary>
        public Uri Uri { get; }

        /// <summary>
        /// Gets the name of the bundle file without version information.
        /// </summary>
        public string BundleName { get; }

        /// <summary>
        /// The authentication token used for HTTP requests.
        /// </summary>
        private string authentication;

        /// <summary>
        /// The HTTP client used for downloading remote bundles.
        /// </summary>
        private HttpClient client;

        /// <summary>
        /// The HTTP response message from the initial request.
        /// </summary>
        private HttpResponseMessage response;

        /// <summary>
        /// Initializes a new instance of the <see cref="BundleUri"/> class with a string URI.
        /// </summary>
        /// <param name="uri">The URI string of the bundle.</param>
        /// <param name="authentication">Optional authentication token for remote downloads.</param>
        public BundleUri(string uri, string authentication = null)
            : this(new Uri(uri, UriKind.RelativeOrAbsolute), authentication)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BundleUri"/> class with a <see cref="Uri"/> object.
        /// </summary>
        /// <param name="uri">The URI of the bundle.</param>
        /// <param name="authentication">Optional authentication token for remote downloads.</param>
        public BundleUri(Uri uri, string authentication = null)
        {
            this.Uri = uri;
            this.authentication = authentication;
            this.BundleName = GetBundleName();
        }

        /// <summary>
        /// Gets the bundle name from the URI, removing version information.
        /// </summary>
        /// <returns>The bundle file name without version information.</returns>
        private string GetBundleName()
        {
            var bundleName = string.Empty;
            if (Uri.IsAbsoluteUri)
            {
                bundleName = Uri.IsFile ? Path.GetFileName(Uri.LocalPath) : ClientGetBundleNameAsync().GetAwaiter().GetResult();
            }
            else
            {
                bundleName = Path.GetFileName(Uri.OriginalString);
            }
            return NameAndVersionBundleUtils.RemoveVersionBundle(bundleName);
        }
        /// <summary>
        /// Validates whether the bundle has a valid format (*.bundle.zip).
        /// </summary>
        /// <returns><c>true</c> if the bundle name is valid; otherwise, <c>false</c>.</returns>
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(BundleName))
                return false;

            var extensionZip = Path.GetExtension(BundleName);
            var extensionBundle = Path.GetExtension(Path.GetFileNameWithoutExtension(BundleName));

            return extensionZip == ".zip" && extensionBundle == ".bundle";
        }

        /// <summary>
        /// Downloads the bundle to the specified destination folder asynchronously.
        /// </summary>
        /// <param name="destinationFolder">The destination folder where the bundle will be saved. If <c>null</c>, uses a temporary folder.</param>
        /// <param name="progress">Optional callback to report download progress. First parameter is bytes downloaded, second is total bytes (-1 if unknown).</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the full path to the downloaded bundle file.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the HttpClient is not initialized for remote downloads.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the specified local bundle file does not exist.</exception>
        public async Task<string> DownloadAsync(string destinationFolder, Action<long, long> progress = null)
        {
            if (Uri.IsAbsoluteUri && !Uri.IsFile)
            {
                return await ClientDownloadZipAsync(destinationFolder, progress);
            }

            var fullPath = Uri.IsAbsoluteUri ? Uri.LocalPath : Path.GetFullPath(Uri.OriginalString);
            if (File.Exists(fullPath))
            {
                var bundlePath = Path.Combine(destinationFolder, BundleName);
                File.Copy(fullPath, bundlePath, true);
                return bundlePath;
            }
            throw new FileNotFoundException("The specified local bundle file does not exist.", fullPath);
        }

        /// <summary>
        /// Asynchronously retrieves the bundle name from the remote server by making an HTTP request.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the bundle file name.</returns>
        private async Task<string> ClientGetBundleNameAsync()
        {
            client ??= new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "AppBundleTool");

            if (string.IsNullOrWhiteSpace(authentication) == false)
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authentication}");

            response = await client.GetAsync(Uri, HttpCompletionOption.ResponseHeadersRead);

            //var contentType = response.Content.Headers.ContentType?.MediaType;
            //Console.WriteLine(contentType);

            var finalUrl = response.RequestMessage?.RequestUri ?? Uri;
            var contentDisposition = response.Content.Headers.ContentDisposition;
            var bundleName = contentDisposition?.FileName?.Trim('"') ?? Path.GetFileName(finalUrl.LocalPath);

            return bundleName;
        }

        private async Task<string> ClientDownloadZipAsync(string destinationFolder, Action<long, long> progress)
        {
            if (client is null)
                await ClientGetBundleNameAsync();

            var bundlePath = Path.Combine(destinationFolder, BundleName);

            // Total size (might be null if server doesn't send Content-Length)
            var contentLengthHeader = response.Content.Headers.ContentLength;
            var contentLength = contentLengthHeader.HasValue ? contentLengthHeader.Value : -1L;

            await using var contentStream = await response.Content.ReadAsStreamAsync();
            await using var fileStream = new FileStream(bundlePath, FileMode.Create, FileAccess.Write, FileShare.None);

            try
            {
                var buffer = new byte[81920]; // 80 KB chunks
                long totalRead = 0;
                int read;
                progress?.Invoke(totalRead, contentLength);
                do
                {
                    read = await contentStream.ReadAsync(buffer.AsMemory(0, buffer.Length));
                    if (read == 0) break;

                    if (totalRead == 0)
                    {
                        bool isZip = IsZipFile(buffer, read);
                        if (!isZip)
                        {
                            throw new HttpRequestException("The downloaded file is not a valid ZIP file.");
                        }
                    }

                    await fileStream.WriteAsync(buffer.AsMemory(0, read));
                    totalRead += read;

                    progress?.Invoke(totalRead, contentLength);
                } while (true);
            }
            finally
            {
                response?.Dispose();
                client?.Dispose();
                client = null;
            }

            return bundlePath;
        }

        /// <summary>
        /// Validates whether the provided byte buffer starts with a valid ZIP file signature.
        /// </summary>
        /// <param name="buffer">The byte array containing the file data to check.</param>
        /// <param name="bytesRead">The number of bytes read into the buffer.</param>
        /// <returns>
        /// <c>true</c> if the buffer contains a valid ZIP file signature (PK + 0x03/0x05/0x07); otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// ZIP files start with the signature "PK" (0x50 0x4B), representing Phil Katz's initials.
        /// The third byte indicates the ZIP variant: 0x03 for standard ZIP, 0x05 for spanned ZIP, and 0x07 for split ZIP.
        /// </remarks>
        private bool IsZipFile(byte[] buffer, int bytesRead)
        {
            return
                bytesRead >= 4 &&
                buffer[0] == 0x50 && // P
                buffer[1] == 0x4B && // K
                (
                    buffer[2] == 0x03 ||
                    buffer[2] == 0x05 ||
                    buffer[2] == 0x07
                );
        }

        /// <summary>
        /// Releases the HTTP client and response resources.
        /// </summary>
        public void Dispose()
        {
            response?.Dispose();
            client?.Dispose();
        }
    }
}
