using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ComponentModel;

namespace Healthbot {
    public class HealthBotCommand : IChatbotCommand {
        private readonly EnvironmentRepository _environments = new EnvironmentRepository();

        public bool CanAccept(string receivedText, bool wasMentioned, bool isDirectMessage) {
            var matcher = new Regex("^(?:@?checky|<@[0-9A-Za-z]+>):?\\s([s|S][0-9A-Za-z]+)", RegexOptions.Compiled);
            var match = matcher.Match(receivedText);
            if (match.Success) {
                var command = match.Groups[2].Value;
                return "status".StartsWith(command, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }

        public int Priority => 50;

        public Task Process(string receivedText, string user, Func<string, Task> responseHandler) {
            var matcher =
                new Regex("^(?:@?checky|<@[0-9A-Za-z]+>):?\\s[s|S][0-9A-Za-z]+\\s([0-9A-Za-z]+)\\s([0-9A-Za-z]+)",
                    RegexOptions.Compiled);
            var match = matcher.Match(receivedText);

            Log($"Recieved: '{receivedText}' from {user} (Matched: {match.Success})");

            if (!match.Success) {
                return responseHandler($"Sorry, I didn't understand `{receivedText}` try `{Example}`.");
            }

            var environmentText = match.Groups[1].Value;
            var serviceText = match.Groups[2].Value;
            var environment = _environments.Get(environmentText);

            if (environment == null)
                return responseHandler($"Unable to find {environmentText}");

            var services = environment.Services.Select(x => x.Name).ToList();

            if (!services.Any(x => x.StartsWith(serviceText, StringComparison.InvariantCultureIgnoreCase)))
                return
                    responseHandler(
                        $"Environment `{environment.Id}` exists but {serviceText} wasn't found, try one of these: {string.Join(", ", services)}");

            var service =
                environment.Services.Single(
                    x => x.Name.StartsWith(serviceText, StringComparison.InvariantCultureIgnoreCase));
            var client = new HealthcheckClient();
            var state = client.GetHealth(service.BaseUri);
            var formatter = new HealthcheckFormatter();

            return responseHandler(formatter.Render(environment.Id, service.Name, state));
        }

        public string HelpText => "@checky: status _environment_ _service_";

        public string Example => "@checky: status 52dev delivery";

        private static void Log(string message) {
            var date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            Console.WriteLine($"{date} - {message}");
        }
    }
}