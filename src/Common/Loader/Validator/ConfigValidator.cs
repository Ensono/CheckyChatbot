using Checky.Common.Datastore.Environment;
using Checky.Common.Datastore.Test;
using Checky.Common.Loader.Model;

namespace Checky.Common.Loader.Validator {
    public class ConfigValidator : IValidator<CheckyConfiguration> {
        public ErrorModel Environments { get; private set; }
        public ErrorModel Tests { get; private set; }

        public ErrorModel Validate(string context, CheckyConfiguration configuration) {
            if (configuration == null) {
                return ErrorModel.FromErrorMessage("The specified configuration directory does not exist.");
            }

            var environmentDocumentCollectionValidator = new DocumentCollectionValidator<EnvironmentDocument>();
            var testDocumentCollectionValidator = new DocumentCollectionValidator<HttpTestDocument>();

            Environments = environmentDocumentCollectionValidator.Validate("Environments", configuration.Environments);
            Tests = testDocumentCollectionValidator.Validate("Tests", configuration.Tests);

            return ErrorModel.FromErrorModels(Environments, Tests);
        }
    }
}