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
            parser.WithParsed<Options>(ExecuteCommandException)
                  .WithNotParsed(ExecuteError);
        }

        private static void ExecuteCommandException(Options options)
        {
            try
            {
                ExecuteCommand(options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message.ToConsoleRed()}");
                if (options.Verbosity)
                {
                    Console.WriteLine(ex.ToConsoleYellow());
                }
            }
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
                ExecuteApp(options);
            }
            else
            {
                Console.WriteLine(displayHelp);
            }
        }

        private static void ExecuteApp(Options options)
        {
            var bundle = new BundleUri(options.App);
            var appBundleName = bundle.AppName;
                Console.WriteLine($"{bundle.BundleNameZip} {bundle.BundleName} {bundle.AppName} {bundle.IsValid()}");
            if (bundle.IsValid())
            {
                if (options.Install)
                {
                    var appBundleInfoTemp = DownloadBundleUriToTemp(bundle);
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
                    var appBundleInfo = AppBundleUtils.FindAppBundleByAppName(appBundleName);
                    appBundleInfo.Show(Verbosity);
                }
                else if (options.Uninstall)
                {
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
                else
                {
                    var appBundleInfo = AppBundleUtils.FindAppBundle(appBundleName);
                    if (appBundleInfo is null)
                    {
                        Console.WriteLine($"AppBundle '{appBundleName}' not found.".ToConsoleRed());
                        return;
                    }
                    appBundleInfo.Show(Verbosity);
                }
                return;
            }
            else
            {
                var appBundleInfo = AppBundleUtils.FindAppBundle(bundle.BundleName);
                if (appBundleInfo is null)
                {
                    Show();
                    Console.WriteLine($"AppBundle '{bundle.BundleName}' not found.".ToConsoleYellow());
                    return;
                }

                appBundleInfo.Show(Verbosity);
            }
        }

        private static AppBundleInfo DownloadBundleUriToTemp(BundleUri bundle)
        {
            var downloadProgress = $"Download: {bundle.BundleName.ToConsoleGreen()}";
            var processPercentage = "";
            Action<long, long> progress = (value, total) =>
            {
                if (total > 0)
                    processPercentage = $"{100.0 * value / total:0.00}%";
            };
            var tempFolder = BundleDownloadUtils.GetTempFolder();
            var bundlePathZip = bundle.DownloadAsync(tempFolder, progress)
                .ConsoleWaitResult(downloadProgress, () => { return processPercentage; });

            var bundlePathFolderName = Path.GetFileNameWithoutExtension(bundlePathZip);

            // unzip file to folder
            var bundlePathFolder = Path.Combine(Path.GetDirectoryName(bundlePathZip), bundlePathFolderName);
            if (Directory.Exists(bundlePathFolder))
                Directory.Delete(bundlePathFolder, true);

            var extractProgress = $"Extract: {bundle.BundleName.ToConsoleGreen()}";
            Task.Run(() =>
            {
                ZipFile.ExtractToDirectory(bundlePathZip, bundlePathFolder, true);
                return true;
            }).ConsoleWaitResult(extractProgress);

            var tempAppBundleInfo = AppBundleInfo.FindAppBundle(bundlePathFolder);
            return tempAppBundleInfo;
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

        public static void Show()
        {
            var appBundles = AppBundleUtils.GetAppBundles();
            appBundles.Show(Verbosity);
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
