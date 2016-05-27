using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Loader.Loader;
using Loader.Specifications;
using Loader.Validator;
using ManyConsole;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Specifications.Extensions;

namespace Loader {
    public class UploadCommand : ConsoleCommand {
        private readonly BlobRequestOptions requestOptions = new BlobRequestOptions {
            RetryPolicy = new ExponentialRetry(),
            StoreBlobContentMD5 = true,
            UseTransactionalMD5 = true
        };

        public string AccessKey;
        public string ConfigPath;
        public string StorageAccount;

        public UploadCommand() {
            IsCommand("upload", "Upload configuration.");
            HasOption("p|path:", "Configuration Path", p => ConfigPath = Path.GetFullPath(p));
            HasOption("s|account:", "Storage Account Name", s => StorageAccount = s);
            HasOption("k|key:", "Storage Account Key", k => AccessKey = k);
        }

        private static Action<string, bool> Assert => ConsoleUtilities.WriteResult;

        private string CalculateHash(FileInfo file) {
            using (var md5 = MD5.Create()) {
                using (var stream = File.OpenRead(file.FullName)) {
                    return Encoding.UTF8.GetString(md5.ComputeHash(stream));
                }
            }
        }

        public override int Run(string[] remainingArguments) {
            ConsoleUtilities.WriteAscii("@CHECKY", ConsoleColor.DarkBlue);
            ConsoleUtilities.WriteAscii("UPLOAD", ConsoleColor.Cyan);

            var loader = new ConfigurationLoader();
            var configuration = loader.Load(ConfigPath);
            var result = configuration.Validate();

            if (!result.IsValid) {
                Console.WriteLine("One ore more documents have validation errors, please run:");
                Console.WriteLine($"  checky-loader validate --path:{ConfigPath}");
                ConsoleUtilities.WriteAscii("FAILED", ConsoleUtilities.Failure);
                return 1;
            }

            var credentials = new StorageCredentials(StorageAccount, AccessKey);
            var account = new CloudStorageAccount(credentials, true);
            var client = account.CreateCloudBlobClient();

            ConsoleUtilities.WriteLine("Containers:", ConsoleUtilities.Normal);
            var environmentsContainer = EnsureBlobContainerExists(client, "environments").NotNull(" ├─ Environments");
            var testsContainer = EnsureBlobContainerExists(client, "tests").NotNull(" └─ Tests");

            if (environmentsContainer == null || testsContainer == null) {
                ConsoleUtilities.WriteAscii("FAILED", ConsoleUtilities.Failure);
                return 1;
            }

            Console.WriteLine();
            Console.WriteLine("Environments:");
            var environments = configuration.Environments.Documents.ToArray();
            foreach (var environment in environments) {
                var blob = environmentsContainer.GetBlobReference(environment.File.Name);
                var divide = environment != environments.Last() ? "├" : "└";
                var spacer = environment != environments.Last() ? "│" : " ";
                var access = new AccessCondition();
                ConsoleUtilities.WriteLine($" {divide}─ {environment.File.Name}");
                if (blob.Satisfies<Exists>()) {
                    Assert($" {spacer}  ├─ is block blob", blob.Satisfies<IsBlockBlob>());
                    Assert($" {spacer}  ├─ is leasable", blob.Satisfies<IsLeasable>());
                    if (!blob.Satisfies<ExistingBlobIsWritable>()) {
                        continue;
                    }

                    var requiresUpdate = new RequiresUpdate(CalculateHash(environment.File));

                    if (blob.Satisfies(requiresUpdate.Not())) {
                        Assert($" {spacer}  └─ already up to date", true);
                    }

                    access.LeaseId = blob.AcquireLease(TimeSpan.FromSeconds(15), null);
                }

                var blockBlob = environmentsContainer.GetBlockBlobReference(environment.File.Name);
                blockBlob.UploadFromFile(environment.File.FullName, access, requestOptions);

                if (access.LeaseId != null) {
                    blob.ReleaseLease(access);
                }
            }

            return 0;
        }

        private static CloudBlobContainer EnsureBlobContainerExists(CloudBlobClient client, string containerName) {
            var containers = client.ListContainers();
            CloudBlobContainer container;

            try {
                container = client.GetContainerReference(containerName);
            } catch (StorageException stex) {
                ConsoleUtilities.WriteLine(
                    $"Encountered error when retreving the '{containerName}' container {stex.Message}",
                    ConsoleUtilities.Normal);
                return null;
            }

            try {
                container.CreateIfNotExists();
            } catch (StorageException stex) {
                ConsoleUtilities.WriteLine(
                    $"Encountered error when creating the '{containerName}' container {stex.Message}",
                    ConsoleUtilities.Normal);
                return null;
            }

            try {
                container.SetPermissions(new BlobContainerPermissions {PublicAccess = BlobContainerPublicAccessType.Off});
            } catch (StorageException stex) {
                ConsoleUtilities.WriteLine(
                    $"Encountered error when setting permissions on the '{containerName}' container {stex.Message}",
                    ConsoleUtilities.Normal);
                return null;
            }

            return container;
        }
    }
}