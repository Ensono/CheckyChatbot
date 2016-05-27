using Datastore;

namespace Loader.Validator {
    public class TestValidator : ModelValidator<TestDocument> {
        public override ErrorModel Validate(string context, TestDocument model) {
            return ErrorModel.Valid();
        }
    }
}