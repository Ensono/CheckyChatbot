using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComponentModel;
using Hunabku.Skive;

namespace CheckyChatbotSlack {
    public class SlackMessageHandler : ISlackEventHandler {
        private readonly IEnumerable<IChatbotCommand> _commands;
        
        public SlackMessageHandler(IEnumerable<IChatbotCommand> commands) {
            _commands = commands;
        }

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

            try {
                return winningCommand.Process(receivedText, user, response, _commands);
            } catch (AggregateException agex) {
                var builder =
                    new StringBuilder("One or more exceptions occured while processing command: `{receivedText}`.");
                foreach (var ex in agex.InnerExceptions) {
                    builder.Append($"*#1*:\n```{ex.Message}\n{ex.StackTrace}```");
                }
                return response(builder.ToString());
            } catch (Exception ex) {
                return
                    response(
                        $"Exception thrown when processing command: `{receivedText}`: ```{ex.Message}\n{ex.StackTrace}```");
            }
        }
    }
}