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

        public string GetBaseName(string filename) {
            if (!filename.Contains(".")) {
                return filename;
            }

            var dotLocation = filename.LastIndexOf(".", StringComparison.Ordinal);
            var extension = filename.Substring(dotLocation);

            return extension.Length > 0
                ? filename.Remove(filename.Length - extension.Length)
                : filename;
        }

        public string Mask(string input, double coverage = 0.7d) {
            if (string.IsNullOrEmpty(input)) return input;
            if (coverage < 0.1d) coverage = 0.1d;
            if (coverage > 1d) coverage = 1d;

            var len = input.Length;
            var unmaskedChars = (int) Math.Floor((1.0d - coverage)*len/2);
            var firstPart = input.Substring(0, unmaskedChars);
            var lastPart = input.Substring(len - unmaskedChars, unmaskedChars);
            var maskedPart = new string('*', len - unmaskedChars*2);
            return $"{firstPart}{maskedPart}{lastPart}";
        }
    }
}