using Hunabku.Skive;
using Ninject.Modules;

namespace Checky.Common.Chatbot.Slack {
    public class CheckyChatbotModule : NinjectModule {
        public override void Load() {
            Bind<ISlackEventHandler>().To<SlackMessageHandler>();
            Bind<IRunner>().To<SlackRunner>();
        }
    }
}