using System;
using System.Threading;
using System.Threading.Tasks;

namespace ricaun.AppBundleTool.Utils
{
    /// <summary>
    /// Provides extension methods for waiting on tasks with console feedback.
    /// </summary>
    public static class ConsoleTaskExtension
    {
        /// <summary>
        /// Waits for the specified task to complete while displaying a spinner and optional processing text in the console.
        /// </summary>
        /// <typeparam name="T">The type of the result produced by the task.</typeparam>
        /// <param name="task">The task to wait for.</param>
        /// <param name="processingText">Optional text to display while processing. Defaults to "Processing..." if not specified.</param>
        /// <param name="processFunc"></param>
        /// <returns>The result produced by the completed task.</returns>
        public static T ConsoleWaitResult<T>(this Task<T> task, string processingText = null, Func<string> processFunc = null)
        {
            var spinner = new[] { '|', '/', '-', '\\' };
            int counter = 0;

            if (string.IsNullOrWhiteSpace(processingText))
            {
                processingText = "Processing...";
            }

            while (!task.IsCompleted)
            {
                var processText = processFunc?.Invoke();
                Console.Write($"\r{processingText} {spinner[counter++ % spinner.Length]} {processText} ");
                Thread.Sleep(50);
            }
            var spaces = new string(' ', processFunc?.Invoke()?.Length ?? 0);
            Console.WriteLine($"\r{processingText}  {spaces} ");

            return task.GetAwaiter().GetResult();
        }
    }
}
