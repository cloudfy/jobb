using Jobb.IO;
using Jobb.Schemas;
using System.Reflection;

namespace Jobb; 

class Program 
{
    static async Task Main(string[] args)
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        Console.WriteLine($"Jobb v{version}\n");

        if (args.Length == 0) 
        { 
            ShowHelp();
        }
        else
        {
            string? file;

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

                ColorConsole.WriteLineInfo("Generating schema:\n* output: " + jobbFile.OutputFile + "\n* from  : " + file);
                ColorConsole.WriteLineInfo($"\nOptions:\n* schema           : {jobbFile.ScriptOptions?.ScriptSchema ?? false}\n* database         : {jobbFile.ScriptOptions?.ScriptDatabase ?? false}\n* stored procedures: {jobbFile.ScriptOptions?.ScriptStoredProcedures ?? false}");
                ColorConsole.WriteLineWarning("\nPlease wait...\n");

                SchemaGenerator generator = new();
                await generator.GenerateFile(jobbFile);

                ColorConsole.WriteLineSuccess("\n{0} generated", jobbFile.OutputFileName);
            }
        }
    }

    static void ShowHelp()
    {
        Console.WriteLine("Usage: jobb [file.jobb] [outputfile]\n\nOptions:\n  outputfile\t Optional outputfile. If not file is specified, file will be file.jobb.sql in same location.");
    }
}