using System;
using System.IO;

namespace EB_Utility
{
    public static class Logger
    {
        public static void Log(string text, string filename = "program.log")
        {
            string ex_msg = text +
                            "\nDate: "+DateTime.Now.ToString("dd/MM/yyyy, HH:mm:ss") +
                            "\n---------------\n";
            File.AppendAllText(filename, ex_msg);
        }

        public static void LogException(Exception ex, string additional_msg="", string filename = "program.log")
        {
            string ex_msg = "Exception: "+ex.GetType().FullName +
                            "\nMessage: "+ex.Message +
                            "\nStack trace: "+ex.StackTrace.Trim() +
                            "\nDate: "+DateTime.Now.ToString("dd/MM/yyyy, HH:mm:ss") +
                            (additional_msg != "" ? "\n"+additional_msg : "") +
                            "\n---------------\n";
            File.AppendAllText(filename, ex_msg);
        }
    }
}
