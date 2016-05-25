using System.IO;

namespace Loader.Model {
    public class CheckyConfiguration {
        public DirectoryInfo Directory { get; set; }
        public CheckyDocumentCollection Environments { get; set; }
        public CheckyDocumentCollection Tests { get; set; }
    }
}