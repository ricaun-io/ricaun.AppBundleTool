using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ricaun.AppBundleTool.PackageContents
{
    /// <summary>
    /// Provides extension methods for the <see cref="ApplicationPackage"/> class.
    /// </summary>
    public static class ApplicationPackageExtensions
    {
        /// <summary>
        /// Displays the details of the application package and its components in the console.
        /// This method is only included in the build when the DEBUG symbol is defined.
        /// </summary>
        /// <param name="applicationPackage">The application package to display.</param>
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

        /// <summary>
        /// Converts the application package to a string representation.
        /// </summary>
        /// <param name="applicationPackage">The application package to convert.</param>
        /// <returns>A string representation of the application package, or an empty string if the application package is null.</returns>
        public static string AsString(this ApplicationPackage applicationPackage)
        {
            if (applicationPackage is null) return string.Empty;

            return $"{applicationPackage.Name} {applicationPackage.AppVersion}";
        }

        /// <summary>
        /// Finds the component entries in the application package that match the specified platform, operating system, and minimum series version.
        /// </summary>
        /// <param name="applicationPackage">The application package to search.</param>
        /// <param name="platform">The platform to match.</param>
        /// <param name="os">The operating system to match.</param>
        /// <param name="seriesMin">The minimum series version to match.</param>
        /// <returns>A list of matching component entries.</returns>
        public static List<ComponentEntry> FindComponentsEntry(this ApplicationPackage applicationPackage, string platform, string os, string seriesMin)
        {
            if (applicationPackage.Components is null)
                return new List<ComponentEntry>();

            return applicationPackage.Components
                .Where(e => e.RuntimeRequirements.IsValidPlatform(platform, os, seriesMin))
                .SelectMany(e => e.ComponentEntry)
                .ToList();
        }

        /// <summary>
        /// Determines whether the runtime requirements match the specified platform, operating system, and minimum series version.
        /// </summary>
        /// <param name="runtimeRequirements">The runtime requirements to check.</param>
        /// <param name="platform">The platform to match.</param>
        /// <param name="os">The operating system to match.</param>
        /// <param name="seriesMin">The minimum series version to match.</param>
        /// <returns>True if the runtime requirements match; otherwise, false.</returns>
        internal static bool IsValidPlatform(this RuntimeRequirements runtimeRequirements, string platform, string os, string seriesMin)
        {
            if (runtimeRequirements is null)
                return false;

            return SplitBarContain(runtimeRequirements.Platform, platform) &&
                   SplitBarContain(runtimeRequirements.OS, os) &&
                   SplitBarContain(runtimeRequirements.SeriesMin, seriesMin);
        }

        /// <summary>
        /// Determines whether the source string, which may contain multiple values separated by bars ('|'), contains the specified value.
        /// </summary>
        /// <param name="sourceWithBar">The source string containing values separated by bars ('|').</param>
        /// <param name="value">The value to check for.</param>
        /// <returns>True if the source string contains the value; otherwise, false.</returns>
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