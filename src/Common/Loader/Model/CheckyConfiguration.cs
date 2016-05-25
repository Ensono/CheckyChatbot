using System.IO;
using Loader.Validator;

namespace Loader.Model {
    public class CheckyConfiguration {
        public DirectoryInfo Directory { get; set; }
        public CheckyDocumentCollection Environments { get; set; }
        public CheckyDocumentCollection Tests { get; set; }

        public ErrorModel Validate() {
            var validator = new ConfigValidator();
            return validator.Validate(this);
        }
    }
}