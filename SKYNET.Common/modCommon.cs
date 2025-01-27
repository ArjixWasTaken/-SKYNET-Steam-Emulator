﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public partial class Common
{
    public static Bitmap UpdatedAvatar { get; set; }

    public static void EnsureDirectoryExists(string filePath, bool isFile = false)
    {
        if (!string.IsNullOrEmpty(filePath))
        {
            filePath = filePath.Trim().Replace("\0", string.Empty);
            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    string text = isFile ? Path.GetDirectoryName(filePath) : filePath;
                    if (Path.IsPathRooted(filePath))
                    {
                        text = text.Trim();
                        if (!Directory.Exists(text))
                        {
                            Directory.CreateDirectory(text);
                        }
                    }
                }
                catch (Exception exception)
                {

                }
            }
        }
    }

    public static int GetRandom()
    {
        return new Random().Next(1, 9999);
    }

    public static string GetPath()
    {
        Process currentProcess;
        try
        {
            currentProcess = Process.GetCurrentProcess();
            return new FileInfo(currentProcess.MainModule.FileName).Directory?.FullName;
        }
        finally { currentProcess = null; }
    }

    public static string GetRootPath(string executablePath)
    {
        return new FileInfo(executablePath).Directory?.FullName;
    }

    public static bool Is64Bit => IntPtr.Size == 8;

    public static bool ShowShadow { get; set; }
    public static int BrowserHandle { get; set; }

    public static void OpenFolderAndSelectFile(string filePath)
    {
        if (filePath == null)
            return;

        IntPtr pidl = ILCreateFromPathW(filePath);
        SHOpenFolderAndSelectItems(pidl, 0, IntPtr.Zero, 0);
        ILFree(pidl);
    }
    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr ILCreateFromPathW(string pszPath);

    [DllImport("shell32.dll")]
    private static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder, int cild, IntPtr apidl, int dwFlags);

    [DllImport("shell32.dll")]
    private static extern void ILFree(IntPtr pidl);

    public static DialogResult Show(object msg, MessageBoxButtons buttons = MessageBoxButtons.OK)
    {
        return MessageBox.Show(msg.ToString(), "SKYNET", buttons);
    }

    public static void InvokeAction(Control control, Action Action)
    {
        if (control.InvokeRequired)
        {
            control.Invoke(Action);
        }
        else
        {
            Action();
        }
    }
}