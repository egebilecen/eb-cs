// Determine which excel library will be used
#define MICROSOFT_OFFICE_EXCEL
//#define NPOI_EXCEL

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

#if MICROSOFT_OFFICE_EXCEL
using MExcel = Microsoft.Office.Interop.Excel;
#endif

// Type defines
using SettingPair = System.Collections.Generic.KeyValuePair<string, string>;

namespace EB_Utility
{
    public static class Logging
    {
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

    public static class Settings
    {
        public static string settings_file = "settings.eb";
        private static List<SettingPair> settings = new List<SettingPair>();

		private static List<SettingPair> default_settings()
        {
            return new List<SettingPair>()
            {
                new SettingPair("last_page", null),
                new SettingPair("excel_visible", "0")
            };

            //return null;
        }
		
        public static void load_settings()
        {
            if(!File.Exists(settings_file))
            {
                File.Create(settings_file).Close();

                List<SettingPair> default_settings = Settings.default_settings();

                if(default_settings != null)
                {
                    for(int i=0; i < default_settings.Count; i++)
                        set_setting(default_settings[i].Key, default_settings[i].Value, true);
                }

                return;
            }

            settings.Clear();

            string[] settings_content = File.ReadAllLines(settings_file);

            for(int i=0; i < settings_content.Length; i++)
            {
                string   setting = settings_content[i];
                string[] setting_split = setting.Split(new char[] { '=' }, 2);

                SettingPair pair = new SettingPair(setting_split[0], setting_split[1]);

                settings.Add(pair);
            }
        }

        public static T get_setting<T>(string key)
        {
            for(int i=0; i < settings.Count; i++)
            {
                SettingPair pair = settings[i];

                if(pair.Key   == key
                && pair.Value != "")
                {
                    if(typeof(T) == typeof(string))
                        return (T)(object) pair.Value;
                    if(typeof(T) == typeof(int))
                        return (T)(object) int.Parse(pair.Value);
                    if(typeof(T) == typeof(float))
                        return (T)(object) float.Parse(pair.Value); 
                    if(typeof(T) == typeof(double))
                        return (T)(object) double.Parse(pair.Value); 
                    if(typeof(T) == typeof(long))
                        return (T)(object) long.Parse(pair.Value);

                    throw new FormatException("Unsupported type "+(typeof(T)).ToString());
                }
            }

            return default;
        }

        public static void set_setting(string key, object value, bool add_if_not_exist=false)
        {
            if(value == null) value = "";
            else value = value.ToString();

            List<string> settings_content = new List<string>(File.ReadAllLines(settings_file));

            for(int i=0; i < settings.Count; i++)
            {
                SettingPair setting = settings[i];

                if(setting.Key == key)
                {
                    settings_content[i] = key + "=" + value;
                    File.WriteAllLines(settings_file, settings_content.ToArray());
                    
                    load_settings();
                    return;
                }
            }

            if(add_if_not_exist)
            {
                settings_content.Add(key + "=" + value);
                File.WriteAllLines(settings_file, settings_content.ToArray());
                    
                load_settings();
                return;
            }
        }
    }

#if MICROSOFT_OFFICE_EXCEL
    public static class Excel
	{
		/*
			[Code Example]
			if(!File.Exists(input_filename.Text+".xlsx"))
            {
                excel_app = create_empty_excel_file(Directory.GetCurrentDirectory()+"\\"+input_filename.Text, false);
                excel_wb  = excel_app.Workbooks.get_Item(1);
            }
            else
            {
                excel_app = create_excel_application();
                excel_wb  = open_excel_file(excel_app, Directory.GetCurrentDirectory()+"\\"+input_filename.Text);
            }

            var excel_ws = excel_wb.Worksheets.get_Item(1);

            excel_ws.Cells[1, 1] = "Test";
            excel_ws.Cells[1, 2] = "123";
            excel_ws.Cells[2, 1] = "Hello";
            excel_ws.Cells[2, 2] = "World";

            append_to_worksheet(excel_ws, new string[] { "Append", "Example" });

            save_and_close_excel(excel_app, excel_wb);
		*/
		
		public static MExcel.Application create_empty_excel_file(string path, bool close_excel=true)
        {
            MExcel.Application excel_app = new MExcel.Application();
            excel_app.Visible = false;

            object misvalue = System.Reflection.Missing.Value;

            MExcel.Workbook  wb = excel_app.Workbooks.Add(misvalue);
            MExcel.Worksheet ws = wb.Worksheets.get_Item(1);

            wb.SaveAs(path, 
                        MExcel.XlFileFormat.xlOpenXMLWorkbook, 
                        misvalue, misvalue, misvalue, misvalue, 
                        MExcel.XlSaveAsAccessMode.xlExclusive,
                        misvalue, misvalue, misvalue, misvalue, misvalue);

            if(close_excel)
            {
				wb.Close(true, misvalue, misvalue);
                excel_app.Quit();
                return null;
            }

            return excel_app; // get the workbook or worksheet by using excel_app.Workbooks.get_Item(1);
        }

        public static MExcel.Application create_excel_application()
        {
            MExcel.Application excel_app = new MExcel.Application();
            excel_app.Visible = false;
            return excel_app;
        }

        public static MExcel.Workbook open_excel_file(MExcel.Application excel_app, string path)
        {
            return excel_app.Workbooks.Open(path);
        }

        public static void save_and_close_excel(MExcel.Application excel_app, MExcel.Workbook wb=null, MExcel.Worksheet ws=null)
        {
            try
            {
                if(wb != null)
                {
                    object misvalue = System.Reflection.Missing.Value;
                    wb.Close(true, misvalue, misvalue);
                    Marshal.ReleaseComObject(wb);
                }
            }
            catch (Exception) { }

            if(ws != null) Marshal.ReleaseComObject(ws);

            excel_app.Quit();
            Marshal.ReleaseComObject(excel_app);
        }
		
		public static int get_last_row_in_worksheet(MExcel.Worksheet ws)
        {
            return ws.Cells.SpecialCells(MExcel.XlCellType.xlCellTypeLastCell, Type.Missing).Row;
        }
		
		public static void append_to_worksheet(MExcel.Worksheet ws, string[] values, int offset_top=0, int offset_left=0)
        {
            int last_row = get_last_row_in_worksheet(ws);

            for(int i=0; i < values.Length; i++)
            {
                string value = values[i];
                ws.Cells[last_row + 1 + offset_top, i + 1 + offset_left] = value;
            }
        }
		
		public static void insert_image_to_cell(MExcel.Worksheet ws, string image_path, int row_index, int column_index, float width, float height)
        {
            MExcel.Range range = (MExcel.Range) ws.Cells[row_index, column_index];
            float left = (float) ((double) range.Left);
            float top  = (float) ((double) range.Top);
            
            ws.Shapes.AddPicture(image_path, 
                                    Microsoft.Office.Core.MsoTriState.msoFalse, 
                                    Microsoft.Office.Core.MsoTriState.msoCTrue,
                                    left,
                                    top,
                                    width, height);
        }
	}
#endif

    public static class Image
	{
		public static Bitmap base64_image_to_bitmap(string base64_image)
        {
            var bitmap_data   = Convert.FromBase64String(base64_image);
            var stream_bitmap = new MemoryStream(bitmap_data);
            var bitmap        = new Bitmap((Bitmap)System.Drawing.Image.FromStream(stream_bitmap));

            return bitmap;
        }
	}
    
    public static class Time
    {
        public static long get_timestamp_sec()
        {
            return (long) (DateTime.Now.ToUniversalTime() - new DateTime (1970, 1, 1)).TotalSeconds;
        }
        
        public static long get_timestamp_ms()
        {
            return (long) (DateTime.Now.ToUniversalTime() - new DateTime (1970, 1, 1)).TotalMilliseconds;
        }
    }
}
