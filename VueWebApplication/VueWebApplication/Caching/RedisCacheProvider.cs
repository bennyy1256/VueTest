using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using VueWebApplication.Factories;
using VueWebApplication.Interface;

namespace VueWebApplication.Caching
{
    public class RedisCacheProvider : CacheProvider
    {
        private static ConnectionMultiplexer _redisConn;

        private static IDatabase _dataBase;

        private const string _lockKey = "_lockString_";

        public RedisCacheProvider() 
            : base(new TimeSpan (0, 20, 0))
        {
            _redisConn = RedisConnectionFactory.GetConnection();
            _dataBase = _redisConn.GetDatabase();
        }

        //----------------------------------------------------------------------------------------------------

        #region Implementation of ICacheService

        public override bool Exists(string key)
        {
            return _dataBase.KeyExists(key);
        }

        public override IDictionary<string, object> Get(string[] keys)
        {
            var tasks = new Dictionary<string,Task<RedisValue>>();

            var batch = _dataBase.CreateBatch();

            var dic = new Dictionary<string, string>();
            foreach (var key in keys)
            {
                var getAsync = batch.StringGetAsync(key);
                tasks.Add(key, getAsync);
            }

            batch.Execute();

            Task.WhenAll(tasks.Values.ToArray());
            var result = new Dictionary<string, object>();
            foreach(var key in keys)
            {
                foreach(var item in tasks)
                {
                    if(key == item.Key)
                    {
                        result.Add(key, item.Value.Result);
                    }
                }
            }
            
            return result;
        }

        public override object Get(string key)
        {
            return _dataBase.StringGet(key);
        }

        public override T Get<T>(string key)
        {
            var cacheData = this.Exists(key) ? JsonConvert.DeserializeObject<T>(_dataBase.StringGet(key)) : default(T);
            
            return cacheData;
        }

        public override T GetOrSet<T>(string key, T value)
        {
            T cacheData = this.Get<T>(key);

            if (cacheData == null)
            {
                cacheData = Save<T>(key, value);
            }

            return cacheData;
        }
        
        public override bool Save(string key, object value, CacheItemPolicy policy)
        {
            string lockKey = $"{_lockKey}{key}";

            bool result = false;

            if (_dataBase.StringGet(lockKey).IsNull)
            {
                // Lock 標示為寫入中
                _dataBase.StringSet(lockKey, true.ToString());

                result = _dataBase.StringSet(key, JsonConvert.SerializeObject(value), base.defaultDuration);

                // Unlock 移除寫入中標示
                _dataBase.KeyDelete(lockKey);
            }
            else
            {
                // 遞回
                result = Save(key, value, policy);
            }

            return result;
        }

        public override T Save<T>(string key, T value)
        {
            string lockKey = $"{_lockKey}{key}";

            T cacheData = value;

            if (_dataBase.StringGet(lockKey).IsNull)
            {
                // Lock 標示為寫入中
                _dataBase.StringSet(lockKey, true.ToString());

                _dataBase.StringSet(key, JsonConvert.SerializeObject(cacheData), base.defaultDuration);

                // Unlock 移除寫入中標示
                _dataBase.KeyDelete(lockKey);
            }
            else
            {
                // 遞回
                cacheData = Save<T>(key, value);
            }

            return cacheData;
        }

        public override bool Remove(string key)
        {
            return _dataBase.KeyDelete(key);
        }

        #endregion

        public void Append(string key, object value)
        {
            _dataBase.StringAppend(key, JsonConvert.SerializeObject(value));
        }

        // ---------------------------------------------------------------------------------------------------

        // List

        public List<T> ListGetOrSave<T>(string key, List<T> value) where T : IRedisList
        {
            List<T> result = new List<T>();

            if (this.Exists(key))
            {
                var length = _dataBase.ListLength(key);

                var list = _dataBase.ListRange(key, 0, length);

                long index = 0;

                foreach(var item in list)
                {
                    
                    T thisItem = JsonConvert.DeserializeObject<T>(item);
                    thisItem.RedisIndex = index++;
                    result.Add(thisItem);
                }
            }
            
            if(result.Count == 0)
            {
                result = ListSave<T>(key, value);
            }

            return result;
        }

        public List<T> ListSave<T>(string key, List<T> value) where T : IRedisList
        {
            string lockKey = $"{_lockKey}{key}";

            List<T> cacheData = value;

            if (_dataBase.StringGet(lockKey).IsNull)
            {
                // Lock 標示為寫入中
                _dataBase.StringSet(lockKey, true.ToString());

                var batch = _dataBase.CreateBatch();

                long index = 0;

                foreach (var item in value)
                {
                    item.RedisIndex = index++;
                    _dataBase.ListRightPushAsync(key, JsonConvert.SerializeObject(item));
                }

                batch.Execute();

                // Unlock 移除寫入中標示
                _dataBase.KeyDelete(lockKey);
            }
            else
            {
                // 遞回
                cacheData = ListSave<T>(key, value);
            }
            //--
           
            return cacheData;
        }

        public void ListInsert(string key, object value)
        {
            if (this.Exists(key))
            {
                _dataBase.ListRightPush(key, JsonConvert.SerializeObject(value));
            }
        }

        public void ListUpdate<T>(long index, string key, T value) where T : IRedisList
        {
            if (this.Exists(key))
            {
                value.RedisIndex = index;
                _dataBase.ListSetByIndex(key, index, JsonConvert.SerializeObject(value));
            }
        }

    }
}