using System.IO;
using System.Linq;
using Loader.Model;

namespace Loader.Loader {
    public class DocumentLoader {
        public CheckyDocumentCollection Load(string documentPath) {
            var resolvedPath = Path.GetFullPath(documentPath);
            if (!Directory.Exists(resolvedPath)) {
                return null;
            }

            return new CheckyDocumentCollection {
                Directory = new DirectoryInfo(resolvedPath),
                Documents = Directory.GetFiles(resolvedPath)
                    .Select(x => new CheckyDocument {
                        File = new FileInfo(x),
                        Content = File.ReadAllText(x)
                    })
            };
        }
    }
}