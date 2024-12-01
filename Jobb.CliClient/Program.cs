using Jobb.IO;
using Jobb.Schemas;

namespace Jobb; 

class Program 
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Jobb v1.0.4\n");

        if (args.Length == 0)
            ShowHelp();
        else
        {
            string? file = null;

            if (Path.IsPathRooted(args[0]) == false)
            {
                file = Path.Combine(Environment.CurrentDirectory, args[0]);
            }
            else
            {
                file = args[0];
            }

            if (File.Exists(file) == false)
            {
                ColorConsole.WriteLine(ConsoleColor.Red, "File not found: " + file);
                return;
            }

            if (IOHelper.IsJobbFile(file) == false)
            {
                ColorConsole.WriteLine(ConsoleColor.Red, "File not JOBB file: " + file);
                return;
            }
            //foreach (var arg in args)
            //    Console.WriteLine(arg);

            if (file is not null)
            {
                var jobbFile = IOHelper.ReadFile(file);
                ColorConsole.WriteLineInfo("Generating schema: " + jobbFile.OutputFileName);

                ColorConsole.WriteLineWarning("\nPlease wait...\n");

                SchemaGenerator generator = new();
                await generator.GenerateFile(jobbFile);

                ColorConsole.WriteLineSuccess("\n{0} generated", jobbFile.OutputFileName);
            }
        }
    }

    static void ShowHelp()
    {
        Console.WriteLine("jobb [file]");
    }
}