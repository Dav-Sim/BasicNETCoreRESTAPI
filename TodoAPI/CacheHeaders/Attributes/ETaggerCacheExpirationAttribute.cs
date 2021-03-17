using CacheHeaders.Common;
using CacheHeaders.Domain;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace CacheHeaders.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ETaggerCacheExpirationAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly Lazy<ExpirationOptions> _expirationOptions;

        public int MaxAge { get; set; } = 60;

        public int? SharedMaxAge { get; set; }

        public ETaggerCommon.CacheLocation CacheLocation { get; set; } = ETaggerCommon.CacheLocation.Public;

        public bool NoStore { get; set; } = false;

        public bool NoTransform { get; set; } = false;

        public ETaggerCacheExpirationAttribute()
        {
            _expirationOptions = new Lazy<ExpirationOptions>(() => new ExpirationOptions
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

            if (!context.HttpContext.Items.ContainsKey(ETaggerCommon.ExpirationOptionsName))
            {
                context.HttpContext.Items[ETaggerCommon.ExpirationOptionsName] = _expirationOptions.Value;
            }
        }
    }
}
