using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;

namespace Configuration
{
    public class ConfigurationModule : NinjectModule
    {
        public override void Load() {
            Bind<IConfigurationRepository>().To<ConfigurationRepository>().InSingletonScope();
        }
    }
}
