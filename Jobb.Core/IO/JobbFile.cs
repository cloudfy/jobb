using System;
using System.Collections.Generic;
using System.Text;

namespace Jobb.IO
{
    public sealed class JobbFile
    {
        public string ConnectionString { get; set; }
        public string OutputFile { get; internal set; }
        public string OutputFileName { get; internal set; }
        public ExportOptionsSection ExportOptions { get; set; }
        public ScriptOptionsSection ScriptOptions { get; set; }
    }
    public class ScriptOptionsSection
    {
        public bool ScriptDatabase { get; set; }
        public bool ScriptSchema { get; set; }
    }
    public class ExportOptionsSection
    {
        public bool ExportStatistics { get; set; }
        public bool ExportFullText { get; set; }
        public bool ExportIndexes { get; set; }
        public bool ExportViews { get; set; }

    }
}
