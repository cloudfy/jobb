using Jobb.IO;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;

using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace Jobb.Schemas;

public sealed class SchemaGenerator
{
    public event EventHandler<GenerationProgressEventArgs>? GenerationProgress;

    private string TryGetDatabase(string connectionString)
    {
        var regex = new Regex("(database|initial catalog)=([a-zA-Z0-9_-]+)"
            , RegexOptions.IgnoreCase | RegexOptions.Compiled);

        var match = regex.Match(connectionString);
        if (match.Success)
            return match.Groups[match.Groups.Count - 1].Value;

        return null;
    }

    public async Task GenerateFile(JobbFile options)
    {
        var buffer = await Generate(options);

        File.WriteAllBytes(options.OutputFile, buffer);
    }
    public async Task<byte[]> Generate(JobbFile options, CancellationToken cancellationToken = default)
    {
        string databaseName = TryGetDatabase(options.ConnectionString);

        var cn = new SqlConnection(options.ConnectionString);
        
        try
        {
            await cn.OpenAsync(cancellationToken);

            var sc = new ServerConnection(cn);
            sc.Connect();

            Server server = new Server(sc);

            MemoryStream memoryStream = new MemoryStream();
            StreamWriter outputWriter = new StreamWriter(memoryStream);

            foreach (var db in server.Databases.Cast<Database>().AsQueryable().Where(o => o.IsSystemObject == false))
            {
                if (string.IsNullOrEmpty(databaseName) == false
                    && db.Name.Equals(databaseName, StringComparison.CurrentCultureIgnoreCase) == false)
                    continue;

                if (db.IsSystemObject)
                    continue;

                //    Console.WriteLine("Database: {0}", db.Name);

                if (options.ScriptOptions != null
                    && options.ScriptOptions.ScriptDatabase == true)
                {
                    /// *************************************************************
                    /// Database
                    await Helpers.WriteSQLInner<Database>(db.Name, "", "DB", db.Name, outputWriter, db, ScriptOption.Default);
                }
                if (options.ScriptOptions != null
                    && options.ScriptOptions.ScriptSchema == true)
                {
                    /// *************************************************************
                    /// Schema
                    foreach (var schema2 in db.Schemas.Cast<Schema>().AsQueryable())
                    {
                        //filePath = PrepareSqlFile(db.Name, "", "Schema", schema2.Name, currentPath, "");
                        await Helpers.WriteSQLInner<Schema>(db.Name, "", "Schema", schema2.Name, outputWriter, schema2, ScriptOption.Default);
                    }
                }

                GenerationProgress?.Invoke(this, new GenerationProgressEventArgs(1, 8));

                /// *************************************************************
                /// DB USER TYPES
                // currentPath = csFile.CreateFolder(dbPath, pathify("UTYPE"));
                foreach (UserDefinedType o in db.UserDefinedTypes)
                {
                    //filePath = PrepareSqlFile(db.Name, o.Schema, "UTYPE", o.Name, currentPath, "");
                    await Helpers.WriteSQLInner<UserDefinedType>(db.Name, o.Schema, "UTYPE", o.Name, outputWriter, o, ScriptOption.Default);
                }

                /// *************************************************************
                /// DB TRIGGERS
                //currentPath = csFile.CreateFolder(dbPath, pathify("TRIGGER"));
                foreach (DatabaseDdlTrigger o in db.Triggers.Cast<DatabaseDdlTrigger>().AsQueryable().Where(o => o.IsSystemObject == false))
                {
                    //filePath = PrepareSqlFile(db.Name, "dbo", "TRIGGER", o.Name, currentPath, "");
                    await Helpers.WriteSQLInner<DatabaseDdlTrigger>(db.Name, "dbo", "TRIGGER", o.Name, outputWriter, o, ScriptOption.Default);
                }

                GenerationProgress?.Invoke(this, new GenerationProgressEventArgs(2, 8));

                /// *************************************************************
                /// DB USER TABLE TYPES
                //currentPath = csFile.CreateFolder(dbPath, pathify("TTYPES"));
                foreach (UserDefinedTableType o in db.UserDefinedTableTypes)
                {
                    //filePath = PrepareSqlFile(db.Name, o.Schema, "TTYPES", o.Name, currentPath, "");
                    await Helpers.WriteSQLInner<UserDefinedTableType>(db.Name, o.Schema, "TTYPES", o.Name, outputWriter, o, ScriptOption.Default);
                }

                GenerationProgress?.Invoke(this, new GenerationProgressEventArgs(3, 8));

                /// *************************************************************
                /// DB FULLTEXT CATALOGS
                if (options.ExportOptions?.ExportFullText ?? false)
                {
                    //currentPath = csFile.CreateFolder(dbPath, pathify("FTC"));
                    foreach (FullTextCatalog o in db.FullTextCatalogs)
                    {
                        //filePath = PrepareSqlFile(db.Name, "dbo", "FTC", o.Name, currentPath, "");
                        await Helpers.WriteSQLInner<FullTextCatalog>(db.Name, "dbo", "FTC", o.Name, outputWriter, o, ScriptOption.Default);
                    }

                    /// *************************************************************
                    /// DB FULLTEXT STOPLISTS
                    //currentPath = csFile.CreateFolder(dbPath, pathify("FTL"));
                    foreach (FullTextStopList o in db.FullTextStopLists)
                    {
                        // filePath = PrepareSqlFile(db.Name, "dbo", "FTL", o.Name, currentPath, "");
                        await Helpers.WriteSQLInner<FullTextStopList>(db.Name, "dbo", "FTL", o.Name, outputWriter, o, ScriptOption.Default);
                    }
                }

                GenerationProgress?.Invoke(this, new GenerationProgressEventArgs(4, 8));

                /// *************************************************************
                /// STORED PROCEDURES
                //currentPath = csFile.CreateFolder(dbPath, pathify("PROCEDURE"));
                foreach (StoredProcedure o in db.StoredProcedures.Cast<StoredProcedure>().AsQueryable().Where(o => o.IsSystemObject == false))
                {
                    // filePath = PrepareSqlFile(db.Name, o.Schema, "PROCEDURE", o.Name, currentPath, "");
                    await Helpers.WriteSQLInner<StoredProcedure>(db.Name, o.Schema, "PROCEDURE", o.Name, outputWriter, o, ScriptOption.Default);
                }

                GenerationProgress?.Invoke(this, new GenerationProgressEventArgs(5, 8));

                /// *************************************************************
                /// FUNCTIONS
                // currentPath = csFile.CreateFolder(dbPath, pathify("FUNCTION"));
                foreach (UserDefinedFunction o in db.UserDefinedFunctions.Cast<UserDefinedFunction>().Where(oo => oo.IsSystemObject == false))
                {
                    // filePath = PrepareSqlFile(db.Name, o.Schema, "FUNCTION", o.Name, currentPath, "");
                    await Helpers.WriteSQLInner<UserDefinedFunction>(db.Name, o.Schema, "FUNCTION", o.Name, outputWriter, o, ScriptOption.Default);
                }

                GenerationProgress?.Invoke(this, new GenerationProgressEventArgs(6, 8));

                /// *************************************************************
                /// TABLE
                foreach (Table o in db.Tables.Cast<Table>().AsQueryable().Where(o => o.IsSystemObject == false))
                {

                    //currentPath = csFile.CreateFolder(dbPath, pathify("TABLE"));
                    //filePath = PrepareSqlFile(db.Name, o.Schema, "TABLE", o.Name, currentPath, "");
                    await Helpers.WriteSQLInner<Table>(db.Name, o.Schema, "TABLE", o.Name, outputWriter, o, ScriptOption.Default);
                    await Helpers.WriteSQLInner<Table>(db.Name, o.Schema, "TABLE", o.Name, outputWriter, o, ScriptOption.Indexes);
                    await Helpers.WriteSQLInner<Table>(db.Name, o.Schema, "TABLE", o.Name, outputWriter, o, ScriptOption.DriAll);


                    //////////////////////////////////////////////////////////////////////////
                    /// TABLE TRIGGERS
                    //currentPath = csFile.CreateFolder(dbPath, pathify("TRIGGER"));
                    foreach (Trigger ot in o.Triggers.Cast<Trigger>().AsQueryable().Where(oo => oo.IsSystemObject == false))
                    {
                        //filePath = PrepareSqlFile(db.Name, o.Schema, "TRIGGER", ot.Name, currentPath, "TABLE_" + o.Name);
                        await Helpers.WriteSQLInner<Trigger>(db.Name, o.Schema, "TRIGGER", ot.Name, outputWriter, ot, ScriptOption.Default);
                    }

                    //////////////////////////////////////////////////////////////////////////
                    /// TABLE STATISTICS
                    if (options.ExportOptions?.ExportStatistics ?? false)
                    {
                        //currentPath = csFile.CreateFolder(dbPath, pathify("STATISTIC"));
                        foreach (Statistic ot in o.Statistics.Cast<Statistic>().AsQueryable())
                        {
                            //filePath = PrepareSqlFile(db.Name, o.Schema, "STATISTIC", ot.Name, currentPath, "TABLE_" + o.Name);
                            await Helpers.WriteSQLInner<Statistic>(db.Name, o.Schema, "STATISTIC", ot.Name, outputWriter, ot, ScriptOption.OptimizerData);
                        }
                    }
                }

                GenerationProgress?.Invoke(this, new GenerationProgressEventArgs(7, 8));

                //////////////////////////////////////////////////////////////////////////
                // VIEWS
                if (options.ExportOptions?.ExportViews ?? true)
                {
                    foreach (View o in db.Views.Cast<View>().AsQueryable().Where(o => o.IsSystemObject == false))
                    {

                        // currentPath = csFile.CreateFolder(dbPath, pathify("VIEW"));
                        // filePath = PrepareSqlFile(db.Name, o.Schema, "VIEW", o.Name, currentPath, "");
                        await Helpers.WriteSQLInner<View>(db.Name, o.Schema, "VIEW", o.Name, outputWriter, o, ScriptOption.Default);
                        await Helpers.WriteSQLInner<View>(db.Name, o.Schema, "VIEW", o.Name, outputWriter, o, ScriptOption.Indexes);
                        await Helpers.WriteSQLInner<View>(db.Name, o.Schema, "VIEW", o.Name, outputWriter, o, ScriptOption.DriAllConstraints);

                        //////////////////////////////////////////////////////////////////////////
                        //VIEW TRIGGERS
                        //currentPath = csFile.CreateFolder(dbPath, pathify("TRIGGER"));
                        foreach (Trigger ot in o.Triggers.Cast<Trigger>().AsQueryable().Where(oo => oo.IsSystemObject == false))
                        {
                            //filePath = PrepareSqlFile(db.Name, o.Schema, "TRIGGER", ot.Name, currentPath, "VIEW_" + o.Name);
                            await Helpers.WriteSQLInner<Trigger>(db.Name, o.Schema, "TRIGGER", ot.Name, outputWriter, ot, ScriptOption.Default);
                        }

                        //////////////////////////////////////////////////////////////////////////
                        //VIEW STATISTICS
                        if (options.ExportOptions?.ExportStatistics ?? false)
                        {
                            //currentPath = csFile.CreateFolder(dbPath, pathify("STATISTIC"));
                            foreach (Statistic ot in o.Statistics.Cast<Statistic>().AsQueryable())
                            {
                                //filePath = PrepareSqlFile(db.Name, o.Schema, "STATISTIC", ot.Name, currentPath, "VIEW_" + o.Name);
                                await Helpers.WriteSQLInner<Statistic>(db.Name, o.Schema, "STATISTIC", ot.Name, outputWriter, ot, ScriptOption.OptimizerData);
                            }
                        }
                    }
                }

                GenerationProgress?.Invoke(this, new GenerationProgressEventArgs(8, 8));

            }

            await outputWriter.FlushAsync();

            memoryStream.Position = 0;
            var returnValue = memoryStream.ToArray();

            outputWriter.Dispose();
            memoryStream.Dispose();

            return returnValue;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (cn.State == System.Data.ConnectionState.Open)
            {
                cn.Close();
            }
        }
    }
}
