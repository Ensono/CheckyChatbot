using System.Collections.Generic;

namespace Checky.Common.Datastore.Environment {
    public class Service : ServiceBase {
        public IEnumerable<Region> Regions { get; set; }
    }
}