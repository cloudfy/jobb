using Microsoft.SqlServer.Management.Smo;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Jobb;

internal static class Helpers 
{ 
    internal static async Task<bool> WriteSQLInner<T>(
        string db
        , string schema
        , string objType
        , string objName
        , TextWriter writer
        , T o
        , ScriptingOptions so
        , bool UseDAC = false) where T : SqlSmoObject
    {
        if (schema == "")
            schema = "dbo";
        if (db == "*")
            Console.WriteLine(objType + ": " + objName);
        else
            Console.WriteLine(objType + ": " + db + "." + schema + "." + objName + " (" + so.ToString() + ")");

        System.Collections.Specialized.StringCollection cs;
        try
        {
            so.ClusteredIndexes = true;
            so.ColumnStoreIndexes = true;
            
            cs = (o as dynamic).Script(so);
        }
        catch (Exception)
        {
            throw;
        }

        if (cs != null)
        {
            var ts = "";
            foreach (var s in cs)
                ts += s + Environment.NewLine;
            if (!String.IsNullOrWhiteSpace(ts.Trim()))
            {
                await SqlSchema.Write(writer, SqlComments(db, schema, objType, objName), true);

                await SqlSchema.Write(writer, ts + "GO" + Environment.NewLine, true);
            }
        }

        return true;
    }

    private static string SqlComments(string db, string schema, string type, string name, bool dateMark = false)
    {
        var s = "--****************************************************" + Environment.NewLine;
        s += "--Jobb v1" + Environment.NewLine;
        s += "--Export database schema." + Environment.NewLine;
        s += "-------------------------------------------------------" + Environment.NewLine;
        s += "--DB: " + db + Environment.NewLine;
        s += "--SCHEMA: " + schema + Environment.NewLine;
        s += "--" + type + ": " + name + Environment.NewLine;
        if (dateMark)
            s += "--" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + Environment.NewLine;
        s += "--****************************************************" + Environment.NewLine + Environment.NewLine;
        return s;
    }
}
