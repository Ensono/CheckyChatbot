using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checky.Common.Datastore.Cache;
using ComponentModel;
using Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace Datastore.Test {
    public class BlobStorageHttpTestRepository : IHttpTestRepository {
        private const string Any = "*";
        private readonly IObjectCache<IEnumerable<HttpTestDocument>> _blobCache;
        private readonly Uri _containerUri;
        private readonly IHelpers _helpers;
        private readonly IObjectCache<HttpTestDocument> _testCache;

        public BlobStorageHttpTestRepository(IConfigurationRepository config,
                                             IObjectCache<HttpTestDocument> testCache,
                                             IObjectCache<IEnumerable<HttpTestDocument>> blobCache, IHelpers helpers) {
            _testCache = testCache;
            _blobCache = blobCache;
            _helpers = helpers;
            _containerUri = new Uri(config.GetConnectionString("HttpTestsStore"));
        }

        public IEnumerable<string> Find(string environment = null, string service = null) {
            Func<HttpTestDocument, bool> environmentPredicate;
            Func<HttpTestDocument, bool> servicePredicate;

            if (string.IsNullOrWhiteSpace(environment)) {
                environment = Any;
            }

            if (string.IsNullOrWhiteSpace(service)) {
                service = Any;
            }

            var blobs = GetBlobs();

            if (environment == Any) {
                environmentPredicate = x => x.EnvironmentFilter.Any();
            } else {
                environmentPredicate =
                    x =>
                        x.EnvironmentFilter.Contains(Any) ||
                        x.EnvironmentFilter.Contains(environment, StringComparer.InvariantCultureIgnoreCase);
            }

            if (service == Any) {
                servicePredicate = x => x.ServiceFilter.Any();
            } else {
                servicePredicate =
                    x =>
                        x.ServiceFilter.Contains(Any) ||
                        x.ServiceFilter.Contains(service, StringComparer.InvariantCultureIgnoreCase);
            }

            return blobs
                .Where(environmentPredicate)
                .Where(servicePredicate)
                .Select(x => x.Id);
        }

        public IEnumerable<HttpTestDocument> GetAll(IEnumerable<string> ids) {
            return GetBlobs().Where(x => ids.Contains(x.Id));
        }

        private IEnumerable<HttpTestDocument> GetBlobs() {
            const string cacheKey = "AllHttpTestBlobsInContainer";
            IEnumerable<HttpTestDocument> blobs;
            if (_blobCache.Contains(cacheKey)) {
                blobs = _blobCache.Get(cacheKey);
            } else {
                var container = new CloudBlobContainer(_containerUri);
                blobs = container.ListBlobs()
                    .Cast<ICloudBlob>()
                    .Select(x => container.GetBlockBlobReference(x.Name))
                    .Where(x => !_testCache.Contains(_helpers.GetBaseName(x.Name)))
                    .Select(x => x.DownloadText(Encoding.UTF8))
                    .Select(JsonConvert.DeserializeObject<HttpTestDocument>);

                _blobCache.Add(cacheKey, blobs);
            }

            return blobs ?? new HttpTestDocument[0];
        }
    }
}