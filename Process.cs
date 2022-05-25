using System;
using System.Diagnostics;
using System.IO;
using IWshRuntimeLibrary;

namespace EB_Utility
{
    public static class ProcessUtil
    {
        // Project > References > Add Reference > COM > Windows Script Host Object Model
        public static void AddToStartup()
        {
            string appName = Process.GetCurrentProcess().ProcessName;
            string appPath = Process.GetCurrentProcess().MainModule.FileName;
            string startUpFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

            WshShell wshShell = new WshShell();
            IWshShortcut shortcut;
            shortcut = (IWshShortcut)wshShell.CreateShortcut(startUpFolderPath + "\\" + appName + ".lnk");

            shortcut.TargetPath = appPath;
            shortcut.WorkingDirectory = Path.GetDirectoryName(appPath);
            // shortcut.Description = appName;
            // shortcut.IconLocation = Application.StartupPath + @"\App.ico";
            shortcut.Save();
        }
    }
}
