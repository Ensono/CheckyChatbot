using System.Collections.Generic;
using Checky.Common.ComponentModel;
using Checky.Common.Datastore.Cache;
using Checky.Common.Datastore.Environment;
using Checky.Common.Datastore.Test;
using Microsoft.WindowsAzure.Storage.Blob;
using Ninject.Modules;

namespace Checky.Common.Datastore {
    public class DatastoreModule : NinjectModule {
        public override void Load() {
            Bind<IEnvironmentRepository>().To<BlobStorageEnvironmentRepository>().InSingletonScope();
            Bind<IObjectCache<EnvironmentDocument>>().To<InMemoryCache<EnvironmentDocument>>().InSingletonScope();
            Bind<IObjectCache<IEnumerable<ICloudBlob>>>()
                .To<InMemoryCache<IEnumerable<ICloudBlob>>>()
                .InSingletonScope();
            Bind<IHttpTestRepository>().To<BlobStorageHttpTestRepository>().InSingletonScope();
            Bind<IObjectCache<HttpTestDocument>>().To<InMemoryCache<HttpTestDocument>>().InSingletonScope();
            Bind<IObjectCache<IEnumerable<HttpTestDocument>>>()
                .To<InMemoryCache<IEnumerable<HttpTestDocument>>>()
                .InSingletonScope();
            Bind<IChatbotCommand>().To<CacheCommand>().InSingletonScope();
        }
    }
}