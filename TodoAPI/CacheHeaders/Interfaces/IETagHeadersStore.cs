namespace CacheHeaders.Interfaces
{
    /// <summary>
    /// ETag Store interface
    /// </summary>
    public interface IETagHeadersStore
    {
        void Add(string key, string value);
        void Remove(string key);
        bool TryGet(string key, out string value);
    }
}
