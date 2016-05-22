using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComponentModel;
using Ninject.Modules;

namespace Healthbot
{
    public class HealthbotModule : NinjectModule
    {
        public override void Load() {
            Bind<IChatbotCommand>().To<HealthBotCommand>().InTransientScope();
            Bind<IChatbotCommand>().To<UrlBotCommand>().InTransientScope();
        }
    }
}
