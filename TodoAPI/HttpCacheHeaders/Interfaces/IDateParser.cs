using HttpCacheHeaders.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HttpCacheHeaders.Interfaces
{
    /// <summary>
    /// Contract for a date parser, used to parse Last-Modified, Expires, If-Modified-Since and If-Unmodified-Since headers.
    /// </summary>
    public interface IDateParser
    {
        Task<string> LastModifiedToString(DateTimeOffset lastModified);

        Task<string> ExpiresToString(DateTimeOffset lastModified);

        Task<DateTimeOffset?> IfModifiedSinceToDateTimeOffset(string ifModifiedSince);

        Task<DateTimeOffset?> IfUnmodifiedSinceToDateTimeOffset(string ifUnmodifiedSince);
    }

    /// <summary>
    /// Contract for an E-Tag Generator, used to generate the unique weak or strong E-Tags for cache items
    /// </summary>
    public interface IETagGenerator
    {
        Task<ETag> GenerateETag(
            StoreKey storeKey,
            string responseBodyContent);
    }

    /// <summary>
    /// Contract for a LastModifiedInjector, which can be used to inject custom last modified dates for resources
    /// of which you know when they were last modified (eg: a DB timestamp, custom logic, ...)
    /// </summary>
    public interface ILastModifiedInjector
    {
        Task<DateTimeOffset> CalculateLastModified(
            ResourceContext context);
    }

    /// <summary>
    /// Contract for finding (a) <see cref="StoreKey" />(s)
    /// </summary>    
    public interface IStoreKeyAccessor
    {
        /// <summary>
        /// Find a  <see cref="StoreKey" /> by part of the key
        /// </summary>
        /// <param name="valueToMatch">The value to match as part of the key</param>
        /// <returns></returns>
        Task<IEnumerable<StoreKey>> FindByKeyPart(string valueToMatch);

        /// <summary>
        /// Find a  <see cref="StoreKey" /> of which the current resource path is part of the key
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<StoreKey>> FindByCurrentResourcePath();
    }

    /// <summary>
    /// Contract for a key generator, used to generate a <see cref="StoreKey" /> 
    /// </summary>
    public interface IStoreKeyGenerator
    {
        /// <summary>
        /// Generate a key for storing a <see cref="ValidatorValue"/> in a <see cref="IValidatorValueStore"/>.
        /// </summary>
        /// <param name="context">The <see cref="StoreKeyContext"/>.</param>         
        /// <returns></returns>
        Task<StoreKey> GenerateStoreKey(
            StoreKeyContext context);
    }

    /// <summary>
    /// Contract for the <see cref="ValidatorValueInvalidator" />
    /// </summary>
    public interface IValidatorValueInvalidator
    {
        /// <summary>
        /// Get the list of <see cref="StoreKey" /> of items marked for invalidation
        /// </summary>
        List<StoreKey> KeysMarkedForInvalidation { get; }

        /// <summary>
        /// Mark an item stored with a <see cref="StoreKey" /> for invalidation
        /// </summary>
        /// <param name="storeKey">The <see cref="StoreKey" /></param>
        /// <returns></returns>
        Task MarkForInvalidation(StoreKey storeKey);

        /// <summary>
        /// Mark a set of items for invalidation by their collection of <see cref="StoreKey" /> 
        /// </summary>
        /// <param name="storeKeys">The collection of <see cref="StoreKey" /></param>
        /// <returns></returns>
        Task MarkForInvalidation(IEnumerable<StoreKey> storeKeys);
    }

    /// <summary>
    /// Contract for a store for validator values.  Each item is stored with a <see cref="StoreKey" /> as key 
    /// and a <see cref="ValidatorValue" /> as value (consisting of an ETag and Last-Modified date).   
    /// </summary>
    public interface IValidatorValueStore
    {
        /// <summary>
        /// Get a value from the store.
        /// </summary>
        /// <param name="key">The <see cref="StoreKey"/> of the value to get.</param>
        /// <returns></returns>
        Task<ValidatorValue> GetAsync(StoreKey key);

        /// <summary>
        /// Set a value in the store.
        /// </summary>
        /// <param name="key">The <see cref="StoreKey"/> of the value to store.</param>
        /// <param name="validatorValue">The <see cref="ValidatorValue"/> to store.</param>
        /// <returns></returns>
        Task SetAsync(StoreKey key, ValidatorValue validatorValue);

        /// <summary>
        /// Remove a value from the store.
        /// </summary>
        /// <param name="key">The <see cref="StoreKey"/> of the value to remove.</param> 
        /// <returns></returns>
        Task<bool> RemoveAsync(StoreKey key);

        /// <summary>
        /// Find one or more keys that contain the inputted valueToMatch 
        /// </summary>
        /// <param name="valueToMatch">The value to match as part of the key</param>
        /// <returns></returns>
        Task<IEnumerable<StoreKey>> FindStoreKeysByKeyPartAsync(string valueToMatch);
    }

}
