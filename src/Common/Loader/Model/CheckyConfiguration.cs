using System.IO;
using Checky.Common.Datastore.Environment;
using Checky.Common.Datastore.Test;

namespace Checky.Common.Loader.Model {
    public class CheckyConfiguration {
        public DirectoryInfo Directory { get; set; }
        public CheckyDocumentCollection<EnvironmentDocument> Environments { get; set; }
        public CheckyDocumentCollection<HttpTestDocument> Tests { get; set; }
    }
}