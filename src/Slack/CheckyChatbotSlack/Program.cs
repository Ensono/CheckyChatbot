using System;
using System.Configuration;
using Hunabku.Skive;

namespace CheckyChatbotSlack {
    internal class Program {
        private static void Main(string[] args) {
            var configuration = ConfigurationManager.AppSettings;
            var authToken = configuration["SlackBotToken"];

            if (string.IsNullOrWhiteSpace(authToken)) {
                authToken = Environment.GetEnvironmentVariable("APPSETTING_SlackBotToken");
            }

            var store = new EventHandlersStore()
                .Register("message", () => new HealthcheckMessageHandler());

            using (var sb = new BotEngine(store)) {
                sb.Connect(authToken).Wait();
                Console.ReadLine();
            }
        }
    }
}