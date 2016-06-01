using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ComponentModel;
using Datastore.Environment;
using Ninject.Extensions.Logging;

namespace Healthbot {
    public class UrlBotCommand : IChatbotCommand {
        private readonly IEnvironmentRepository _environments;
        private readonly IHelpers _helpers;
        private readonly ILogger _logger;

        private readonly Regex _matcher = new Regex("\\b([url]+)\\s([0-9A-Za-z]+)\\s([0-9A-Za-z]+)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public UrlBotCommand(IEnvironmentRepository environments, IHelpers helpers, ILogger logger) {
            _environments = environments;
            _helpers = helpers;
            _logger = logger;
        }

        public int Priority => 50;
        public string HelpText => "@checky: url _environment_ _service_";

        public string Example => "@checky: url 52dev delivery";

        public string Verb => "url";

        public bool CanAccept(string receivedText, bool wasMentioned, bool isDirectMessage) {
            if (!wasMentioned && !isDirectMessage) {
                return false;
            }
            return _helpers.CanAcceptWithRegex(receivedText, _matcher, "url");
        }

        public Task Process(string receivedText, string user, Func<string, Task> responseHandler,
                            IEnumerable<IChatbotCommand> otherCommands) {
            var match = _matcher.Match(receivedText);

            _logger.Debug("Recieved: '{0}' from {1} (Matched: {2})", receivedText, user, match.Success);

            if (!match.Success) {
                return responseHandler($"Sorry, I didn't understand `{receivedText}` try `{Example}`.");
            }

            var environmentText = match.Groups[2].Value;
            var serviceText = match.Groups[3].Value;
            var matchingEnvironments = _environments.Find(environmentText).ToArray();

            if (!matchingEnvironments.Any()) {
                return responseHandler($"Unable to find {environmentText}");
            }

            if (matchingEnvironments.Count() > 1) {
                return
                    responseHandler(
                        $"`{environmentText}` matched too many environments, be more speicific.  I matched {string.Join(", ", matchingEnvironments)}");
            }

            var environment = _environments.Get(matchingEnvironments.Single());

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
            if (matchedServices.Count > 1) {
                return
                    responseHandler(
                        $"Matched {matchedServices.Count} services: `{string.Join("`, `", matchedServices)}` be more specific!");
            }

            var service =
                environment.Services.Single(servicePredicate);

            return
                responseHandler(
                    $"*Base Uri*: {service.BaseUri}\n*Healthcheck*: {service.BaseUri}healthcheck\n*Version*: {service.BaseUri}version");
        }
    }
}