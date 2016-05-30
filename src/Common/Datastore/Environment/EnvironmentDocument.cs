using System.Collections.Generic;

namespace Datastore.Environment {
    public class EnvironmentDocument : PersistentDocument {
        public IEnumerable<Service> Services { get; set; }
    }
}