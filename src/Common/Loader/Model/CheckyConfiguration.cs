using System.IO;
using Datastore;

namespace Loader.Model {
    public class CheckyConfiguration {
        public DirectoryInfo Directory { get; set; }
        public CheckyDocumentCollection<Environment> Environments { get; set; }
        public CheckyDocumentCollection<Test> Tests { get; set; }
    }
}