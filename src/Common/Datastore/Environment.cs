using System.Collections.Generic;

namespace Datastore {
    public class Environment : PersistentDocument {
        public IEnumerable<Service> Services { get; set; }
    }
}