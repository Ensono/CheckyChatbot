using System.IO;
using Datastore;

namespace Loader.Model {
    public class CheckyDocument<T> where T : PersistentDocument {
        public FileInfo File { get; set; }
        public string Content { get; set; }

        public T Document { get; set; }
        public string MD5 { get; set; }
    }
}