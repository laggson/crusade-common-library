using System;
using System.Runtime.InteropServices;

namespace Laggson.Common.Notifications
{
    // https://www.whitebyte.info/programming/c/how-to-make-a-notification-from-a-c-desktop-application-in-windows-10
    internal class ErrorHelper
    {
        public static void VerifySucceeded(uint hresult)
        {
            if (hresult > 1)
            {
                throw new Exception("Failed with HRESULT: " + hresult.ToString("X"));
            }
        }
    }


    [ComImport,
     Guid(ShellIidGuid.C_SHELL_LINK),
     ClassInterface(ClassInterfaceType.None)]
    internal class ShellLink
    {
    }

    internal static class ShellIidGuid
    {
        internal const string SHELL_LINK_W = "000214F9-0000-0000-C000-000000000046";
        internal const string C_SHELL_LINK = "00021401-0000-0000-C000-000000000046";
        internal const string PERSIST_FILE = "0000010b-0000-0000-C000-000000000046";
        internal const string PROPERTY_STORE = "886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99";
    }

    internal enum Stgm : long
    {
        STGM_READ = 0x00000000L,
        STGM_WRITE = 0x00000001L,
        STGM_READWRITE = 0x00000002L,
        STGM_SHARE_DENY_NONE = 0x00000040L,
        STGM_SHARE_DENY_READ = 0x00000030L,
        STGM_SHARE_DENY_WRITE = 0x00000020L,
        STGM_SHARE_EXCLUSIVE = 0x00000010L,
        STGM_PRIORITY = 0x00040000L,
        STGM_CREATE = 0x00001000L,
        STGM_CONVERT = 0x00020000L,
        STGM_FAILIFTHERE = 0x00000000L,
        STGM_DIRECT = 0x00000000L,
        STGM_TRANSACTED = 0x00010000L,
        STGM_NOSCRATCH = 0x00100000L,
        STGM_NOSNAPSHOT = 0x00200000L,
        STGM_SIMPLE = 0x08000000L,
        STGM_DIRECT_SWMR = 0x00400000L,
        STGM_DELETEONRELEASE = 0x04000000L,
    }
}