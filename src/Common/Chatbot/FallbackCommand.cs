using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checky.Common.ComponentModel;
using Ninject.Infrastructure.Language;

namespace Checky.Common.Chatbot {
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class FallbackCommand : IChatbotCommand {
        public int Priority => int.MaxValue;
        public string HelpText => string.Empty;
        public string Example => string.Empty;
        public string Verb => string.Empty;
        public string Description => string.Empty;

        public bool CanAccept(string receivedText, bool wasMentioned, bool isDirectMessage) {
            return true;
        }

        public Task Process(string command, string user, Func<string, Task> responseHandler,
                            IEnumerable<IChatbotCommand> otherCommands) {
            var verb = command.Split(' ')
                .First(IsVerb);

            var chatbotCommands = otherCommands
                .Where(x => !string.IsNullOrWhiteSpace(x.Verb))
                .ToList();

            var possibleVerbs = chatbotCommands
                .Where(x => x.Verb.StartsWith(verb))
                .Select(FormatCommand)
                .ToList();

            var response = $"Sorry, I didn't understand `{command}`, try: ";
            if (!possibleVerbs.Any()) {
                var supportedVerbs = chatbotCommands
                    .Select(x => x.Verb)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => $"`{x}`");

                return
                    responseHandler(
                        $"{response}`@checky: <verb> [<arguments>]` where _<verb>_ is one of these: {string.Join(", ", supportedVerbs)}");
            }

            var formattedOutput = string.Join(Environment.NewLine, possibleVerbs);
                
            return responseHandler($"{response}{Environment.NewLine}{formattedOutput}");
        }

        private bool IsVerb(string input) {
            if (input.StartsWith("@", StringComparison.CurrentCultureIgnoreCase))
                return false;

            return !input.StartsWith("<@", StringComparison.CurrentCultureIgnoreCase);
        }

        private static string FormatCommand(IChatbotCommand command) {
            var builder = new StringBuilder();
            builder.AppendLine($"*{command.Verb}*: {command.Description}");
            builder.AppendLine($"> {command.HelpText}");
            builder.AppendLine($"> e.g. `{command.Example}`");
            return builder.ToString();
        }
    }
}