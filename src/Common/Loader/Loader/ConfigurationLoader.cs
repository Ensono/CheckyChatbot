using System.IO;
using Checky.Common.Datastore.Environment;
using Checky.Common.Datastore.Test;
using Checky.Common.Loader.Model;

namespace Checky.Common.Loader.Loader {
    public class ConfigurationLoader {
        public CheckyConfiguration Load(string configurationPath) {
            var resolvedPath = Path.GetFullPath(configurationPath);
            if (!Directory.Exists(resolvedPath)) {
                return null;
            }

            var environmentDocumentLoader = new DocumentLoader<EnvironmentDocument>();
            var testDocumentLoader = new DocumentLoader<HttpTestDocument>();

            return new CheckyConfiguration {
                Directory = new DirectoryInfo(resolvedPath),
                Environments = environmentDocumentLoader.Load(Path.Combine(resolvedPath, "environments")),
                Tests = testDocumentLoader.Load(Path.Combine(resolvedPath, "tests"))
            };
        }
    }
}