using System;
using System.IO;
using System.Linq;
using System.Text;
using Loader.Loader;
using Loader.Validator;
using ManyConsole;

namespace Loader {
    public class ValidateCommand : ConsoleCommand {
        public string ConfigPath;

        public ValidateCommand() {
            IsCommand("validate", "Validate configuration.");
            HasOption("p|path:", "Configuration Path", p => ConfigPath = Path.GetFullPath(p));
        }

        public override int Run(string[] remainingArguments) {
            Console.OutputEncoding = Encoding.UTF8;

            ConsoleUtilities.WriteAscii("@CHECKY", ConsoleColor.DarkBlue);
            ConsoleUtilities.WriteAscii("VALIDATE", ConsoleUtilities.Notice);

            var loader = new ConfigurationLoader();
            var configuration = loader.Load(ConfigPath);
            var result = configuration.Validate();

            ConsoleUtilities.WriteResult($"{configuration.Directory.FullName}", result);

            ConsoleUtilities.WriteResult(" ├─ environments", configuration.Environments.Validate("Environments"));
            var documents = configuration.Environments.Documents.ToArray();
            foreach (var file in documents) {
                var divide = documents.Last() != file ? "├" : "└";
                ConsoleUtilities.WriteResult($" │  {divide}─ {file.File.Name}", file.Validate("Environments"));
            }

            ConsoleUtilities.WriteResult(" └─ tests", configuration.Tests.Validate("Tests"));
            var tests = configuration.Tests.Documents.ToArray();
            foreach (var file in tests) {
                var divide = tests.Last() != file ? "├" : "└";
                ConsoleUtilities.WriteResult($"    {divide}─ {file.File.Name}", file.Validate("Tests"));
            }

            if (!result.IsValid) {
                ConsoleUtilities.WriteAscii("FAILED", ConsoleUtilities.Failure);
                ConsoleUtilities.WriteLine($"The following {result.Errors.Count()} errors were encountered:", ConsoleUtilities.Normal);
                foreach (var error in result.Errors) {
                    ConsoleUtilities.Write(" ✘ ", ConsoleUtilities.Failure);
                    ConsoleUtilities.WriteLine(error, ConsoleUtilities.Normal);
                }
                return 1;
            }

            ConsoleUtilities.WriteAscii("PASSED", ConsoleUtilities.Success);
            return 0;
        }
    }
}