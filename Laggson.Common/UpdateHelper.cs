using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Laggson.Common
{
   public static class UpdateHelper
   {
      private const string API_URL = "http://h2608125.stratoserver.net:5000/api/";
      private static string MainDir => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Laggson Softworks";
      private static string UpdateDir => MainDir + @"\Common";

      /// <summary>
      /// Prüft, ob das Verzeichnis und die Datei des Updaters vorhanden sind und erstellt sie andernfalls.
      /// </summary>
      public static void InstallUpdater()
      {
         if (!Directory.Exists(UpdateDir))
            Directory.CreateDirectory(UpdateDir);

         if (!File.Exists(UpdateDir + @"Updater.exe"))
            RetrieveUpdater();

      }

      /// <summary>
      /// Installiert den Updater, falls dieser nicht vorhanden ist und startet ihn danach mit den angegebenen Argumenten.
      /// </summary>
      /// <param name="name"></param>
      /// <param name="version"></param>
      /// <param name="path"></param>
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

         if (string.IsNullOrEmpty(result.Trim()))
            return false;

         var newest = new Version(result);

         return newest.IsLargerThan(version);
      }

      /// <summary>
      /// Überprüft die WebApi, ob eine neuere Version der Software verfügbar ist.
      /// Löst eine Ausnahme aus, falls die Client-Version neuer ist.
      /// </summary>
      /// <returns>True, wenn eine neuer Version vorhanden ist. Sonst false</returns>
      public static async Task<bool> IsNewVersionAvailableAsync(string name, Version version)
      {
         var cl = new WebClient();
         var result = await cl.DownloadStringTaskAsync(API_URL + "version/" + name);

         if (string.IsNullOrEmpty(result.Trim()))
            return false;

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

      /// <summary>
      /// Prüft, ob die Version größer ist als die übergebene und löst eine Ausnahme aus, wenn sie kleiner ist.
      /// </summary>
      /// <param name="maybeLargerVer"></param>
      /// <param name="otherVer"></param>
      /// <returns></returns>
      public static bool IsLargerThan(this Version maybeLargerVer, Version otherVer)
      {
         int compared = maybeLargerVer.CompareTo(otherVer);

         if (compared == -1)
            throw new ArgumentException("Du kannst doch keine neuere Version haben als der Server vallah.");

         return compared == 1;
      }

      /// <summary>
      /// Versucht, die aktuelle Version des Updaters herunterzuladen und legt sie im localappdata-Verzeichnis ab.
      /// </summary>
      private static void RetrieveUpdater()
      {
         string updFilePath = "http://h2608125.stratoserver.net:5000/Content/Updater.exe";

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

      /// <summary>
      /// Startet den Update-Prozess und übergibt die angegebenen Daten.
      /// </summary>
      /// <param name="name">Der Name der Anwendung, nach der gesucht wird.</param>
      /// <param name="version">Die aktuelle Client-Version der Anwendung.</param>
      /// <param name="path">Der Pfad, in den die Dateien übertragen werden sollen.</param>
      /// <exception cref="ArgumentException">Bei ungültigen Parametern.</exception>
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
