using System;
using Configuration;
using Hunabku.Skive;
using Ninject;

namespace CheckyChatbotSlack {
    internal class Program {
        private static void Main() {
            IKernel kernel = new StandardKernel(new CheckyChatbotModule());
            kernel.Load("*.dll");

            var configuration = kernel.Get<IConfigurationRepository>();
            var authToken = configuration.GetAppSetting("SlackBotToken");

            var store = new EventHandlersStore()
                .Register("message", () => kernel.Get<ISlackEventHandler>());

            using (var sb = new BotEngine(store)) {
                sb.Connect(authToken).Wait();
                Console.ReadLine();
            }
        }
    }
}