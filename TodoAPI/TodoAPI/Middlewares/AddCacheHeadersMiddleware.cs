using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.Middlewares
{
    public class AddCacheHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string environmentName;

        public AddCacheHeadersMiddleware(RequestDelegate next, string environmentName)
        {
            _next = next;
            this.environmentName = environmentName;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            await _next(httpContext);

            //zjistit metodu GET nebo PUT/PATCH
            //pokud je GET, tak se jen přidají informace
            //pokud je PUT tak přečíst z hlavičky HeaderNames.IfMatch a HeaderNames.If.... pak porovnat a eventuálně vrátit chybu

            //vytvořit klíč z url

            //přečíst body do byte[]

            //vypočítat hash

            //zapsat hash do headers, a zapsat i ostatní věci

        }
    }
}
