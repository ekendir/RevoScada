using Newtonsoft.Json;
using Revo.Core;
using RevoScada.Cache;
using RevoScada.Synchronization.Enums;
using RevoScada.Synchronization.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RevoScada.Synchronization
{
    public class SyncIssueManager
    {
        private readonly CacheManager _mainCacheManager;
        private readonly CacheManager _remoteMainCacheManager;
        
        
        /// <summary>
        /// After batch and finish batch sync issues occurs.
        /// </summary>
        /// <param name="localCacheAddress"></param>
        public SyncIssueManager(string localCacheAddress) {
            _mainCacheManager = new CacheManager( CacheDBType.Main, localCacheAddress);
        }

        public SyncIssueManager(string localCacheAddress,string remoteCacheAddress)
        {
            _mainCacheManager = new CacheManager(CacheDBType.Main, localCacheAddress);
            if (!string.IsNullOrEmpty( remoteCacheAddress))
            {
                _remoteMainCacheManager = new CacheManager(CacheDBType.Main, remoteCacheAddress);
            }
        }

        /// <summary>
        /// Creates Issue for running environment.
        /// </summary>
        /// <returns></returns>
        public bool CreateNewSyncIssue(FromToDirection direction, TransferType transferType,SyncStatus syncStatus, int plcDeviceId, int batchId)
        {
            string keySuffix = Guid.NewGuid().ToString("N");

            SyncIssue syncIssue = new SyncIssue
            {
                CachedKey = $"syncIssue{direction}{transferType}PLC{plcDeviceId}Batch{batchId}KeySuffix{keySuffix}",
                BatchId = batchId,
                PlcDeviceId = plcDeviceId,
                CreateDate = DateTime.Now,
                FromToDirection = direction,
                SyncStatus = syncStatus,
                ModifiedDate = DateTime.Now,
                TransferType= transferType,
            };

            var serialized = JsonConvert.SerializeObject(syncIssue);
            bool setResult = _mainCacheManager.Set(syncIssue.CachedKey, serialized, TimeSpan.FromDays(60));
            LogManager.Instance.Log($"syncIssue Created! Detail: \n {JsonConvert.SerializeObject(syncIssue, new JsonSerializerSettings { Formatting = Formatting.Indented })}", LogType.Information);

            return setResult;
        }

        public bool CreateNewSyncIssue(SyncIssue syncIssue)
        {
            string keySuffix = Guid.NewGuid().ToString("N");
            syncIssue.CachedKey = $"syncIssue{syncIssue.FromToDirection}{syncIssue.TransferType}PLC{syncIssue.PlcDeviceId}Batch{syncIssue.BatchId}KeySuffix{keySuffix}";
            syncIssue.CreateDate = DateTime.Now;
            syncIssue.ModifiedDate = DateTime.Now;
            var serialized = JsonConvert.SerializeObject(syncIssue);
            bool setResult = _mainCacheManager.Set(syncIssue.CachedKey, serialized, TimeSpan.FromDays(60));
            LogManager.Instance.Log($"syncIssue Created! Detail: \n {JsonConvert.SerializeObject(syncIssue, new JsonSerializerSettings { Formatting = Formatting.Indented })}", LogType.Information);
            return setResult;
        }


        /// <summary>
        /// Updates given Issue with modifiying its modify date.
        /// It uses local cache environment
        /// </summary>
        /// <param name="syncIssue"></param>
        /// <returns></returns>
        public bool UpdateSyncIssue(SyncIssue syncIssue)
        {
            bool setResult = false;
            try
            {
                syncIssue.ModifiedDate = DateTime.Now;
                var serialized = JsonConvert.SerializeObject(syncIssue);
                setResult = _mainCacheManager.Set(syncIssue.CachedKey, serialized, TimeSpan.FromDays(60));
                LogManager.Instance.Log($"syncIssue Updated! Detail: \n {JsonConvert.SerializeObject(syncIssue, new JsonSerializerSettings { Formatting = Formatting.Indented })}", LogType.Information);

            }
            catch (Exception ex)
            {
                setResult = false;
            }
            return setResult;
        }
        /// <summary>
        /// Updates given Issue with modifiying its modify date.
        /// It uses local cache environment
        /// </summary>
        /// <param name="syncIssue"></param>
        /// <returns></returns>
        public bool UpdateRemoteSyncIssue(SyncIssue syncIssue)
        {
            bool setResult = false;

            //todo:h refactor    run after server slave
            Thread.Sleep(1000*10);

            try
            {
                syncIssue.ModifiedDate = DateTime.Now;
                var serialized = JsonConvert.SerializeObject(syncIssue);
                setResult = _remoteMainCacheManager.Set(syncIssue.CachedKey, serialized, TimeSpan.FromDays(60));
                LogManager.Instance.Log($"syncIssue Updated! Detail: \n {JsonConvert.SerializeObject(syncIssue, new JsonSerializerSettings { Formatting = Formatting.Indented })}", LogType.Information);

            }
            catch (Exception ex)
            {
                setResult = false;
            }
            return setResult;
        }


        /// <summary>
        /// Deletes given Issue with modifiying its modify date.
        /// It uses local cache environment
        /// </summary>
        /// <param name="syncIssue"></param>
        /// <returns></returns>
        public bool DeleteSyncIssue(SyncIssue syncIssue)
        {
            bool deleteResult;

            try
            {
                deleteResult = _mainCacheManager.DeleteKey(syncIssue.CachedKey);
                LogManager.Instance.Log($"syncIssue Deleted! Detail: \n {JsonConvert.SerializeObject(syncIssue, new JsonSerializerSettings { Formatting = Formatting.Indented })}", LogType.Information);

            }
            catch (Exception ex)
            {
                deleteResult = false;
            }
            return deleteResult;
        }

        public int DeleteAllSyncIssues()
        {
            int deleteResult;

            try
            {
                deleteResult = _mainCacheManager.DeleteManyKeys("syncIssue*");
                
            }
            catch (Exception ex)
            {
                deleteResult = 0;
                LogManager.Instance.Log($"syncIssues Deletion error! Detail: {ex.Message}", LogType.Information);
            }
            return deleteResult;
        }

        /// <summary>
        /// Retrives Issue list
        /// </summary>
        /// <param name="plcDeviceId">Parameter is optional. If the parameter empty it retrieves all list items. </param>
        public List<SyncIssue> SyncIssues(int plcDeviceId=default)
        {
            List<SyncIssue> syncIssues = new List<SyncIssue>();
            var keys = _mainCacheManager.GetKeyNames("syncIssue*");

           
                foreach (var key in keys)
                {
                    var serialized = _mainCacheManager.GetString(key);
                    SyncIssue deserializedItem = JsonConvert.DeserializeObject<SyncIssue>(serialized);

                    if (deserializedItem != null)
                    {
                        syncIssues.Add(deserializedItem);
                    }
                }

                if (plcDeviceId > default(int))
                {
                    syncIssues = syncIssues.Where(x => x.PlcDeviceId == plcDeviceId).ToList();
                }
                syncIssues = syncIssues.OrderBy(x => x.CreateDate).ToList();
           


            return syncIssues;
        }

        public List<SyncIssue> RemoteSyncIssues(int plcDeviceId = default)
        {
            List<SyncIssue> syncIssues = new List<SyncIssue>();

            var keys = _remoteMainCacheManager.GetKeyNames("syncIssue*");

            foreach (var key in keys)
            {
                var serialized =_remoteMainCacheManager.GetString(key);
                SyncIssue deserializedItem = JsonConvert.DeserializeObject<SyncIssue>(serialized);

                if (deserializedItem != null)
                {
                    syncIssues.Add(deserializedItem);
                }
            }

            if (plcDeviceId > default(int))
            {
                syncIssues = syncIssues.Where(x => x.PlcDeviceId == plcDeviceId).ToList();
            }
            syncIssues = syncIssues.OrderBy(x => x.CreateDate).ToList();
            return syncIssues;
        }
    }
}
