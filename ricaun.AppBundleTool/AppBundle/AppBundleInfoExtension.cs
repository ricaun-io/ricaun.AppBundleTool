using ricaun.AppBundleTool.PackageContents;
using System;

namespace ricaun.AppBundleTool.AppBundle
{
    internal static class AppBundleInfoExtension
    {
        public static void Show(this AppBundleInfo appBundleInfo, bool showComponents = false)
        {
            if (appBundleInfo is null) return;
            Console.WriteLine($"Name: \t{appBundleInfo.Name}");
            Console.WriteLine($"Path: \t{appBundleInfo.PathBundle}");
            //Console.WriteLine($"PathPackageContents: \t{appBundleInfo.PathPackageContents}");
            Console.WriteLine($"App: \t{appBundleInfo.ApplicationPackage.AsString()}");

            if (showComponents == false) return;
            if (appBundleInfo.ApplicationPackage is not null)
            {
                foreach (var component in appBundleInfo.ApplicationPackage.Components)
                {
                    foreach (var componentEntry in component.ComponentEntry)
                    {
                        Console.WriteLine($" ComponentEntry: {component.RuntimeRequirements.Platform}  {component.RuntimeRequirements.SeriesMin} {component.RuntimeRequirements.SeriesMax} \t{componentEntry.AppName} \t{componentEntry.ModuleName}");
                    }
                }
            }
        }
    }
}