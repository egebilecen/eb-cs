// NuGet Packet Requirements:
// [+] NPOI by NPOI Contributors

using NPOI.XSSF.UserModel; // XLSX
using NPOI.HSSF.UserModel; // XLS
using NPOI.SS.UserModel;
using System.IO;
using System;

namespace EB_Utility
{
    public static class NPOI_Excel
    {
        public static IWorkbook OpenExcelFile(string path)
        {
            IWorkbook wb;
            
            using(FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                try 
                { wb = new XSSFWorkbook(fs); }
                catch(Exception) 
                { wb = null; }

                if(wb == null) wb = new HSSFWorkbook(fs);
            }

            return wb;
        }

        public static void SaveExcel(string path, IWorkbook wb)
        {
            using(FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                wb.Write(fs);
            }
        }

        public static IWorkbook CreateEmptyExcelFile(string path)
        {
            IWorkbook wb;

            if(path.LastIndexOf(".xlsx") != -1) wb = new XSSFWorkbook(); // xlsx
            else                                wb = new HSSFWorkbook(); // xls

            wb.CreateSheet();

            using(FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                wb.Write(fs);
            }

            return wb;
        }

        public static int GetLastRowInWorksheet(ISheet ws)
        {
            return ws.LastRowNum;
        }

        public static void AppentToWorksheet(ISheet ws, string[] values, int offset_top=0, int offset_left=0)
        {
            int last_row  = GetLastRowInWorksheet(ws);
            IRow curr_row = ws.CreateRow(last_row + 1 + offset_top);

            for(int i=0; i < values.Length; i++)
            {
                string value = values[i];
                curr_row.CreateCell(i + offset_left).SetCellValue(value);
            }
        }
    }
}
