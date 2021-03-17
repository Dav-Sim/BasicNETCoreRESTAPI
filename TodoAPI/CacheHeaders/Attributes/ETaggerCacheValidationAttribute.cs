using CacheHeaders.Common;
using CacheHeaders.Domain;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace CacheHeaders.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ETaggerCacheValidationAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly Lazy<ValidationOptions> _validationOptions;

        public string[] Vary { get; set; } = new string[] { "Accept", "Accept-Language", "Accept-Encoding" };

        public bool NoCache { get; set; } = false;

        public bool MustRevalidate { get; set; } = false;

        public bool ProxyRevalidate { get; set; } = false;

        public ETaggerCacheValidationAttribute()
        {
            _validationOptions = new Lazy<ValidationOptions>(() => new ValidationOptions
            {
                Vary = Vary,
                NoCache = NoCache,
                MustRevalidate = MustRevalidate,
                ProxyRevalidate = ProxyRevalidate
            });
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            await next();

            if (!context.HttpContext.Items.ContainsKey(ETaggerCommon.ValidationOptionsName))
            {
                context.HttpContext.Items[ETaggerCommon.ValidationOptionsName] = _validationOptions.Value;
            }
        }
    }
}
