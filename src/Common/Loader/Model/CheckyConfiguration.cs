using System.IO;
using Datastore;
using Datastore.Environment;
using Datastore.Test;

namespace Loader.Model {
    public class CheckyConfiguration {
        public DirectoryInfo Directory { get; set; }
        public CheckyDocumentCollection<EnvironmentDocument> Environments { get; set; }
        public CheckyDocumentCollection<HttpTestDocument> Tests { get; set; }
    }
}