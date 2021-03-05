﻿// NuGet Packet Requirements:
// [+] Microsoft.Office.Interop.Excel by Microsoft
// [+] MicrosoftOfficeCore by alexey.lukinov

/*
    -==[Usage Example]==-

	if(!File.Exists(input_filename.Text+".xlsx"))
    {
        excel_app = Excel.Microsoft.create_empty_excel_file(Directory.GetCurrentDirectory()+"\\"+input_filename.Text, false);
        excel_wb  = excel_app.Workbooks.get_Item(1);
    }
    else
    {
        excel_app = Excel.Microsoft.create_excel_application();
        excel_wb  = open_excel_file(excel_app, Directory.GetCurrentDirectory()+"\\"+input_filename.Text);
    }

    var excel_ws = excel_wb.Worksheets.get_Item(1);

    excel_ws.Cells[1, 1] = "Test";
    excel_ws.Cells[1, 2] = "123";
    excel_ws.Cells[2, 1] = "Hello";
    excel_ws.Cells[2, 2] = "World";

    append_to_worksheet(excel_ws, new string[] { "Append", "Example" });

    Excel.Microsoft.save_and_close_excel(excel_app, excel_wb);
*/

using System;
using System.Runtime.InteropServices;
using _Excel     = Microsoft.Office.Interop.Excel;
using _Microsoft = Microsoft;

namespace EB_Utility
{
    public static class Microsoft_Excel
    {
        public static bool is_excel_visible = false;

        public static _Excel.Application create_empty_excel_file(string path, bool close_excel=true)
        {
            _Excel.Application excel_app = new _Excel.Application();
            excel_app.Visible = is_excel_visible;

            object misvalue = System.Reflection.Missing.Value;

            _Excel.Workbook wb = excel_app.Workbooks.Add(misvalue);

            wb.SaveAs(path, 
                        _Excel.XlFileFormat.xlOpenXMLWorkbook, 
                        misvalue, misvalue, misvalue, misvalue, 
                        _Excel.XlSaveAsAccessMode.xlExclusive,
                        misvalue, misvalue, misvalue, misvalue, misvalue);

            if(close_excel)
            {
				wb.Close(true, misvalue, misvalue);
                excel_app.Quit();
                return null;
            }

            return excel_app;
        }

        public static _Excel.Application create_excel_application()
        {
            _Excel.Application excel_app = new _Excel.Application
            {
                Visible = is_excel_visible
            };

            return excel_app;
        }

        public static _Excel.Workbook open_excel_file(_Excel.Application excel_app, string path)
        {
            return excel_app.Workbooks.Open(path);
        }

        public static void save_and_close_excel(_Excel.Application excel_app, _Excel.Workbook wb=null, _Excel.Worksheet ws=null)
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
		
		public static int get_last_row_in_worksheet(_Excel.Worksheet ws)
        {
            return ws.Cells.SpecialCells(_Excel.XlCellType.xlCellTypeLastCell, Type.Missing).Row;
        }
		
		public static void append_to_worksheet(_Excel.Worksheet ws, string[] values, int offset_top=0, int offset_left=0)
        {
            int last_row = get_last_row_in_worksheet(ws);

            for(int i=0; i < values.Length; i++)
            {
                string value = values[i];
                ws.Cells[last_row + 1 + offset_top, i + 1 + offset_left] = value;
            }
        }
		
		public static void insert_image_to_cell(_Excel.Worksheet ws, string image_path, int row_index, int column_index, float width, float height)
        {
            _Excel.Range range = (_Excel.Range) ws.Cells[row_index, column_index];
            float left = (float) (double) range.Left;
            float top  = (float) (double) range.Top;
            
            ws.Shapes.AddPicture(image_path, 
                                    _Microsoft.Office.Core.MsoTriState.msoTrue, 
                                    _Microsoft.Office.Core.MsoTriState.msoCTrue,
                                    left,
                                    top,
                                    width, height);
        }
    }
}