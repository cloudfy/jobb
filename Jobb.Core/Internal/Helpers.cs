using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobb
{
    internal static class Helpers 
    { 
        internal static bool WriteSQLInner<T>(
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


            System.Collections.Specialized.StringCollection cs = new System.Collections.Specialized.StringCollection();
            try
            {
                cs = (o as dynamic).Script(so);
            }
            catch (Exception)
            {
                throw;
                //if (UseDAC)
                //{
                //    try
                //    {
                //        DB.ChangeDB(db);
                //        var dt = DB.GetDecryptedObject(objName, objType);
                //        cs.Clear();
                //        cs.Add(dt.Rows[0]["script"].ToString());
                //    }
                //    catch (Exception ex2)
                //    {
                //        Console.WriteLine(ex2.Message);
                //        return false;
                //    }
                //}
                //else
                //{
                //    Console.WriteLine(ex.Message);
                //    return false;
                //}
            }

            if (cs != null)
            {
                var ts = "";
                foreach (var s in cs)
                    ts += s + Environment.NewLine;
                if (!String.IsNullOrWhiteSpace(ts.Trim()))
                {
                   // if (!File.Exists(filePath))
                        SqlSchema.Write(writer, SqlComments(db, schema, objType, objName), true);

                    SqlSchema.Write(writer, ts + ";" + Environment.NewLine, true);
                }
            }

            return true;
        }


        private static string SqlComments(string db, string schema, string type, string name)
        {
            var s = "--****************************************************" + Environment.NewLine;
            s += "--Jobb v1" + Environment.NewLine;
            s += "--Export database schema." + Environment.NewLine;
            s += "-------------------------------------------------------" + Environment.NewLine;
            s += "--DB: " + db + Environment.NewLine;
            s += "--SCHEMA: " + schema + Environment.NewLine;
            s += "--" + type + ": " + name + Environment.NewLine;
            s += "--" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + Environment.NewLine;
            s += "--****************************************************" + Environment.NewLine + Environment.NewLine;
            return s;
        }
    }
}
