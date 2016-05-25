using System;
using ConfigurationLoader.Model;
using Newtonsoft.Json.Linq;

namespace ConfigurationLoader.Validator {
    public class DocumentValidator {
        public ErrorModel Validate(string collectionName, CheckyDocument checkyDocument) {
            if (checkyDocument == null) {
                return ErrorModel.FromErrorMessage($"{collectionName} contains an invalid file");
            }

            JObject json;

            try {
                json = JObject.Parse(checkyDocument.Content);
            } catch (Exception ex) {
                return
                    ErrorModel.FromErrorMessage(
                        $"Unable to parse {checkyDocument.File.Name} in {collectionName}: {ex.Message}");
            }

            var idToken = json.SelectToken("$.id");

            if (idToken == null) {
                return
                    ErrorModel.FromErrorMessage(
                        $"Invalid document {checkyDocument.File.Name} in {collectionName}, document does not contain an id property");
            }

            var baseName = checkyDocument.File.Extension.Length > 0
                ? checkyDocument.File.Name.Remove(checkyDocument.File.Name.Length - checkyDocument.File.Extension.Length)
                : checkyDocument.File.Name;

            var id = idToken.Value<string>();
            if (baseName != id) {
                return
                    ErrorModel.FromErrorMessage(
                        $"Invalid document {checkyDocument.File.Name} or Id in {collectionName}, filename ('{baseName}') does not match id ('{id}') in content");
            }

            return ErrorModel.Valid();
        }
    }
}