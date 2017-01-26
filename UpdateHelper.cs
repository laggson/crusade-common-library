using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Laggson.Common
{
    public static class UpdateHelper
    {
        private const string API_URL = "http://h2608125.stratoserver.net:5000/api/";
        private static string MainDir => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Laggson Softworks";
        private static string UpdateDir => MainDir + @"\Shared";
        
        public static void InstallUpdater()
        {
            if (!Directory.Exists(UpdateDir))
                Directory.CreateDirectory(UpdateDir);

            if (!File.Exists(UpdateDir + @"Updater.exe"))
                RetrieveUpdater();

        }

        public static void InstallAndLaunchUpdater(string name, Version version, string path)
        {
            InstallUpdater();

            LaunchUpdater(name, version, path);
        }

        /// <summary>
        /// Überprüft die WebApi, ob eine neuere Version der Software verfügbar ist.
        /// Löst eine Ausnahme aus, falls die Client-Version neuer ist.
        /// </summary>
        /// <returns>True, wenn eine neuer Version vorhanden ist. Sonst false</returns>
        public static bool IsNewVersionAvailable(string name, Version version)
        {
            var cl = new WebClient();
            var result = cl.DownloadString(API_URL + "version/" + name);
            var newest = new Version(result);

            return newest.IsLargerThan(version);
        }

        /// <summary>
        /// Überprüft die WebApi, ob eine neuere Version der Software verfügbar ist.
        /// Löst eine Ausnahme aus, falls die Client-Version neuer ist.
        /// </summary>
        /// <returns>True, wenn eine neuer Version vorhanden ist. Sonst false</returns>
        public static bool IsNewVersionAvailable(string name, string version)
        {
            return IsNewVersionAvailable(name, new Version(version));
        }

        public static bool IsLargerThan(this Version maybeLargerVer, Version otherVer)
        {
            int compared = maybeLargerVer.CompareTo(otherVer);

            if (compared == -1)
                throw new ArgumentException("Du kannst doch keine neuere Version haben als der Server vallah.");

            return compared == 1;
        }

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
