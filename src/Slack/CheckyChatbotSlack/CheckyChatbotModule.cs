using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hunabku.Skive;
using Ninject.Modules;

namespace CheckyChatbotSlack
{
    public class CheckyChatbotModule : NinjectModule
    {
        public override void Load() {
            Bind<ISlackEventHandler>().To<SlackMessageHandler>();
        }
    }
}
