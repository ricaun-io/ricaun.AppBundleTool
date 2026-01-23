using NUnit.Framework;
using ricaun.AppBundleTool.Utils;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ricaun.AppBundleTool.Tests
{
    public class BundleDownloadTests
    {
        [TestCase("https://github.com/ricaun-io/RevitAddin.CommandLoader/releases/download/1.1.0/RevitAddin.CommandLoader.bundle.zip")]
        [TestCase("https://github.com/ricaun-io/RevitAddin.CommandLoader/releases/latest/download/RevitAddin.CommandLoader.bundle.zip")]
        [TestCase("https://github.com/ricaun-io/RevitAddin.CommandLoader/releases/latest/download/RevitAddin.CommandLoader.bundle.zip?test=123")]
        public async Task Download_Test(string bundleUri)
        {
            var path = await BundleDownloadUtils.DownloadAsync(bundleUri);
            try
            {
                Console.WriteLine(path);
                Assert.IsTrue(File.Exists(path), "Downloaded file should exist.");
            }
            finally
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        [TestCase("RevitAddin.CommandLoader.1.1.0.bundle.zip", "RevitAddin.CommandLoader.bundle.zip")]
        [TestCase("RevitAddin.CommandLoader.bundle.zip", "RevitAddin.CommandLoader.bundle.zip")]
        public async Task Download_FileAbsolute_Test(string fileName, string expected)
        {
            var bundleUri = "https://github.com/ricaun-io/RevitAddin.CommandLoader/releases/latest/download/RevitAddin.CommandLoader.bundle.zip";
            var path = await BundleDownloadUtils.DownloadAsync(bundleUri);

            var tempDirectory = Path.GetTempPath();
            var pathMove = Path.Combine(tempDirectory, fileName);
            if (File.Exists(pathMove))
                File.Delete(pathMove);
            File.Move(path, pathMove);

            try
            {
                Console.WriteLine(pathMove);
                Assert.IsFalse(File.Exists(path), "Downloaded file should exist.");
                Assert.IsTrue(File.Exists(pathMove), "Downloaded file should exist.");

                Assert.AreEqual(fileName, Path.GetFileName(pathMove), "File name should match expected.");

                pathMove = await BundleDownloadUtils.DownloadAsync(pathMove);

                Assert.AreEqual(expected, Path.GetFileName(pathMove), "File name should match expected.");
            }
            finally
            {
                if (File.Exists(pathMove))
                    File.Delete(pathMove);
            }
        }

        [TestCase("RevitAddin.CommandLoader.1.1.0.bundle.zip", "RevitAddin.CommandLoader.bundle.zip")]
        [TestCase("RevitAddin.CommandLoader.bundle.zip", "RevitAddin.CommandLoader.bundle.zip")]
        public async Task Download_FileRelative_Test(string fileName, string expected)
        {
            var bundleUri = "https://github.com/ricaun-io/RevitAddin.CommandLoader/releases/latest/download/RevitAddin.CommandLoader.bundle.zip";
            var path = await BundleDownloadUtils.DownloadAsync(bundleUri);

            var pathMove = Path.GetFullPath(fileName);
            if (File.Exists(pathMove))
                File.Delete(pathMove);
            File.Move(path, pathMove);

            try
            {
                Console.WriteLine(pathMove);
                Assert.IsFalse(File.Exists(path), "Downloaded file should exist.");
                Assert.IsTrue(File.Exists(pathMove), "Downloaded file should exist.");

                Assert.AreEqual(fileName, Path.GetFileName(pathMove), "File name should match expected.");

                pathMove = await BundleDownloadUtils.DownloadAsync(pathMove);

                Assert.AreEqual(expected, Path.GetFileName(pathMove), "File name should match expected.");
            }
            finally
            {
                if (File.Exists(pathMove))
                    File.Delete(pathMove);
            }
        }

        [TestCase(@"C:\path\to\file.zip")]
        [TestCase(@"C:\path\to\file2.zip")]
        [TestCase("file.zip")]
        [TestCase("file2.zip")]
        [TestCase("https://github.com/ricaun-io/RevitAddin.CommandLoader/releases/latest/download/RevitAddin.CommandLoader.bundle")]
        public async Task Download_Test_Exception(string bundleUri)
        {
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await BundleDownloadUtils.DownloadAsync(bundleUri);
            });
        }

        [TestCase("https://github.com/ricaun-io/RevitAddin.CommandLoader.bundle.zip")]
        [TestCase("https://github.com/ricaun-io/AnyUrl.bundle.zip")]
        public async Task Download_Test_Exception_FileNotValidZip(string bundleUri)
        {
            Assert.ThrowsAsync<System.Net.Http.HttpRequestException>(async () =>
            {
                await BundleDownloadUtils.DownloadAsync(bundleUri);
            });
        }
    }
}