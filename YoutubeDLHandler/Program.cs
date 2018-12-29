using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace YoutubeDLHandler
{
    // watch?v= Regex v=(?'v'[a-zA-Z0-9-_]+)
    class Program
    {
        static readonly string DEFAULT_YOUTUBE_DL = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "youtube-dl", "youtube-dl.exe");
        static readonly string DEFAULT_DESTINATION = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        static string Destination => string.IsNullOrWhiteSpace(Properties.Settings.Default.Destination) ? Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) : Properties.Settings.Default.Destination;

        [STAThread]
        static void Main(string[] args)
        {
            /* Setup */
            if (args.Length == 0)
            {
                Setup();
                return;
            }
            if (args.Length == 1 && args[0].Contains("%20")) args = args[0].Split(new string[] { "%20" }, StringSplitOptions.None);

            // Check youtube-dl
            if (!File.Exists(DEFAULT_YOUTUBE_DL))
            {
                WaitAndExit("youtube-dl.exe not found. Did you install the software correctly?");
                return;
            }

            /* Arguments */
            args[0] = args[0].Substring(args[0].IndexOf(':') + 1);
            string ytdlArgs = args.Where(p => !p.Contains("youtube.com/watch")).Aggregate("", (cur, next) => $"{cur} {next}").Trim();
            string yt = args.Where(p => p.Contains("youtube.com/watch")).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(yt))
            {
                Logger.WriteErrorLine(@"No youtube.com/watch URL found in parameters.\nPress any key to exit...");
                Console.ReadKey();
                return;
            }

            // output folder
            string destination = Destination;
            if (!Directory.Exists(destination))
            {
                Logger.WriteErrorLine("Destination directory {0} does not exist. Please run the application without any arguments to configure the download destination.");
                Console.ReadKey();
                return;
            }

            // Run
            Console.WriteLine("Running youtube-dl...");
            string path = Path.Combine(destination, "%(title)s.%(ext)s");
            try
            {
                Handler(path, yt, ytdlArgs);
            }
            catch (Exception exc)
            {
                Logger.WriteErrorLine("Failed to run youtube-dl. Error:");
                Logger.WriteErrorLine(exc.ToString());
                Console.ReadKey();
                return;
            }

            // Show output
            ShowRecentFile(destination);
            
#if DEBUG
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
#endif
        }

        // Runs youtube-dl
        static void Handler(string destination, string target, string args)
        {
            if (!File.Exists(DEFAULT_YOUTUBE_DL)) throw new FileNotFoundException("youtube-dl.exe not found.");

            Process process = new Process();
            process.StartInfo.FileName = DEFAULT_YOUTUBE_DL;
            string path = Path.Combine(destination, "%(title)s.%(ext)s");
            process.StartInfo.Arguments = $"--newline --no-playlist -o {destination} {args} \"{target}\"";

            Console.WriteLine(" youtube-dl: {0}", Path.GetFullPath(process.StartInfo.FileName));
            Console.WriteLine(" arguments: {0}", process.StartInfo.Arguments);

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
            process.EnableRaisingEvents = true;

            // Handle output
            process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
            process.ErrorDataReceived += (sender, e) => Logger.WriteErrorLine(e.Data);
            
            // Wait for youtube-dl
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
        }

        // Because I don't know how to get the file from the Process, we get the last created file.
        static void ShowRecentFile(string destination)
        {
            DirectoryInfo di = new DirectoryInfo(destination);
            var createdFile = di.GetFiles().OrderByDescending(f => f.CreationTime).First();

            // Copy to clipboard.
            StringCollection paths = new StringCollection { createdFile.FullName };
            Clipboard.SetFileDropList(paths);

            // Show in explorer.
            Process.Start("explorer.exe", "/select, \"" + createdFile.FullName + "\"");
        }

        static void Setup()
        {
            // Download youtube-dl
            if (!File.Exists(DEFAULT_YOUTUBE_DL))
            {
                Console.WriteLine("Downloading youtube-dl...");
                Downloader.Download("https://yt-dl.org/latest/youtube-dl.exe", DEFAULT_YOUTUBE_DL);
                Console.WriteLine("Download complete.");
            }

            Console.WriteLine("Download destination (empty for 'My Videos'):");
            Properties.Settings.Default.Destination = Console.ReadLine();
            Properties.Settings.Default.Save();
        }
        
        static void WaitAndExit(string message, params object[] args)
        {
            Console.WriteLine(message, args);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
