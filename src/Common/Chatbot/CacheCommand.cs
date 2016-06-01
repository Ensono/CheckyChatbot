using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ComponentModel;
using Datastore;

namespace Chatbot {
    public class CacheCommand : IChatbotCommand {
        private readonly IEnumerable<ObjectCache> _caches;
        private readonly IHelpers _helpers;
        private readonly Regex _matcher = new Regex("\\b([cahe]+)\\s([clear]+)$", RegexOptions.Compiled);

        public CacheCommand(IEnumerable<ObjectCache> caches, IHelpers helpers) {
            _caches = caches;
            _helpers = helpers;
        }

        public int Priority => 100;
        public string HelpText => "@checky: cache clear";
        public string Example => "@checky: cache clear";
        public string Verb => "cache";

        public bool CanAccept(string receivedText, bool wasMentioned, bool isDirectMessage) {
            if (!wasMentioned && !isDirectMessage) {
                return false;
            }
            return _helpers.CanAcceptWithRegex(receivedText, _matcher, "cache");
        }

        public Task Process(string command, string user, Func<string, Task> responseHandler,
                            IEnumerable<IChatbotCommand> otherCommands) {
            var match = _matcher.Match(command);
            var result = match.Groups[2].Value;
            if (!"clear".StartsWith(result)) {
                return responseHandler($"Sorry, I didn't understand `{command}` try `{Example}`.");
            }

            var output = new StringBuilder();
            foreach (var cache in _caches) {
                output.AppendLine($"Decache of `{cache.Name}` successful.");
                SignaledChangeMonitor.Signal();
            }
            return responseHandler(output.ToString());
        }
    }
}