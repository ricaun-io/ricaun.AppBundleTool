using ricaun.AppBundleTool.PackageContents;
using System;
using System.Data;

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

        public static DataTable ToDataTable(this AppBundleInfo[] appBundleInfoArray, bool detail = false)
        {
            var table = new DataTable("AppBundles");
            table.Columns.Add("Folder", typeof(string));
            table.Columns.Add("Bundle", typeof(string));
            table.Columns.Add("AppName", typeof(string));
            if (detail)
            {
                table.Columns.Add("AppProduct", typeof(string));
                table.Columns.Add("AppDescription", typeof(string));
            }
            table.Columns.Add("Access", typeof(string));
            foreach (var appBundleInfo in appBundleInfoArray)
            {
                if (appBundleInfo == null) continue;
                var row = table.NewRow();
                row["Folder"] = appBundleInfo.AppBundleFolder;
                row["Bundle"] = appBundleInfo.Name;
                row["AppName"] = appBundleInfo.ApplicationPackage?.Name ?? string.Empty;
                if (detail)
                {
                    row["AppProduct"] = appBundleInfo.ApplicationPackage?.AutodeskProduct ?? string.Empty;
                    row["AppDescription"] = appBundleInfo.ApplicationPackage?.Description ?? string.Empty;
                }
                row["Access"] = appBundleInfo.AppBundleAccess;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}