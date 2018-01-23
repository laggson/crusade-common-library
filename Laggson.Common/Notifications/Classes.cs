using System;

namespace Laggson.Common.Notifications
{
    // https://www.whitebyte.info/programming/c/how-to-make-a-notification-from-a-c-desktop-application-in-windows-10

    public enum ToastDismissalReason
    {
        UserCanceled = 0,
        ApplicationHidden = 1,
        TimedOut = 2
    }

    public delegate void DismissedEventHandler(object sender, ToastDismissalReason reason);
    public delegate void FailedEventHandler(object sender, Exception reason);
}