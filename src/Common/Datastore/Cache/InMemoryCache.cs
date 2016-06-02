using System;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using Configuration;

namespace Checky.Common.Datastore.Cache {
    public class InMemoryCache<T> : IObjectCache<T>, IObjectCache where T : class {
        private readonly TimeSpan _cacheDuration;
        private MemoryCache _objectCache;

        public InMemoryCache(IConfigurationRepository config) {
            Name = GetTypeKey(typeof(T));
            var cacheConfig = config.GetAppSetting($"CacheAbsoluteExpirationTimeSpan_{Name}");
            _cacheDuration = TimeSpan.Parse(cacheConfig);
            _objectCache = new MemoryCache(Name);
        }

        public T Get(string key) {
            return _objectCache.Get(key) as T;
        }

        public bool Contains(string key) {
            return _objectCache.Contains(key);
        }

        public string Name { get; }

        public void Clear(string key = null) {
            var oldCache = _objectCache;
            oldCache.Dispose();
            _objectCache = new MemoryCache(Name);
        }

        public void Add(string key, T value) {
            var policy = new CacheItemPolicy {
                AbsoluteExpiration = DateTimeOffset.UtcNow.Add(_cacheDuration)
            };
            _objectCache.Add(key, value, policy);
        }

        private string GetTypeKey(Type type) {
            if (!type.IsGenericType) {
                return type.Name;
            }

            var builder = new StringBuilder();
            builder.Append(type.Name.Split('`')[0]);
            builder.Append("Of");

            var arguments = type.GenericTypeArguments.Select(GetTypeKey);
            builder.Append(string.Join("And", arguments));

            return builder.ToString();
        }
    }
}