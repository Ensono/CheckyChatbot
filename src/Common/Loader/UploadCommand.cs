using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Datastore;
using Loader.Loader;
using Loader.Model;
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

            var environmentUploader = new Uploader<EnvironmentDocument>();
            var testsUploader = new Uploader<TestDocument>();

            Console.WriteLine("\nEnvironments:");
            var environmentResult = environmentUploader.Upload(configuration.Environments.Documents,
                environmentsContainer);

            Console.WriteLine("\nTests:");
            var testResult = testsUploader.Upload(configuration.Tests.Documents, testsContainer);

            var finalResult = ErrorModel.FromErrorModels(environmentResult, testResult);

            if (!finalResult.IsValid) {
                ConsoleUtilities.WriteAscii("FAILED", ConsoleUtilities.Failure);
                ConsoleUtilities.WriteLine($"The following {result.Errors.Count()} errors were encountered:",
                    ConsoleUtilities.Normal);
                foreach (var error in finalResult.Errors)
                {
                    ConsoleUtilities.Write(" ✘ ", ConsoleUtilities.Failure);
                    ConsoleUtilities.WriteLine(error, ConsoleUtilities.Normal);
                }
                return 1;
            }
            ConsoleUtilities.WriteAscii("SUCCEEDED", ConsoleUtilities.Success);
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

    public class Uploader<T> where T : PersistentDocument {
        private readonly BlobRequestOptions requestOptions = new BlobRequestOptions {
            RetryPolicy = new ExponentialRetry(),
            StoreBlobContentMD5 = true,
            UseTransactionalMD5 = true
        };

        public static bool Assert(string message, bool result) {
            ConsoleUtilities.WriteResult(message, result);
            return result;
        }

        public ErrorModel Upload(IEnumerable<CheckyDocument<T>> doucments, CloudBlobContainer container) {
            var checkyDocuments = doucments.ToArray();
            var errors = new List<string>();
            foreach (var document in checkyDocuments) {
                var access = new AccessCondition();
                var blob = container.GetBlobReference(document.File.Name);
                try {
                    var divide = document != checkyDocuments.Last() ? "├" : "└";
                    var spacer = document != checkyDocuments.Last() ? "│" : " ";
                    ConsoleUtilities.WriteLine($" {divide}─ {document.File.Name}");
                    if (blob.Satisfies<Exists>()) {
                        Assert($" {spacer}  ├─ is block blob", blob.Satisfies<IsBlockBlob>());

                        if (!blob.Satisfies<IsBlockBlob>()) {
                            errors.Add($"{document.File.Name} is not a block blob");
                            continue;
                        }

                        if (!Assert($" {spacer}  ├─ is leasable", blob.Satisfies<IsLeasable>())) {
                            errors.Add($"{document.File.Name} is not leasable");
                            continue;
                        }

                        access.LeaseId = blob.AcquireLease(TimeSpan.FromSeconds(15), null);

                        var requiresUpdate = new RequiresUpdate(document.MD5);

                        if (blob.Satisfies(requiresUpdate.Not())) {
                            Assert($" {spacer}  └─ already up to date", true);
                            continue;
                        }

                        Assert($" {spacer}  └─ needs refresh", true);
                    }

                    var blockBlob = container.GetBlockBlobReference(document.File.Name);
                    blockBlob.UploadFromFile(document.File.FullName, access, requestOptions);
                } finally {
                    if (access.LeaseId != null) {
                        blob.ReleaseLease(access);
                    }
                }
            }
            return new ErrorModel {
                IsValid = !errors.Any(),
                Errors = errors
            };
        }
    }
}