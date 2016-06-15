using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using Checky.Common.Configuration;

namespace Checky.Common.Datastore.Cache {
    public class InMemoryCache<T> : IObjectCache<T>, IObjectCache where T : class {
        private readonly TimeSpan _cacheDuration;
        private readonly List<HitOrMiss> _performance = new List<HitOrMiss>();
        private MemoryCache _objectCache;

        public InMemoryCache(IConfigurationRepository config, ICacheManager cacheManager) {
            Name = GetTypeKey(typeof(T));
            var cacheConfig = config.GetAppSetting($"CacheAbsoluteExpirationTimeSpan_{Name}");
            _cacheDuration = TimeSpan.Parse(cacheConfig, CultureInfo.InvariantCulture);
            _objectCache = new MemoryCache(Name);
            cacheManager.Register(Name, this);
        }

        public float HitRate => (float) _performance.Count(x => x == HitOrMiss.Hit)/_performance.Count();

        public T Get(string key) {
            return _objectCache.Get(key) as T;
        }

        public bool Contains(string key) {
            var contains = _objectCache.Contains(key);
            _performance.Add(contains ? HitOrMiss.Hit : HitOrMiss.Miss);
            return contains;
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

        private enum HitOrMiss {
            Hit,
            Miss
        }
    }
}