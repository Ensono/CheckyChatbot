using System.IO;
using System.Linq;
using Datastore;
using Loader.Model;

namespace Loader.Loader {
    public class DocumentLoader<T> where T : PersistentDocument {
        public CheckyDocumentCollection<T> Load(string documentPath) {
            var resolvedPath = Path.GetFullPath(documentPath);
            if (!Directory.Exists(resolvedPath)) {
                return null;
            }

            return new CheckyDocumentCollection<T> {
                Directory = new DirectoryInfo(resolvedPath),
                Documents = Directory.GetFiles(resolvedPath)
                    .Select(x => new CheckyDocument<T> {
                        File = new FileInfo(x),
                        Content = File.ReadAllText(x)
                    })
            };
        }
    }
}