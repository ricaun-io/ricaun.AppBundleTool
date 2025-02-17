using CommandLine;
using System;

namespace ricaun.AppBundleTool
{
    internal class Options
    {
        #region Parser
        public static Parser Parser { get; } = CreateParser();
        private static Parser CreateParser()
        {
            var parser = new Parser(with =>
            {
                with.HelpWriter = System.Console.Error;
                with.IgnoreUnknownArguments = true;
            });
            return parser;
        }
        #endregion
        /// <summary>
        /// Gets or sets the AppBundle file to be installed.
        /// </summary>
        [Option('a', "app", Required = false, HelpText = "AppBundle file to be installed.")]
        public string App { get; set; }
        /// <summary>
        /// Install AppBundle.
        /// </summary>
        [Option('i', "install", Required = false, HelpText = "Install AppBundle.")]
        public bool Install { get; set; }
        /// <summary>
        /// Uninstall AppBundle.
        /// </summary>
        [Option('u', "uninstall", Required = false, HelpText = "Uninstall AppBundle.")]
        public bool Uninstall { get; set; }
        ///// <summary>
        ///// Force to install AppBundle.
        ///// </summary>
        //[Option('f', "force", Required = false, HelpText = "Force to install AppBundle.")]
        //public bool ForceInstall { get; set; }
        /// <summary>
        /// Show list of available AppBundle.
        /// </summary>
        [Option('l', "list", Required = false, HelpText = "Show list of available AppBundle.")]
        public bool Show { get; set; }
    }
}
