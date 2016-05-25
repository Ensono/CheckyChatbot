using System.IO;
using ConfigurationLoader.Validator;

namespace ConfigurationLoader.Model {
    public class CheckyDocument {
        public FileInfo File { get; set; }
        public string Content { get; set; }

        public ErrorModel Validate(string collectionName) {
            var validator = new DocumentValidator();
            return validator.Validate(collectionName, this);
        }
    }
}