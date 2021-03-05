using System;
using System.IO;

namespace EB_Utility
{
    public static class Logger
    {
        public static string log_file   = "program_log.eb";
        public static string error_file = "error.eb";

        public static void log_exception(Exception ex, string additional_msg="")
        {
            string ex_msg = "Exception: "+ex.GetType().FullName +
                            "\nMessage: "+ex.Message +
                            "\nStack trace: "+ex.StackTrace.Trim() +
                            "\nDate: "+DateTime.Now.ToString("dd/MM/yyyy, HH:mm:ss") +
                            (additional_msg != "" ? "\n"+additional_msg : "") +
                            "\n---------------\n";
            File.AppendAllText(error_file, ex_msg);
        }
    }
}
