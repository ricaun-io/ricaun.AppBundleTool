using System;

namespace ricaun.AppBundleTool.Utils
{
    /// <summary>
    /// Provides extension methods for formatting strings with console color escape codes.
    /// </summary>
    public static class ConsoleColorExtension
    {
        internal static int ToConsoleLength(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            // Regex to match ANSI escape codes: starts with \x1b[, ends with m
            var regex = new System.Text.RegularExpressions.Regex(@"\x1b\[\d+m");
            // Remove all ANSI escape codes
            string cleanText = regex.Replace(value, string.Empty);
            return cleanText.Length;
        }

        /// <summary>
        /// Formats the specified value as a string with red console color escape codes.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>A string wrapped in red color escape codes, or the original string if output is redirected.</returns>
        public static string ToConsoleRed(this object value)
        {
            return $"{RED}{value}{NORMAL}";
        }

        /// <summary>
        /// Formats the specified value as a string with green console color escape codes.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>A string wrapped in green color escape codes, or the original string if output is redirected.</returns>
        public static string ToConsoleGreen(this object value)
        {
            return $"{GREEN}{value}{NORMAL}";
        }

        /// <summary>
        /// Formats the specified value as a string with yellow console color escape codes.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>A string wrapped in yellow color escape codes, or the original string if output is redirected.</returns>
        public static string ToConsoleYellow(this object value)
        {
            return $"{YELLOW}{value}{NORMAL}";
        }

        /// <summary>
        /// Formats the specified value as a string with blue console color escape codes.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>A string wrapped in blue color escape codes, or the original string if output is redirected.</returns>
        public static string ToConsoleBlue(this object value)
        {
            return $"{BLUE}{value}{NORMAL}";
        }

        /// <summary>
        /// Formats the specified value as a string with magenta console color escape codes.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>A string wrapped in magenta color escape codes, or the original string if output is redirected.</returns>
        public static string ToConsoleMagenta(this object value)
        {
            return $"{MAGENTA}{value}{NORMAL}";
        }

        /// <summary>
        /// Formats the specified value as a string with cyan console color escape codes.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>A string wrapped in cyan color escape codes, or the original string if output is redirected.</returns>
        public static string ToConsoleCyan(this object value)
        {
            return $"{CYAN}{value}{NORMAL}";
        }

        /// <summary>
        /// Formats the specified value as a string with grey console color escape codes.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>A string wrapped in grey color escape codes, or the original string if output is redirected.</returns>
        public static string ToConsoleGrey(this object value)
        {
            return $"{GREY}{value}{NORMAL}";
        }

        static string NORMAL => Console.IsOutputRedirected ? "" : "\x1b[39m";
        static string RED => Console.IsOutputRedirected ? "" : "\x1b[91m";
        static string GREEN => Console.IsOutputRedirected ? "" : "\x1b[92m";
        static string YELLOW => Console.IsOutputRedirected ? "" : "\x1b[93m";
        static string BLUE => Console.IsOutputRedirected ? "" : "\x1b[94m";
        static string MAGENTA => Console.IsOutputRedirected ? "" : "\x1b[95m";
        static string CYAN => Console.IsOutputRedirected ? "" : "\x1b[96m";
        static string GREY => Console.IsOutputRedirected ? "" : "\x1b[97m";
        static string BOLD => Console.IsOutputRedirected ? "" : "\x1b[1m";
        static string NOBOLD => Console.IsOutputRedirected ? "" : "\x1b[22m";
        static string UNDERLINE => Console.IsOutputRedirected ? "" : "\x1b[4m";
        static string NOUNDERLINE => Console.IsOutputRedirected ? "" : "\x1b[24m";
        static string REVERSE => Console.IsOutputRedirected ? "" : "\x1b[7m";
        static string NOREVERSE => Console.IsOutputRedirected ? "" : "\x1b[27m";
    }
}
