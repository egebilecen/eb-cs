using System;
using System.IO;

namespace EB_Utility
{
    public static class Logger
    {
        public const string LogFile = ".\\program.log";
        
        public static string GetLoggingDate()
        {
            return DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss") + " UTC";
        }
        
        public static void Log(string text, string filename = null)
        {
            string ex_msg = text +
                            "\nDate: "+GetLoggingDate() +
                            "\n---------------\n";
            File.AppendAllText(filename ?? LogFile, ex_msg);
        }

        public static void LogException(Exception ex, string additional_msg="", string filename = null)
        {
            string ex_msg = "Exception: "+ex.GetType().FullName +
                            "\nMessage: "+ex.Message +
                            "\nStack trace: "+ex.StackTrace.Trim() +
                            "\nDate: "+GetLoggingDate() +
                            (additional_msg != "" ? "\n"+additional_msg : "") +
                            "\n---------------\n";
            File.AppendAllText(filename ?? LogFile, ex_msg);
        }
    }
}
