using System.Linq;
using Loader.Model;

namespace Loader.Validator {
    public class DocumentCollectionValidator : IValidator<CheckyDocumentCollection> {
        public DocumentValidator DocumentValidator = new DocumentValidator();

        public ErrorModel Validate(string context, CheckyDocumentCollection collection) {
            if (collection == null) {
                return
                    ErrorModel.FromErrorMessage(
                        $"Invalid {context} collection: the specified directory does not exist.");
            }

            var errorRecords = collection.Documents
                .Select(x => DocumentValidator.Validate(context, x));

            return ErrorModel.FromErrorModels(errorRecords);
        }
    }
}