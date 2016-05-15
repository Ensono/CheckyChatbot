using System.Collections;
using System.Collections.Generic;

namespace Healthbot {
    public class Environment {
        public string Id { get; set; }
        public IEnumerable<Service> Services { get; set; }
    }
}