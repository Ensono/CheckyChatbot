using Checky.Common.Datastore.Test;

namespace Checky.Common.Loader.Validator {
    public class TestValidator : ModelValidator<HttpTestDocument> {
        public override ErrorModel Validate(string context, HttpTestDocument model) {
            return ErrorModel.Valid();
        }
    }
}