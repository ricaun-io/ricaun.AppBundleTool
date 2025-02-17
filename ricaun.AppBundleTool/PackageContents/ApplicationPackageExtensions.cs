using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ricaun.AppBundleTool.PackageContents
{
    public static class ApplicationPackageExtensions
    {
        [Conditional("DEBUG")]
        public static void Show(this ApplicationPackage applicationPackage)
        {
            if (applicationPackage is not null)
            {
                System.Console.WriteLine($" ApplicationPackage: {applicationPackage.Name} {applicationPackage.AppVersion} {applicationPackage.AutodeskProduct} {applicationPackage.ProductType} {applicationPackage.ProductCode}");
                System.Console.WriteLine($" CompanyDetails: {applicationPackage.CompanyDetails?.Name}");
                foreach (var component in applicationPackage.Components)
                {
                    foreach (var componentEntry in component.ComponentEntry)
                    {
                        System.Console.WriteLine($" ComponentEntry: {component.RuntimeRequirements.Platform}  {component.RuntimeRequirements.SeriesMin} {component.RuntimeRequirements.SeriesMax} \t{componentEntry.AppName} \t{componentEntry.ModuleName}");
                    }
                }
                foreach (var componentEntry in applicationPackage.FindComponentsEntry("Revit", "Win64", "R2021"))
                {
                    System.Console.WriteLine($"FindComponentsEntry >>> {componentEntry.AppName} {componentEntry.ModuleName}");
                }
            }
        }

        public static List<ComponentEntry> FindComponentsEntry(this ApplicationPackage applicationPackage, string platform, string os, string seriesMin)
        {
            if (applicationPackage.Components is null)
                return new List<ComponentEntry>();

            return applicationPackage.Components
                .Where(e => e.RuntimeRequirements.IsValidPlatform(platform,os,seriesMin))
                .SelectMany(e => e.ComponentEntry)
                .ToList();
        }

        internal static bool IsValidPlatform(this RuntimeRequirements runtimeRequirements, string platform, string os, string seriesMin)
        {
            if (runtimeRequirements is null)
                return false;

            return SplitBarContain(runtimeRequirements.Platform, platform) &&
                   SplitBarContain(runtimeRequirements.OS, os) &&
                   SplitBarContain(runtimeRequirements.SeriesMin, seriesMin);
        }

        internal static bool SplitBarContain(string sourceWithBar, string value)
        {
            if (value == "*")
                return true;

            if (string.IsNullOrWhiteSpace(sourceWithBar) || string.IsNullOrWhiteSpace(value))
                return false;

            var sources = sourceWithBar.Split('|', System.StringSplitOptions.RemoveEmptyEntries);
            return sources.Contains(value);
        }
    }
}