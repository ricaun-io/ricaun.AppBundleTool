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
        public AppBundleInfo(string pathBundle)
        {
            PathBundle = pathBundle;
            Name = Path.GetFileName(pathBundle);
            PathPackageContents = Path.Combine(pathBundle, ApplicationPluginsUtils.PackageContents);
            if (AppBundleFolderUtils.TyrGetAppBundleFolder(pathBundle, out AppBundleFolder appBundleFolder))
            {
                AppBundleFolder = appBundleFolder;
            }
            WriteAccess = AccessUtils.CheckWriteAccess(pathBundle);
        }
        public bool IsValid()
        {
            return File.Exists(PathPackageContents);
        }
        public override string ToString()
        {
            return string.Format("{0} \t{1} \t{2}", Name, AppBundleFolder, WriteAccess);
        }
    }
}
