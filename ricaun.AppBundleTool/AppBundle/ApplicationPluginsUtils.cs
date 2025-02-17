using System;
using System.Collections.Generic;
using System.IO;

namespace ricaun.AppBundleTool.AppBundle
{
    internal static class ApplicationPluginsUtils
    {
        private const string SearchBundle = "*.bundle";
        public const string PackageContents = "PackageContents.xml";

        private static string GetApplicationPlugins(Environment.SpecialFolder specialFolder)
        {
            return Path.Combine(Environment.GetFolderPath(specialFolder), "Autodesk", "ApplicationPlugins");
        }

        public static string[] GetAppBundles()
        {
            var specialFolders = AppBundleFolderUtils.GetSpecialFolders();
            return GetAppBundles(specialFolders);
        }

        public static string[] GetAppBundles(Environment.SpecialFolder[] specialFolders)
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
