using HttpCacheHeaders.Domain;
using HttpCacheHeaders.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HttpCacheHeaders.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HttpCacheExpirationAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly Lazy<ExpirationModelOptions> _expirationModelOptions;

        /// <summary>
        /// Maximum age, in seconds, after which a response expires. Has an effect on Expires & on the max-age directive
        /// of the Cache-Control header.
        ///
        /// Defaults to 60.
        /// </summary>
        public int MaxAge { get; set; } = 60;

        /// <summary>
        /// Maximum age, in seconds, after which a response expires for shared caches.  If included,
        /// a shared cache should use this value rather than the max-age value. (s-maxage directive)
        ///
        /// Not set by default.
        /// </summary>
        public int? SharedMaxAge { get; set; }

        /// <summary>
        /// The location where a response can be cached.  Public means it can be cached by both
        /// public (shared) and private (client) caches.  Private means it can only be cached by
        /// private (client) caches. (public or private directive)
        ///
        /// Defaults to public.
        /// </summary>
        public CacheLocation CacheLocation { get; set; } = CacheLocation.Public;

        /// <summary>
        /// When true, the no-store directive is included in the Cache-Control header.
        /// When this directive is included, a cache must not store any part of the message,
        /// mostly for confidentiality reasons.
        ///
        /// Defaults to false.
        /// </summary>
        public bool NoStore { get; set; } = false;

        /// <summary>
        /// When true, the no-transform directive is included in the Cache-Control header.
        /// When this directive is included, a cache must not convert the media type of the
        /// response body.
        ///
        /// Defaults to false.
        /// </summary>
        public bool NoTransform { get; set; } = false;

        public HttpCacheExpirationAttribute()
        {
            _expirationModelOptions = new Lazy<ExpirationModelOptions>(() => new ExpirationModelOptions
            {
                MaxAge = MaxAge,
                SharedMaxAge = SharedMaxAge,
                CacheLocation = CacheLocation,
                NoStore = NoStore,
                NoTransform = NoTransform
            });
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            await next();

            // add options to Items dictionary.  If the dictionary already contains a value, don't overwrite it - this 
            // means the value was already set at method level and the current class level attribute is trying
            // to overwrite it.  Method (action) should win over class (controller).

            if (!context.HttpContext.Items.ContainsKey(HttpContextExtensions.ContextItemsExpirationModelOptions))
            {
                context.HttpContext.Items[HttpContextExtensions.ContextItemsExpirationModelOptions] = _expirationModelOptions.Value;
            }
        }
    }
}
