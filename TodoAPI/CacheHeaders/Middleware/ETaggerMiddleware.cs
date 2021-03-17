using CacheHeaders.Common;
using CacheHeaders.Domain;
using CacheHeaders.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CacheHeaders.Middleware
{
    /// <summary>
    /// Middleware which adds ETag/CacheControl etc. headers
    /// In case of GET/HEAD request with IfNoneMatch header, will serve 304 NotModified (if not modified).
    /// In case of PUT/PATCH request with IfMatch header, will serve 412 Precondition failed (if ETag not match)
    /// </summary>
    public class ETaggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IETagHeadersStore _eTagStore;
        private readonly ValidationOptions _validationOptions;
        private readonly ExpirationOptions _expirationOptions;

        public ETaggerMiddleware(RequestDelegate next, IETagHeadersStore eTagStore,
            IOptions<ExpirationOptions> expirationOptions,
            IOptions<ValidationOptions> validationOptions)
        {
            _next = next;
            _eTagStore = eTagStore ?? throw new ArgumentNullException(nameof(eTagStore));
            _expirationOptions = expirationOptions?.Value ?? throw new ArgumentNullException(nameof(expirationOptions));
            _validationOptions = validationOptions?.Value ?? throw new ArgumentNullException(nameof(validationOptions));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //Options from attribute or global options
            var expirationOptions = context.Items.ContainsKey(ETaggerCommon.ExpirationOptionsName) ?
                context.Items[ETaggerCommon.ExpirationOptionsName] as ExpirationOptions :
                _expirationOptions;
            var validationOptions = context.Items.ContainsKey(ETaggerCommon.ValidationOptionsName) ?
                context.Items[ETaggerCommon.ValidationOptionsName] as ValidationOptions :
                _validationOptions;

            var response = context.Response;
            var originalStream = response.Body;

            //create key from request (headers+path+querystring)
            var reqKey = new Lazy<string>(() =>
            {
                return string.Join("-", context.Request
                        .Headers
                        .Where(h => (validationOptions.Vary).Any(s => s.Equals(h.Key, StringComparison.CurrentCultureIgnoreCase) || s == "*"))
                        .SelectMany(h => h.Value)
                        .ToList()) + //headers
                context.Request.Path.ToString() + //path
                context.Request.QueryString.ToString(); //querystring
            });

            using (var ms = new MemoryStream())
            {
                response.Body = ms;

                //GET or HEAD
                if (IsGetHead(context.Request))
                {
                    //next
                    await _next(context);

                    if (IsEtagSupported(response))
                    {
                        //calc checksum
                        string checksum = CalculateChecksum(ms);

                        //add key to dictionary
                        _eTagStore.Add(reqKey.Value, checksum);

                        response.Headers[HeaderNames.ETag] = checksum;
                        response.Headers[HeaderNames.LastModified] = DateTime.UtcNow.ToString("r", System.Globalization.CultureInfo.InvariantCulture);
                        response.Headers[HeaderNames.Expires] = DateTime.UtcNow.AddSeconds(expirationOptions.MaxAge).ToString("r", System.Globalization.CultureInfo.InvariantCulture);

                        var cacheControlHeaderValue = string.Format(
                            CultureInfo.InvariantCulture,
                            "{0},max-age={1}{2}{3}{4}{5}{6}{7}{8}",
                            expirationOptions.CacheLocation.ToString().ToLowerInvariant(),
                            expirationOptions.MaxAge,
                            expirationOptions.SharedMaxAge == null ? null : ",s-maxage=",
                            expirationOptions.SharedMaxAge,
                            expirationOptions.NoStore ? ",no-store" : null,
                            expirationOptions.NoTransform ? ",no-transform" : null,
                            validationOptions.NoCache ? ",no-cache" : null,
                            validationOptions.MustRevalidate ? ",must-revalidate" : null,
                            validationOptions.ProxyRevalidate ? ",proxy-revalidate" : null);
                        response.Headers[HeaderNames.CacheControl] = cacheControlHeaderValue;

                        if (context.Request.Headers.TryGetValue(HeaderNames.IfNoneMatch, out var etag) &&
                            checksum == etag)
                        {
                            //not modifies
                            response.ContentLength = 0;
                            response.StatusCode = StatusCodes.Status304NotModified;
                            return;
                        }
                    }

                    //copy back original message and leave
                    ms.Position = 0;
                    await ms.CopyToAsync(originalStream);
                    return;
                }

                //PUT PATCH with if-match
                if (IsPutPatch(context.Request) &&
                    (context.Request.Headers.ContainsKey(HeaderNames.IfMatch)))
                {
                    var hashExists = _eTagStore.TryGet(reqKey.Value, out string oldHash);
                    var hashEquals = hashExists ?
                        oldHash.Equals(context.Request.Headers[HeaderNames.IfMatch], StringComparison.CurrentCultureIgnoreCase) :
                        false;

                    if (!hashEquals)
                    {
                        //not ok - old hash is different than required in If-Match or old hash dont exists
                        response.StatusCode = StatusCodes.Status412PreconditionFailed;
                        response.ContentLength = 0;
                        return;
                    }

                    //it is ok, clear hash (item will be updated) and continue
                    _eTagStore.Remove(reqKey.Value);
                }

                //it is ok, or it is another type of request! so proceed to next...
                await _next(context);

                //copy back original message and leave
                ms.Position = 0;
                await ms.CopyToAsync(originalStream);
            }
        }

        /// <summary>
        /// is request get or head
        /// </summary>
        private bool IsGetHead(HttpRequest request)
        {
            return HttpMethods.IsGet(request.Method) ||
                HttpMethods.IsHead(request.Method);
        }

        /// <summary>
        /// is request put or patch
        /// </summary>
        private bool IsPutPatch(HttpRequest request)
        {
            return HttpMethods.IsPut(request.Method) ||
                HttpMethods.IsPatch(request.Method);
        }

        /// <summary>
        /// size limit, status code(success) and ETag exists Condition
        /// </summary>
        private bool IsEtagSupported(HttpResponse response)
        {
            //only for successful messages
            if (response.StatusCode != StatusCodes.Status200OK &&
                response.StatusCode != StatusCodes.Status201Created &&
                response.StatusCode != StatusCodes.Status202Accepted &&
                response.StatusCode != StatusCodes.Status203NonAuthoritative &&
                response.StatusCode != StatusCodes.Status204NoContent &&
                response.StatusCode != StatusCodes.Status205ResetContent &&
                response.StatusCode != StatusCodes.Status206PartialContent &&
                response.StatusCode != StatusCodes.Status207MultiStatus &&
                response.StatusCode != StatusCodes.Status208AlreadyReported &&
                response.StatusCode != StatusCodes.Status226IMUsed)
            {
                return false;
            }

            //length limit
            if (response.Body.Length > 500 * 1024)
                return false;

            //ETag already in there
            if (response.Headers.ContainsKey(HeaderNames.ETag))
                return false;

            return true;
        }

        /// <summary>
        /// md5 checksum
        /// </summary>

        private string CalculateChecksum(MemoryStream ms)
        {
            string checksum = "";

            using (var algo = MD5.Create())
            {
                ms.Position = 0;
                string hash = string.Join("", algo.ComputeHash(ms).Select(b => b.ToString("X2")));
                checksum = $"\"{hash}\"";
            }

            return checksum;
        }
    }
}
