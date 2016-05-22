using Ninject.Modules;

namespace Configuration {
    public class ConfigurationModule : NinjectModule {
        public override void Load() {
            Bind<IConfigurationRepository>().To<ConfigurationRepository>().InSingletonScope();
        }
    }
}