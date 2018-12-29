using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace YoutubeDLHandler
{
    public static class Downloader
    {
        public static void Download(string url, string destination)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(destination));

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(url, destination);
            }
        }
    }
}
