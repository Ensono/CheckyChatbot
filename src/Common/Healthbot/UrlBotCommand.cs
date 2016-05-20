using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ComponentModel;

namespace Healthbot {
    public class UrlBotCommand : IChatbotCommand {
        private readonly EnvironmentRepository _environments = new EnvironmentRepository();

        public int Priority => 50;
        public string HelpText => "@checky: url _environment_ _service_";

        public string Example => "@checky: url 52dev delivery";

        public string Verb => "url";

        public bool CanAccept(string receivedText, bool wasMentioned, bool isDirectMessage) {
            var matcher = new Regex("^(?:@?checky|<@[0-9A-Za-z]+>):?\\s([u|U][0-9A-Za-z]*)", RegexOptions.Compiled);
            return Helpers.CanAcceptWithRegex(receivedText, matcher, "url");
        }

        public Task Process(string receivedText, string user, Func<string, Task> responseHandler,
                            IEnumerable<IChatbotCommand> otherCommands)
        {
            var matcher =
                new Regex("^(?:@?checky|<@[0-9A-Za-z]+>):?\\s[u|U][0-9A-Za-z]*\\s([0-9A-Za-z]+)\\s([0-9A-Za-z]+)",
                    RegexOptions.Compiled);
            var match = matcher.Match(receivedText);

            Helpers.Log($"Recieved: '{receivedText}' from {user} (Matched: {match.Success})");

            if (!match.Success)
            {
                return responseHandler($"Sorry, I didn't understand `{receivedText}` try `{Example}`.");
            }

            var environmentText = match.Groups[1].Value;
            var serviceText = match.Groups[2].Value;
            var environment = _environments.Get(environmentText);

            if (environment == null)
                return responseHandler($"Unable to find {environmentText}");

            var services = environment.Services.ToList();

            Func<Service, bool> servicePredicate =
                x =>
                    x.Name.StartsWith(serviceText, StringComparison.InvariantCultureIgnoreCase);

            if (!services.Any(servicePredicate))
                return
                    responseHandler(
                        $"Environment `{environment.Id}` exists but {serviceText} wasn't found, try one of these: {string.Join(", ", services)}");

            var matchedServices = services.Where(servicePredicate).Select(x => x.Name).ToList();
            if (matchedServices.Count > 1)
            {
                return
                    responseHandler(
                        $"Matched {matchedServices.Count} services: `{string.Join("`, `", matchedServices)}` be more specific!");
            }

            var service =
                environment.Services.Single(servicePredicate);

            return responseHandler($"*Base Uri*: {service.BaseUri}\n*Healthcheck*: {service.BaseUri}healthcheck\n*Version*: {service.BaseUri}version");
        }
    }
}