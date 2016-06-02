using System.Collections.Generic;

namespace Checky.Common.Datastore.Environment {
    public class EnvironmentDocument : PersistentDocument {
        public IEnumerable<Service> Services { get; set; }
    }
}