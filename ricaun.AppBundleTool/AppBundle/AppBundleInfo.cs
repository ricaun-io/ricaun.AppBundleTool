using ricaun.AppBundleTool.PackageContents;
using System.IO;

namespace ricaun.AppBundleTool.AppBundle
{
    internal class AppBundleInfo
    {
        public string Name { get; set; }
        public string PathBundle { get; set; }
        public string PathPackageContents { get; set; }
        public AppBundleFolder AppBundleFolder { get; set; }
        public bool WriteAccess { get; set; }
        public ApplicationPackage ApplicationPackage { get; set; }
        public AppBundleInfo(string pathBundle)
        {
            PathBundle = pathBundle;
            Name = Path.GetFileName(pathBundle);
            PathPackageContents = Path.Combine(pathBundle, AppBundleUtils.PackageContents);
            if (AppBundleFolderUtils.TyrGetAppBundleFolder(pathBundle, out AppBundleFolder appBundleFolder))
            {
                AppBundleFolder = appBundleFolder;
            }
            WriteAccess = AccessUtils.CheckWriteAccess(pathBundle);
            ApplicationPackage = ApplicationPackage.Parse(PathPackageContents);
            ApplicationPackage.Show();
        }
        public bool IsValid()
        {
            return File.Exists(PathPackageContents);
        }

        private string GetWriteAccessMessage()
        {
            return WriteAccess ? "" : "(Administrator Permission Required)";
        }

        public override string ToString()
        {
            var writeAccess = GetWriteAccessMessage();
            return string.Format("[{0}] \t{1} \t{2}", AppBundleFolder, Name, writeAccess);
        }
    }
}