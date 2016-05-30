using System.Collections.Specialized;
using System.Runtime.Caching;
using Datastore.Environment;
using Ninject.Modules;

namespace Datastore {
    public class DatastoreModule : NinjectModule {
        public override void Load() {
            Bind<IEnvironmentRepository>().To<BlobStorageEnvironmentRepository>();
            Bind<ObjectCache>().To<MemoryCache>()
                .Named("EnvironmentCache")
                .WithConstructorArgument("name", ParameterNames.EnvironmentCache)
                .WithConstructorArgument("config", (NameValueCollection) null);
            Bind<ObjectCache>().To<MemoryCache>()
                .Named("HttpTestCache")
                .WithConstructorArgument("name", ParameterNames.HttpTestCache)
                .WithConstructorArgument("config", (NameValueCollection) null);
        }
    }
}