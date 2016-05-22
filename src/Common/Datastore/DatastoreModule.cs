using Ninject.Modules;

namespace Datastore {
    public class DatastoreModule : NinjectModule {
        public override void Load() {
            Bind<IEnvironmentRepository>().To<EnvironmentRepository>();
        }
    }
}