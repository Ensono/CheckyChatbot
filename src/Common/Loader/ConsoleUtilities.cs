using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loader.Validator;

namespace Loader
{
    class ConsoleUtilities
    {
        public const ConsoleColor Success = ConsoleColor.DarkGreen;
        public const ConsoleColor Failure = ConsoleColor.DarkRed;
        public const ConsoleColor Normal = ConsoleColor.Gray;
        public const ConsoleColor Notice = ConsoleColor.Yellow;

        private static void WriteInternal(Action<string> writer, string line, ConsoleColor color)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            writer(line);
            Console.ForegroundColor = previousColor;
        }

        public static void Write(string line, ConsoleColor color)
        {
            WriteInternal(Console.Write, line, color);
        }

        public static void WriteLine(string line, ConsoleColor color)
        {
            WriteInternal(Console.WriteLine, line, color);
        }

        public static void WriteAscii(string line, ConsoleColor color)
        {
            WriteInternal(Colorful.Console.WriteAscii, line, color);
        }

        public static void WriteResult(string item, ErrorModel model)
        {
            Write($"{item} ", Normal);
            if (model.IsValid)
            {
                WriteLine("✔", Success);
            }
            else
            {
                WriteLine("✘", Failure);
            }
        }
    }
}
