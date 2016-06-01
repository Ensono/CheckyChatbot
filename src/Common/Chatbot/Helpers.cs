using System;
using System.Text.RegularExpressions;
using ComponentModel;

namespace Chatbot {
    public class Helpers : IHelpers {
        public bool CanAcceptWithRegex(string receivedText, Regex matcher, string expectedCommand) {
            var match = matcher.Match(receivedText);
            if (!match.Success) return false;
            var command = match.Groups[1].Value;
            return expectedCommand.StartsWith(command, StringComparison.InvariantCultureIgnoreCase);
        }

        public void Log(string message) {
            var date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            Console.WriteLine($"{date} - {message}");
        }
    }
}