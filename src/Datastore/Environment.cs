using System.Collections.Generic;
using Healthbot;

namespace Datastore {
    public class Environment {
        public string Id { get; set; }
        public IEnumerable<Service> Services { get; set; }
    }
}