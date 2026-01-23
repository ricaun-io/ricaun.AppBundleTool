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

        static string NORMAL => IsAnsiSupported ? "\x1b[39m" : string.Empty;
        static string RED => IsAnsiSupported ? "\x1b[91m" : string.Empty;
        static string GREEN => IsAnsiSupported ? "\x1b[92m" : string.Empty;
        static string YELLOW => IsAnsiSupported ? "\x1b[93m" : string.Empty;
        static string BLUE => IsAnsiSupported ? "\x1b[94m" : string.Empty;
        static string MAGENTA => IsAnsiSupported ? "\x1b[95m" : string.Empty;
        static string CYAN => IsAnsiSupported ? "\x1b[96m" : string.Empty;
        static string GREY => IsAnsiSupported ? "\x1b[97m" : string.Empty;
        static string BOLD => IsAnsiSupported ? "\x1b[1m" : string.Empty;
        static string NOBOLD => IsAnsiSupported ? "\x1b[22m" : string.Empty;
        static string UNDERLINE => IsAnsiSupported ? "\x1b[4m" : string.Empty;
        static string NOUNDERLINE => IsAnsiSupported ? "\x1b[24m" : string.Empty;
        static string REVERSE => IsAnsiSupported ? "\x1b[7m" : string.Empty;
        static string NOREVERSE => IsAnsiSupported ? "\x1b[27m" : string.Empty;

        internal static bool IsAnsiSupported { get; } = SupportsAnsi();
        internal static bool SupportsAnsi()
        {
            if (Console.IsOutputRedirected)
                return false;

            if (!OperatingSystem.IsWindows())
                return true;

            if (!OperatingSystem.IsWindowsVersionAtLeast(10))
                return false;

            return Windows.IsAnsiEnabledOnWindows();
        }
        static class Windows
        {
            internal static bool IsAnsiEnabledOnWindows()
            {
                const int STD_OUTPUT_HANDLE = -11;
                const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

                IntPtr handle = GetStdHandle(STD_OUTPUT_HANDLE);
                if (handle == IntPtr.Zero)
                    return false;

                if (!GetConsoleMode(handle, out uint mode))
                    return false;

                return (mode & ENABLE_VIRTUAL_TERMINAL_PROCESSING) != 0;
            }

            [System.Runtime.InteropServices.DllImport("kernel32.dll")]
            static extern IntPtr GetStdHandle(int nStdHandle);

            [System.Runtime.InteropServices.DllImport("kernel32.dll")]
            static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);
        }
    }
}
