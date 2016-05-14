using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hunabku.Skive;
using System.Configuration;

namespace CheckyChatbotSlack {
    class Program {
        static void Main(string[] args) {
            var configuration = ConfigurationManager.AppSettings;
            string authToken = configuration["SlackBotToken"];

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