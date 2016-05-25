using System.IO;
using Datastore;
using Loader.Model;

namespace Loader.Loader {
    public class ConfigurationLoader {
        public CheckyConfiguration Load(string configurationPath) {
            var resolvedPath = Path.GetFullPath(configurationPath);
            if (!Directory.Exists(resolvedPath)) {
                return null;
            }

            var environmentDocumentLoader = new DocumentLoader<Environment>();
            var testDocumentLoader = new DocumentLoader<Test>();

            return new CheckyConfiguration {
                Directory = new DirectoryInfo(resolvedPath),
                Environments = environmentDocumentLoader.Load(Path.Combine(resolvedPath, "environments")),
                Tests = testDocumentLoader.Load(Path.Combine(resolvedPath, "tests"))
            };
        }
    }
}