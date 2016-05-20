using System;
using System.Text.RegularExpressions;

namespace Healthbot {
    public static class Helpers {
        public static bool CanAcceptWithRegex(string receivedText, Regex matcher, string expectedCommand) {
            var match = matcher.Match(receivedText);
            if (!match.Success) return false;
            var command = match.Groups[1].Value;
            return expectedCommand.StartsWith(command, StringComparison.InvariantCultureIgnoreCase);
        }

        public static void Log(string message) {
            var date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            Console.WriteLine($"{date} - {message}");
        }
    }
}