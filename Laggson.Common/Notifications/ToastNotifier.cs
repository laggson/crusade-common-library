using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using IWshRuntimeLibrary;

namespace Laggson.Common.Notifications
{
   // Größtenteils https://www.whitebyte.info/programming/c/how-to-make-a-notification-from-a-c-desktop-application-in-windows-10
   public class ToastNotifier
   {
      /// <summary>
      /// Enthält den Namen der Anwendung, die die Toasts verwendet.
      /// </summary>
      private static string ApplicationName { get; set; }
      /// <summary>
      /// Gibt an, ob die Klasse bereits initialisiert wurde.
      /// </summary>
      public static bool IsInitialized { get; private set; }

      /// <summary>
      /// Initialisiert die Klasse für die angegebene Application.
      /// </summary>
      /// <param name="applicationName">Der Name der zu registrierenden Anwendung. (z.B. Microsoft.Samples.DesktopToastsSample)</param>
      /// <param name="iconPath">Der Pfad zur ICO-Datei, die in der Startmenü-Verknüpfung hinterlegt werden soll.</param>
      public static void Init(string applicationName, string iconPath = "")
      {
         ApplicationName = applicationName;
         IsInitialized = true;

         // War mal nicht nötig. 1709 der Sack.
         TryCreateShortcut(iconPath);
      }

      /// <summary>
      /// Versucht, einen Shortcut im Startmenü zu erstellen.
      /// </summary>
      private static void TryCreateShortcut(string iconPath)
      {
         var shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
             + "\\Microsoft\\Windows\\Start Menu\\Programs\\Laggson Softworks\\" + ApplicationName + ".lnk"; //

         var directory = Path.GetDirectoryName(shortcutPath) ?? "";

         if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

         if (!System.IO.File.Exists(shortcutPath))
         {
            InstallShortcut(shortcutPath, iconPath);
         }
      }

      private static void InstallShortcut(string shortcutPath, string iconPath)
      {
         string destinationPath = Process.GetCurrentProcess().MainModule.FileName;
         string description = "Eine Verknüpfung zu einer Laggson SoftWorks-Produktion.";

         WshShell wsh = new WshShell();

         if (!(wsh.CreateShortcut(shortcutPath) is IWshShortcut shortcut))
            throw new ArgumentNullException(nameof(wsh));

         shortcut.Arguments = "";
         shortcut.TargetPath = destinationPath;
         // not sure about what this is for
         shortcut.WindowStyle = 1;
         shortcut.Description = description;
         shortcut.WorkingDirectory = Path.GetDirectoryName(destinationPath);
         shortcut.IconLocation = iconPath;
         shortcut.Save();
      }

      /// <summary>
      /// Bastelt sich den <see cref="ToastTemplateType"/> zusammmen, der zur message passt und schmeißt was, wenn die message leer ist.
      /// </summary>
      /// <param name="message">Die anzuzeigende Nachricht.</param>
      /// <exception cref="ArgumentException">Falls der Titel von <paramref name="message"/> leer ist.</exception>
      /// <exception cref="ArgumentNullException">Falls <paramref name="message"/> null ist.</exception>
      /// <returns>Den passenden <see cref="ToastTemplateType"/></returns>
      private static ToastTemplateType GetTemplateTypeForMessage(MessageItem message)
      {
         if (message == null)
            throw new ArgumentNullException(nameof(message), "Brudi, was kaputt mit dir?");

         if (string.IsNullOrEmpty(message.Header))
            throw new ArgumentException("Der Titel der Nachricht darf nicht leer sein.", nameof(message));

         if (string.IsNullOrEmpty(message.ImagePath))
         {
            if (string.IsNullOrEmpty(message.Message1))
            {
               return ToastTemplateType.ToastText01;
            }
            else
            {
               return string.IsNullOrEmpty(message.Message2) 
                  ? ToastTemplateType.ToastText02 
                  : ToastTemplateType.ToastText04;
            }
         }
         else
         {
            if (string.IsNullOrEmpty(message.Message1))
            {
               return ToastTemplateType.ToastImageAndText01;
            }
            else
            {
               return string.IsNullOrEmpty(message.Message2)
                  ? ToastTemplateType.ToastImageAndText02
                  : ToastTemplateType.ToastImageAndText04;
            }
         }
      }

      /// <summary>
      /// Zeigt eine Windows-10-Toast-Notification mit dem Inhalt der angegebenen <paramref name="message"/> aus.
      /// </summary>
      /// <exception cref="InvalidOperationException">Falls nicht vorher die <see cref="Init(string,string)"/>-Methode aufgerufen wurde.</exception>
      /// <param name="message">Die anzuzeigende Nachricht.</param>
      public static void Show(MessageItem message)
      {
         if (!IsInitialized)
            throw new InvalidOperationException("Du musst zuerst die Init-Methode callen.");

         var templateType = GetTemplateTypeForMessage(message);

         XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(templateType);

         XmlNodeList stringElements = toastXml.GetElementsByTagName("text");

         string templateString = templateType.ToString();
         int range = int.Parse(templateString.Last().ToString());
         stringElements[0].AppendChild(toastXml.CreateTextNode(message.Header));

         if (range >= 2)
            stringElements[1].AppendChild(toastXml.CreateTextNode(message.Message1));

         if (range >= 4)
            stringElements[2].AppendChild(toastXml.CreateTextNode(message.Message2));

         if (!string.IsNullOrEmpty(message.ImagePath))
         {
            XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
            imageElements[0].Attributes.GetNamedItem("src").NodeValue = message.ImagePath;
         }

         ToastNotification toast = new ToastNotification(toastXml);
         toast.Activated += Toast_Activated;
         toast.Dismissed += Toast_Dismissed;
         toast.Failed += Toast_Failed;

         ToastNotificationManager.CreateToastNotifier(ApplicationName).Show(toast);
      }

      public static event EventHandler ToastClicked;
      public static event DismissedEventHandler ToastDismissed;
      public static event FailedEventHandler ToastFailed;

      private static void Toast_Activated(ToastNotification sender, object args)
      {
         ToastClicked?.Invoke(sender, EventArgs.Empty);
      }

      private static void Toast_Dismissed(ToastNotification sender, ToastDismissedEventArgs args)
      {
         ToastDismissed?.Invoke(sender, (ToastDismissalReason) (int) args.Reason);
      }

      private static void Toast_Failed(ToastNotification sender, ToastFailedEventArgs args)
      {
         ToastFailed?.Invoke(sender, args.ErrorCode);
      }
   }
}