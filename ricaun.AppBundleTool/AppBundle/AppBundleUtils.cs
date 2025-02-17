using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ricaun.AppBundleTool.AppBundle
{
    internal static class AppBundleUtils
    {
        private const string SearchBundle = "*.bundle";
        public const string PackageContents = "PackageContents.xml";

        public static string GetApplicationPlugins(this Environment.SpecialFolder specialFolder)
        {
            return Path.Combine(Environment.GetFolderPath(specialFolder), "Autodesk", "ApplicationPlugins");
        }

        public static AppBundleInfo FindAppBundle(string appBundleName)
        {
            var bundles = GetAppBundleFolders();
            return bundles.Select(e => new AppBundleInfo(e)).FirstOrDefault(e => e.Name == appBundleName);
        }

        /// <summary>
        /// Retrieves all valid AppBundles.
        /// </summary>
        /// <returns>An array of <see cref="AppBundleInfo"/> representing the valid AppBundles.</returns>
        public static AppBundleInfo[] GetAppBundles()
        {
            var bundles = GetAppBundleFolders();
            return bundles.Select(e => new AppBundleInfo(e))
                .Where(e => e.IsValid())
                .ToArray();
        }

        public static string[] GetAppBundleFolders()
        {
            var specialFolders = AppBundleFolderUtils.GetSpecialFolders();
            return GetAppBundleFolders(specialFolders);
        }

        public static string[] GetAppBundleFolders(Environment.SpecialFolder[] specialFolders)
        {
            var bundles = new List<string>();

            foreach (var specialFolder in specialFolders)
            {
                var applicationPluginsFolder = GetApplicationPlugins(specialFolder);
                if (Directory.Exists(applicationPluginsFolder))
                    bundles.AddRange(Directory.GetDirectories(applicationPluginsFolder, SearchBundle));

            }
            return bundles.ToArray();
        }
    }
}
