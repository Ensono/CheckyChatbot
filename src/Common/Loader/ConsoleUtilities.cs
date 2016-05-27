using System;
using Loader.Validator;

namespace Loader {
    internal static class ConsoleUtilities {
        public const ConsoleColor Success = ConsoleColor.DarkGreen;
        public const ConsoleColor Failure = ConsoleColor.DarkRed;
        public const ConsoleColor Normal = ConsoleColor.Gray;
        public const ConsoleColor Notice = ConsoleColor.Yellow;

        private static void WriteInternal(Action<string> writer, string line, ConsoleColor color) {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            writer(line);
            Console.ForegroundColor = previousColor;
        }

        public static void Write(string line, ConsoleColor color) {
            WriteInternal(Console.Write, line, color);
        }

        public static void WriteLine(string line, ConsoleColor color) {
            WriteInternal(Console.WriteLine, line, color);
        }

        public static void WriteAscii(string line, ConsoleColor color) {
            WriteInternal(Colorful.Console.WriteAscii, line, color);
        }

        public static void WriteResult(string item, ErrorModel model) {
            WriteResult(item, model.IsValid);
        }

        public static T NotNull<T>(this T obj, string message) {
            WriteResult(message, obj != null);

            return obj;
        }

        public static void WriteResult(string item, bool b) {
            Write($"{item} ", Normal);
            if (b) {
                WriteLine("✔", Success);
            } else {
                WriteLine("✘", Failure);
            }
        }

        public static void WriteLine(string line) {
            WriteLine(line, Normal);
        }
    }
}