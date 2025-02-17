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
        static void Main(string[] args)
        {
#if DEBUG
            Show();
#endif
            var parser = Options.Parser.ParseArguments<Options>(args);
            parser
                .WithParsed<Options>(ExecuteCommand)
                .WithNotParsed(ExecuteError);
        }

        private static void ExecuteCommand(Options options)
        {
            if (options.Show)
            {
                Show();
            }
            if (string.IsNullOrWhiteSpace(options.App) == false)
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
                }
                else if (options.Uninstall)
                {
                    if (Path.GetExtension(appBundleName) == ".zip")
                        appBundleName = Path.GetFileNameWithoutExtension(options.App);

                    Console.WriteLine($"Uninstall '{appBundleName}'");
                }
                else
                {
                    if (Path.GetExtension(appBundleName) == ".zip")
                        appBundleName = Path.GetFileNameWithoutExtension(options.App);

                    Console.WriteLine($"App '{appBundleName}'");
                }
            }
        }

        public static void Show()
        {
            var bundles = AppBundle.ApplicationPluginsUtils.GetAppBundles();

            var appBundleModels = bundles.Select(e => new AppBundleInfo(e))
                .Where(e => e.IsValid());

            foreach (var appBundleModel in appBundleModels)
            {
                Console.WriteLine(appBundleModel);
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
