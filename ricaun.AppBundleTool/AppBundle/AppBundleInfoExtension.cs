using ricaun.AppBundleTool.PackageContents;
using System;

namespace ricaun.AppBundleTool.AppBundle
{
    internal static class AppBundleInfoExtension
    {
        public static void Show(this AppBundleInfo appBundleInfo)
        {
            if (appBundleInfo is null) return;
            Console.WriteLine($"Name: \t{appBundleInfo.Name}");
            Console.WriteLine($"PathBundle: \t{appBundleInfo.PathBundle}");
            Console.WriteLine($"PathPackageContents: \t{appBundleInfo.PathPackageContents}");
            Console.WriteLine($"ApplicationPackage: \t{appBundleInfo.ApplicationPackage.AsString()}");
        }
    }
}