using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chatbot;
using ComponentModel;
using Healthbot;
using Hunabku.Skive;

namespace CheckyChatbotSlack {
    public class HealthcheckMessageHandler : ISlackEventHandler {
        private readonly IEnumerable<IChatbotCommand> _commands = new List<IChatbotCommand> {
            new HealthBotCommand(),
            new FallbackCommand()
        };

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

            var wasMentioned = context.BotProfile.WasMentionedIn(receivedText);
            var isDirectMessage = channelType == ChannelType.DirectMessage;
            var winningCommand = _commands
                .Where(x => x.CanAccept(receivedText, wasMentioned, isDirectMessage))
                .OrderBy(x => x.Priority)
                .First();

            return winningCommand.Process(receivedText, user, response, _commands);
        }
    }
}