namespace Jobb.IO;

public sealed class JobbFile
{
    public string ConnectionString { get; set; } = null!;
    /// <summary>
    /// Get full path
    /// </summary>
    public string? OutputFile { get; internal set; }
    /// <summary>
    /// Get filename
    /// </summary>
    public string? OutputFileName { get; internal set; }
    public ExportOptionsSection? ExportOptions { get; set; }
    public ScriptOptionsSection? ScriptOptions { get; set; }
}
public class ScriptOptionsSection
{
    public bool ScriptDatabase { get; set; }
    public bool ScriptSchema { get; set; }
    public bool ScriptStoredProcedures { get; set; }
    public bool ScriptUserDefinedFunctions { get; set; }
}
public class ExportOptionsSection
{
    public bool ExportStatistics { get; set; }
    public bool ExportFullText { get; set; }
    public bool ExportIndexes { get; set; }
    public bool ExportViews { get; set; }

}
