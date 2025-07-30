using CommandLine;
using ricaun.AppBundleTool.AppBundle;
using ricaun.AppBundleTool.PackageContents;
using ricaun.AppBundleTool.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

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
                        Console.WriteLine($"'{appBundleName}' need to be 'zip' extension.".ToConsoleRed());
                        return;
                    }

                    var bundleUrl = options.App;
                    var appBundleInfoTemp = DownloadAppBundleInfo(bundleUrl);
                    var applicationPluginsFolder = AppBundleFolder.AppData.GetApplicationPlugins();

                    appBundleName = appBundleInfoTemp.ApplicationPackage.Name;

                    if (appBundleInfoTemp.IsValid())
                    {
                        appBundleName = appBundleInfoTemp.ApplicationPackage.Name;

                        var installMessage = $"Install: {appBundleInfoTemp.ApplicationPackage.AsString().ToConsoleGreen()}";

                        var appBundleInstalled = AppBundleUtils.FindAppBundleByAppName(appBundleName);
                        if (appBundleInstalled is not null)
                        {
                            applicationPluginsFolder = appBundleInstalled.AppBundleFolder.GetApplicationPlugins();
                        }

                        // Copy all files of the folder to a different folder.
                        if (appBundleInstalled is not null)
                        {
                            Console.WriteLine($"Replace: {appBundleInstalled.ApplicationPackage.AsString().ToConsoleYellow()}");
                            DirectoryUtils.CopyFilesRecursively(appBundleInfoTemp.PathBundle, appBundleInstalled.PathBundle);
                        }
                        else
                        {
                            var appBundleFolder = Path.Combine(applicationPluginsFolder, appBundleInfoTemp.Name);
                            DirectoryUtils.CopyFilesRecursively(appBundleInfoTemp.PathBundle, appBundleFolder);
                        }
                        Console.WriteLine(installMessage);
                    }
                    AppBundleUtils.FindAppBundleByAppName(appBundleName)?.ToDataTable().Print();
                }
                else if (options.Uninstall)
                {
                    if (Path.GetExtension(appBundleName) == ".zip")
                    {
                        var bundleUrl = options.App;
                        var appBundleInfoTemp = DownloadAppBundleInfo(bundleUrl);
                        if (appBundleInfoTemp is null)
                        {
                            Console.WriteLine($"AppBundle '{bundleUrl}' not found.".ToConsoleRed());
                            return;
                        }

                        if (appBundleInfoTemp.IsValid())
                        {
                            appBundleName = appBundleInfoTemp.ApplicationPackage.Name;
                        }
                        else
                        {
                            Console.WriteLine($"AppBundleInfo '{appBundleInfoTemp.Name}' not valid.".ToConsoleRed());
                            return;
                        }
                    }

                    var appBundle = AppBundleUtils.FindAppBundle(appBundleName);
                    if (appBundle is null)
                    {
                        Console.WriteLine($"AppBundle '{appBundleName}' not found.".ToConsoleRed());
                        return;
                    }
                    var applicationPluginsFolder = appBundle.AppBundleFolder.GetApplicationPlugins();
                    Console.WriteLine($"Uninstall: {appBundle.ApplicationPackage.AsString().ToConsoleRed()}");

                    UninstallAppBundle(appBundle);
                }
                else if (Path.GetExtension(appBundleName) == ".zip")
                {
                    var bundleUrl = options.App;
                    var appBundleInfoTemp = DownloadAppBundleInfo(bundleUrl);
                    if (appBundleInfoTemp.IsValid())
                    {
                        appBundleName = appBundleInfoTemp.ApplicationPackage.Name;
                        var appBundleInfo = AppBundleUtils.FindAppBundleByAppName(appBundleName);
                        appBundleInfo?.Show();
                        if (appBundleInfo is null)
                        {
                            Console.WriteLine($"AppBundle '{appBundleName}' not found.".ToConsoleRed());
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"AppBundleInfo '{appBundleInfoTemp.Name}' not valid.".ToConsoleRed());
                        return;
                    }
                }
                else
                {
                    var appBundle = AppBundleUtils.FindAppBundle(appBundleName);
                    if (appBundle is null)
                    {
                        Show();
                        Console.WriteLine($"AppBundle '{appBundleName}' not found.".ToConsoleYellow());
                        return;
                    }

                    foreach (var table in appBundle.ToDataTables(Verbosity))
                    {
                        table.Print();
                    }
                }
            }
            else
            {
                Console.WriteLine(displayHelp);
            }
        }

        private static void UninstallAppBundle(AppBundleInfo appBundle)
        {
            try
            {
                RecycleBinUtils.DirectoryToRecycleBin(appBundle.PathBundle);
                Console.WriteLine($"Send to Recycle Bin: {appBundle.PathBundle}".ToConsoleYellow());
            }
            catch (Exception)
            {
                Console.WriteLine($"Fail to send to Recycle Bin: {appBundle.PathBundle}".ToConsoleRed());
            }

            try
            {
                if (RecycleBinUtils.FileToRecycleBin(appBundle.PathPackageContents))
                {
                    Console.WriteLine($"Send to Recycle Bin: {appBundle.PathPackageContents}".ToConsoleYellow());
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"Fail to send to Recycle Bin: {appBundle.PathPackageContents}".ToConsoleRed());
            }
        }

        private static AppBundleInfo DownloadAppBundleInfo(string bundleUrl)
        {
            var bundleName = Path.GetFileName(bundleUrl);
            var downloadProgress = $"Download: {bundleName.ToConsoleGreen()}";

            var bundlePathZip = DownloadUtils.DownloadAsync(bundleUrl).ConsoleWaitResult(downloadProgress);

            // unzip file to folder
            var bundlePathFolder = Path.Combine(Path.GetDirectoryName(bundlePathZip), Path.GetFileNameWithoutExtension(bundlePathZip));
            if (Directory.Exists(bundlePathFolder))
                Directory.Delete(bundlePathFolder, true);

            var extractProgress = $"Extract: {bundleName.ToConsoleGreen()}";
            Task.Run(() =>
            {
                ZipFile.ExtractToDirectory(bundlePathZip, bundlePathFolder, true);
                return true;
            }).ConsoleWaitResult(extractProgress);

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
