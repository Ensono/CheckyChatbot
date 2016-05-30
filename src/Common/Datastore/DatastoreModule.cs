using System.Collections.Specialized;
using System.Runtime.Caching;
using Ninject.Modules;

namespace Datastore {
    public class DatastoreModule : NinjectModule {
        public override void Load() {
            Bind<IEnvironmentRepository>().To<BlobStorageEnvironmentRepository>();
            Bind<ObjectCache>().To<MemoryCache>()
                .Named("EnvironmentCache")
                .WithConstructorArgument("name", "EnvironmentCache")
                .WithConstructorArgument("config", (NameValueCollection) null);
        }
    }
}