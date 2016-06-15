using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checky.Common.Datastore.Cache {
    internal class CacheManager : ICacheManager {
        private readonly Dictionary<string, IObjectCache> _caches = new Dictionary<string, IObjectCache>();

        public Task ClearAll(Func<string, Task> responseHandler) {
            _caches.Values.ToList().ForEach(c => c.Clear());
            return responseHandler($"Cleared {_caches.Count} caches: {string.Join(", ", _caches.Keys)}");
        }

        public void Register(string cacheName, IObjectCache cache) {
            if (!_caches.ContainsKey(cacheName)) {
                _caches.Add(cacheName, cache);
            } else {
                _caches[cacheName] = cache;
            }
        }

        public Task Performance(Func<string, Task> responseHandler) {
            var builder = new StringBuilder();
            foreach (var cache in _caches.Values) {
                builder.AppendLine(!float.IsNaN(cache.HitRate)
                    ? $"{cache.Name}: {cache.HitRate*100}%"
                    : $"{cache.Name}: Cold");
            }
            return responseHandler(builder.ToString());
        }
    }
}