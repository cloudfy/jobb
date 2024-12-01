using System.IO;
using System.Threading.Tasks;

namespace Jobb;

internal static class SqlSchema
{
    internal static async Task Write(TextWriter writer, string value, bool doWrite = true)
    {
        if (doWrite)
        {
            await writer.WriteAsync(value);
        }
    }
}
