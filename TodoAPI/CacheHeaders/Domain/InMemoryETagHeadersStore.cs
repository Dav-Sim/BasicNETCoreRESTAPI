using CacheHeaders.Interfaces;
using System.Collections.Concurrent;

namespace CacheHeaders.Domain
{
    /// <summary>
    /// In memory ETag Store
    /// </summary>
    public class InMemoryETagHeadersStore : IETagHeadersStore
    {
        private ConcurrentDictionary<string, string> RequestKeyETagValueDictionary = new ConcurrentDictionary<string, string>();

        public void Remove(string key) => RequestKeyETagValueDictionary.TryRemove(key, out _);
        public void Add(string key, string value) => RequestKeyETagValueDictionary[key] = value;
        public bool TryGet(string key, out string value) => RequestKeyETagValueDictionary.TryGetValue(key, out value);
    }
}
