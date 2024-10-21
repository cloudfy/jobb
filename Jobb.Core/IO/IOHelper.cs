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
        public static JobbFile ReadFile(string fullname)
        {
            var content = File.ReadAllText(fullname);
            var file = JsonConvert.DeserializeObject<JobbFile>(content);
            file.OutputFile = fullname + ".sql";
            file.OutputFileName = Path.GetFileName(file.OutputFile);

            return file;
        }

        public static JobbFile ReadContent(string fullname, string content)
        {
            var file = JsonConvert.DeserializeObject<JobbFile>(content);
            file.OutputFile = fullname + ".sql";
            file.OutputFileName = Path.GetFileName(file.OutputFile);

            return file;
        }
    }
}
