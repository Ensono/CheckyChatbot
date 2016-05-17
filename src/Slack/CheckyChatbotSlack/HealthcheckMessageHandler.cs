using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Healthbot;
using Hunabku.Skive;

namespace CheckyChatbotSlack {
    public class HealthcheckMessageHandler : ISlackEventHandler {
        public Task Handle(ISlackEventContext context) {
            dynamic eventData = context.Event;
            string subtype = eventData.subtype;
            var channelType = context.GetChannelType();
            string receivedText = eventData.text;
            string channelId = eventData.channel;
            string user = eventData.user;

            if (!string.IsNullOrEmpty(subtype) || context.UserIsBot() ||
                (channelType != ChannelType.DirectMessage && !context.BotProfile.WasMentionedIn(receivedText))) {
                return Task.FromResult((object) null);
            }

            Func<string, Task> response =
                message =>
                    context.BotChatPostMessage(channelId, $"<@{user}>: {message}");

            var command = new HealthBotCommand();
            if (command.CanAccept(receivedText, context.BotProfile.WasMentionedIn(receivedText),
                channelType == ChannelType.DirectMessage)) {
                return command.Process(receivedText, user, response);
            }

            return Task.FromResult((object) null);
        }
    }
}