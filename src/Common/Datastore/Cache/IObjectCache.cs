namespace Checky.Common.Datastore.Cache {
    public interface IObjectCache<T> where T : class {
        string Name { get; }
        void Add(string key, T value);
        T Get(string key);
        void Clear(string key = null);
        bool Contains(string key);
    }

    public interface IObjectCache {
        string Name { get; }
        float HitRate { get; }
        void Clear(string key = null);
        bool Contains(string key);
    }
}