using Datastore;
using Loader.Model;

namespace Loader.Validator {
    public static class ValidationExtensions {
        public static ErrorModel Validate(this CheckyConfiguration configuration) {
            var validator = new ConfigValidator();
            return validator.Validate("Configuration", configuration);
        }

        public static ErrorModel Validate<T>(this CheckyDocument<T> document, string context)
            where T : PersistentDocument {
            var validator = new DocumentValidator<T>();
            return validator.Validate(context, document);
        }

        public static ErrorModel Validate<T>(this CheckyDocumentCollection<T> documentCollection, string context)
            where T : PersistentDocument {
            var validator = new DocumentCollectionValidator<T>();
            return validator.Validate(context, documentCollection);
        }

        public static ErrorModel Validate<T>(this T target) where T : PersistentDocument {
            if (target is Environment) {
                var validator = new EnvironmentValidator();
                return validator.Validate("Environments", target as Environment);
            }

            if (target is Test) {
                var validator = new TestValidator();
                return validator.Validate("Tests", target as Test);
            }

            return ErrorModel.FromErrorMessage($"Unable to find validator to match {nameof(T)}");
        }
    }
}