using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hunabku.Skive;
using System.Configuration;

namespace CheckyChatbotSlack {
    class Program {
        static void Main(string[] args) {
            var configuration = ConfigurationManager.AppSettings;
            string authToken = configuration["SlackBotToken"];

            var store = new EventHandlersStore()
                .Register("message", () => new EchoMessageHandler());

            using (var sb = new BotEngine(store))
            {
                sb.Connect(authToken).Wait();
                Console.ReadLine();
            }
        }
    }
}