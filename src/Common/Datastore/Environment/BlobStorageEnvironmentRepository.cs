using System;
using System.Collections.Generic;
using System.Linq;
using Checky.Common.ComponentModel;
using Checky.Common.Configuration;
using Checky.Common.Datastore.Cache;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace Checky.Common.Datastore.Environment {
    public class BlobStorageEnvironmentRepository : IEnvironmentRepository {
        private readonly IObjectCache<IEnumerable<ICloudBlob>> _blobCache;
        private readonly Uri _containerUri;
        private readonly IObjectCache<EnvironmentDocument> _environmentCache;
        private readonly IHelpers _helpers;

        public BlobStorageEnvironmentRepository(
            IConfigurationRepository config,
            IObjectCache<EnvironmentDocument> environmentCache, IObjectCache<IEnumerable<ICloudBlob>> blobCache,
            IHelpers helpers) {
            _environmentCache = environmentCache;
            _blobCache = blobCache;
            _helpers = helpers;

            _containerUri = new Uri(config.GetConnectionString("EnvironmentsStore"));
        }

        public IEnumerable<string> Find(string environmentStartsWith) {
            return GetBlobs().Where(x => x.StartsWith(environmentStartsWith));
        }

        public EnvironmentDocument Get(string environment) {
            if (_environmentCache.Contains(environment)) {
                return _environmentCache.Get(environment);
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
            _environmentCache.Add(environment, document);

            return document;
        }

        private IEnumerable<string> GetBlobs() {
            const string cacheKey = "AllEnvironmentBlobsInContainer";
            IEnumerable<ICloudBlob> blobs;
            if (_blobCache.Contains(cacheKey)) {
                blobs = _blobCache.Get(cacheKey).ToArray();
            } else {
                blobs = new CloudBlobContainer(_containerUri)
                    .ListBlobs()
                    .Cast<ICloudBlob>()
                    .ToArray();

                _blobCache.Add(cacheKey, blobs);
            }

            return blobs
                .Select(x => x.Name)
                .Select(_helpers.GetBaseName);
        }
    }
}