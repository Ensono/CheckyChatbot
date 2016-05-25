namespace Loader.Validator {
    public interface IValidator<in T> {
        ErrorModel Validate(string context, T checkyDocument);
    }
}