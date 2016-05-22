using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ComponentModel;

namespace Chatbot {
    public class UsageCommand : IChatbotCommand {
        private readonly DateTime _startupTime = DateTime.UtcNow;
        private int _usageCount;

        public int Priority => 100;
        public string HelpText => "@checky usage";
        public string Example => "@checky usage";
        public string Verb => "usage";

        public bool CanAccept(string receivedText, bool wasMentioned, bool isDirectMessage) {
            _usageCount++;
            var matcher = new Regex("^(?:@?checky|<@[0-9A-Za-z]+>):?\\s([u|U][0-9A-Za-z]*)", RegexOptions.Compiled);
            return Helpers.CanAcceptWithRegex(receivedText, matcher, "usage");
        }

        public Task Process(string command, string user, Func<string, Task> responseHandler,
                            IEnumerable<IChatbotCommand> otherCommands) {
            var delta = DateTime.UtcNow - _startupTime;
            return
                responseHandler(
                    $"I have been operational for {delta.TotalDays:N0} days, {delta.Hours} hours and {delta.Minutes} minutes. In that time I have responded to {_usageCount} requests");
        }
    }
}