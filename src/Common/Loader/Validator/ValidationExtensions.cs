using Loader.Model;

namespace Loader.Validator {
    public static class ValidationExtensions {
        public static ErrorModel Validate(this CheckyConfiguration configuration) {
            var validator = new ConfigValidator();
            return validator.Validate("Configuration", configuration);
        }

        public static ErrorModel Validate(this CheckyDocument document, string context) {
            var validator = new DocumentValidator();
            return validator.Validate(context, document);
        }

        public static ErrorModel Validate(this CheckyDocumentCollection documentCollection, string context) {
            var validator = new DocumentCollectionValidator();
            return validator.Validate(context, documentCollection);
        }
    }
}