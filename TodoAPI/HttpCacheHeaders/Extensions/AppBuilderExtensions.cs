using HttpCacheHeaders.Domain;
using HttpCacheHeaders.Interfaces;
using HttpCacheHeaders.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpCacheHeaders.Extensions
{
    /// <summary>
    /// Extension methods for the HttpCache middleware (on IApplicationBuilder)
    /// </summary>
    public static class AppBuilderExtensions
    {
        /// <summary>
        /// Add HttpCacheHeaders to the request pipeline.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseHttpCacheHeaders(this IApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.UseMiddleware<HttpCacheHeadersMiddleware>();
        }
    }

    public static class ByteExtensions
    {
        // from http://jakzaprogramowac.pl/pytanie/20645,implement-http-cache-etag-in-aspnet-core-web-api
        public static string GenerateMD5Hash(this byte[] data)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var hash = md5.ComputeHash(data);
                var hex = BitConverter.ToString(hash);
                return hex.Replace("-", "");
            }
        }
    }


    internal static class HttpContextExtensions
    {
        internal static readonly string ContextItemsExpirationModelOptions = "HttpCacheHeadersMiddleware-ExpirationModelOptions";
        internal static readonly string ContextItemsValidationModelOptions = "HttpCacheHeadersMiddleware-ValidationModelOptions";

        internal static ExpirationModelOptions ExpirationModelOptionsOrDefault(this HttpContext httpContext, ExpirationModelOptions @default) =>
            httpContext.Items.ContainsKey(ContextItemsExpirationModelOptions)
                ? (ExpirationModelOptions)httpContext.Items[ContextItemsExpirationModelOptions]
                : @default;

        internal static ValidationModelOptions ValidationModelOptionsOrDefault(this HttpContext httpContext, ValidationModelOptions @default) =>
            httpContext.Items.ContainsKey(ContextItemsValidationModelOptions)
                ? (ValidationModelOptions)httpContext.Items[ContextItemsValidationModelOptions]
                : @default;
    }


    /// <summary>
    /// Extension methods for the HttpCache middleware (on IServiceCollection)
    /// </summary>
    public static class ServicesExtensions
    {

        /// <summary>
        /// Add HttpCacheHeaders services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <returns>An <see cref="IServiceCollection" />.</returns>
        public static IServiceCollection AddHttpCacheHeaders(
          this IServiceCollection services)
        {
            AddModularParts(
                services,
                null,
                null,
                null,
                null,
                null);

            return services;
        }

        /// <summary>
        /// Add HttpCacheHeaders services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="dateParserFunc">Func to provide a custom <see cref="IDateParser" /></param>
        /// <param name="validatorValueStoreFunc">Func to provide a custom <see cref="IValidatorValueStore" /></param>
        /// <param name="storeKeyGeneratorFunc">Func to provide a custom <see cref="IStoreKeyGenerator" /></param>
        /// <param name="eTagGeneratorFunc">Func to provide a custom <see cref="IETagGenerator" /></param>
        /// <param name="lastModifiedInjectorFunc">Func to provide a custom <see cref="ILastModifiedInjector" /></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpCacheHeaders(
            this IServiceCollection services,
            Func<IServiceProvider, IDateParser> dateParserFunc = null,
            Func<IServiceProvider, IValidatorValueStore> validatorValueStoreFunc = null,
            Func<IServiceProvider, IStoreKeyGenerator> storeKeyGeneratorFunc = null,
            Func<IServiceProvider, IETagGenerator> eTagGeneratorFunc = null,
            Func<IServiceProvider, ILastModifiedInjector> lastModifiedInjectorFunc = null)
        {
            AddModularParts(
                services,
                dateParserFunc,
                validatorValueStoreFunc,
                storeKeyGeneratorFunc,
                eTagGeneratorFunc,
                lastModifiedInjectorFunc);

            return services;
        }

        /// <summary>
        /// Add HttpCacheHeaders services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="expirationModelOptionsAction">Action to provide custom <see cref="ExpirationModelOptions" /></param>
        /// <param name="dateParserFunc">Func to provide a custom <see cref="IDateParser" /></param>
        /// <param name="validatorValueStoreFunc">Func to provide a custom <see cref="IValidatorValueStore" /></param>
        /// <param name="storeKeyGeneratorFunc">Func to provide a custom <see cref="IStoreKeyGenerator" /></param>
        /// <param name="eTagGeneratorFunc">Func to provide a custom <see cref="IETagGenerator" /></param>
        /// <param name="lastModifiedInjectorFunc">Func to provide a custom <see cref="ILastModifiedInjector" /></param>
        public static IServiceCollection AddHttpCacheHeaders(
            this IServiceCollection services,
            Action<ExpirationModelOptions> expirationModelOptionsAction,
            Func<IServiceProvider, IDateParser> dateParserFunc = null,
            Func<IServiceProvider, IValidatorValueStore> validatorValueStoreFunc = null,
            Func<IServiceProvider, IStoreKeyGenerator> storeKeyGeneratorFunc = null,
            Func<IServiceProvider, IETagGenerator> eTagGeneratorFunc = null,
            Func<IServiceProvider, ILastModifiedInjector> lastModifiedInjectorFunc = null)
        {
            AddConfigureExpirationModelOptions(services, expirationModelOptionsAction);

            AddModularParts(
                services,
                dateParserFunc,
                validatorValueStoreFunc,
                storeKeyGeneratorFunc,
                eTagGeneratorFunc,
                lastModifiedInjectorFunc);

            return services;
        }

        /// <summary>
        /// Add HttpCacheHeaders services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="validationModelOptionsAction">Action to provide custom <see cref="ValidationModelOptions" /></param>
        /// <param name="dateParserFunc">Func to provide a custom <see cref="IDateParser" /></param>
        /// <param name="validatorValueStoreFunc">Func to provide a custom <see cref="IValidatorValueStore" /></param>
        /// <param name="storeKeyGeneratorFunc">Func to provide a custom <see cref="IStoreKeyGenerator" /></param>
        /// <param name="eTagGeneratorFunc">Func to provide a custom <see cref="IETagGenerator" /></param>
        /// <param name="lastModifiedInjectorFunc">Func to provide a custom <see cref="ILastModifiedInjector" /></param>
        public static IServiceCollection AddHttpCacheHeaders(
            this IServiceCollection services,
            Action<ValidationModelOptions> validationModelOptionsAction,
            Func<IServiceProvider, IDateParser> dateParserFunc = null,
            Func<IServiceProvider, IValidatorValueStore> validatorValueStoreFunc = null,
            Func<IServiceProvider, IStoreKeyGenerator> storeKeyGeneratorFunc = null,
            Func<IServiceProvider, IETagGenerator> eTagGeneratorFunc = null,
            Func<IServiceProvider, ILastModifiedInjector> lastModifiedInjectorFunc = null)
        {
            AddConfigureValidationModelOptions(services, validationModelOptionsAction);

            AddModularParts(
                services,
                dateParserFunc,
                validatorValueStoreFunc,
                storeKeyGeneratorFunc,
                eTagGeneratorFunc,
                lastModifiedInjectorFunc);

            return services;
        }

        /// <summary>
        /// Add HttpCacheHeaders services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="expirationModelOptionsAction">Action to provide custom <see cref="ExpirationModelOptions" /></param>
        /// <param name="validationModelOptionsAction">Action to provide custom <see cref="ValidationModelOptions" /></param>
        /// <param name="dateParserFunc">Func to provide a custom <see cref="IDateParser" /></param>
        /// <param name="validatorValueStoreFunc">Func to provide a custom <see cref="IValidatorValueStore" /></param>
        /// <param name="storeKeyGeneratorFunc">Func to provide a custom <see cref="IStoreKeyGenerator" /></param>
        /// <param name="eTagGeneratorFunc">Func to provide a custom <see cref="IETagGenerator" /></param>
        /// <param name="lastModifiedInjectorFunc">Func to provide a custom <see cref="ILastModifiedInjector" /></param>
        public static IServiceCollection AddHttpCacheHeaders(
            this IServiceCollection services,
            Action<ExpirationModelOptions> expirationModelOptionsAction,
            Action<ValidationModelOptions> validationModelOptionsAction,
            Func<IServiceProvider, IDateParser> dateParserFunc = null,
            Func<IServiceProvider, IValidatorValueStore> validatorValueStoreFunc = null,
            Func<IServiceProvider, IStoreKeyGenerator> storeKeyGeneratorFunc = null,
            Func<IServiceProvider, IETagGenerator> eTagGeneratorFunc = null,
            Func<IServiceProvider, ILastModifiedInjector> lastModifiedInjectorFunc = null)
        {
            AddConfigureExpirationModelOptions(services, expirationModelOptionsAction);
            AddConfigureValidationModelOptions(services, validationModelOptionsAction);

            AddModularParts(
                services,
                dateParserFunc,
                validatorValueStoreFunc,
                storeKeyGeneratorFunc,
                eTagGeneratorFunc,
                lastModifiedInjectorFunc);

            return services;
        }

        private static void AddModularParts(
            IServiceCollection services,
            Func<IServiceProvider, IDateParser> dateParserFunc,
            Func<IServiceProvider, IValidatorValueStore> validatorValueStoreFunc,
            Func<IServiceProvider, IStoreKeyGenerator> storeKeyGeneratorFunc,
            Func<IServiceProvider, IETagGenerator> eTagGeneratorFunc,
            Func<IServiceProvider, ILastModifiedInjector> lastModifiedInjectorFunc)
        {
            AddDateParser(services, dateParserFunc);
            AddValidatorValueStore(services, validatorValueStoreFunc);
            AddStoreKeyGenerator(services, storeKeyGeneratorFunc);
            AddETagGenerator(services, eTagGeneratorFunc);
            AddLastModifiedInjector(services, lastModifiedInjectorFunc);

            // register dependencies for required services
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // add required additional services
            services.AddScoped<IValidatorValueInvalidator, ValidatorValueInvalidator>();
            services.AddTransient<IStoreKeyAccessor, StoreKeyAccessor>();
        }

        private static void AddLastModifiedInjector(
            IServiceCollection services,
            Func<IServiceProvider,
                ILastModifiedInjector> lastModifiedInjectorFunc)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (lastModifiedInjectorFunc == null)
            {
                lastModifiedInjectorFunc = _ => new DefaultLastModifiedInjector();
            }

            services.Add(ServiceDescriptor.Singleton(typeof(ILastModifiedInjector), lastModifiedInjectorFunc));
        }

        private static void AddDateParser(
            IServiceCollection services,
            Func<IServiceProvider, IDateParser> dateParserFunc)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (dateParserFunc == null)
            {
                dateParserFunc = _ => new DefaultDateParser();
            }

            services.Add(ServiceDescriptor.Singleton(typeof(IDateParser), dateParserFunc));
        }

        private static void AddValidatorValueStore(
            IServiceCollection services,
            Func<IServiceProvider, IValidatorValueStore> validatorValueStoreFunc)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (validatorValueStoreFunc == null)
            {
                validatorValueStoreFunc = _ => new InMemoryValidatorValueStore();
            }

            services.Add(ServiceDescriptor.Singleton(typeof(IValidatorValueStore), validatorValueStoreFunc));
        }

        private static void AddStoreKeyGenerator(
            IServiceCollection services,
            Func<IServiceProvider, IStoreKeyGenerator> storeKeyGeneratorFunc)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (storeKeyGeneratorFunc == null)
            {
                storeKeyGeneratorFunc = _ => new DefaultStoreKeyGenerator();
            }

            services.Add(ServiceDescriptor.Singleton(typeof(IStoreKeyGenerator), storeKeyGeneratorFunc));
        }

        private static void AddETagGenerator(
            IServiceCollection services,
            Func<IServiceProvider, IETagGenerator> eTagGeneratorFunc)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (eTagGeneratorFunc == null)
            {
                eTagGeneratorFunc = _ => new DefaultStrongETagGenerator();
            }

            services.Add(ServiceDescriptor.Singleton(typeof(IETagGenerator), eTagGeneratorFunc));
        }

        private static void AddConfigureExpirationModelOptions(
            IServiceCollection services,
            Action<ExpirationModelOptions> configureExpirationModelOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureExpirationModelOptions == null)
            {
                throw new ArgumentNullException(nameof(configureExpirationModelOptions));
            }

            services.Configure(configureExpirationModelOptions);
        }

        private static void AddConfigureValidationModelOptions(
            IServiceCollection services,
            Action<ValidationModelOptions> configureValidationModelOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureValidationModelOptions == null)
            {
                throw new ArgumentNullException(nameof(configureValidationModelOptions));
            }

            services.Configure(configureValidationModelOptions);
        }
    }
}
