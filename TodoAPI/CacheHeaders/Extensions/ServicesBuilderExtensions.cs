using CacheHeaders.Domain;
using CacheHeaders.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CacheHeaders.Extensions
{

    /// <summary>
    /// Extensions for IServiceCollection
    /// </summary>
    public static class ServicesBuilderExtensions
    {
        /// <summary>
        /// Adds ETagger to service collection
        /// ETagger is middleware which adds ETag/CacheControl etc. headers
        /// In case of GET/HEAD request with IfNoneMatch header, will serve 304 NotModified (if not modified).
        /// In case of PUT/PATCH request with IfMatch header, will serve 412 Precondition failed (if ETag not match)
        /// </summary>
        public static IServiceCollection AddETagger(
          this IServiceCollection services,
          Action<ExpirationOptions> expirationOptions = null,
          Action<ValidationOptions> validationOptions = null)
        {
            services.AddSingleton<IETagHeadersStore, InMemoryETagHeadersStore>();

            if (expirationOptions != null)
            {
                services.Configure(expirationOptions);
            }

            if (validationOptions != null)
            {
                services.Configure(validationOptions);
            }

            return services;
        }
    }
}
