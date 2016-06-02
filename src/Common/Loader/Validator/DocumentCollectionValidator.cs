using System.Linq;
using Checky.Common.Datastore;
using Checky.Common.Loader.Model;

namespace Checky.Common.Loader.Validator {
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