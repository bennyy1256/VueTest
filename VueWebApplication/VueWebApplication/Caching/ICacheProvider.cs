using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace VueWebApplication.Caching
{
    public interface ICacheProvider
    {
        /// <summary>
        /// Validates if an object with this key exists in the cache
        /// </summary>
        /// <param name="key">The key of the object to check</param>
        /// <returns>True if it exists, false if it doesn't</returns>
        bool Exists(string key);

        /// <summary>
        /// Insert an item into the cache with no specifics as to how it will be used
        /// </summary>
        /// <param name="key">The key used to map this object</param>
        /// <param name="value">The value to be saved to the cache</param>
        bool Save(string key, object value);

        /// <summary>
        /// Insert an item in the cache with the expiration that it will expire if not used past its window
        /// </summary>
        /// <param name="key">The key used to map this object</param>
        /// <param name="value">The object to insert</param>
        /// <param name="slidingExpiration">The expiration window</param>
        bool Save(string key, object value, TimeSpan slidingExpiration);

        /// <summary>
        /// Insert an item in the cache with the expiration that will expire at a specific point in time
        /// </summary>
        /// <param name="key">The key used to map this object</param>
        /// <param name="value">The object to insert</param>
        /// <param name="absoluteExpiration">The DateTime in which this object will expire</param>
        bool Save(string key, object value, DateTime absoluteExpiration);

        /// <summary>
        /// Saves the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="policy">The policy.</param>
        /// <returns></returns>
        bool Save(string key, object value, CacheItemPolicy policy);

        /// <summary>
        /// Insert an item into the cache with specifics as to how it will be used
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        T Save<T>(string key, T value);

        /// <summary>
        /// Get cached data, if data not exist, insert into cache. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        T GetOrSet<T>(string key, T value);

        /// <summary>
        /// Retrieves a cached object from the cache
        /// </summary>
        /// <param name="key">The key used to identify this object</param>
        /// <returns>The object from the database, or an exception if the object doesn't exist</returns>
        object Get(string key);

        /// <summary>
        /// Retrieves a cached object from the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key used to identify this object.</param>
        /// <returns>T.</returns>
        T Get<T>(string key);

        /// <summary>
        /// Gets a set of cached objects
        /// </summary>
        /// <param name="keys">All of the keys of the objects we wish to retrieve</param>
        /// <returns>A key / value dictionary containing all of the keys and objects we wanted to retrieve</returns>
        IDictionary<string, object> Get(string[] keys);

        /// <summary>
        /// Retrieves a cached object using an indexers
        /// </summary>
        /// <param name="key">The key used to identify this object</param>
        /// <returns>The cached object</returns>
        object this[string key] { get; set; }

        /// <summary>
        /// Removes an object from the cache with the specified key
        /// </summary>
        /// <param name="key">The key used to identify this object</param>
        /// <returns>True if the object was removed, false if it didn't exist or was unable to be removed</returns>
        bool Remove(string key);
    }
}