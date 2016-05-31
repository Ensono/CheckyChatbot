using Ninject.Modules;

namespace Network {
    public class NetworkModule : NinjectModule {
        public override void Load() {
            Bind<IHttpClientFactory>().To<HttpClientFactory>().InSingletonScope();
        }
    }
}