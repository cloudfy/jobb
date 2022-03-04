using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Jobb.IO
{
    public static class IOHelper
    {
        public static bool IsJobbFile(string fullname)
        {
            return System.IO.Path.IsPathRooted(fullname) && System.IO.Path.GetExtension(fullname) == ".jobb";
        }
        public static async Task<JobbFile> ReadFile(string fullname)
        {
            var content = await File.ReadAllTextAsync(fullname);
            var file = JsonConvert.DeserializeObject<JobbFile>(content);
            file.OutputFile = fullname + ".sql";
            file.OutputFileName = Path.GetFileName(file.OutputFile);

            return file;
        }
    }
}
