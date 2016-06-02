using System;
using System.Runtime.Caching;

namespace Datastore {
    public static class CachePolicy {
        public static readonly CacheItemPolicy CommonPolicy = new CacheItemPolicy {
            AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(24)
        };

        public static readonly CacheItemPolicy Environments = CommonPolicy;
        public static readonly CacheItemPolicy HttpTests = CommonPolicy;
    }
}