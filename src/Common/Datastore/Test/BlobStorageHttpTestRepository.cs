using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Ninject;

namespace Datastore.Test {
    public class BlobStorageHttpTestRepository {
        private const string Any = "*";

        private readonly ObjectCache _cache;
        private readonly Uri _containerUri;

        public BlobStorageHttpTestRepository(IConfigurationRepository config,
                                             [Named(ParameterNames.HttpTestCache)] ObjectCache cache) {
            _cache = cache;
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
                environmentPredicate = x => x.EnvironmentFilter.Contains(environment);
            }

            if (service == Any) {
                servicePredicate = x => x.ServiceFilter.Any();
            } else {
                servicePredicate = x => x.ServiceFilter.Contains(service);
            }

            return blobs
                .Where(environmentPredicate)
                .Where(servicePredicate)
                .Select(x => x.Id);
        }

        private IEnumerable<HttpTestDocument> GetBlobs() {
            const string cacheKey = "AllHttpTestBlobsInContainer";
            IEnumerable<HttpTestDocument> blobs;
            if (_cache.Contains(cacheKey)) {
                blobs = _cache.Get(cacheKey) as IEnumerable<HttpTestDocument>;
            } else {
                var container = new CloudBlobContainer(_containerUri);
                blobs = container.ListBlobs()
                    .Cast<ICloudBlob>()
                    .Select(x => container.GetBlockBlobReference(x.Name))
                    .Select(x => x.DownloadText(Encoding.UTF8))
                    .Select(JsonConvert.DeserializeObject<HttpTestDocument>);

                _cache.Add(cacheKey, blobs, CachePolicy.HttpTests);
            }

            return blobs ?? new HttpTestDocument[0];
        }
    }
}