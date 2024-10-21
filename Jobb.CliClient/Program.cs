using Jobb.IO;
using Jobb.Schemas;

namespace Jobb 
{
    class Program 
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Jobb v1.0.3\n");

            if (args.Length == 0)
                ShowHelp();
            else
            {
                string? file = null;

                if (IOHelper.IsJobbFile(args[0]))
                    file = args[0];

                //foreach (var arg in args)
                //    Console.WriteLine(arg);

                if (file is not null)
                {
                    var jobbFile = IOHelper.ReadFile(file);
                    ColorConsole.WriteLineInfo("Generating schema: " + jobbFile.OutputFileName);

                    ColorConsole.WriteLineWarning("\nPlease wait...\n");

                    SchemaGenerator generator = new();
                    generator.GenerateFile(jobbFile);

                    ColorConsole.WriteLineSuccess("\n{0} generated", jobbFile.OutputFileName);
                }
            }
        }

        static void ShowHelp()
        {
            Console.WriteLine("jobb [file] [options]\nOptions:");
        }
    }
}