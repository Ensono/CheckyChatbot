using Ninject.Modules;

namespace Checky.Common.Configuration {
    public class ConfigurationModule : NinjectModule {
        public override void Load() {
            Bind<IConfigurationRepository>().To<ConfigurationRepository>().InSingletonScope();
        }
    }
}