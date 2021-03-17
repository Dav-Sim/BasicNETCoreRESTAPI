using CacheHeaders.Middleware;
using Microsoft.AspNetCore.Builder;

namespace CacheHeaders.Extensions
{
    /// <summary>
    /// Extensions for IApplicationBuilder
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Use ETagger middleware (Put this statement before UseRouting)
        /// See <seealso cref="ETaggerMiddleware"/>
        /// </summary>
        /// <param name="app"></param>
        public static void UseETagger(this IApplicationBuilder app)
        {
            app.UseMiddleware<ETaggerMiddleware>();
        }
    }
}
