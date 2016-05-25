using System.IO;
using ConfigurationLoader.Model;

namespace ConfigurationLoader.Loader {
    public class ConfigurationLoader {
        public CheckyConfiguration Load(string configurationPath) {
            var resolvedPath = Path.GetFullPath(configurationPath);
            if (!Directory.Exists(resolvedPath)) {
                return null;
            }

            var documentLoader = new DocumentLoader();

            return new CheckyConfiguration {
                Directory = new DirectoryInfo(resolvedPath),
                Environments = documentLoader.Load(Path.Combine(resolvedPath, "environments")),
                Tests = documentLoader.Load(Path.Combine(resolvedPath, "tests"))
            };
        }
    }
}