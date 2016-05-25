using System;
using Datastore;

namespace Loader.Validator {
    public class TestValidator : ModelValidator<Test> {
        public override ErrorModel Validate(string context, Test model) {
            return ErrorModel.Valid();
        }
    }
}