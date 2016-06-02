using System.Collections.Generic;
using System.IO;
using Checky.Common.Datastore;

namespace Checky.Common.Loader.Model {
    public class CheckyDocumentCollection<T> where T : PersistentDocument {
        public DirectoryInfo Directory { get; set; }
        public IEnumerable<CheckyDocument<T>> Documents { get; set; }
    }
}