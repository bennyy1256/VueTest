using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace VueWebApplication.Caching
{
    public abstract class CacheProvider : ICacheProvider
    {
        protected readonly TimeSpan defaultDuration;

        protected CacheProvider(TimeSpan duration)
        {
            this.defaultDuration = duration;
        }

        #region abstract ICacheService members

        public abstract object Get(string key);

        public abstract T Get<T>(string key);

        public abstract T GetOrSet<T>(string key, T value);

        public abstract IDictionary<string, object> Get(string[] keys);

        public abstract bool Remove(string key);

        #endregion

        #region Implementation of ICacheService

        public abstract bool Exists(string key);

        public bool Save(string key, object value)
        {
            return Save(key, value, GetDefaultPolicy());
        }

        public abstract T Save<T>(string key, T value);

        public bool Save(string key, object value, TimeSpan slidingExpiration)
        {
            return Save(key, value, GetSlidingPolicy(slidingExpiration));
        }

        public bool Save(string key, object value, DateTime absoluteExpiration)
        {
            return Save(key, value, GetAbsolutePolicy(absoluteExpiration));
        }

        public abstract bool Save(string key, object value, CacheItemPolicy policy);

        public object this[string key]
        {
            get { return Get(key); }
            set { Save(key, value, GetDefaultPolicy()); }
        }

        #endregion


        #region Expiration Policy Helpers

        protected CacheItemPolicy GetDefaultPolicy()
        {
            return new CacheItemPolicy() { SlidingExpiration = defaultDuration };
        }

        protected CacheItemPolicy GetAbsolutePolicy(DateTime absoluteExpiration)
        {
            return new CacheItemPolicy { AbsoluteExpiration = absoluteExpiration };
        }

        protected CacheItemPolicy GetSlidingPolicy(TimeSpan slidingExpiration)
        {
            return new CacheItemPolicy { SlidingExpiration = slidingExpiration };
        }
        
        #endregion
    }
}