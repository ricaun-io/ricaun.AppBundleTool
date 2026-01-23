using NUnit.Framework;
using ricaun.AppBundleTool.Utils;
using System.Threading.Tasks;

namespace ricaun.AppBundleTool.Tests
{
    public class BundleDownloadTests
    {
        [TestCase("https://github.com/ricaun-io/RevitAddin.CommandLoader/releases/download/1.1.0/RevitAddin.CommandLoader.bundle.zip")]
        [TestCase("https://github.com/ricaun-io/RevitAddin.CommandLoader/releases/latest/download/RevitAddin.CommandLoader.bundle.zip")]
        [TestCase("https://github.com/ricaun-io/RevitAddin.CommandLoader/releases/latest/download/RevitAddin.CommandLoader.bundle.zip?test=123")]
        public async Task SampleDownloadTest(string bundleUri)
        {
            var path = await DownloadUtils.DownloadAsync(bundleUri);
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
    }
}