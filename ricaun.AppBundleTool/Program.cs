using CommandLine;
using ricaun.AppBundleTool.AppBundle;
using ricaun.Revit.Installation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                    if (Path.GetExtension(appBundleName) != ".zip")
                    {
                        Console.WriteLine($"'{appBundleName}' need to be 'zip' extension.");
                        return;
                    }

                    appBundleName = Path.GetFileNameWithoutExtension(options.App);
                    Console.WriteLine($"Install '{appBundleName}'");

                    var appBundle = AppBundle.AppBundleUtils.FindAppBundle(appBundleName);
                    var applicationPluginsFolder = AppBundleFolder.AppData.GetApplicationPlugins();
                    if (appBundle != null)
                    {
                        applicationPluginsFolder = appBundle.AppBundleFolder.GetApplicationPlugins();
                    }
                    var bundleUrl = options.App;
                    Console.WriteLine($"DownloadBundle: {bundleUrl}");
                    ApplicationPluginsUtils.DownloadBundle(applicationPluginsFolder, bundleUrl, (ex) => {
                        if (options.Verbosity)
                            Console.WriteLine(ex);
                    }, (message) =>
                    {
                        if (options.Verbosity)
                            Console.WriteLine(message);
                    });
                    Console.WriteLine("---");
                    AppBundle.AppBundleUtils.FindAppBundle(appBundleName)?.Show();
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
