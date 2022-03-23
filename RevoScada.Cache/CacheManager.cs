using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace RevoScada.Cache
{
    /// <summary>
    /// Manages redis cache stackexchange.redis library for scada.
    /// Mainly sets and gets string(or json serialized object) or byte array 
    /// </summary>
    public class CacheManager
    {
        private readonly IDatabase _database;
        private readonly int _databaseIndex;
        private readonly string _server;

        /// <summary>
        /// Initialization of cachemanager.
        /// </summary>
        /// <param name="scadaDatabaseType">Write or Read cache db type</param>
        /// <param name="server">Server address for communicating with redis</param>
        public CacheManager(CacheDBType scadaDatabaseType, string server)
        {
            _server = server;
            RedisConnectorHelper.Instance.SetServer(server);
            _databaseIndex = (int)scadaDatabaseType;
            _database = RedisConnectorHelper.Instance.Connection(server).GetDatabase(_databaseIndex);
        }

        /// <summary>
        /// Pushes item to left of a list defined.
        /// </summary>
        /// <param name="listName">Redis list name</param>
        /// <param name="value">string list item</param>
        /// <returns>CacheResponse type</returns>
        public CacheResponse ListLeftPushString(string listName, string value, byte tryCount)
        {

            CacheResponse cacheResponse = new CacheResponse();
            RedisValue redisValue = value;

            do
            {

                if (tryCount == 0)
                {
                    cacheResponse.ResultValue = null;
                    cacheResponse.Message = "Redis service may be down or cannot reach redis server!";
                    cacheResponse.CacheResponseState = CacheResponseStates.EmergencyError;

                    break;
                }

                tryCount--;

                try
                {
                    var checkListCount = _database.ListLength(listName);
                    long pushResult = _database.ListLeftPush(listName, redisValue, When.Always, CommandFlags.None);
                    cacheResponse.CacheResponseState = (pushResult > 0) ? CacheResponseStates.Success : CacheResponseStates.Fail;
                }
                catch (Exception)
                {
                    cacheResponse.ResultValue = null;
                    cacheResponse.Message = "Redis service may be down or cannot reach redis server!";
                    cacheResponse.CacheResponseState = CacheResponseStates.EmergencyError;
                }

                Thread.Sleep(20);

            } while (cacheResponse.CacheResponseState == CacheResponseStates.Fail || cacheResponse.CacheResponseState == CacheResponseStates.EmergencyError);

            return cacheResponse;
        }

        /// <summary>
        /// Pushes items to left of a list defined.
        /// </summary>
        /// <param name="listName">Redis list name</param>
        /// <param name="value">string list item</param>
        /// <returns>CacheResponse type</returns>
        public CacheResponse ListLeftPushString(string listName, List<string> values, byte tryCount)
        {

            CacheResponse cacheResponse = new CacheResponse();
            RedisValue[] redisValues = new RedisValue[values.Count];


            for (int i = 0; i < values.Count; i++)
            {
                RedisValue redisValue = values[i];
                redisValues[i] = redisValue;
            }

            do
            {

                if (tryCount == 0)
                {
                    cacheResponse.ResultValue = null;
                    cacheResponse.Message = "Redis service may be down or cannot reach redis server!";
                    cacheResponse.CacheResponseState = CacheResponseStates.EmergencyError;

                    break;
                }

                tryCount--;

                try
                {
                    var checkListCount = _database.ListLength(listName);
                    long pushResult = _database.ListLeftPush(listName, redisValues, CommandFlags.None);
                    cacheResponse.CacheResponseState = (pushResult > 0) ? CacheResponseStates.Success : CacheResponseStates.Fail;
                }
                catch (Exception)
                {
                    cacheResponse.ResultValue = null;
                    cacheResponse.Message = "Redis service may be down or cannot reach redis server!";
                    cacheResponse.CacheResponseState = CacheResponseStates.EmergencyError;
                }

                Thread.Sleep(20);

            } while (cacheResponse.CacheResponseState == CacheResponseStates.Fail || cacheResponse.CacheResponseState == CacheResponseStates.EmergencyError);

            return cacheResponse;
        }

        /// <summary>
        /// /// Pushes item to right of a list defined.
        /// </summary>
        /// <param name="listName">Redis list name</param>
        /// <param name="value">string list item</param>
        /// <returns>CacheResponse type</returns>
        public CacheResponse ListRightPushString(string listName, string value, byte tryCount)
        {
            CacheResponse cacheResponse = new CacheResponse();
            RedisValue redisValue = value;

            do
            {

                if (tryCount == 0)
                {
                    cacheResponse.ResultValue = null;
                    cacheResponse.Message = "Redis service may be down or cannot reach redis server!";
                    cacheResponse.CacheResponseState = CacheResponseStates.EmergencyError;
                    break;
                }

                tryCount--;

                try
                {
                    var checkListCount = _database.ListLength(listName);
                    long pushResult = _database.ListRightPush(listName, redisValue, When.Always, CommandFlags.None);
                    cacheResponse.CacheResponseState = (pushResult > 0) ? CacheResponseStates.Success : CacheResponseStates.Fail;
                }
                catch (Exception)
                {
                    cacheResponse.ResultValue = null;
                    cacheResponse.Message = "Redis service may be down or cannot reach redis server!";
                    cacheResponse.CacheResponseState = CacheResponseStates.EmergencyError;
                }

                Thread.Sleep(20);

            } while (cacheResponse.CacheResponseState == CacheResponseStates.Fail || cacheResponse.CacheResponseState == CacheResponseStates.EmergencyError);

            return cacheResponse;
        }

        /// <summary>
        /// Pops and removes from left of list given
        /// </summary>
        /// <param name="listName">Redis list name</param>
        /// <returns>CacheResponse type</returns>
        public CacheResponse ListLeftPop(string listName, byte tryCount)
        {
            CacheResponse cacheResponse = new CacheResponse();

            do
            {

                if (tryCount == 0)
                {
                    cacheResponse.ResultValue = null;
                    cacheResponse.Message = "Redis service may be down or cannot reach redis server!";
                    cacheResponse.CacheResponseState = CacheResponseStates.EmergencyError;
                    break;
                }

                tryCount--;

                try
                {
                    long listLength = _database.ListLength(listName);


                    if (listLength == 0)
                    {
                        cacheResponse.CacheResponseState = CacheResponseStates.EmptyQueue;
                        break;
                    }



                    RedisValue redisValue = _database.ListLeftPop(listName);

                    if (redisValue.HasValue)
                    {
                        cacheResponse.ResultValue = Convert.ToString(redisValue);
                        cacheResponse.CacheResponseState = CacheResponseStates.Success;
                    }
                    else
                    {
                        cacheResponse.ResultValue = null;
                        cacheResponse.Message = "Redis has not that value";
                        cacheResponse.CacheResponseState = CacheResponseStates.Fail;
                    }

                }
                catch (Exception)
                {
                    cacheResponse.ResultValue = null;
                    cacheResponse.Message = "Redis service may be down or cannot reach redis server!";
                    cacheResponse.CacheResponseState = CacheResponseStates.EmergencyError;
                }

                Thread.Sleep(20);

            } while (cacheResponse.CacheResponseState == CacheResponseStates.Fail || cacheResponse.CacheResponseState == CacheResponseStates.EmergencyError);


            return cacheResponse;


        }

        /// <summary>
        /// Pops and removes from right of list given
        /// </summary>
        /// <param name="listName">Redis list name</param>
        /// <returns>CacheResponse type</returns>
        public CacheResponse ListRightPop(string listName, byte tryCount)
        {

            CacheResponse cacheResponse = new CacheResponse();

            do
            {

                if (tryCount == 0)
                {
                    cacheResponse.ResultValue = null;
                    cacheResponse.Message = "Redis service may be down or cannot reach redis server!";
                    cacheResponse.CacheResponseState = CacheResponseStates.EmergencyError;
                    break;
                }

                tryCount--;

                try
                {
                    long listLength = _database.ListLength(listName);


                    if (listLength == 0)
                    {
                        cacheResponse.CacheResponseState = CacheResponseStates.EmptyQueue;
                        break;
                    }

                    RedisValue redisValue = _database.ListRightPop(listName);

                    if (redisValue.HasValue)
                    {
                        cacheResponse.ResultValue = Convert.ToString(redisValue);
                        cacheResponse.CacheResponseState = CacheResponseStates.Success;
                    }
                    else
                    {
                        cacheResponse.ResultValue = null;
                        cacheResponse.Message = "Redis has not that value";
                        cacheResponse.CacheResponseState = CacheResponseStates.Fail;
                    }

                }
                catch (Exception)
                {
                    cacheResponse.ResultValue = null;
                    cacheResponse.Message = "Redis service may be down or cannot reach redis server!";
                    cacheResponse.CacheResponseState = CacheResponseStates.EmergencyError;
                }

                Thread.Sleep(20);

            } while (cacheResponse.CacheResponseState == CacheResponseStates.Fail || cacheResponse.CacheResponseState == CacheResponseStates.EmergencyError);

            return cacheResponse;
        }

        /// <summary>
        /// Pops and removes from right of list given
        /// </summary>
        /// <param name="listName">Redis list name</param>
        /// <returns>CacheResponse type</returns>
        public CacheResponse GetAll(string listName, byte tryCount)
        {

            CacheResponse cacheResponse = new CacheResponse();

            do
            {

                if (tryCount == 0)
                {
                    cacheResponse.ResultValue = null;
                    cacheResponse.Message = "Redis service may be down or cannot reach redis server!";
                    cacheResponse.CacheResponseState = CacheResponseStates.EmergencyError;
                    break;
                }

                tryCount--;

                try
                {
                    long listLength = _database.ListLength(listName);


                    if (listLength == 0)
                    {
                        cacheResponse.CacheResponseState = CacheResponseStates.EmptyQueue;
                        break;
                    }

                    RedisValue[] redisValue = _database.ListRange(listName);

                    if (redisValue.Count()>0)
                    {
                        List<string> resultList = new List<string>();
                        foreach (var item in redisValue)
                        {
                            resultList.Add(item);
                        }
                        cacheResponse.ResultValue = resultList;
                        cacheResponse.CacheResponseState = CacheResponseStates.Success;
                    }
                    else
                    {
                        cacheResponse.ResultValue = null;
                        cacheResponse.Message = "Redis has not that value";
                        cacheResponse.CacheResponseState = CacheResponseStates.Fail;
                    }



                }
                catch (Exception)
                {
                    cacheResponse.ResultValue = null;
                    cacheResponse.Message = "Redis service may be down or cannot reach redis server!";
                    cacheResponse.CacheResponseState = CacheResponseStates.EmergencyError;
                }

                Thread.Sleep(20);

            } while (cacheResponse.CacheResponseState == CacheResponseStates.Fail || cacheResponse.CacheResponseState == CacheResponseStates.EmergencyError);

            return cacheResponse;
        }

        /// <summary>
        /// Retrieves string value by key 
        /// </summary>
        /// <param name="key">key as string</param>
        /// <returns>string</returns>
        public string GetString(string key)
        {
            var redisValue = _database.StringGet(key);

            return (redisValue);
        }

        public bool GetBool(string key)
        {
            var redisValue = _database.StringGet(key);
            bool converted = int.TryParse(redisValue, out int boolValueAsInt);
            bool result = Convert.ToBoolean(boolValueAsInt);
            return converted ? result : converted;
        }

        /// <summary>
        /// Retrieves byte array value by key 
        /// </summary>
        /// <param name="key">key as string</param>
        /// <returns>Byte array</returns>
        public byte[] GetByte(string key)
        {
            var redisValue = _database.StringGet(key);

            return (byte[])(redisValue);
        }

        /// <summary>
        /// Generic method for setting value for given key.
        /// </summary>
        /// <param name="key">Key as string</param>
        /// <param name="value">Generic type</param>
        /// <param name="expiry">Define lifespan for value</param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, TimeSpan? expiry = null)
        {
            RedisValue redisValue = RedisValue.Unbox(value);
            bool setResult = _database.StringSet(key, redisValue, expiry);
            return setResult;
        }
        public bool DeleteKey(string key)
        {
            
            bool keyDeleteResult = _database.KeyDelete(key);
            return keyDeleteResult;
        }

        public int DeleteManyKeys(string keyPattern)
        {
            int deleteCount = 0;
            var keys = GetKeyNames(keyPattern);
            foreach (var key in keys)
            {
                bool result= _database.KeyDelete(key);
                deleteCount = result ? deleteCount + 1 : deleteCount;
            }
            return deleteCount;
        }

        public Task<bool> SetAsync(string key, string value)
        {
            return _database.StringSetAsync(key, value, null, When.Always);
        }
        public bool SetStringWithTransaction(string key, string value)
        {

           
            var transaction = _database.CreateTransaction();
         


            transaction.StringSetAsync(key, value);

            Thread.Sleep(20);
            bool committed = transaction.Execute();


            return committed;
        }
        public bool SetBeginLockInfo(string lockedKey, byte tryAmount,int tryCycleIntervalInMiliseconds)
        {
            bool beginResult = false;

            string value = GetString(lockedKey);

            if (value == null)
            {
                TimeSpan span = TimeSpan.FromSeconds(10);
                bool setCacheResult = Set(lockedKey, "true", span);
                beginResult = true;
            }
            else
            {
                do
                {

                    if (tryAmount == 0)
                    {
                        break;
                    }

                    tryAmount--;
                    value= GetString(lockedKey);
                    if (value == null)
                    {
                        TimeSpan span = TimeSpan.FromSeconds(10);
                        bool setCacheResult = Set(lockedKey, "true", span);
                    }

                    beginResult = !Convert.ToBoolean(value);
                    Thread.Sleep(tryCycleIntervalInMiliseconds);

                } while (Convert.ToBoolean(value) == true);
            }


            return beginResult;
        }
        public bool SetEndLockInfo(string lockedKey )
        {
            bool endResult = false;

            string alarmSetLocked = GetString(lockedKey);

            if (alarmSetLocked == null)
            {
                endResult = true;
            }
            else if (Convert.ToBoolean(alarmSetLocked) == true)
            {
                TimeSpan span = TimeSpan.FromSeconds(10);
                bool setCacheResult =Set(lockedKey, "false", span);
                endResult = setCacheResult;

            }

            return endResult;
        }
        private T GetItem<T>(RedisValue redisResult)
        {
            T objResult = FromByteArray<T>(redisResult);
            return objResult;
        }
        public byte[] ToByteArray<T>(T obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        public T FromByteArray<T>(byte[] data)
        {
            if (data == null)
                return default(T);
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }
        
        /// <summary>
        /// h?llo matches hello, hallo and hxllo
        /// h* llo matches hllo and heeeello
        /// h[ae] llo matches hello and hallo, but not hillo
        /// h[^e] llo matches hallo, hbllo, ... but not hello
        /// h[a - b]llo matches hallo and hbllo 
        /// </summary>
        /// <param name="pattern">search pattern as string</param>
        /// <returns>List of keys filtered by pattern</returns>
        public List<string> GetKeyNames(string pattern)
        {
            var server = RedisConnectorHelper.Instance.Connection(_server).GetServer(_server,6379);
            
            List<string> keyList = new List<string>();
            
            foreach (var key in server.Keys(_databaseIndex,pattern: pattern))
            {
                keyList.Add(key);
            }

            return keyList;
        }

        public bool Publish(string channelName, string message)
        {
            var subscriber = RedisConnectorHelper.Instance.Connection(_server).GetSubscriber();
            long receivedSubscriber=subscriber.Publish(channelName, message);
            return receivedSubscriber>0;
        }


        public void Subscribe(string channelName,Action subscribeAction)
        {
            var subscriber = RedisConnectorHelper.Instance.Connection(_server).GetSubscriber();
            subscriber.Subscribe(channelName, (channel, message) => {
                subscribeAction.Invoke();
            });
        }

    }
}



/////////**/
/////////*
////////   async methods cause problems
////////     public async Task<bool> ListLeftPushString(string listName, string value)
////////        {
////////            RedisValue redisValue = value;

////////            try
////////            {
////////                var checkListCount = await _database.ListLengthAsync(listName);
////////                // It is used for init first time usage so ListRightPushAsync doesnt work in .net but works on redis-cli
////////            }
////////            catch (Exception ex)
////////            {
////////                //[high] log
////////            }

////////            var transaction = _database.CreateTransaction();

////////            long pushResult = await transaction.ListLeftPushAsync(listName, redisValue, When.Always, CommandFlags.FireAndForget);

////////            bool transactionResult = transaction.Execute();

////////            return transactionResult;
////////        }

////////        public async Task<bool> ListRightPushString(string listName, string value)
////////        {

////////            RedisValue redisValue = value;

////////            try
////////            {
////////                var checkListCount = await _database.ListLengthAsync(listName);
////////                // It is used for init first time usage so ListRightPushAsync doesnt work in .net but works on redis-cli
////////            }
////////            catch (Exception ex)
////////            {
////////                //[high] log
////////            }


////////            var transaction = _database.CreateTransaction();

////////            long pushResult =  await  transaction.ListRightPushAsync(listName, redisValue,When.Always,CommandFlags.FireAndForget);

////////            bool transactionResult = transaction.Execute();


////////            return transactionResult;
////////        }

////////        public async Task<T> ListLeftPop<T>(string listName)
////////        {

////////            _database.ListLength(listName);

////////            var transaction = _database.CreateTransaction();

////////            RedisValue redisValue= await transaction.ListLeftPopAsync(listName);

////////            bool transactionResult = transaction.Execute();

////////            if (!transactionResult)
////////            {

////////            }

////////            return GetItem<T>(redisValue);
////////        }
////////        public async Task<T> ListRightPop<T>(string listName)
////////        {
////////            var transaction = _database.CreateTransaction();

////////            RedisValue redisValue = await transaction.ListRightPopAsync(listName);

////////            bool transactionResult = transaction.Execute();

////////            if (!transactionResult)
////////            {

////////            }

////////            return GetItem<T>(redisValue);
////////        }
///     /*
////////public bool SetStringWithTransaction(string key, string value)
////////{
////////    var fooTask = tran.SomeCommandAsync(...);
////////    if (await tran.ExecuteAsync())
////////    {
////////        var foo = await fooTask;
////////    }



////////    var transaction = _database.CreateTransaction();
////////    var t = transaction.StringSetAsync(key, value);

////////    if (await t.ExecuteAsync())
////////    {

////////    }
////////    bool transactionSetResult =.Result;
////////    transaction.Execute();
////////    return transactionSetResult;
////////}
////////*////



