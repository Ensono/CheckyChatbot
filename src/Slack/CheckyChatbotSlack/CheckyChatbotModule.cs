using Hunabku.Skive;
using Ninject.Modules;

namespace CheckyChatbotSlack {
    public class CheckyChatbotModule : NinjectModule {
        public override void Load() {
            Bind<ISlackEventHandler>().To<SlackMessageHandler>();
        }
    }
}