using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checky.Common.ComponentModel;
using Hunabku.Skive;
using log4net;
using Ninject.Extensions.Logging;

namespace Checky.Slack.Chatbot {
    public class SlackMessageHandler : ISlackEventHandler {
        private readonly IEnumerable<IChatbotCommand> _commands;
        private readonly ILogger _logger;

        public SlackMessageHandler(IEnumerable<IChatbotCommand> commands, ILogger logger) {
            _commands = commands;
            _logger = logger;
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
            var acceptedCommands = _commands
                .Where(x => x.CanAccept(receivedText, wasMentioned, isDirectMessage))
                .OrderBy(x => x.Priority);
            var winningCommand = acceptedCommands.First();

            _logger.Info($"Winning command: {winningCommand.Verb}");
            acceptedCommands
                .Where(x => x.Priority < int.MaxValue)
                .ToList()
                .ForEach(x => _logger.Info($" - {x.Verb} (Priority {x.Priority})"));

            try {
                using (NDC.Push($"{eventData.Channel}:{eventData.user}")) {
                    return winningCommand.Process(receivedText, user, response, _commands);
                }
            } catch (AggregateException agex) {
                var builder =
                    new StringBuilder($"One or more exceptions occured while processing command: `{receivedText}`.");
                foreach (var ex in agex.InnerExceptions) {
                    builder.Append($"*#1*:\n```{ex.Message}\n{ex.StackTrace}```");
                }
                return response(builder.ToString());
            } catch (Exception ex) {
                _logger.ErrorException("Exception thrown when processing command", ex);
                return
                    response(
                        $"Exception thrown when processing command: `{receivedText}`: ```{ex.Message}\n{ex.StackTrace}```");
            }
        }
    }
}