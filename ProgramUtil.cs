using System;
using System.Diagnostics;
using System.IO;
using IWshRuntimeLibrary;
using Microsoft.Win32;

namespace EB_Utility
{
    public static class ProgramUtil
    {
        public static string GetFileNameWithExtension()
        {
            return Path.GetFileName(GetFilePath());
        }

        public static string GetFileNameWithoutExtension()
        {
            return Path.GetFileNameWithoutExtension(GetFilePath());
        }

        public static string GetFilePath()
        {
            return Process.GetCurrentProcess().MainModule.FileName;
        }
        
        public static string GetFilePathWithoutFileName()
        {
            return Path.GetDirectoryName(GetFilePath());
        }

        // Project > References > Add Reference > COM > Windows Script Host Object Model
        public static void AddToStartup()
        {
            string startUpFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

            WshShell wshShell = new WshShell();
            IWshShortcut shortcut;
            shortcut = (IWshShortcut)wshShell.CreateShortcut(startUpFolderPath + "\\" + GetFileNameWithoutExtension() + ".lnk");

            shortcut.TargetPath = GetFilePath();
            shortcut.WorkingDirectory = Path.GetDirectoryName(GetFilePath());
            // shortcut.Description = appName;
            // shortcut.IconLocation = Application.StartupPath + @"\App.ico";
            shortcut.Save();
        }

        public static bool AddToContextMenu(string displayText, string args="", string iconPath="", string subKeyPath = "SOFTWARE\\Classes\\Directory\\Background\\shell\\")
        {
            string subKey = subKeyPath + displayText;
            if(Registry.CurrentUser.OpenSubKey(subKey) != null) return true;

            RegistryKey key = Registry.CurrentUser.CreateSubKey(subKey);

            if(key != null)
            {
                key.SetValue("", displayText);
                if(iconPath != "") key.SetValue("Icon", iconPath);

                RegistryKey commandKey = key.CreateSubKey("command");

                if(commandKey != null)
                {
                    commandKey.SetValue("", "\""+GetFilePath()+"\""+(args != "" ? " "+ args : ""));
                    commandKey.Close();
                }
                else
                {
                    key.Close();
                    return false;
                }
            }
            else
            {
                key.Close();
                return false;
            }

            key.Close();
            return true;
        }

        public static bool DeleteFromContextMenu(string displayText, string subKeyPath = "SOFTWARE\\Classes\\Directory\\Background\\shell\\")
        {
            string subKey = subKeyPath + displayText;

            Registry.CurrentUser.DeleteSubKeyTree(subKey);
            return Registry.CurrentUser.OpenSubKey(subKey) == null;
        }
        
        public static void OpenPathInExplorer(string path)
        {
            Process.Start(path);
        }
    }
}
