using System;
using System.Linq;
using Checky.Common.Datastore.Environment;

namespace Checky.Common.Loader.Validator {
    public class EnvironmentValidator : ModelValidator<EnvironmentDocument> {
        public override ErrorModel Validate(string context, EnvironmentDocument model) {
            if (model.Services == null || !model.Services.Any()) {
                return ErrorModel.FromErrorMessage($"{model.Id} contains no services, this environment would be unused.");
            }

            var keys = model.Services.GroupBy(x => x.Name).ToArray();
            Func<IGrouping<string, Service>, bool> keysPredicate = x => x.Count() > 1;
            if (keys.Any(keysPredicate)) {
                var duplicates = string.Join(", ", keys.Where(keysPredicate).Select(x => x.Key));
                return ErrorModel.FromErrorMessage(
                    $"Environment '{model.Id}' contains duplicate services, duplicates: {duplicates}.");
            }

            var uris = model.Services.GroupBy(x => x.BaseUri).ToArray();
            Func<IGrouping<Uri, Service>, bool> urisPredicate = x => x.Count() > 1;
            if (uris.Any(urisPredicate)) {
                var duplicates = string.Join(", ", uris.Where(urisPredicate).Select(x => x.Key));
                return ErrorModel.FromErrorMessage(
                    $"Environment '{model.Id}' contains duplicate URIs, duplicates: {duplicates}");
            }

            return ErrorModel.Valid();
        }
    }
}