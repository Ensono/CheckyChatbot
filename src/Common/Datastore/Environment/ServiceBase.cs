using System;

namespace Checky.Common.Datastore.Environment {
    public abstract class ServiceBase {
        public string Name { get; set; }
        public Uri BaseUri { get; set; }
    }
}