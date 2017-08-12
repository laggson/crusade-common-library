using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using MS.WindowsAPICodePack.Internal;

namespace Laggson.Common.Notifications
{
    [ComImport,
     Guid(ShellIidGuid.PERSIST_FILE),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPersistFile
    {
        uint GetCurFile(
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile
        );
        uint IsDirty();
        uint Load(
            [MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
            [MarshalAs(UnmanagedType.U4)] Stgm dwMode);
        uint Save(
            [MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
            bool fRemember);
        uint SaveCompleted(
            [MarshalAs(UnmanagedType.LPWStr)] string pszFileName);
    }
    
    [ComImport]
    [Guid(ShellIidGuid.PROPERTY_STORE)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPropertyStore
    {
        uint GetCount([Out] out uint propertyCount);
        uint GetAt([In] uint propertyIndex, out PropertyKey key);
        uint GetValue([In] ref PropertyKey key, [Out] PropVariant pv);
        uint SetValue([In] ref PropertyKey key, [In] PropVariant pv);
        uint Commit();
    }
    
    [ComImport,
     Guid(ShellIidGuid.SHELL_LINK_W),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IShellLink
    {
        uint GetPath(
            [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile,
            int cchMaxPath,
            //ref _WIN32_FIND_DATAW pfd,
            IntPtr pfd,
            uint fFlags);
        uint GetIDList(out IntPtr ppidl);
        uint SetIDList(IntPtr pidl);
        uint GetDescription(
            [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile,
            int cchMaxName);
        uint SetDescription(
            [MarshalAs(UnmanagedType.LPWStr)] string pszName);
        uint GetWorkingDirectory(
            [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir,
            int cchMaxPath
        );
        uint SetWorkingDirectory(
            [MarshalAs(UnmanagedType.LPWStr)] string pszDir);
        uint GetArguments(
            [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs,
            int cchMaxPath);
        uint SetArguments(
            [MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
        uint GetHotKey(out short wHotKey);
        uint SetHotKey(short wHotKey);
        uint GetShowCmd(out uint iShowCmd);
        uint SetShowCmd(uint iShowCmd);
        uint GetIconLocation(
            [Out(), MarshalAs(UnmanagedType.LPWStr)] out StringBuilder pszIconPath,
            int cchIconPath,
            out int iIcon);
        uint SetIconLocation(
            [MarshalAs(UnmanagedType.LPWStr)] string pszIconPath,
            int iIcon);
        uint SetRelativePath(
            [MarshalAs(UnmanagedType.LPWStr)] string pszPathRel,
            uint dwReserved);
        uint Resolve(IntPtr hwnd, uint fFlags);
        uint SetPath(
            [MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }
}