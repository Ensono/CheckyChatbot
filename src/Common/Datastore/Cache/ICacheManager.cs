using System;
using System.Threading.Tasks;

namespace Checky.Common.Datastore.Cache {
    public interface ICacheManager {
        Task ClearAll(Func<string, Task> responseHandler);

        void Register(string cacheName, IObjectCache cache);
        Task Performance(Func<string, Task> responseHandler);
    }
}