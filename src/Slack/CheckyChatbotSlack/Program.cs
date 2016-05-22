using System;
using System.Configuration;
using Hunabku.Skive;
using Ninject;

namespace CheckyChatbotSlack {
    internal class Program {
        private static void Main(string[] args) {
            IKernel kernel = new StandardKernel();
            kernel.Load(AppDomain.CurrentDomain.GetAssemblies());

            var configuration = ConfigurationManager.AppSettings;
            var authToken = configuration["SlackBotToken"];

            if (string.IsNullOrWhiteSpace(authToken)) {
                authToken = Environment.GetEnvironmentVariable("APPSETTING_SlackBotToken");
            }

            var store = new EventHandlersStore()
                .Register("message", () => kernel.Get<ISlackEventHandler>());

            using (var sb = new BotEngine(store)) {
                sb.Connect(authToken).Wait();
                Console.ReadLine();
            }
        }
    }
}