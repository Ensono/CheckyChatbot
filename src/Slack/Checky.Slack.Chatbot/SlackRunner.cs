using System;
using Checky.Common.Chatbot;
using Checky.Common.ComponentModel;
using Checky.Common.Configuration;
using Hunabku.Skive;
using Ninject.Extensions.Logging;

namespace Checky.Slack.Chatbot {
    public class SlackRunner : IRunner {
        private readonly IConfigurationRepository _config;
        private readonly ISlackEventHandler _eventHandler;
        private readonly IHelpers _helpers;
        private readonly ILogger _logger;

        public SlackRunner(IConfigurationRepository config, ISlackEventHandler eventHandler, ILogger logger,
                           IHelpers helpers) {
            _config = config;
            _eventHandler = eventHandler;
            _logger = logger;
            _helpers = helpers;
        }

        public void Run() {
            _logger.Info("Started Checky ChatBot");

            var authToken = _config.GetAppSetting("SlackBotToken");
            var store = new EventHandlersStore()
                .Register("message", () => _eventHandler);

            using (var sb = new BotEngine(store)) {
                _logger.Info(
                    $"Authentication started with {_helpers.Mask(authToken)}");
                sb.Connect(authToken).Wait();
                _logger.Info("Authentication successful, ready to accept commands.");
                Console.ReadLine();
                _logger.Info("Stopped ChatBot");
            }
            _logger.Info("Exiting");
        }
    }
}