using System.IO;
using Checky.Common.Datastore;

namespace Checky.Common.Loader.Model {
    public class CheckyDocument<T> where T : PersistentDocument {
        public FileInfo File { get; set; }
        public string Content { get; set; }

        public T Document { get; set; }
        public string Md5 { get; set; }
    }
}