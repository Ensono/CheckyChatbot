using System.Collections.Generic;
using System.IO;

namespace Loader.Model {
    public class CheckyDocumentCollection {
        public DirectoryInfo Directory { get; set; }
        public IEnumerable<CheckyDocument> Documents { get; set; }
    }
}