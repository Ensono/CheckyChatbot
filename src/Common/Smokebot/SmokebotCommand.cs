using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Chatbot;
using ComponentModel;
using Datastore.Environment;
using Datastore.Test;
using Network;

namespace Smokebot {
    public class SmokebotCommand : IChatbotCommand {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IEnvironmentRepository _environments;

        private readonly Regex _matcher = new Regex("\\b([tes]+)\\s([0-9A-Za-z]+)\\s([0-9A-Za-z]+)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly IHttpTestRequestMessageFactory _messageFactory;
        private readonly IHttpTestResponseValidator _responseValidator;
        private readonly IHttpTestRepository _tests;

        public SmokebotCommand(IEnvironmentRepository environments, IHttpTestRepository tests,
                               IHttpTestRequestMessageFactory messageFactory, IHttpClientFactory clientFactory,
                               IHttpTestResponseValidator responseValidator) {
            _environments = environments;
            _tests = tests;
            _messageFactory = messageFactory;
            _clientFactory = clientFactory;
            _responseValidator = responseValidator;
        }

        public int Priority => 50;
        public string HelpText => "@checky: test _environment_ _service_";
        public string Example => "@checky: test 52dev delivery";
        public string Verb => "test";

        public bool CanAccept(string receivedText, bool wasMentioned, bool isDirectMessage) {
            if (!wasMentioned && !isDirectMessage) {
                return false;
            }
            return Helpers.CanAcceptWithRegex(receivedText, _matcher, "test");
        }

        public Task Process(string command, string user, Func<string, Task> responseHandler,
                            IEnumerable<IChatbotCommand> otherCommands) {
            var match = _matcher.Match(command);

            Helpers.Log($"Smoke Recieved: '{command}' from {user} (Matched: {match.Success})");

            if (!match.Success) {
                return responseHandler($"Sorry, I didn't understand `{command}` try `{Example}`.");
            }

            var environmentText = match.Groups[2].Value;
            var serviceText = match.Groups[3].Value;
            var matchingEnvironments = _environments.Find(environmentText).ToArray();

            if (!matchingEnvironments.Any()) {
                return responseHandler($"Unable to find {environmentText}");
            }

            if (matchingEnvironments.Length > 1) {
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

            var service = environment.Services.Single(servicePredicate);

            var matchingTests = _tests.Find(environment.Id, service.Name);
            var tests = _tests.GetAll(matchingTests).ToArray();
            var httpClient = _clientFactory.GetClient();
            var responses = new Dictionary<HttpTestDocument, Task<HttpResponseMessage>>();

            foreach (var test in tests) {
                var message = _messageFactory.CreateMessage(service.BaseUri, test);
                responses.Add(test, httpClient.SendAsync(message));
            }

            var output = new StringBuilder();
            foreach (var test in tests) {
                var response = responses[test].Result;
                var testOutput = new StringBuilder();
                if (_responseValidator.Validate(test, response, x => testOutput.AppendLine(x))) {
                    output.AppendLine($":white_check_mark: {test.Id}");
                } else {
                    output.AppendLine($":warning: {test.Id}:");
                    output.AppendLine($"```{testOutput}```");
                }
            }

            return responseHandler(output.ToString());
        }
    }
}