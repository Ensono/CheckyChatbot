using Checky.Common.Chatbot;
using log4net.Config;
using Ninject;

namespace Checky.Slack.Chatbot {
    internal class Program {
        private static void Main() {
            XmlConfigurator.Configure();
            IKernel kernel = new StandardKernel(new CheckyChatbotModule());
            kernel.Load("Checky.*.dll");
            kernel.Get<IRunner>().Run();
        }
    }
}