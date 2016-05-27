using System.IO;
using Datastore;

namespace Loader.Model {
    public class CheckyConfiguration {
        public DirectoryInfo Directory { get; set; }
        public CheckyDocumentCollection<EnvironmentDocument> Environments { get; set; }
        public CheckyDocumentCollection<TestDocument> Tests { get; set; }
    }
}