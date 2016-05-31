using System.Collections.Specialized;
using System.Runtime.Caching;
using Datastore.Environment;
using Datastore.Test;
using Ninject.Modules;

namespace Datastore {
    public class DatastoreModule : NinjectModule {
        public override void Load() {
            Bind<IEnvironmentRepository>().To<BlobStorageEnvironmentRepository>().InSingletonScope();
            Bind<ObjectCache>().To<MemoryCache>().InSingletonScope()
                .Named(ParameterNames.EnvironmentCache)
                .WithConstructorArgument("name", ParameterNames.EnvironmentCache)
                .WithConstructorArgument("config", (NameValueCollection) null);
            Bind<IHttpTestRepository>().To<BlobStorageHttpTestRepository>().InSingletonScope();
            Bind<ObjectCache>().To<MemoryCache>().InSingletonScope()
                .Named(ParameterNames.EnvironmentCache)
                .WithConstructorArgument("name", ParameterNames.HttpTestCache)
                .WithConstructorArgument("config", (NameValueCollection) null);
        }
    }
}