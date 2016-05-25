using ConfigurationLoader.Model;

namespace ConfigurationLoader.Validator {
    public class ConfigValidator {
        public ErrorModel Environments { get; private set; }
        public ErrorModel Tests { get; private set; }

        public ErrorModel Validate(CheckyConfiguration configuration) {
            if (configuration == null) {
                return ErrorModel.FromErrorMessage("The specified configuration directory does not exist.");
            }

            var documentCollectionValidator = new DocumentCollectionValidator();
            Environments = documentCollectionValidator.Validate("Environments", configuration.Environments);
            Tests = documentCollectionValidator.Validate("Tests", configuration.Tests);

            return ErrorModel.FromErrorModels(Environments, Tests);
        }
    }
}