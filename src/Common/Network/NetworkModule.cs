using Ninject.Modules;

namespace Checky.Common.Network {
    public class NetworkModule : NinjectModule {
        public override void Load() {
            Bind<IHttpClientFactory>().To<HttpClientFactory>().InSingletonScope();
        }
    }
}