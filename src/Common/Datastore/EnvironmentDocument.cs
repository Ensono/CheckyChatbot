using System.Collections.Generic;

namespace Datastore {
    public class EnvironmentDocument : PersistentDocument {
        public IEnumerable<Service> Services { get; set; }
    }
}