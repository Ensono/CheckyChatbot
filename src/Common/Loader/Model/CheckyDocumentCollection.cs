using System.Collections.Generic;
using System.IO;
using Datastore;

namespace Loader.Model {
    public class CheckyDocumentCollection<T> where T : PersistentDocument {
        public DirectoryInfo Directory { get; set; }
        public IEnumerable<CheckyDocument<T>> Documents { get; set; }
    }
}