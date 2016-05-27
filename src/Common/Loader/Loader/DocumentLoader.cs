using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
                    .Select(CheckyDocument)
            };
        }

        private static CheckyDocument<T> CheckyDocument(string x) {
            var content = File.ReadAllText(x);
            return new CheckyDocument<T> {
                File = new FileInfo(x),
                Content = content,
                MD5 = CalculateHash(content)
            };
        }

        private static string CalculateHash(string content) {
            using (var md5 = MD5.Create()) {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content))) {
                    return Convert.ToBase64String(md5.ComputeHash(stream));
                }
            }
        }
    }
}