using Jobb.IO;
using Jobb.Schemas;

namespace Jobb 
{
    class Program 
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Jobb v1.0.0\n");

            if (args.Length == 0)
                ShowHelp();
            else
            {
                MainAsync(args).Wait();
            }
        }
        static async Task MainAsync(string[] args)
        {
            string? file = null;

            if (IOHelper.IsJobbFile(args[0]))
                file = args[0];

            //foreach (var arg in args)
            //    Console.WriteLine(arg);

            if (file is not null)
            {
                var jobbFile = await IOHelper.ReadFile(file);
                ColorConsole.WriteLineInfo("Generating schema: " + jobbFile.OutputFileName);

                SchemaGenerator generator = new();
                generator.Generate(jobbFile);
            }
        }

        static void ShowHelp()
        {
            Console.WriteLine("jobb [file] [options]\nOptions:");
        }
    }
}