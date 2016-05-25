using System.Linq;
using Datastore;
using Loader.Model;

namespace Loader.Validator {
    public class DocumentCollectionValidator<T> : IValidator<CheckyDocumentCollection<T>> where T : PersistentDocument {
        public DocumentValidator<T> DocumentValidator = new DocumentValidator<T>();

        public ErrorModel Validate(string context, CheckyDocumentCollection<T> collection) {
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