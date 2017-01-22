using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Laggson.Common
{
    public static class UpdateHelper
    {
        private static string MainDir => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Laggson Softworks";
        private static string UpdateDir => MainDir + @"\Shared";
        
        public static void InstallUpdater()
        {
            if (!Directory.Exists(UpdateDir))
                Directory.CreateDirectory(UpdateDir);

            if (!File.Exists(UpdateDir + @"Updater.exe"))
                RetrieveUpdater();

        }

        // TODO: Teste diesen shit, yo!
        // TODO: versions-Datei erstellen
        private static void RetrieveUpdater()
        {
            string updFilePath = "http://h2608125.stratoserver.net:54157/Updater.exe";

            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(new Uri(updFilePath), UpdateDir + @"\Updater.exe");
                }
            }
            catch (WebException e) when (e.Message.Contains("404"))
            { }
        }

        public static void LaunchUpdater(string name, Version version, string path)
        {
            if (string.IsNullOrEmpty(name?.Trim()) || string.IsNullOrEmpty(path?.Trim())
                || version == null || version.Major == -1 || version.Minor == -1)
                throw new ArgumentException("Die eingegebenen Werte müssen gültig sein.");

            string args = $"Name={name};Version={version.ToString()};Path=\"{path}\"";

            Process p = new Process();
            p.StartInfo.FileName = UpdateDir + @"\Updater.exe";
            p.StartInfo.Arguments = args;

            p.Start();
        }
    }
}
