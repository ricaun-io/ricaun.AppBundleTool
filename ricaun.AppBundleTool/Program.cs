using CommandLine;
using ricaun.AppBundleTool.AppBundle;
using ricaun.AppBundleTool.Utils;
using ricaun.Revit.Installation;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace ricaun.AppBundleTool
{
    internal class Program
    {
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
            if (options.Show)
            {
                Show();
            }
            else if (string.IsNullOrWhiteSpace(options.App) == false)
            {
                var appBundleName = Path.GetFileName(options.App);
                if (options.Install)
                {
                    //if (Path.GetExtension(appBundleName) != ".zip")
                    //{
                    //    Console.WriteLine($"'{appBundleName}' need to be 'zip' extension.");
                    //    return;
                    //}

                    appBundleName = Path.GetFileNameWithoutExtension(options.App);
                    Console.WriteLine($"Install '{appBundleName}'");

                    var appBundle = AppBundleUtils.FindAppBundle(appBundleName);
                    var applicationPluginsFolder = AppBundleFolder.AppData.GetApplicationPlugins();
                    if (appBundle != null)
                    {
                        applicationPluginsFolder = appBundle.AppBundleFolder.GetApplicationPlugins();
                    }
                    var bundleUrl = options.App;
                    Console.WriteLine($"DownloadBundle: {bundleUrl}");

                    Console.WriteLine("Downloaded....");

                    var bundlePathZip = DownloadUtils.DownloadAsync(bundleUrl).GetAwaiter().GetResult();

                    // unzip file to folder
                    var bundlePathFolder = Path.Combine(Path.GetDirectoryName(bundlePathZip), Path.GetFileNameWithoutExtension(bundlePathZip));
                    if (Directory.Exists(bundlePathFolder))
                        Directory.Delete(bundlePathFolder, true);

                    ZipFile.ExtractToDirectory(bundlePathZip, bundlePathFolder, true);

                    Console.WriteLine(bundlePathFolder);

                    Console.WriteLine("Downloaded....Finish");

                    var appBundleInfoTemp = AppBundleInfo.FindAppBundle(bundlePathFolder);
                    appBundleInfoTemp?.Show();
                    Console.WriteLine("---");

                    if (appBundleInfoTemp.IsValid())
                    {
                        appBundleName = appBundleInfoTemp.ApplicationPackage.Name;

                        Console.WriteLine(appBundleName);

                        var appBundleInfo = AppBundleUtils.FindAppBundle(appBundleName);

                        Console.WriteLine($"{appBundleInfoTemp.ApplicationPackage.AppVersion} -> {appBundleInfo?.ApplicationPackage.AppVersion}");

                        Console.WriteLine("---");

                        Console.WriteLine(applicationPluginsFolder);
                        // Copy all files of the folder to a different folder.

                        if (!appBundleInfo.IsValid())
                        {
                            appBundleInfo.PathBundle = Path.Combine(applicationPluginsFolder, appBundleName);
                        }

                        DirectoryUtils.CopyFilesRecursively(appBundleInfoTemp.PathBundle, appBundleInfo.PathBundle);

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

                        appBundleInfo = AppBundleUtils.FindAppBundle(appBundleName);
                        appBundleInfo?.Show();
                    }


                    
                }
                else if (options.Uninstall)
                {
                    if (Path.GetExtension(appBundleName) == ".zip")
                        appBundleName = Path.GetFileNameWithoutExtension(options.App);

                    var appBundle = AppBundle.AppBundleUtils.FindAppBundle(appBundleName);

                    if (appBundle is null)
                    {
                        Console.WriteLine($"AppBundle '{appBundleName}' not found.");
                        return;
                    }

                    appBundle.Show();

                    var applicationPluginsFolder = appBundle.AppBundleFolder.GetApplicationPlugins();
                    ApplicationPluginsUtils.DeleteBundle(applicationPluginsFolder, appBundleName);
                    Console.WriteLine($"Uninstall '{appBundleName}'");
                }
                else
                {
                    if (Path.GetExtension(appBundleName) == ".zip")
                        appBundleName = Path.GetFileNameWithoutExtension(options.App);

                    var appBundle = AppBundle.AppBundleUtils.FindAppBundle(appBundleName);

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

        public static void Show()
        {
            var appBundles = AppBundleUtils.GetAppBundles();

            foreach (var appBundle in appBundles)
            {
                Console.WriteLine(appBundle);
            }
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
