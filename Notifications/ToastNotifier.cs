using MS.WindowsAPICodePack.Internal;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

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
        public static void Init(string applicationName)
        {
            ApplicationName = applicationName;
            IsInitialized = true;
            // Das Tutorial bestand drauf, geht aber auch ziemlich gut ohne..
            //TryCreateShortcut();
        }

        /// <summary>
        /// Versucht, einen Shortcut im Startmenü zu erstellen.
        /// </summary>
        private static void TryCreateShortcut()
        {
            var shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + "\\Microsoft\\Windows\\Start Menu\\Programs\\" + ApplicationName + ".lnk";

            if (!File.Exists(shortcutPath))
            {
                InstallShortcut(shortcutPath);
            }
        }

        // In order to display toasts, a desktop application must have a shortcut on the Start menu.
        // Also, an AppUserModelID must be set on that shortcut.
        // The shortcut should be created as part of the installer. The following code shows how to create
        // a shortcut and assign an AppUserModelID using Windows APIs. You must download and include the 
        // Windows API Code Pack for Microsoft .NET Framework for this code to function
        private static void InstallShortcut(string shortcutPath)
        {
            string exePath = Process.GetCurrentProcess().MainModule.FileName;
            var shortcut = (IShellLink)new ShellLink();

            ErrorHelper.VerifySucceeded(shortcut.SetPath(exePath));
            ErrorHelper.VerifySucceeded(shortcut.SetArguments(""));

            var shortcutProperties = (IPropertyStore)shortcut;

            using (var appId = new PropVariant(ApplicationName))
            {
                //ErrorHelper.VerifySucceeded(shortcutProperties.SetValue(SystemProperties.System.AppUserModel.ID, appId));
                ErrorHelper.VerifySucceeded(shortcutProperties.Commit());
            }

            IPersistFile shortcutSave = (IPersistFile)shortcut;
            ErrorHelper.VerifySucceeded(shortcutSave.Save(shortcutPath, true));
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
                throw new ArgumentNullException("Brudi, was kaputt mit dir?");

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
                    if (string.IsNullOrEmpty(message.Message2))
                    {
                        return ToastTemplateType.ToastText02;
                    }
                    else
                    {
                        return ToastTemplateType.ToastText04;
                    }
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
                    if (string.IsNullOrEmpty(message.Message2))
                    {
                        return ToastTemplateType.ToastImageAndText02;
                    }
                    else
                    {
                        return ToastTemplateType.ToastImageAndText04;
                    }
                }
            }
        }

        /// <summary>
        /// Zeigt eine Windows-10-Toast-Notification mit dem Inhalt der angegebenen <paramref name="message"/> aus.
        /// </summary>
        /// <exception cref="InvalidOperationException">Falls nicht vorher die <see cref="Init(string)"/>-Methode aufgerufen wurde.</exception>
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

            if(range >= 2)
                stringElements[1].AppendChild(toastXml.CreateTextNode(message.Message1));

            if(range >= 4)
                stringElements[2].AppendChild(toastXml.CreateTextNode(message.Message2));

            if (!string.IsNullOrEmpty(message.ImagePath))
            {
                XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
                imageElements[0].Attributes.GetNamedItem("src").NodeValue = message.ImagePath;
            }

            ToastNotification toast = new ToastNotification(toastXml);
            //toast.Activated += Toast_Activated;
            //toast.Dismissed += Toast_Dismissed;
            //toast.Failed += Toast_Failed;

            ToastNotificationManager.CreateToastNotifier(ApplicationName).Show(toast);
        }

        private static void Toast_Activated(ToastNotification sender, object args)
        {
            //throw new NotImplementedException();
        }

        private static void Toast_Dismissed(ToastNotification sender, ToastDismissedEventArgs args)
        {
            //throw new NotImplementedException();
        }

        private static void Toast_Failed(ToastNotification sender, ToastFailedEventArgs args)
        {
            //throw new NotImplementedException();
        }
    }
}