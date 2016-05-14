using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Healthbot;
using Hunabku.Skive;

namespace CheckyChatbotSlack {
    public class HealthcheckMessageHandler : ISlackEventHandler {
        private EnvironmentRepository _environments = new EnvironmentRepository();

        public Task Handle(ISlackEventContext context) {
            dynamic eventData = context.Event;
            string subtype = eventData.subtype;
            var channelType = context.GetChannelType();
            string receivedText = eventData.text;
            if (!string.IsNullOrEmpty(subtype) || context.UserIsBot() ||
                (channelType != ChannelType.DirectMessage && !context.BotProfile.WasMentionedIn(receivedText))) {
                return Task.FromResult((object) null);
            }
            string channelId = eventData.channel;

            var matcher = new Regex("^<@U([A-Z0-9]+)>\\s([0-9A-Za-z]+)\\s([0-9A-Za-z]+)", RegexOptions.Compiled);
            var match = matcher.Match(receivedText);

            if (!match.Success) {
                return context.BotChatPostMessage(channelId, $"<@{eventData.user}>: Sorry, I didn't understand `{receivedText}` try `@checky 52dev delivery`.");
            }

            var environmentText = match.Groups[2].Value;
            var serviceText = match.Groups[3].Value;

            var environment = _environments.Get(environmentText);

            if (environment == null) return context.BotChatPostMessage(channelId, $"<@{eventData.user}>: Unable to find {environmentText}");

            var services = environment.Services.Select(x => x.Name).ToList();

            if (!services.Any(x => string.Equals(x, serviceText, StringComparison.InvariantCultureIgnoreCase)))
                return context.BotChatPostMessage(channelId,
                    $"<@{eventData.user}>: Environment `{environment.Id}` exists but {serviceText} wasn't found, try one of these: {String.Join(", ", services)}");

            var service =
                environment.Services.Single(
                    x => string.Equals(x.Name, serviceText, StringComparison.InvariantCultureIgnoreCase));
            var client = new HealthcheckClient();
            var state = client.GetHealth(service.BaseUri);
            var formatter = new HealthcheckFormatter();

            return context.BotChatPostMessage(channelId, formatter.Render(environment.Id, state));
        }
    }
}