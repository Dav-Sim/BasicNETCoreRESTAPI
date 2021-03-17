using CacheHeaders.Common;

namespace CacheHeaders.Domain
{
    /// <summary>
    /// Options to ETagger middleware related to expiration
    /// </summary>
    public class ExpirationOptions
    {
        public int MaxAge { get; set; } = 60;

        public int? SharedMaxAge { get; set; }

        public ETaggerCommon.CacheLocation CacheLocation { get; set; } = ETaggerCommon.CacheLocation.Public;

        public bool NoStore { get; set; } = false;

        public bool NoTransform { get; set; } = false;
    }
}
