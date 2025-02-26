using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobb.Vsix
{
    internal static class FileLog
    {
        internal static void Write(string content)
        {
            System.IO.File.AppendAllText("C:\\temp\\Jobb.Vsix.log", $"{content}\n\r");
        }

        internal static void WriteError(Exception ex)
        {
            Write($"{ex.Message}, {ex.StackTrace}, {ex.Source}");
        }
    }
}
