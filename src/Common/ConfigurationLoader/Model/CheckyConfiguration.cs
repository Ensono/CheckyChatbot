using System.IO;
using ConfigurationLoader.Validator;

namespace ConfigurationLoader.Model {
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