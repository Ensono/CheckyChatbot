using System.Drawing;
using System.IO;
using System.Linq;
using Colorful;
using ManyConsole;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ConfigurationLoader {
    public class UploadCommand : ConsoleCommand {
        public string AccessKey;
        public string ConfigPath;
        public string StorageAccount;

        public UploadCommand() {
            IsCommand("upload", "Upload configuration.");
            HasOption("p|path:", "Configuration Path", p => ConfigPath = Path.GetFullPath(p));
            HasOption("s|account:", "Storage Account Name", s => StorageAccount = s);
            HasOption("k|key:", "Storage Account Key", k => AccessKey = k);
        }

        public override int Run(string[] remainingArguments) {
            if (!Directory.Exists(ConfigPath)) {
                Console.WriteLine($"FATAL: Unable to find path: {ConfigPath}", Color.Red);
                return 1;
            }

            var credentials = new StorageCredentials(StorageAccount, AccessKey);
            var account = new CloudStorageAccount(credentials, true);
            var client = account.CreateCloudBlobClient();
            var containers = client.ListContainers();

            CloudBlobContainer environmentsContainer;
            CloudBlobContainer testsContainer;

            try {
                environmentsContainer = containers.SingleOrDefault(x => x.Name == "environments");
                testsContainer = containers.SingleOrDefault(x => x.Name == "tests");
            } catch (StorageException stex) {
                Console.WriteLine($"FATAL: {stex.Message}", Color.Red);
                return 1;
            }
            return 0;
        }
    }
}