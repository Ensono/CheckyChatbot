using Ninject.Modules;

namespace Smokebot {
    public class HttpTestModule : NinjectModule {
        public override void Load() {
            Bind<IHttpTestRequestMessageFactory>().To<HttpTestRequestMessageFactory>().InSingletonScope();
            Bind<IHttpTestResponseBodyValidator>().To<HttpTestResponseBodyValidator>().InSingletonScope();
            Bind<IHttpTestResponseValidator>().To<HttpTestResponseValidator>().InSingletonScope();
            Bind<IHttpTestRequestMessageFactory>().To<HttpTestRequestMessageFactory>().InSingletonScope();
        }
    }
}