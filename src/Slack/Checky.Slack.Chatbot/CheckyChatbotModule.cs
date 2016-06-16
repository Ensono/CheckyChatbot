using Checky.Common.Chatbot;
using Hunabku.Skive;
using Ninject.Modules;

namespace Checky.Slack.Chatbot {
    public class CheckyChatbotModule : NinjectModule {
        public override void Load() {
            Bind<ISlackEventHandler>().To<SlackMessageHandler>();
            Bind<IRunner>().To<SlackRunner>();
        }
    }
}