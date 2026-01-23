using NUnit.Framework;
using System;
using System.IO;

namespace ricaun.AppBundleTool.Tests
{
    public class UriTests
    {
        [TestCase("https://github.com/ricaun-io/RevitAddin.CommandLoader/releases/download/1.1.0/RevitAddin.CommandLoader.bundle.zip")]
        [TestCase("https://github.com/ricaun-io/RevitAddin.CommandLoader/releases/download/1.1.0/RevitAddin.CommandLoader.bundle.zip?test=123")]
        public void UriAbsoluteTest(string bundleUri)
        {
            var uri = new Uri(bundleUri, UriKind.Absolute);
            Assert.AreEqual("github.com", uri.Host, "Host should be github.com");
            Assert.IsTrue(uri.IsAbsoluteUri, "URI should be absolute.");
            Assert.IsFalse(uri.IsFile, "URI should not be a file.");
            Assert.IsTrue(uri.AbsolutePath.EndsWith(".zip"), "Path should end with .zip");
        }

        [TestCase(@"C:\path\to\file.zip")]
        [TestCase(@"C:\path\to\file2.zip")]
        public void UriAbsoluteFileTest(string bundleUri)
        {
            var uri = new Uri(bundleUri, UriKind.Absolute);
            Assert.IsTrue(uri.IsAbsoluteUri, "URI should be absolute.");
            Assert.IsTrue(uri.IsFile, "URI should be a file.");
            Assert.IsTrue(uri.AbsolutePath.EndsWith(".zip"), "Path should end with .zip");
        }

        [TestCase("file.zip")]
        [TestCase("file2.zip")]
        public void UriRelativeTest(string bundleUri)
        {
            var uri = new Uri(bundleUri, UriKind.Relative);
            Assert.IsFalse(uri.IsAbsoluteUri, "URI should be relative.");
            Assert.IsTrue(uri.OriginalString.EndsWith(".zip"), "Path should end with .zip");
            var fullPath = Path.GetFullPath(uri.OriginalString);
            Assert.IsTrue(fullPath.EndsWith(".zip"), "Full path should end with .zip");
        }
    }
}