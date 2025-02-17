using CommandLine;
using ricaun.AppBundleTool.AppBundle;
using ricaun.Revit.Installation;
using System;
using System.Collections.Generic;
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
