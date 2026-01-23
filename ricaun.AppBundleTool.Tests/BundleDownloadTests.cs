using NUnit.Framework;
using ricaun.AppBundleTool.Utils;
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
                System.Console.WriteLine(path);
                Assert.IsTrue(System.IO.File.Exists(path), "Downloaded file should exist.");
            }
            finally
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
        }

        [TestCase("RevitAddin.CommandLoader.1.1.0.bundle.zip", "RevitAddin.CommandLoader.bundle.zip")]
        [TestCase("RevitAddin.CommandLoader.bundle.zip", "RevitAddin.CommandLoader.bundle.zip")]
        public async Task Download_FileAbsolute_Test(string fileName, string expected)
        {
            var bundleUri = "https://github.com/ricaun-io/RevitAddin.CommandLoader/releases/latest/download/RevitAddin.CommandLoader.bundle.zip";
            var path = await BundleDownloadUtils.DownloadAsync(bundleUri);

            var tempDirectory = System.IO.Path.GetTempPath();
            var pathMove = System.IO.Path.Combine(tempDirectory, fileName);
            if (System.IO.File.Exists(pathMove))
                System.IO.File.Delete(pathMove);
            System.IO.File.Move(path, pathMove);

            try
            {
                System.Console.WriteLine(pathMove);
                Assert.IsFalse(System.IO.File.Exists(path), "Downloaded file should exist.");
                Assert.IsTrue(System.IO.File.Exists(pathMove), "Downloaded file should exist.");

                Assert.AreEqual(fileName, System.IO.Path.GetFileName(pathMove), "File name should match expected.");

                pathMove = await BundleDownloadUtils.DownloadAsync(pathMove);

                Assert.AreEqual(expected, System.IO.Path.GetFileName(pathMove), "File name should match expected.");
            }
            finally
            {
                if (System.IO.File.Exists(pathMove))
                    System.IO.File.Delete(pathMove);
            }
        }

        [TestCase("RevitAddin.CommandLoader.1.1.0.bundle.zip", "RevitAddin.CommandLoader.bundle.zip")]
        [TestCase("RevitAddin.CommandLoader.bundle.zip", "RevitAddin.CommandLoader.bundle.zip")]
        public async Task Download_FileRelative_Test(string fileName, string expected)
        {
            var bundleUri = "https://github.com/ricaun-io/RevitAddin.CommandLoader/releases/latest/download/RevitAddin.CommandLoader.bundle.zip";
            var path = await BundleDownloadUtils.DownloadAsync(bundleUri);

            var pathMove = System.IO.Path.GetFullPath(fileName);
            if (System.IO.File.Exists(pathMove))
                System.IO.File.Delete(pathMove);
            System.IO.File.Move(path, pathMove);

            try
            {
                System.Console.WriteLine(pathMove);
                Assert.IsFalse(System.IO.File.Exists(path), "Downloaded file should exist.");
                Assert.IsTrue(System.IO.File.Exists(pathMove), "Downloaded file should exist.");

                Assert.AreEqual(fileName, System.IO.Path.GetFileName(pathMove), "File name should match expected.");

                pathMove = await BundleDownloadUtils.DownloadAsync(pathMove);

                Assert.AreEqual(expected, System.IO.Path.GetFileName(pathMove), "File name should match expected.");
            }
            finally
            {
                if (System.IO.File.Exists(pathMove))
                    System.IO.File.Delete(pathMove);
            }
        }

        [TestCase(@"C:\path\to\file.zip")]
        [TestCase(@"C:\path\to\file2.zip")]
        [TestCase("file.zip")]
        [TestCase("file2.zip")]
        [TestCase("https://github.com/ricaun-io/RevitAddin.CommandLoader/releases/latest/download/RevitAddin.CommandLoader.bundle")]
        public async Task Download_Test_FileNotFoundException(string bundleUri)
        {
            Assert.ThrowsAsync<FileNotFoundException>(async () =>
            {
                await BundleDownloadUtils.DownloadAsync(bundleUri);
            });
        }
    }
}