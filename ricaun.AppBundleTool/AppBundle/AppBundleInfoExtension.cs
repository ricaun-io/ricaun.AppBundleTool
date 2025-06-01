using ricaun.AppBundleTool.PackageContents;
using ricaun.AppBundleTool.Utils;
using System;
using System.Collections.Generic;
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
            var table = new DataTable();
            table.Columns.Add("Folder", typeof(string));
            table.Columns.Add("Bundle", typeof(string));
            table.Columns.Add("AppName", typeof(string));
            table.Columns.Add("AppProduct", typeof(string));
            if (detail)
            {
                table.Columns.Add("Company", typeof(string));
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
                row["AppProduct"] = appBundleInfo.ApplicationPackage?.AutodeskProduct ?? string.Empty;
                if (detail)
                {
                    row["Company"] = appBundleInfo.ApplicationPackage?.CompanyDetails?.Name ?? string.Empty;
                    row["AppDescription"] = appBundleInfo.ApplicationPackage?.Description ?? string.Empty;
                }
                row["Access"] = appBundleInfo.AppBundleAccess.ToConsoleString();

                table.Rows.Add(row);
            }
            return table;
        }

        public static DataTable[] ToDataTables(this AppBundleInfo appBundleInfo, bool detail = false)
        {
            var tables = new List<DataTable>();

            tables.Add(appBundleInfo.ToDataTable(detail));

            if (detail)
            {
                foreach (var component in appBundleInfo.ApplicationPackage?.Components)
                {
                    if (component is null) continue;
                    tables.Add(component.ToDataTable());
                }
            }

            return tables.ToArray();
        }

        public static DataTable ToDataTable(this AppBundleInfo appBundleInfo, bool detail = false)
        {
            if (appBundleInfo == null) return null;

            var table = new DataTable();
            table.CreateDataColumns();

            table.DataRow("Bundle", appBundleInfo.Name);
            table.DataRow("AppName", appBundleInfo.ApplicationPackage?.Name ?? string.Empty);
            table.DataRow("AppVersion", appBundleInfo.ApplicationPackage?.AppVersion ?? string.Empty);
            table.DataRow("AppProduct", appBundleInfo.ApplicationPackage?.AutodeskProduct ?? string.Empty);
            if (detail)
                table.DataRow("AppDescription", appBundleInfo.ApplicationPackage?.Description ?? string.Empty);
            table.DataRow("AppProductType", appBundleInfo.ApplicationPackage?.ProductType ?? string.Empty);
            table.DataRow("AppProductCode", appBundleInfo.ApplicationPackage?.ProductCode ?? string.Empty);
            table.DataRow("AppCompanyName", appBundleInfo.ApplicationPackage?.CompanyDetails?.Name);
            if (detail)
            {
                table.DataRow("AppNameSpace", appBundleInfo.ApplicationPackage?.AppNameSpace ?? string.Empty);
                table.DataRow("AppUpgradeCode", appBundleInfo.ApplicationPackage?.UpgradeCode ?? string.Empty);
            }
            table.DataRow("PathBundle", appBundleInfo.PathBundle);
            table.DataRow("Access", appBundleInfo.AppBundleAccess.ToConsoleString());

            //if (detail)
            //{
            //    table.DataRow("Components", string.Empty);
            //    foreach (var component in appBundleInfo.ApplicationPackage?.Components)
            //    {
            //        if (component is null) continue;

            //        table.DataRow("Component.Description", component.Description);
            //        table.DataRow("Requirements.Platform", component.RuntimeRequirements?.Platform);
            //        table.DataRow("Requirements.OS", component.RuntimeRequirements?.OS);
            //        table.DataRow("Requirements.SeriesMin", component.RuntimeRequirements?.SeriesMin);
            //        table.DataRow("Requirements.SeriesMax", component.RuntimeRequirements?.SeriesMax);

            //        foreach (var componentEntry in component.ComponentEntry)
            //        {
            //            table.DataRow("ComponentEntry.AppName", componentEntry.AppName);
            //            table.DataRow("ComponentEntry.ModuleName", componentEntry.ModuleName);
            //        }
            //    }
            //}

            return table;
        }

        public static DataTable ToDataTable(this Component component)
        {
            DataTable table = new DataTable();
            table.CreateDataColumns();

            table.DataRow("Component.Description", component.Description);
            table.DataRow("Requirements.Platform", component.RuntimeRequirements?.Platform);
            table.DataRow("Requirements.OS", component.RuntimeRequirements?.OS);
            table.DataRow("Requirements.SeriesMin", component.RuntimeRequirements?.SeriesMin);
            table.DataRow("Requirements.SeriesMax", component.RuntimeRequirements?.SeriesMax);

            foreach (var componentEntry in component.ComponentEntry)
            {
                table.DataRow("ComponentEntry.AppName", componentEntry.AppName);
                table.DataRow("ComponentEntry.ModuleName", componentEntry.ModuleName);
            }

            return table;
        }

        private static string ToConsoleString(this AppBundleAccess appBundleAccess)
        {
            return appBundleAccess switch
            {
                AppBundleAccess.Admin => appBundleAccess.ToConsoleRed(),
                _ => appBundleAccess.ToString()
            };
        }

        private static DataTable CreateDataColumns(this DataTable table)
        {
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Value", typeof(string));
            return table;
        }

        private static DataRow DataRow(this DataTable table, string name, object value)
        {
            var row = table.NewRow();
            row["Name"] = name;
            row["Value"] = value;
            table.Rows.Add(row);
            return row;
        }
    }
}