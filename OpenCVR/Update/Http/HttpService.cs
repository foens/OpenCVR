using System;
using System.IO;
using System.Net;
using NLog;

namespace OpenCVR.Update.Http
{
    internal class HttpService : IHttpService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly string downloadFolder;

        public HttpService(string downloadFolder)
        {
            this.downloadFolder = downloadFolder;
        }

        public string Download(Uri url, ICredentials authenticationCredentials)
        {
            var outputPath = Path.Combine(downloadFolder, Path.GetFileName(url.LocalPath));
            if (File.Exists(outputPath))
            {
                Logger.Info("Skipping download since local file already exists {0}", outputPath);
                return outputPath;
            }
            Logger.Info("Downloading file {0}", url);
            DownloadFileToTemporaryLocationAndMoveWhenComplete(url, authenticationCredentials, outputPath);
            Logger.Info("File downloaded to {0}", outputPath);
            return outputPath;
        }

        private static void DownloadFileToTemporaryLocationAndMoveWhenComplete(Uri url, ICredentials authenticationCredentials,
            string outputPath)
        {
            CreateDirectoryIfNotExists(new FileInfo(outputPath).Directory.FullName);
            var client = new WebClient {Credentials = authenticationCredentials};
            client.DownloadFile(url, outputPath + ".tmp");
            File.Move(outputPath + ".tmp", outputPath);
        }

        private static void CreateDirectoryIfNotExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }
    }
}