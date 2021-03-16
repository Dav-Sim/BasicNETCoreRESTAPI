using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpCacheHeaders.Domain
{
    public enum CacheLocation
    {
        Public,
        Private
    }

    public class ETag
    {
        public ETagType ETagType { get; }
        public string Value { get; }

        public ETag(ETagType eTagType, string value)
        {
            ETagType = eTagType;
            Value = value;
        }

        public override string ToString()
        {
            switch (ETagType)
            {
                case ETagType.Strong:
                    return $"\"{Value}\"";

                case ETagType.Weak:
                    return $"W\"{Value}\"";

                default:
                    return $"\"{Value}\"";
            }
        }
    }

    public enum ETagType
    {
        Strong,
        Weak
    }

    /// <summary>
    /// Options that have to do with the expiration model, mainly related to Cache-Control & Expires headers on the response.
    /// </summary>
    public class ExpirationModelOptions
    {
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
    }


    /// <summary>
    /// Context containing information on a specific resource
    /// </summary>
    public sealed class ResourceContext
    {
        /// <summary>
        /// The current <see cref="HttpRequest"/>
        /// </summary>
        public HttpRequest HttpRequest { get; }

        /// <summary>
        /// The current <see cref="StoreKey"/> for the resource, if available
        /// </summary>
        public StoreKey StoreKey { get; }

        /// <summary>
        /// The current <see cref="ValidatorValue"/> for the resource, if available
        /// </summary>
        public ValidatorValue ValidatorValue { get; }


        public ResourceContext(
            HttpRequest httpRequest,
            StoreKey storeKey)
        {
            HttpRequest = httpRequest;
            StoreKey = storeKey;
        }

        public ResourceContext(
            HttpRequest httpRequest,
            StoreKey storeKey,
            ValidatorValue validatorValue) : this(httpRequest, storeKey)
        {
            ValidatorValue = validatorValue;
        }
    }

    public class StoreKey : Dictionary<string, string>
    {
        public override string ToString() => string.Join("-", Values);
    }

    /// <summary>
    /// Context containing information that might be useful when generating a custom store key.
    /// </summary>
    public class StoreKeyContext
    {
        /// <summary>
        /// The current <see cref="HttpRequest"/>
        /// </summary>
        public HttpRequest HttpRequest { get; }

        /// <summary>
        /// The Vary header keys as set on <see cref="ValidationModelOptions"/> or through <see cref="HttpCacheValidationAttribute"/>
        /// </summary>
        public IEnumerable<string> Vary { get; }

        /// <summary>
        /// The VaryByAll option as set on <see cref="ValidationModelOptions"/> or through <see cref="HttpCacheValidationAttribute"/>
        /// </summary>
        public bool VaryByAll { get; }

        public StoreKeyContext(
            HttpRequest httpRequest,
            IEnumerable<string> vary,
            bool varyByAll)
        {
            HttpRequest = httpRequest;
            Vary = vary;
            VaryByAll = varyByAll;
        }
    }


    /// <summary>
    /// Options that have to do with the validation model, mainly related to ETag generation, Last-Modified on the response,
    /// but also to the Cache-Control header (as that is used for both expiration & validation requirements)
    /// </summary>
    public class ValidationModelOptions
    {
        /// <summary>
        /// A case-insensitive list of headers from the request to take into account as differentiator
        /// between requests (eg: for generating ETags)
        ///
        /// Defaults to Accept, Accept-Language, Accept-Encoding.  * indicates all request headers can be taken into account.
        /// </summary>
        public IEnumerable<string> Vary { get; set; }
            = new List<string> { "Accept", "Accept-Language", "Accept-Encoding" };

        /// <summary>
        /// Indicates that all request headers are taken into account as differentiator.
        /// When set to true, this is the same as Vary *.  The Vary list will thus be ignored.
        ///
        /// Note that a Vary field value of "*" implies that a cache cannot determine
        /// from the request headers of a subsequent request whether this response is
        /// the appropriate representation.  This should thus only be set to true for
        /// exceptional cases.
        ///
        /// Defaults to false.
        /// </summary>
        public bool VaryByAll { get; set; } = false;

        /// <summary>
        /// When true, the no-cache directive is added to the Cache-Control header.
        /// This indicates to a cache that the response should not be used for subsequent requests
        /// without successful revalidation with the origin server.
        ///
        /// Defaults to false.
        /// </summary>
        public bool NoCache { get; set; } = false;

        /// <summary>
        /// When true, the must-revalidate directive is added to the Cache-Control header.
        /// This tells a cache that if a response becomes stale, ie: it's expired, revalidation has to happen.
        /// By adding this directive, we can force revalidation by the cache even if the client
        /// has decided that stale responses are for a specified amount of time (which a client can
        /// do by setting the max-stale directive).
        ///
        /// Defaults to false.
        /// </summary>
        public bool MustRevalidate { get; set; } = false;

        /// <summary>
        /// When true, the proxy-revalidate directive is added to the Cache-Control header.
        /// Exactly the same as must-revalidate, but only for shared caches.
        /// So: this tells a shared cache that if a response becomes stale, ie: it's expired, revalidation has to happen.
        /// By adding this directive, we can force revalidation by the cache even if the client
        /// has decided that stale responses are for a specified amount of time (which a client can
        /// do by setting the max-stale directive).
        ///
        /// Defaults to false.
        /// </summary>
        public bool ProxyRevalidate { get; set; } = false;

    }

    public class ValidatorValue
    {
        public ETag ETag { get; }
        public DateTimeOffset LastModified { get; }

        public ValidatorValue(ETag eTag, DateTimeOffset lastModified)
        {
            ETag = eTag;
            LastModified = lastModified;
        }
    }

}
