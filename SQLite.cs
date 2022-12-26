// NuGet Packet Requirements:
// [+] System.Data.SQLite by SQLite Development Team

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;

public class SQLite
{
    private readonly SQLiteConnection conn;
    private readonly int commandTimeout;

    public SQLite(string dbFilePath, int connectionTimeout=0, int commandTimeout=5, string password=null, uint version=3, bool readOnly=false, bool utf16=false, bool createDBIfMissing=false)
    {
        string connectionStr = $"Data Source={dbFilePath};Version={version};";
        if(connectionTimeout > 0) connectionStr += $"Connection Timeout={connectionTimeout};";
        if(password != null)      connectionStr += $"Password={password};";
        if(readOnly)              connectionStr += "Read Only=True;";
        if(utf16)                 connectionStr += "UseUTF16Encoding=True;";
        if(!createDBIfMissing)    connectionStr += "FailIfMissing=True;";

        this.commandTimeout = commandTimeout;

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

    public DataRowCollection QueryRows(string query, List<object> paramList=null, Action<SQLiteException> errorHandler = null)
    {
        return Query(query, paramList, errorHandler).Rows;
    }

    public int QueryRowCount(string query, List<object> paramList=null, Action<SQLiteException> errorHandler = null)
    {
        return QueryRowCount(query, paramList, out _, errorHandler);
    }

    public int QueryRowCountLastInsertId(string query, List<object> paramList, out long lastInsertId, Action<SQLiteException> errorHandler = null)
    {
        return QueryRowCount(query, paramList, out lastInsertId, errorHandler);
    }

    // Returns number of rows affected by an INSERT, UPDATE or DELETE and last inserted row id (if it's the case) with "out" parameter.
    private int QueryRowCount(string query, List<object> paramList, out long lastInsertId, Action<SQLiteException> errorHandler = null)
    {
        int affectedRows = 0;

        try
        {
            conn.Open();

            SQLiteCommand cmd = new SQLiteCommand(query, conn)
            {
                CommandTimeout = commandTimeout
            };

            if (paramList != null)
            {
                int i=0;
                paramList.ForEach(e =>
                {
                    cmd.Parameters.AddWithValue($"param{++i}", e);
                });
            }

            affectedRows = cmd.ExecuteNonQuery();
            lastInsertId = conn.LastInsertRowId;
        }
        catch(SQLiteException err)
        {
            errorHandler?.Invoke(err);
            lastInsertId = -1;
            return 0;
        }
        finally { conn.Close(); }

        conn.Close();
        return affectedRows;
    }
}
