using NUnit.Framework;
using ricaun.AppBundleTool.Utils;

namespace ricaun.AppBundleTool.Tests
{
    public class NameAndVersionTests
    {
        [TestCase("MyLib.1.2.3.bundle", "MyLib", "1.2.3")]
        [TestCase("Plugin.3.4.5-beta.bundle", "Plugin", "3.4.5-beta")]
        [TestCase("My.Lib.Core.2.0.0.bundle", "My.Lib.Core", "2.0.0")]
        [TestCase("ToolName.10.0.1-alpha.bundle", "ToolName", "10.0.1-alpha")]
        [TestCase("Library.1.0.0.1.bundle", "Library", "1.0.0.1")] // supports 4 numeric parts
        public void TryGetNameAndVersionBundle_ValidNames_ShouldReturnTrue(string fileName, string expectedName, string expectedVersion)
        {
            // Act
            var result = NameAndVersionBundleUtils.TryGetNameAndVersionBundle(fileName, out var name, out var version);

            // Assert
            Assert.IsTrue(result, $"Expected success for '{fileName}'");
            Assert.AreEqual(expectedName, name);
            Assert.AreEqual(expectedVersion, version);
        }

        [TestCase("InvalidFile.bundle")]
        [TestCase("AnotherFile.txt")]
        [TestCase("Package.1.2.bundle")] // only 2 numeric parts
        [TestCase("Package.1.2.3.bundle.zip")] // not ending with .bundle
        [TestCase("Package.1.2.3")] // missing .bundle
        [TestCase("Package1.2.3.bundle")] // missing dot before version
        public void TryGetNameAndVersionBundle_InvalidNames_ShouldReturnFalse(string fileName)
        {
            // Act
            var result = NameAndVersionBundleUtils.TryGetNameAndVersionBundle(fileName, out var name, out var version);

            // Assert
            Assert.IsFalse(result, $"Expected failure for '{fileName}'");
            Assert.IsNull(name);
            Assert.IsNull(version);
        }

        [TestCase("MyLib.1.2.3.bundle", "MyLib.bundle")]
        [TestCase("Plugin.3.4.5-beta.bundle", "Plugin.bundle")]
        [TestCase("My.Lib.Core.2.0.0.bundle", "My.Lib.Core.bundle")]
        [TestCase("ToolName.10.0.1-alpha.bundle", "ToolName.bundle")]
        [TestCase("Library.1.0.0.1.bundle", "Library.bundle")] // supports 4 numeric parts
        [TestCase("MyLib.bundle", "MyLib.bundle")]
        [TestCase("MyLib.bundle.zip", "MyLib.bundle.zip")]
        [TestCase("MyLib.1.2.3.bundle.zip", "MyLib.bundle.zip")]
        public void RemoveVersionBundle_ValidNames(string fileName, string expectedName)
        {
            // Act
            var result = NameAndVersionBundleUtils.RemoveVersionBundle(fileName);

            // Assert
            Assert.AreEqual(expectedName, result);
        }

        [TestCase("MyLib 1.2.3.bundle.zip")]
        [TestCase("MyLib.test")]
        public void RemoveVersionBundle_NotValidNames(string fileName)
        {
            // Act
            var result = NameAndVersionBundleUtils.RemoveVersionBundle(fileName);

            // Assert
            var expectedName = fileName;
            Assert.AreEqual(expectedName, result);
        }
    }
}