using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Loader {
    public static class Utilities {
        public static CloudBlobContainer EnsureBlobContainerExists(CloudBlobClient client, string containerName) {
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