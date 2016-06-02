using System;
using Checky.Common.Datastore;
using Checky.Common.Loader.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Checky.Common.Loader.Validator {
    public class DocumentValidator<T> : IValidator<CheckyDocument<T>> where T : PersistentDocument {
        public ErrorModel Validate(string context, CheckyDocument<T> checkyDocument) {
            if (checkyDocument == null) {
                return ErrorModel.FromErrorMessage($"{context} contains an invalid file");
            }

            JObject json;

            try {
                json = JObject.Parse(checkyDocument.Content);
            } catch (Exception ex) {
                return
                    ErrorModel.FromErrorMessage(
                        $"Unable to parse {checkyDocument.File.Name} in {context}: {ex.Message}");
            }

            var idToken = json.SelectToken("$.id");

            if (idToken == null) {
                return
                    ErrorModel.FromErrorMessage(
                        $"Invalid document {checkyDocument.File.Name} in {context}, document does not contain an id property");
            }

            var baseName = checkyDocument.File.Extension.Length > 0
                ? checkyDocument.File.Name.Remove(checkyDocument.File.Name.Length - checkyDocument.File.Extension.Length)
                : checkyDocument.File.Name;

            var id = idToken.Value<string>();
            if (baseName != id) {
                return
                    ErrorModel.FromErrorMessage(
                        $"Invalid document {checkyDocument.File.Name} or Id in {context}, filename ('{baseName}') does not match id ('{id}') in content");
            }

            T actualObject;

            try {
                actualObject = JsonConvert.DeserializeObject<T>(checkyDocument.Content);
            } catch (Exception ex) {
                return ErrorModel.FromErrorMessage(
                    $"Document does not match the {nameof(T)} model {checkyDocument.File.Name} or Id in {context}: {ex.Message}");
            }

            return actualObject.Validate();
        }
    }
}