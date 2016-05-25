using System.IO;
using Loader.Validator;

namespace Loader.Model {
    public class CheckyDocument {
        public FileInfo File { get; set; }
        public string Content { get; set; }

        public ErrorModel Validate(string collectionName) {
            var validator = new DocumentValidator();
            return validator.Validate(collectionName, this);
        }
    }
}