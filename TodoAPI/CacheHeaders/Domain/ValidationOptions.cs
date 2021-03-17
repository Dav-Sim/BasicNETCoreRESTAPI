using System.Collections.Generic;

namespace CacheHeaders.Domain
{
    /// <summary>
    /// Options to ETagger middleware related to validation
    /// </summary>
    public class ValidationOptions
    {
        /// <summary>
        /// Defaults: Accept, Accept-Language, Accept-Encoding.  * indicates all request headers can be taken into account.
        /// </summary>
        public IList<string> Vary { get; set; } = new List<string> { "Accept", "Accept-Language", "Accept-Encoding" };

        public bool NoCache { get; set; } = false;

        public bool MustRevalidate { get; set; } = false;

        public bool ProxyRevalidate { get; set; } = false;

    }
}
