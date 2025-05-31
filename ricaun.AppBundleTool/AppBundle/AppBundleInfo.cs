using ricaun.AppBundleTool.PackageContents;
using System.IO;
using System.Linq;

namespace ricaun.AppBundleTool.AppBundle
{
    internal class AppBundleInfo
    {
        public string Name { get; set; }
        public string PathBundle { get; set; }
        public string PathPackageContents { get; set; }
        public AppBundleFolder AppBundleFolder { get; set; }
        public AppBundleAccess AppBundleAccess { get; set; }
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
            WriteAccess = AccessUtils.CheckReadAndWriteAccess(PathPackageContents);
            AppBundleAccess = WriteAccess ? AppBundleAccess.Allow : AppBundleAccess.Admin;
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
            return string.Format("[{0}] \t{1} \t{2}", AppBundleFolder, ApplicationPackage?.Name ?? Name, writeAccess);
        }

        /// <summary>
        /// Finds the application bundle in the specified folder.
        /// </summary>
        /// <param name="appBundleFolder">The folder to search for the application bundle.</param>
        /// <returns>
        /// An <see cref="AppBundleInfo"/> object representing the application bundle found in the specified folder,
        /// or a new <see cref="AppBundleInfo"/> object if no application bundle is found.
        /// </returns>
        public static AppBundleInfo FindAppBundle(string appBundleFolder)
        {
            var packageContentsFile = Directory.GetFiles(appBundleFolder, AppBundleUtils.PackageContents, SearchOption.AllDirectories)
                .FirstOrDefault();

            if (packageContentsFile == null)
                return new AppBundleInfo(appBundleFolder);

            return new AppBundleInfo(Path.GetDirectoryName(packageContentsFile));
        }
    }
}