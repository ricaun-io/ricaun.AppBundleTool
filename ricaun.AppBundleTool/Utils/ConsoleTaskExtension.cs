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
        /// <returns>The result produced by the completed task.</returns>
        public static T ConsoleWaitResult<T>(this Task<T> task, string processingText = null)
        {
            var spinner = new[] { '|', '/', '-', '\\' };
            int counter = 0;

            if (string.IsNullOrWhiteSpace(processingText))
            {
                processingText = "Processing...";
            }

            while (!task.IsCompleted)
            {
                Console.Write($"\r{processingText} {spinner[counter++ % spinner.Length]} ");
                Thread.Sleep(50);
            }
            Console.WriteLine($"\r{processingText}  ");

            return task.GetAwaiter().GetResult();
        }
    }
}
