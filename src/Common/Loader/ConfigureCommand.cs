using System;
using System.Collections.Generic;
using System.Linq;
using ManyConsole;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Checky.Common.Loader {
    public class ConfigureCommand : ConsoleCommand {
        public string AccessKey;
        public string ConfigPath;
        public string StorageAccount;

        public ConfigureCommand() {
            IsCommand("configure", "Display account configuration.");
            HasOption("s|account:", "Storage Account Name", s => StorageAccount = s);
            HasOption("k|key:", "Storage Account Key", k => AccessKey = k);
        }

        public override int Run(string[] remainingArguments) {
            ConsoleUtilities.WriteAscii("@CHECKY", ConsoleColor.DarkBlue);
            ConsoleUtilities.WriteAscii("CONFIGURE", ConsoleColor.Cyan);

            var credentials = new StorageCredentials(StorageAccount, AccessKey);
            var account = new CloudStorageAccount(credentials, true);
            var client = account.CreateCloudBlobClient();

            ConsoleUtilities.WriteLine("Containers:", ConsoleUtilities.Normal);
            var containers = new Dictionary<string, CloudBlobContainer> {
                {
                    "Environments",
                    Utilities.EnsureBlobContainerExists(client, "environments").NotNull(" ├─ Environments")
                }, {
                    "Tests",
                    Utilities.EnsureBlobContainerExists(client, "tests").NotNull(" └─ Tests")
                }
            };

            var policy = new SharedAccessBlobPolicy {
                Permissions = SharedAccessBlobPermissions.List | SharedAccessBlobPermissions.Read,
                SharedAccessStartTime = DateTimeOffset.UtcNow,
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddYears(1)
            };

            foreach (var key in containers.Keys) {
                var sharedAccessPolicyName = $"{key}ReadAccessPolicy";
                var permissions = containers[key].GetPermissions();
                if (!permissions.SharedAccessPolicies.ContainsKey(sharedAccessPolicyName)) {
                    permissions.SharedAccessPolicies.Add(sharedAccessPolicyName, policy);
                    containers[key].SetPermissions(permissions);
                }
                var permission = permissions.SharedAccessPolicies[sharedAccessPolicyName];
                var sasToken = containers[key].GetSharedAccessSignature(null, sharedAccessPolicyName);
                ConsoleUtilities.WriteLine($"\n{key}:", ConsoleColor.White);
                ConsoleUtilities.WriteLine($" ├─ Expires: {permission.SharedAccessExpiryTime}");
                ConsoleUtilities.WriteLine(" ├─ Permissions:");

                var possiblePermissions = typeof(SharedAccessBlobPermissions)
                    .GetEnumValues()
                    .Cast<SharedAccessBlobPermissions>()
                    .Where(x => x != SharedAccessBlobPermissions.None)
                    .ToList();

                foreach (var possiblePermission in possiblePermissions) {
                    var separator = possiblePermission == possiblePermissions.Last() ? "└" : "├";
                    ConsoleUtilities.WriteResult($" │  {separator}─ {possiblePermission}",
                        permission.Permissions.HasFlag(possiblePermission));
                }

                ConsoleUtilities.WriteLine($" └─ Key: {containers[key].Uri}{sasToken}");
            }

            ConsoleUtilities.WriteLine(
                Environment.NewLine +
                "NOTE: It is not nessecary to update the key after each upload, only when it is getting close to expiry.",
                ConsoleColor.Yellow);

            ConsoleUtilities.WriteAscii("SUCCESS", ConsoleUtilities.Success);

            return 0;
        }
    }
}