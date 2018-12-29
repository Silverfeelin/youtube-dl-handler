using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace YoutubeDLHandler
{
    public static class Logger
    {
        public static void WriteErrorLine(string err, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(err))
            {
                Console.Error.WriteLine();
                return;
            }

            var old = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(err, args);
            Console.ForegroundColor = old;
        }
    }
}
