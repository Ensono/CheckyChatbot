using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Checky.Common.ComponentModel;
using Checky.Common.Datastore.Cache;

namespace Checky.Common.Datastore {
    public class CacheCommand : IChatbotCommand {
        private readonly ICacheManager _cacheManager;
        private readonly IHelpers _helpers;
        private readonly Regex _matcher = new Regex("\\b([cahe]+)\\s([a-zA-Z]+)$", RegexOptions.Compiled);

        public CacheCommand(IHelpers helpers, ICacheManager cacheManager) {
            _helpers = helpers;
            _cacheManager = cacheManager;
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
            if ("clear".StartsWith(result)) {
                return _cacheManager.ClearAll(responseHandler);
            }

            if ("performance".StartsWith(result)) {
                return _cacheManager.Performance(responseHandler);
            }

            return responseHandler($"Sorry, I didn't understand `{command}` try `{Example}`.");
        }
    }
}