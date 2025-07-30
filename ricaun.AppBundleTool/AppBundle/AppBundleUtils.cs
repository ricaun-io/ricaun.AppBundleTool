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

        /// <summary>
        /// Finds the <see cref="AppBundleInfo"/> by the specified application bundle name.
        /// </summary>
        /// <param name="appBundleName">
        /// The name of the application bundle to search for. This can be either the application package name or the bundle folder name.
        /// </param>
        /// <returns>
        /// An <see cref="AppBundleInfo"/> object representing the application bundle with the specified name,
        /// or <c>null</c> if no matching bundle is found or <paramref name="appBundleName"/> is null or whitespace.
        /// </returns>
        public static AppBundleInfo FindAppBundle(string appBundleName)
        {
            return FindAppBundleByAppName(appBundleName) ?? FindAppBundleByBundleName(appBundleName);
        }

        /// <summary>
        /// Finds the <see cref="AppBundleInfo"/> by the application package name.
        /// </summary>
        /// <param name="appBundleName">
        /// The name of the application package to search for.
        /// </param>
        /// <returns>
        /// An <see cref="AppBundleInfo"/> object representing the application bundle with the specified application package name,
        /// or <c>null</c> if no matching bundle is found or <paramref name="appBundleName"/> is null or whitespace.
        /// </returns>
        public static AppBundleInfo FindAppBundleByAppName(string appBundleName)
        {
            if (string.IsNullOrWhiteSpace(appBundleName))
                return null;

            var bundles = GetAppBundleFolders();
            return bundles.Select(e => new AppBundleInfo(e))
                .FirstOrDefault(e => appBundleName.Equals(e.ApplicationPackage?.Name, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Finds the <see cref="AppBundleInfo"/> by the bundle folder name.
        /// </summary>
        /// <param name="bundleName">
        /// The name of the bundle folder to search for.
        /// </param>
        /// <returns>
        /// An <see cref="AppBundleInfo"/> object representing the application bundle with the specified bundle folder name,
        /// or <c>null</c> if no matching bundle is found or <paramref name="bundleName"/> is null or whitespace.
        /// </returns>
        public static AppBundleInfo FindAppBundleByBundleName(string bundleName)
        {
            if (string.IsNullOrWhiteSpace(bundleName))
                return null;

            var bundles = GetAppBundleFolders();
            return bundles.Select(e => new AppBundleInfo(e))
                .FirstOrDefault(e => e.Name.Equals(bundleName, StringComparison.InvariantCultureIgnoreCase));
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
