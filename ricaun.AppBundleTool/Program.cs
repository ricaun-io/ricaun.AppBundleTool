using CommandLine;
using ricaun.AppBundleTool.AppBundle;
using ricaun.AppBundleTool.PackageContents;
using ricaun.AppBundleTool.Utils;
using ricaun.Revit.Installation;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;

namespace ricaun.AppBundleTool
{
    internal class Program
    {
        public static bool Verbosity { get; set; }
        static string displayHelp { get; set; }
        static void Main(string[] args)
        {
#if DEBUG
            Show();
#endif
            var parser = Options.Parser.ParseArguments<Options>(args);
            displayHelp = DisplayHelp(parser);
            parser.WithParsed<Options>(ExecuteCommand)
                  .WithNotParsed(ExecuteError);
        }

        private static void ExecuteCommand(Options options)
        {
            Verbosity = options.Verbosity;
            if (options.Show)
            {
                Show();
            }
            else if (string.IsNullOrWhiteSpace(options.App) == false)
            {
                var appBundleName = Path.GetFileName(options.App);
                if (options.Install)
                {
                    if (Path.GetExtension(appBundleName) != ".zip")
                    {
                        Console.WriteLine($"'{appBundleName}' need to be 'zip' extension.");
                        return;
                    }

                    var bundleUrl = options.App;
                    var appBundleInfoTemp = DownloadAppBundleInfo(bundleUrl);
                    var applicationPluginsFolder = AppBundleFolder.AppData.GetApplicationPlugins();

                    appBundleName = appBundleInfoTemp.ApplicationPackage.Name;

                    if (Verbosity)
                        appBundleInfoTemp?.Show();

                    if (appBundleInfoTemp.IsValid())
                    {
                        appBundleName = appBundleInfoTemp.ApplicationPackage.Name;

                        Console.WriteLine($"Install: {appBundleInfoTemp.ApplicationPackage.AsString()}");

                        var appBundleInstalled = AppBundleUtils.FindAppBundleByName(appBundleName);
                        if (appBundleInstalled is not null)
                        {
                            applicationPluginsFolder = appBundleInstalled.AppBundleFolder.GetApplicationPlugins();
                            Console.WriteLine($"Replace: {appBundleInstalled.ApplicationPackage.AsString()}");
                        }

                        // Copy all files of the folder to a different folder.

                        if (appBundleInstalled is not null)
                        {
                            DirectoryUtils.CopyFilesRecursively(appBundleInfoTemp.PathBundle, appBundleInstalled.PathBundle);
                        }
                        else
                        {
                            var appBundleFolder = Path.Combine(applicationPluginsFolder, appBundleInfoTemp.Name);
                            DirectoryUtils.CopyFilesRecursively(appBundleInfoTemp.PathBundle, appBundleFolder);
                        }

                        //Console.WriteLine(bundlePathZip);

                        //ApplicationPluginsUtils.DownloadBundle(applicationPluginsFolder, bundlePathZip, (ex) =>
                        //{
                        //    if (options.Verbosity)
                        //        Console.WriteLine(ex);
                        //}, (message) =>
                        //{
                        //    if (options.Verbosity)
                        //        Console.WriteLine(message);
                        //});
                        Console.WriteLine("---");
                    }
                    AppBundleUtils.FindAppBundleByName(appBundleName)?.Show();
                }
                else if (options.Uninstall)
                {
                    if (Path.GetExtension(appBundleName) == ".zip")
                    {
                        var bundleUrl = options.App;
                        var appBundleInfoTemp = DownloadAppBundleInfo(bundleUrl);
                        if (appBundleInfoTemp is null)
                        {
                            Console.WriteLine($"AppBundle '{bundleUrl}' not found.");
                            return;
                        }

                        if (appBundleInfoTemp.IsValid())
                        {
                            appBundleName = appBundleInfoTemp.ApplicationPackage.Name;
                        }
                        else
                        {
                            Console.WriteLine($"AppBundleInfo '{appBundleInfoTemp.Name}' not valid.");
                            return;
                        }
                    }

                    var appBundle = AppBundleUtils.FindAppBundleByName(appBundleName);
                    if (appBundle is null)
                    {
                        Console.WriteLine($"AppBundle '{appBundleName}' not found.");
                        return;
                    }
                    var applicationPluginsFolder = appBundle.AppBundleFolder.GetApplicationPlugins();
                    Console.WriteLine($"Uninstall: {appBundle.ApplicationPackage.AsString()}");
                    ApplicationPluginsUtils.DeleteBundle(applicationPluginsFolder, appBundle.Name);
                }
                else if (Path.GetExtension(appBundleName) == ".zip")
                {
                    var bundleUrl = options.App;
                    var appBundleInfoTemp = DownloadAppBundleInfo(bundleUrl);
                    if (appBundleInfoTemp.IsValid())
                    {
                        Console.WriteLine($"DownloadApp: {appBundleInfoTemp.ApplicationPackage.AsString()}");
                        appBundleName = appBundleInfoTemp.ApplicationPackage.Name;
                        var appBundleInfo = AppBundleUtils.FindAppBundleByName(appBundleName);
                        appBundleInfo?.Show();
                        if (appBundleInfo is null)
                        {
                            Console.WriteLine($"AppBundle '{appBundleName}' not found.");
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"AppBundleInfo '{appBundleInfoTemp.Name}' not valid.");
                        return;
                    }
                }
                else
                {
                    var appBundle = AppBundleUtils.FindAppBundleByName(appBundleName);

                    if (appBundle is null)
                    {
                        Console.WriteLine($"AppBundle '{appBundleName}' not found.");
                        return;
                    }

                    appBundle.Show();
                }
            }
            else
            {
                Console.WriteLine(displayHelp);
            }
        }

        private static AppBundleInfo DownloadAppBundleInfo(string bundleUrl)
        {
            Console.WriteLine($"DownloadBundle: {bundleUrl}");
            WriteLine("Downloaded....");

            var bundlePathZip = DownloadUtils.DownloadAsync(bundleUrl).GetAwaiter().GetResult();

            // unzip file to folder
            var bundlePathFolder = Path.Combine(Path.GetDirectoryName(bundlePathZip), Path.GetFileNameWithoutExtension(bundlePathZip));
            if (Directory.Exists(bundlePathFolder))
                Directory.Delete(bundlePathFolder, true);

            ZipFile.ExtractToDirectory(bundlePathZip, bundlePathFolder, true);
            WriteLine(bundlePathFolder);
            WriteLine("Downloaded....Finish");

            return AppBundleInfo.FindAppBundle(bundlePathFolder);
        }

        public static void WriteLine(object message)
        {
            if (Verbosity)
            {
                Console.WriteLine(message);
            }
        }

        public static void Show()
        {
            var appBundles = AppBundleUtils.GetAppBundles();
            appBundles.ToDataTable(Verbosity).Print();
        }

        private static string DisplayHelp<T>(ParserResult<T> result)
        {
            var helpText = CommandLine.Text.HelpText.AutoBuild(result, h => h, e => e);
            return helpText.ToString();
        }

        private static void ExecuteError(IEnumerable<Error> errors)
        {
            if (errors.IsHelp()) return;
            if (errors.IsVersion()) return;
        }
    }
}
