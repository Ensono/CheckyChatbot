using Checky.Common.Datastore;

namespace Checky.Common.Loader.Validator {
    public abstract class ModelValidator<T> : IValidator<T> where T : PersistentDocument {
        public abstract ErrorModel Validate(string context, T model);
    }
}