using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Ninject;

namespace Datastore.Environment {
    public class BlobStorageEnvironmentRepository : IEnvironmentRepository {
        private readonly ObjectCache _cache;
        private readonly Uri _containerUri;

        public BlobStorageEnvironmentRepository(
            IConfigurationRepository config,
            [Named(ParameterNames.EnvironmentCache)] ObjectCache cache) {
            _cache = cache;

            _containerUri = new Uri(config.GetConnectionString("EnvironmentsStore"));
        }

        public IEnumerable<string> Find(string environmentStartsWith) {
            return GetBlobs().Where(x => x.StartsWith(environmentStartsWith));
        }

        public EnvironmentDocument Get(string environment) {
            if (_cache.Contains(environment)) {
                return _cache.Get(environment) as EnvironmentDocument;
            }
            var blobs = GetBlobs();
            if (!blobs.Contains(environment)) {
                return null;
            }

            var blobPattern = $"{environment}.json";
            var container = new CloudBlobContainer(_containerUri);
            var blob = container.GetBlockBlobReference(blobPattern);
            var content = blob.DownloadText();
            var document = JsonConvert.DeserializeObject<EnvironmentDocument>(content);
            _cache.Add(environment, document, CachePolicy.Environments);

            return document;
        }

        private IEnumerable<string> GetBlobs() {
            const string cacheKey = "AllEnvironmentBlobsInContainer";
            IEnumerable<string> blobs;
            if (_cache.Contains(cacheKey)) {
                blobs = _cache.Get(cacheKey) as IEnumerable<string>;
            } else {
                blobs = new CloudBlobContainer(_containerUri)
                    .ListBlobs()
                    .Cast<ICloudBlob>()
                    .Select(x => x.Name)
                    .Select(GetBaseName);

                _cache.Add(cacheKey, blobs, CachePolicy.Environments);
            }

            return blobs ?? new string[0];
        }

        private string GetBaseName(string filename) {
            if (!filename.Contains(".")) {
                return filename;
            }

            var dotLocation = filename.LastIndexOf(".", StringComparison.Ordinal);
            var extension = filename.Substring(dotLocation);

            return extension.Length > 0
                ? filename.Remove(filename.Length - extension.Length)
                : filename;
        }
    }
}