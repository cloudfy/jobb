using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobb
{
    internal static class ColorConsole
    {
        internal static void WriteLineInfo(string format, params object?[]? args)
        {
            WriteLine(ConsoleColor.Cyan, format, args);
        }
        internal static void Write(ConsoleColor color, string format, params object?[]? args)
        {
            Console.ForegroundColor = color;
            Console.Write(format, args);
            Console.ResetColor();
        }
        internal static void WriteLine(ConsoleColor color, string format, params object?[]? args)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(format, args);
            Console.ResetColor();
        }
        internal static void WriteLineSuccess(string format, params object?[]? args)
        {
            WriteLine(ConsoleColor.Green, format, args);
        }

        internal static void WriteLineWarning(string format, params object?[]? args)
        {
            WriteLine(ConsoleColor.Yellow, format, args);
        }
    }
}
