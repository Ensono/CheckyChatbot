using System;
using System.IO;
using System.Linq;
using System.Text;
using ConfigurationLoader.Validator;
using ManyConsole;

namespace ConfigurationLoader {
    public class ValidateCommand : ConsoleCommand {
        private const ConsoleColor Success = ConsoleColor.DarkGreen;
        private const ConsoleColor Failure = ConsoleColor.DarkRed;
        private const ConsoleColor Normal = ConsoleColor.Gray;
        private const ConsoleColor Notice = ConsoleColor.Yellow;
        public string ConfigPath;

        public ValidateCommand() {
            IsCommand("validate", "Validate configuration.");
            HasOption("p|path:", "Configuration Path", p => ConfigPath = Path.GetFullPath(p));
        }

        private void WriteInternal(Action<string> writer, string line, ConsoleColor color) {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            writer(line);
            Console.ForegroundColor = previousColor;
        }

        private void Write(string line, ConsoleColor color) {
            WriteInternal(Console.Write, line, color);
        }

        private void WriteLine(string line, ConsoleColor color) {
            WriteInternal(Console.WriteLine, line, color);
        }

        private void WriteAscii(string line, ConsoleColor color) {
            WriteInternal(Colorful.Console.WriteAscii, line, color);
        }

        private void WriteResult(string item, ErrorModel model) {
            Write($"{item} ", Normal);
            if (model.IsValid) {
                WriteLine("✔", Success);
            } else {
                WriteLine("✘", Failure);
            }
        }

        public override int Run(string[] remainingArguments) {
            Console.OutputEncoding = Encoding.UTF8;

            WriteAscii("VALIDATE", Notice);

            var loader = new Loader.ConfigurationLoader();
            var configuration = loader.Load(ConfigPath);
            var result = configuration.Validate();

            WriteResult($"{configuration.Directory.FullName}", result);

            WriteResult(" ├─ environments", configuration.Environments.Validate("Environments"));
            var documents = configuration.Environments.Documents.ToArray();
            foreach (var file in documents) {
                var divide = documents.Last() != file ? "├" : "└";
                WriteResult($" │  {divide}─ {file.File.Name}", file.Validate("Environments"));
            }

            WriteResult(" └─ tests", configuration.Tests.Validate("Tests"));
            var tests = configuration.Tests.Documents.ToArray();
            foreach (var file in tests) {
                var divide = tests.Last() != file ? "├" : "└";
                WriteResult($"    {divide}─ {file.File.Name}", file.Validate("Tests"));
            }

            if (!result.IsValid) {
                WriteAscii("FAILED", Failure);
                WriteLine($"The following {result.Errors.Count()} errors were encountered:", Normal);
                foreach (var error in result.Errors) {
                    Write(" ✘ ", Failure);
                    WriteLine(error, Normal);
                }
                return 1;
            }

            WriteAscii("PASSED", Success);
            return 0;
        }
    }
}