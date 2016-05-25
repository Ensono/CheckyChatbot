using System.Linq;
using Loader.Model;

namespace Loader.Validator {
    public class DocumentCollectionValidator {
        public DocumentValidator DocumentValidator = new DocumentValidator();

        public ErrorModel Validate(string collectionName, CheckyDocumentCollection collection) {
            if (collection == null) {
                return
                    ErrorModel.FromErrorMessage(
                        $"Invalid {collectionName} collection: the specified directory does not exist.");
            }

            var errorRecords = collection.Documents
                .Select(x => DocumentValidator.Validate(collectionName, x));

            return ErrorModel.FromErrorModels(errorRecords);
        }
    }
}