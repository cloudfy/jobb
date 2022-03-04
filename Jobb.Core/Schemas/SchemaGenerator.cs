using Jobb.IO;
using Microsoft.Data.SqlClient;
using System;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.IO;
using System.Linq;

namespace Jobb.Schemas
{
    public sealed class SchemaGenerator
    {
        public void Generate(JobbFile options)
        {
            var cn = new SqlConnection(options.ConnectionString);

            try
            {
                cn.Open();
                cn.Close();
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine("ERROR!");
                Console.WriteLine(ex.Message);
                //Console.WriteLine("(Server:" + HOST + ", User:" + USER + ", PASS: " + PASS.Substring(0, 1) + (new String('*', PASS.Length - 2)) + PASS.Substring(PASS.Count() - 1, 1) + ")");
                Console.ReadKey();
                return;
            }

            var sc = new ServerConnection(cn);
            sc.Connect();

            Server server = new Server(sc);

            StreamWriter outputWriter = new StreamWriter(options.OutputFile);
            //var outputWriter = Console.Out;


            foreach (var db in server.Databases.Cast<Database>().AsQueryable().Where(o => o.IsSystemObject == false))
            {
                if (db.IsSystemObject)
                    continue;

                Console.WriteLine(db.Name);

                if (options.ScriptOptions?.ScriptDatabase ?? false) 
                { 
                    /// *************************************************************
                    /// Database
                    Jobb.Helpers.WriteSQLInner<Database>(db.Name, "", "DB", db.Name, outputWriter, db, ScriptOption.Default);
                }
                if (options.ScriptOptions?.ScriptSchema ?? false) 
                { 
                    /// *************************************************************
                    /// Schema
                    foreach (var schema2 in db.Schemas.Cast<Schema>().AsQueryable())
                    {
                        //filePath = PrepareSqlFile(db.Name, "", "Schema", schema2.Name, currentPath, "");
                        Jobb.Helpers.WriteSQLInner<Schema>(db.Name, "", "Schema", schema2.Name, outputWriter, schema2, ScriptOption.Default);
                    }
                }

                /// *************************************************************
                /// DB USER TYPES
                // currentPath = csFile.CreateFolder(dbPath, pathify("UTYPE"));
                foreach (UserDefinedType o in db.UserDefinedTypes)
                {
                    //filePath = PrepareSqlFile(db.Name, o.Schema, "UTYPE", o.Name, currentPath, "");
                    Jobb.Helpers.WriteSQLInner<UserDefinedType>(db.Name, o.Schema, "UTYPE", o.Name, outputWriter, o, ScriptOption.Default);
                }

                /// *************************************************************
                /// DB TRIGGERS
                //currentPath = csFile.CreateFolder(dbPath, pathify("TRIGGER"));
                foreach (DatabaseDdlTrigger o in db.Triggers.Cast<DatabaseDdlTrigger>().AsQueryable().Where(o => o.IsSystemObject == false))
                {
                    //filePath = PrepareSqlFile(db.Name, "dbo", "TRIGGER", o.Name, currentPath, "");
                    Jobb.Helpers.WriteSQLInner<DatabaseDdlTrigger>(db.Name, "dbo", "TRIGGER", o.Name, outputWriter, o, ScriptOption.Default);
                }

                /// *************************************************************
                /// DB USER TABLE TYPES
                //currentPath = csFile.CreateFolder(dbPath, pathify("TTYPES"));
                foreach (UserDefinedTableType o in db.UserDefinedTableTypes)
                {
                    //filePath = PrepareSqlFile(db.Name, o.Schema, "TTYPES", o.Name, currentPath, "");
                    Jobb.Helpers.WriteSQLInner<UserDefinedTableType>(db.Name, o.Schema, "TTYPES", o.Name, outputWriter, o, ScriptOption.Default);
                }

                /// *************************************************************
                /// DB FULLTEXT CATALOGS
                if (options.ExportOptions?.ExportFullText ?? false)
                {
                    //currentPath = csFile.CreateFolder(dbPath, pathify("FTC"));
                    foreach (FullTextCatalog o in db.FullTextCatalogs)
                    {
                        //filePath = PrepareSqlFile(db.Name, "dbo", "FTC", o.Name, currentPath, "");
                        Jobb.Helpers.WriteSQLInner<FullTextCatalog>(db.Name, "dbo", "FTC", o.Name, outputWriter, o, ScriptOption.Default);
                    }

                    /// *************************************************************
                    /// DB FULLTEXT STOPLISTS
                    //currentPath = csFile.CreateFolder(dbPath, pathify("FTL"));
                    foreach (FullTextStopList o in db.FullTextStopLists)
                    {
                        // filePath = PrepareSqlFile(db.Name, "dbo", "FTL", o.Name, currentPath, "");
                        Jobb.Helpers.WriteSQLInner<FullTextStopList>(db.Name, "dbo", "FTL", o.Name, outputWriter, o, ScriptOption.Default);
                    }
                }

                /// *************************************************************
                /// STORED PROCEDURES
                //currentPath = csFile.CreateFolder(dbPath, pathify("PROCEDURE"));
                foreach (StoredProcedure o in db.StoredProcedures.Cast<StoredProcedure>().AsQueryable().Where(o => o.IsSystemObject == false))
                {
                    // filePath = PrepareSqlFile(db.Name, o.Schema, "PROCEDURE", o.Name, currentPath, "");
                    Jobb.Helpers.WriteSQLInner<StoredProcedure>(db.Name, o.Schema, "PROCEDURE", o.Name, outputWriter, o, ScriptOption.Default);
                }

                /// *************************************************************
                /// FUNCTIONS
                // currentPath = csFile.CreateFolder(dbPath, pathify("FUNCTION"));
                foreach (UserDefinedFunction o in db.UserDefinedFunctions.Cast<UserDefinedFunction>().Where(oo => oo.IsSystemObject == false))
                {
                    // filePath = PrepareSqlFile(db.Name, o.Schema, "FUNCTION", o.Name, currentPath, "");
                    Jobb.Helpers.WriteSQLInner<UserDefinedFunction>(db.Name, o.Schema, "FUNCTION", o.Name, outputWriter, o, ScriptOption.Default);
                }

                /// *************************************************************
                /// TABLE
                foreach (Table o in db.Tables.Cast<Table>().AsQueryable().Where(o => o.IsSystemObject == false))
                {

                    //currentPath = csFile.CreateFolder(dbPath, pathify("TABLE"));
                    //filePath = PrepareSqlFile(db.Name, o.Schema, "TABLE", o.Name, currentPath, "");
                    Jobb.Helpers.WriteSQLInner<Table>(db.Name, o.Schema, "TABLE", o.Name, outputWriter, o, ScriptOption.Default);
                    Jobb.Helpers.WriteSQLInner<Table>(db.Name, o.Schema, "TABLE", o.Name, outputWriter, o, ScriptOption.Indexes);
                    Jobb.Helpers.WriteSQLInner<Table>(db.Name, o.Schema, "TABLE", o.Name, outputWriter, o, ScriptOption.DriAll);


                    //////////////////////////////////////////////////////////////////////////
                    /// TABLE TRIGGERS
                    //currentPath = csFile.CreateFolder(dbPath, pathify("TRIGGER"));
                    foreach (Trigger ot in o.Triggers.Cast<Trigger>().AsQueryable().Where(oo => oo.IsSystemObject == false))
                    {
                        //filePath = PrepareSqlFile(db.Name, o.Schema, "TRIGGER", ot.Name, currentPath, "TABLE_" + o.Name);
                        Jobb.Helpers.WriteSQLInner<Trigger>(db.Name, o.Schema, "TRIGGER", ot.Name, outputWriter, ot, ScriptOption.Default);
                    }

                    //////////////////////////////////////////////////////////////////////////
                    /// TABLE STATISTICS
                    if (options.ExportOptions?.ExportStatistics ?? false)
                    {
                        //currentPath = csFile.CreateFolder(dbPath, pathify("STATISTIC"));
                        foreach (Statistic ot in o.Statistics.Cast<Statistic>().AsQueryable())
                        {
                            //filePath = PrepareSqlFile(db.Name, o.Schema, "STATISTIC", ot.Name, currentPath, "TABLE_" + o.Name);
                            Jobb.Helpers.WriteSQLInner<Statistic>(db.Name, o.Schema, "STATISTIC", ot.Name, outputWriter, ot, ScriptOption.OptimizerData);
                        }
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                // VIEWS
                if (options.ExportOptions?.ExportViews ?? true)
                {
                    foreach (View o in db.Views.Cast<View>().AsQueryable().Where(o => o.IsSystemObject == false))
                    {

                        // currentPath = csFile.CreateFolder(dbPath, pathify("VIEW"));
                        // filePath = PrepareSqlFile(db.Name, o.Schema, "VIEW", o.Name, currentPath, "");
                        Jobb.Helpers.WriteSQLInner<View>(db.Name, o.Schema, "VIEW", o.Name, outputWriter, o, ScriptOption.Default);
                        Jobb.Helpers.WriteSQLInner<View>(db.Name, o.Schema, "VIEW", o.Name, outputWriter, o, ScriptOption.Indexes);
                        Jobb.Helpers.WriteSQLInner<View>(db.Name, o.Schema, "VIEW", o.Name, outputWriter, o, ScriptOption.DriAllConstraints);

                        //////////////////////////////////////////////////////////////////////////
                        //VIEW TRIGGERS
                        //currentPath = csFile.CreateFolder(dbPath, pathify("TRIGGER"));
                        foreach (Trigger ot in o.Triggers.Cast<Trigger>().AsQueryable().Where(oo => oo.IsSystemObject == false))
                        {
                            //filePath = PrepareSqlFile(db.Name, o.Schema, "TRIGGER", ot.Name, currentPath, "VIEW_" + o.Name);
                            Jobb.Helpers.WriteSQLInner<Trigger>(db.Name, o.Schema, "TRIGGER", ot.Name, outputWriter, ot, ScriptOption.Default);
                        }

                        //////////////////////////////////////////////////////////////////////////
                        //VIEW STATISTICS
                        if (options.ExportOptions?.ExportStatistics ?? false)
                        {
                            //currentPath = csFile.CreateFolder(dbPath, pathify("STATISTIC"));
                            foreach (Statistic ot in o.Statistics.Cast<Statistic>().AsQueryable())
                            {
                                //filePath = PrepareSqlFile(db.Name, o.Schema, "STATISTIC", ot.Name, currentPath, "VIEW_" + o.Name);
                                Jobb.Helpers.WriteSQLInner<Statistic>(db.Name, o.Schema, "STATISTIC", ot.Name, outputWriter, ot, ScriptOption.OptimizerData);
                            }
                        }
                    }
                }
            }

            outputWriter.Flush();
            outputWriter.Close();

        }
    }
}
