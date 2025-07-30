using System;
using System.Threading;
using System.Threading.Tasks;

namespace ricaun.AppBundleTool.Utils
{
    public static class ConsoleTaskExtension
    {
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
