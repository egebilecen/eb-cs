// NuGet Packet Requirements:
// [+] System.Data.SQLite by SQLite Development Team

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;

public class SQLite
{
    private SQLiteConnection conn;

    public SQLite(string dbFilePath, string password=null, uint version=3, bool readOnly=false, bool utf16=false, bool createDBIfMissing=false)
    {
        string connectionStr = $"Data Source={dbFilePath};Version={version};";
        if(!createDBIfMissing) connectionStr += "FailIfMissing=True;";
        if(readOnly) connectionStr += "Read Only=True;";
        if(utf16) connectionStr += "UseUTF16Encoding=True;";

        conn = new SQLiteConnection(connectionStr);
        conn.Open();
        conn.Close();
    }

    public DataTable Query(string query, List<object> paramList=null, Action<SQLiteException> errorHandler = null)
    {
        DataTable table = null;

        try
        {
            conn.Open();
            table = new DataTable();

            SQLiteCommand cmd = new SQLiteCommand(query, conn);

            if(paramList != null)
            {
                int i=0;
                paramList.ForEach(e =>
                {
                    cmd.Parameters.AddWithValue($"param{++i}", e);
                });
            }

            SQLiteDataReader reader = cmd.ExecuteReader();
            table.Load(reader);
        }
        catch(SQLiteException err)
        {
            errorHandler?.Invoke(err);
            return null;
        }
        finally { conn.Close(); }

        conn.Close();
        return table;
    }

    // Returns number of rows affected by an SELECT, INSERT, UPDATE or DELETE.
    public int QueryRowCount(string query, List<object> paramList=null, Action<SQLiteException> errorHandler = null)
    {
        int affectedRows = 0;

        try
        {
            conn.Open();

            SQLiteCommand cmd = new SQLiteCommand(query, conn);
            if(paramList != null)
            {
                int i=0;
                paramList.ForEach(e =>
                {
                    cmd.Parameters.AddWithValue($"param{++i}", e);
                });
            }

            affectedRows = cmd.ExecuteNonQuery();
        }
        catch(SQLiteException err)
        {
            errorHandler?.Invoke(err);
            return 0;
        }
        finally { conn.Close(); }

        conn.Close();
        return affectedRows;
    }
}
