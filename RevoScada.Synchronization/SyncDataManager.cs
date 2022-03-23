using System;
using Revo.Core;
using System.Linq;
using Newtonsoft.Json;
using RevoScada.Cache;
using RevoScada.Business;
using RevoScada.Entities;
using System.Collections.Generic;
using RevoScada.Synchronization.Types;
using RevoScada.Synchronization.Enums;
using RevoScada.Entities.Configuration.Service;

namespace RevoScada.Synchronization
{
    internal class SyncDataManager
    {
        //private readonly string _connectionString;
        private readonly CacheManager _mainCacheManager;
        private readonly CacheManager _serverCacheManager;
        private readonly Dictionary<int, string> _connectionStrings;
        private readonly SyncConfiguration _syncConfiguration;

        /// <summary>
        /// Manages sync data manipulation between cache to db.
        /// </summary>
        internal SyncDataManager(SyncConfiguration syncConfiguration)
        {
            try
            {
                _syncConfiguration = syncConfiguration;
                _mainCacheManager = new CacheManager(CacheDBType.Main, syncConfiguration.RedisServer);

                if (syncConfiguration.WorkingEnvironment == WorkingEnvironment.pc)
                {
                    _serverCacheManager = new CacheManager(CacheDBType.Main, syncConfiguration.RemoteRedisServer);
                }

                _connectionStrings = syncConfiguration.PostgreSqlConnectionStrings;
            }
            catch (Exception ex)
            {
                ex.Data["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().ToString();
                throw ex;
            }
        }

        /// <summary>
        /// senkronize tamamlanmadan start sonrası adımlar atılmayacak. 
        /// pc to server
        /// </summary>
        /// <param name="plcDeviceId"></param>
        /// <param name="batchId"></param>
        /// <returns></returns>
        internal bool SendSingleBatchDataPCtoServerAfterStart(int plcDeviceId, int batchId)
        {
            FromToDirection fromToDirection = FromToDirection.PCtoServer;
            TransferType transferType = TransferType.AfterStart;

            bool result;
            try
            {
                var batchService = new BatchService(_connectionStrings[plcDeviceId]);
                var integratedCheckResultService = new IntegratedCheckResultService(_connectionStrings[plcDeviceId]);
                var recipeDetailHistoryService = new RecipeDetailHistoryService(_connectionStrings[plcDeviceId]);
                var recipeService = new RecipeService(_connectionStrings[plcDeviceId]);
                var skippedIntegratedCheckResultService = new SkippedIntegratedCheckResultService(_connectionStrings[plcDeviceId]);
                var currentProcessInfoService = new CurrentProcessInfoService(_connectionStrings[plcDeviceId]);
                var currentProcessInfo = currentProcessInfoService.Get();
                var batch = batchService.GetById(batchId);
                var integratedCheckResults = integratedCheckResultService.GetAllByBatchId(batch.id).ToList();
                var recipeDetailHistories = recipeDetailHistoryService.GetByBatch(batch.RecipeId, batch.id).ToList();
                var recipe = recipeService.GetById(batch.RecipeId);
                var skippedIntegratedCheckResult = skippedIntegratedCheckResultService.GetByBatchId(batch.id);

                string syncSingleBatchDataHeaderCachedKey = $"SyncSingleBatchDataHeader{fromToDirection}{transferType}PLC{plcDeviceId}Batch{batchId}";
                string syncSingleBatchDataKey = Guid.NewGuid().ToString("N");

                var syncData = new SyncSingleBatchData
                {
                    Batch = batch,
                    IntegratedCheckResults = integratedCheckResults,
                    RecipeDetailHistories = recipeDetailHistories,
                    Recipe = recipe,
                    SkippedIntegratedCheckResult = skippedIntegratedCheckResult,
                    CurrentProcessInfo = currentProcessInfo,
                };

                var syncDataHeader = new SyncSingleBatchDataHeader
                {
                    CachedKey = syncSingleBatchDataHeaderCachedKey,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    BatchId = batchId,
                    PlcDeviceId = plcDeviceId,
                    SyncState = SyncDataTransferState.ReadyToFetch,
                    FromToDirection = fromToDirection,
                    TransferType = transferType,
                    SyncSingleBatchDataKey = syncSingleBatchDataKey
                };

                var syncSingleBatchDataSerialized = JsonConvert.SerializeObject(syncData);
                bool dataResult = _serverCacheManager.Set(syncSingleBatchDataKey, syncSingleBatchDataSerialized, TimeSpan.FromDays(60));

                var SyncSingleBatchDataHeaderSerialized = JsonConvert.SerializeObject(syncDataHeader);
                bool headerResult = _serverCacheManager.Set(syncSingleBatchDataHeaderCachedKey, SyncSingleBatchDataHeaderSerialized, TimeSpan.FromDays(60));

                result = dataResult && headerResult;
            }
            catch (Exception ex)
            {
                ex.Data["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().ToString();
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// Transfer single batch data to server.
        /// </summary>
        internal bool SendSingleBatchDataPCtoServerAfterHold(int plcDeviceId, int batchId)
        {
            FromToDirection fromToDirection = FromToDirection.PCtoServer;
            TransferType transferType = TransferType.AfterHold;

            bool result;

            try
            {
                var batchService = new BatchService(_connectionStrings[plcDeviceId]);
                var currentProcessInfoService = new CurrentProcessInfoService(_connectionStrings[plcDeviceId]);
                var recipeGroupService = new RecipeGroupService(_connectionStrings[plcDeviceId]);

                var currentProcessInfo = currentProcessInfoService.Get();
                var batch = batchService.GetById(batchId);

                string syncSingleBatchDataHeaderCachedKey = $"SyncSingleBatchDataHeader{fromToDirection}{transferType}PLC{plcDeviceId}Batch{batchId}";
                string syncSingleBatchDataKey = Guid.NewGuid().ToString("N");

                var syncData = new SyncSingleBatchData
                {
                    Batch = batch,
                    CurrentProcessInfo = currentProcessInfo,
                };

                var syncDataHeader = new SyncSingleBatchDataHeader
                {
                    CachedKey = syncSingleBatchDataHeaderCachedKey,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    BatchId = batchId,
                    PlcDeviceId = plcDeviceId,
                    SyncState = SyncDataTransferState.ReadyToFetch,
                    FromToDirection = fromToDirection,
                    TransferType = transferType,
                    SyncSingleBatchDataKey = syncSingleBatchDataKey,
                };

                var syncSingleBatchDataSerialized = JsonConvert.SerializeObject(syncData);
                bool dataResult = _serverCacheManager.Set(syncSingleBatchDataKey, syncSingleBatchDataSerialized, TimeSpan.FromDays(60));

                var SyncSingleBatchDataHeaderSerialized = JsonConvert.SerializeObject(syncDataHeader);
                bool headerResult = _serverCacheManager.Set(syncSingleBatchDataHeaderCachedKey, SyncSingleBatchDataHeaderSerialized, TimeSpan.FromDays(60));

                result = dataResult && headerResult;
            }
            catch (Exception ex)
            {
                ex.Data["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().ToString();
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// Transfer single batch data to server.
        /// </summary>
        internal bool SendSingleBatchDataPCtoServerAfterFinish(int plcDeviceId, int batchId)
        {
            FromToDirection fromToDirection = FromToDirection.PCtoServer;
            TransferType transferType = TransferType.AfterFinish;

            bool result;

            try
            {
                var batchService = new BatchService(_connectionStrings[plcDeviceId]);
                var currentProcessInfoService = new CurrentProcessInfoService(_connectionStrings[plcDeviceId]);
                var processEventLogService = new ProcessEventLogService(_connectionStrings[plcDeviceId]);
                var plcAlarmService = new PlcAlarmService(_connectionStrings[plcDeviceId]);

                var currentProcessInfo = currentProcessInfoService.Get();
                var batch = batchService.GetById(batchId);

                string syncSingleBatchDataHeaderCachedKey = $"SyncSingleBatchDataHeader{fromToDirection}{transferType}PLC{plcDeviceId}Batch{batchId}";
                string syncSingleBatchDataKey = Guid.NewGuid().ToString("N");
                string plcAlarmsCachedRelationKey = Guid.NewGuid().ToString("N");
                var plcAlarms = plcAlarmService.GetByBatchId(batchId);

                List<string> serializedPlcAlarms = new List<string>();
                foreach (var alarmItem in plcAlarms)
                {
                    string plcAlarmSerialized = JsonConvert.SerializeObject(alarmItem);
                    serializedPlcAlarms.Add(plcAlarmSerialized);
                }
                if (plcAlarms != null)
                {
                    _ = _serverCacheManager.ListLeftPushString(plcAlarmsCachedRelationKey, serializedPlcAlarms, 3);
                }

                var syncData = new SyncSingleBatchData
                {
                    Batch = batch,
                    CurrentProcessInfo = currentProcessInfo,
                    PlcAlarmsCachedRelationKey = plcAlarmsCachedRelationKey
                };

                var syncDataHeader = new SyncSingleBatchDataHeader
                {
                    CachedKey = syncSingleBatchDataHeaderCachedKey,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    BatchId = batchId,
                    PlcDeviceId = plcDeviceId,
                    SyncState = SyncDataTransferState.ReadyToFetch,
                    FromToDirection = fromToDirection,
                    TransferType = transferType,
                    SyncSingleBatchDataKey = syncSingleBatchDataKey
                };

                var syncSingleBatchDataSerialized = JsonConvert.SerializeObject(syncData);
                bool dataResult = _serverCacheManager.Set(syncSingleBatchDataKey, syncSingleBatchDataSerialized, TimeSpan.FromDays(60));

                var SyncSingleBatchDataHeaderSerialized = JsonConvert.SerializeObject(syncDataHeader);
                bool headerResult = _serverCacheManager.Set(syncSingleBatchDataHeaderCachedKey, SyncSingleBatchDataHeaderSerialized, TimeSpan.FromDays(60));

                result = dataResult && headerResult;
            }
            catch (Exception ex)
            {
                ex.Data["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().ToString();
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// Creates syncissue with object item in server side
        /// </summary>
        /// <param name="syncIssue"></param>
        /// <returns></returns>
        internal bool TransferIssueDataToServer(SyncIssue syncIssue)
        {
            string serialized = JsonConvert.SerializeObject(syncIssue);
            _serverCacheManager.Set(syncIssue.CachedKey, serialized);
            return true;
        }

        /// <summary>
        /// Writes single batch data from server cache to server db.
        /// </summary>
        internal SyncDataTransferState WriteSingleBatchDataToDB(SyncSingleBatchDataHeader syncSingleBatchDataHeader)
        {
            var serialized = _mainCacheManager.GetString(syncSingleBatchDataHeader.SyncSingleBatchDataKey);
            SyncSingleBatchData syncSingleBatchData = JsonConvert.DeserializeObject<SyncSingleBatchData>(serialized);
            var currentProcessInfoService = new CurrentProcessInfoService(_connectionStrings[syncSingleBatchDataHeader.PlcDeviceId]);
            var batchService = new BatchService(_connectionStrings[syncSingleBatchDataHeader.PlcDeviceId]);
            var activeTagService = new ActiveTagService(_connectionStrings[syncSingleBatchDataHeader.PlcDeviceId]);
            var recipeGroupService = new RecipeGroupService(_connectionStrings[syncSingleBatchDataHeader.PlcDeviceId]);

            Dictionary<string, bool> resultList;

            bool result = false;
            resultList = new Dictionary<string, bool>();

            switch (syncSingleBatchDataHeader.TransferType)
            {
                case TransferType.AfterStart:
                    var recipeService = new RecipeService(_connectionStrings[syncSingleBatchDataHeader.PlcDeviceId]);
                    var integratedCheckResultService = new IntegratedCheckResultService(_connectionStrings[syncSingleBatchDataHeader.PlcDeviceId]);
                    var recipeDetailHistoryService = new RecipeDetailHistoryService(_connectionStrings[syncSingleBatchDataHeader.PlcDeviceId]);
                    var skippedIntegratedCheckResultService = new SkippedIntegratedCheckResultService(_connectionStrings[syncSingleBatchDataHeader.PlcDeviceId]);
                    UpdateSingleBatchCurrentProcessInfo(syncSingleBatchData.CurrentProcessInfo);
                    System.Threading.Thread.Sleep(2000);

                    resultList["currentProcessInfo"] = currentProcessInfoService.InsertOrUpdate(syncSingleBatchData.CurrentProcessInfo);
                    resultList["batch"] = batchService.InsertOrUpdate(syncSingleBatchData.Batch ?? new Batch());
                    resultList["integratedCheck"] = integratedCheckResultService.InsertOrUpdateMany(syncSingleBatchData.IntegratedCheckResults ?? new List<IntegratedCheckResult>());
                    resultList["recipeDetailHistory"] = recipeDetailHistoryService.InsertOrUpdateMany(syncSingleBatchData.RecipeDetailHistories);
                    resultList["recipe"] = recipeService.InsertOrUpdate(syncSingleBatchData.Recipe);
                    resultList["skippedIntegratedCheck"] = skippedIntegratedCheckResultService.InsertOrUpdate(syncSingleBatchData.SkippedIntegratedCheckResult ?? new SkippedIntegratedCheckResult());

                    var recipeGroups = recipeGroupService.GetAll().ToList();
                    var activeTags = activeTagService.GetAll().ToList();
                    result = resultList.Values.Select(x => x).All(x => x == true);
                    break;
                case TransferType.AfterFinish:
                    var plcAlarmService = new PlcAlarmService(_connectionStrings[syncSingleBatchDataHeader.PlcDeviceId]);
                    var processEventLogService = new ProcessEventLogService(_connectionStrings[syncSingleBatchDataHeader.PlcDeviceId]);
                    UpdateSingleBatchCurrentProcessInfo(syncSingleBatchData.CurrentProcessInfo);
                    System.Threading.Thread.Sleep(2000);
                    resultList["currentProcessInfo"] = currentProcessInfoService.InsertOrUpdate(syncSingleBatchData.CurrentProcessInfo);
                    resultList["batch"] = batchService.InsertOrUpdate(syncSingleBatchData.Batch ?? new Batch());
                    resultList["processEventLogs"] = processEventLogService.InsertOrUpdateMany(syncSingleBatchData.ProcessEventLogs ?? new List<ProcessEventLog>());

                    List<PlcAlarm> plcAlarms = new List<PlcAlarm>();
                    CacheResponse cacheResponse;
                    cacheResponse = _serverCacheManager.GetAll($"{syncSingleBatchData.PlcAlarmsCachedRelationKey}", 5);

                    if (cacheResponse.CacheResponseState == CacheResponseStates.Success)
                    {
                        List<string> serializedAlarmList = (List<string>)cacheResponse.ResultValue;
                        foreach (var serializedAlarm in serializedAlarmList)
                        {
                            PlcAlarm plcAlarm = JsonConvert.DeserializeObject<PlcAlarm>(serializedAlarm);
                            plcAlarms.Add(plcAlarm);
                        }
                    }

                    resultList["plcAlarms"] = plcAlarmService.InsertOrUpdate(plcAlarms);
                    result = resultList.Values.Select(x => x).All(x => x == true);

                    if (result)
                    {
                        _serverCacheManager.DeleteKey(syncSingleBatchData.PlcAlarmsCachedRelationKey);
                    }
                    break;
                case TransferType.AfterHold:
                    UpdateSingleBatchCurrentProcessInfo(syncSingleBatchData.CurrentProcessInfo);
                    System.Threading.Thread.Sleep(2000);
                    resultList["currentProcessInfo"] = currentProcessInfoService.InsertOrUpdate(syncSingleBatchData.CurrentProcessInfo);
                    resultList["batch"] = batchService.InsertOrUpdate(syncSingleBatchData.Batch ?? new Batch());
                    result = resultList.Values.Select(x => x).All(x => x == true);
                    break;
            }

            if (result)
            {
                syncSingleBatchDataHeader.SyncState = SyncDataTransferState.FetchCompleted;
            }

            return syncSingleBatchDataHeader.SyncState;
        }

        internal bool UpdateSingleBatchCurrentProcessInfo(CurrentProcessInfo currentProcessInfo)
        {
            try
            {
                var serializedCurrentBatchInfo = JsonConvert.SerializeObject(currentProcessInfo);
                var cacheSaveResult = _mainCacheManager.Set($"CurrentProcessInfoPLC{currentProcessInfo.PlcDeviceId}", serializedCurrentBatchInfo);
                return cacheSaveResult;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal CurrentProcessInfo GetCurrentProcessInfo(int currentProcessInfoPlcId)
        {
            try
            {
                var serializedCurrentBatchInfo = _mainCacheManager.GetString($"CurrentProcessInfoPLC{currentProcessInfoPlcId}");
                var currentProcessInfo = JsonConvert.DeserializeObject<CurrentProcessInfo>(serializedCurrentBatchInfo);
                return currentProcessInfo;
            }
            catch (Exception)
            {
                return new CurrentProcessInfo();
            }
        }

        internal bool CheckSingleBatchDataForServer()
        {
            bool result = false;

            try
            {
                List<string> keyNames = _mainCacheManager.GetKeyNames("SyncSingleBatchDataHeader*");

                foreach (var key in keyNames)
                {
                    var serialized = _mainCacheManager.GetString(key);
                    SyncSingleBatchDataHeader syncSingleBatchDataHeader = JsonConvert.DeserializeObject<SyncSingleBatchDataHeader>(serialized);

                    if (syncSingleBatchDataHeader.FromToDirection == FromToDirection.PCtoServer)
                    {
                        if (syncSingleBatchDataHeader.SyncState == SyncDataTransferState.ReadyToFetch)
                        {
                            new LogFormatter().LogObject(syncSingleBatchDataHeader, $"SyncSingleBatchDataHeader");
                            var syncDataTransferState = WriteSingleBatchDataToDB(syncSingleBatchDataHeader);

                            if (syncDataTransferState == SyncDataTransferState.FetchCompleted)
                            {
                                new LogFormatter().LogObject(syncSingleBatchDataHeader, $"SyncSingleBatchDataHeader");
                                _mainCacheManager.DeleteKey(syncSingleBatchDataHeader.CachedKey);
                                _mainCacheManager.DeleteKey(syncSingleBatchDataHeader.SyncSingleBatchDataKey);
                            }
                        }
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }


            return result;
        }

        internal bool SaveSyncSingleBatchDataHeader(SyncSingleBatchDataHeader syncSingleBatchDataHeader)
        {
            bool result = false;
            try
            {
                var serialized = JsonConvert.SerializeObject(syncSingleBatchDataHeader);
                _serverCacheManager.Set($"SyncSingleBatchDataHeader{syncSingleBatchDataHeader.FromToDirection}{syncSingleBatchDataHeader.TransferType}PLC{syncSingleBatchDataHeader.PlcDeviceId}Batch{syncSingleBatchDataHeader.BatchId}", serialized, TimeSpan.FromDays(60));
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;

        }

        internal SyncSingleBatchDataHeader GetSyncSingleBatchDataHeader(FromToDirection fromToDirection, TransferType transferType, int plcDeviceId, int batchId)
        {
            var serialized = _serverCacheManager.GetString($"SyncSingleBatchDataHeader{fromToDirection}{transferType}PLC{plcDeviceId}Batch{batchId}");
            SyncSingleBatchDataHeader synchronizationTransferDataHeader = JsonConvert.DeserializeObject<SyncSingleBatchDataHeader>(serialized);
            return synchronizationTransferDataHeader;
        }

        #region MissingBulkDataOperations

        /// <summary>
        /// Write Bulk data from remote server cache to local db.
        /// This meethod is pc only.
        /// </summary>
        internal bool WriteMissingBulkDataToLocalDB(MissingBulkDataHeader missingBulkDataHeader)
        {
            MissingBulkData missingBulkData = FetchMissingBulkDataFromServerCache(missingBulkDataHeader);

            bool result = false;
            var currentProcessInfoService = new CurrentProcessInfoService(_connectionStrings[missingBulkDataHeader.PlcDeviceId]);
            Dictionary<string, bool> resultList;

            var batchService = new BatchService(_connectionStrings[missingBulkDataHeader.PlcDeviceId]);
            var recipeDetailHistoryService = new RecipeDetailHistoryService(_connectionStrings[missingBulkDataHeader.PlcDeviceId]);
            var skippedIntegratedCheckResultService = new SkippedIntegratedCheckResultService(_connectionStrings[missingBulkDataHeader.PlcDeviceId]);
            var plcAlarmService = new PlcAlarmService(_connectionStrings[missingBulkDataHeader.PlcDeviceId]);
            var dataLogService = new DataLogService(_connectionStrings[missingBulkDataHeader.PlcDeviceId]);

            resultList = new Dictionary<string, bool>();

            UpdateSingleBatchCurrentProcessInfo(missingBulkData.CurrentProcessInfo);

            System.Threading.Thread.Sleep(2000);
            resultList["currentProcessInfo"] = currentProcessInfoService.InsertOrUpdate(missingBulkData.CurrentProcessInfo);
            resultList["batch"] = batchService.InsertOrUpdateMany(missingBulkData.Batches);
            resultList["recipeDetailHistory"] = recipeDetailHistoryService.InsertOrUpdateMany(missingBulkData.RecipeDetailHistories);
            resultList["skippedIntegratedCheck"] = skippedIntegratedCheckResultService.InsertOrUpdateMany(missingBulkData.SkippedIntegratedCheckResults);
            resultList["currentProcessInfo"] = currentProcessInfoService.InsertOrUpdate(missingBulkData.CurrentProcessInfo);
            resultList["plcAlarms"] = plcAlarmService.InsertOrUpdate(missingBulkData.PlcAlarms);

            if (missingBulkDataHeader.TransferType == TransferType.MissingBatchDataWithDataLogs)
            {
                foreach (var batchItem in missingBulkData.Batches)
                {
                    long maxDataLogId = (long)dataLogService.GetMaxIdByBatchId(batchItem.id);
                    List<DataLog> filtered = new List<DataLog>();

                    if (maxDataLogId == 0)
                    {
                        filtered = missingBulkData.DataLogs.Where(x => x.BatchId == batchItem.id).ToList();
                    }
                    else
                    {
                        var getLatestDataLog = dataLogService.GetById(maxDataLogId);
                        filtered = missingBulkData.DataLogs.Where(x => x.ReceivedDate > getLatestDataLog.ReceivedDate && x.BatchId == batchItem.id).ToList();
                    }
                    missingBulkData.DataLogs = filtered;
                    resultList["dataLogs"] = dataLogService.InsertOrUpdateMany(missingBulkData.DataLogs);
                }
            }
            result = resultList.Values.Select(x => x).All(x => x == true);
            return result;
        }

        private MissingBulkData FetchMissingBulkDataFromServerCache(MissingBulkDataHeader missingBulkDataHeader)
        {
            var missingDataLogsSerialized = _serverCacheManager.GetString($"{missingBulkDataHeader.MissingBulkDataKey}");
            MissingBulkData missingDataLogs = JsonConvert.DeserializeObject<MissingBulkData>(missingDataLogsSerialized);

            List<PlcAlarm> plcAlarms = new List<PlcAlarm>();
            List<DataLog> dataLogs = new List<DataLog>();

            CacheResponse cacheResponse;
            cacheResponse = _serverCacheManager.GetAll($"{missingDataLogs.PlcAlarmsCachedRelationKey}", 5);

            if (cacheResponse.CacheResponseState == CacheResponseStates.Success)
            {
                List<string> serializedAlarmList = (List<string>)cacheResponse.ResultValue;

                foreach (var serializedAlarm in serializedAlarmList)
                {
                    PlcAlarm plcAlarm = JsonConvert.DeserializeObject<PlcAlarm>(serializedAlarm);
                    plcAlarms.Add(plcAlarm);
                }
            }

            cacheResponse = _serverCacheManager.GetAll($"{missingDataLogs.DataLogsCachedRelationKey}", 5);

            if (cacheResponse.CacheResponseState == CacheResponseStates.Success)
            {
                List<string> serializedDataLogList = (List<string>)cacheResponse.ResultValue;

                foreach (var serializedDataLog in serializedDataLogList)
                {
                    DataLog dataLog = JsonConvert.DeserializeObject<DataLog>(serializedDataLog);
                    dataLogs.Add(dataLog);
                }
            }
            dataLogs = dataLogs.OrderBy(x => x.id).ToList();
            missingDataLogs.PlcAlarms = plcAlarms;
            missingDataLogs.DataLogs = dataLogs;
            return missingDataLogs;
        }

        internal bool RequestMissingData(FromToDirection direction, TransferType transferType, int plcDeviceId)
        {
            bool result;

            try
            {
                var missingBulkDataHeader = new MissingBulkDataHeader
                {
                    PlcDeviceId = plcDeviceId,
                    CreateDate = DateTime.Now,
                    SyncState = SyncDataTransferState.NotStarted,
                    FromToDirection = direction,
                    TransferType = transferType,
                    MissingBulkDataKey = Guid.NewGuid().ToString("N")
                };
                result = SaveMissingBulkDataHeader(missingBulkDataHeader);
            }
            catch (Exception ex)
            {
                ex.Data["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().ToString();
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// This method for server usage
        /// </summary>
        /// <param name="transferType"></param>
        /// <param name="plcDeviceId"></param>
        /// <returns></returns>
        internal bool TransferMissingBulkDataFromDBtoCache(TransferType transferType, int plcDeviceId)
        {
            MissingBulkDataHeader missingBulkDataHeader = GetMissingBulkDataHeader(transferType, plcDeviceId);
            missingBulkDataHeader.SyncState = SyncDataTransferState.PreparingData;
            var batchService = new BatchService(_connectionStrings[plcDeviceId]);
            var recipeDetailHistoryService = new RecipeDetailHistoryService(_connectionStrings[plcDeviceId]);
            var skippedIntegratedCheckResultService = new SkippedIntegratedCheckResultService(_connectionStrings[plcDeviceId]);
            var currentProcessInfoService = new CurrentProcessInfoService(_connectionStrings[plcDeviceId]);
            var dataLogService = new DataLogService(_connectionStrings[plcDeviceId]);
            var plcAlarmService = new PlcAlarmService(_connectionStrings[plcDeviceId]);

            SyncIssueManager syncIssueManager = new SyncIssueManager(_syncConfiguration.RedisServer);
            var syncIssues = syncIssueManager.SyncIssues(plcDeviceId).OrderBy(x => x.ModifiedDate);
            var syncIssuesForBulkRequest = syncIssues.Where(x => x.FromToDirection == FromToDirection.ServerToPC && x.SyncStatus != SyncStatus.Completed && x.TransferType != TransferType.NonProcessChanges);

            if (syncIssuesForBulkRequest != null && syncIssuesForBulkRequest.ToList().Count() > 0)
            {
                string plcAlarmsCachedRelationKey = Guid.NewGuid().ToString("N");
                string dataLogsCachedRelationKey = Guid.NewGuid().ToString("N");

                List<Batch> batchs = new List<Batch>();

                foreach (var item in syncIssuesForBulkRequest)
                {
                    var batch = batchService.GetById(item.BatchId);
                    var batchToCompare = batchs.FirstOrDefault(x => x.id == item.BatchId);

                    if (batchToCompare != null && batchToCompare.StartDate <= batch.StartDate)
                    {
                        var removalBatch = batchs.FirstOrDefault(x => x.id == batchToCompare.id);
                        batchs.Remove(removalBatch);
                        batchs.Add(batch);
                    }
                    else
                    {
                        batchs.Add(batch);
                    }
                }

                List<Bag> bags = new List<Bag>();
                List<IntegratedCheckResult> integratedCheckResults = new List<IntegratedCheckResult>();
                List<LotProperty> lotProperties = new List<LotProperty>();
                List<RecipeDetailHistory> recipeDetailHistories = new List<RecipeDetailHistory>();
                List<RecipeDetail> recipeDetails = new List<RecipeDetail>();
                List<Recipe> recipes = new List<Recipe>();
                List<SkippedIntegratedCheckResult> skippedIntegratedCheckResults = new List<SkippedIntegratedCheckResult>();
                List<DataLog> dataLogs = new List<DataLog>();
                List<ActiveTag> activeTags = new List<ActiveTag>();
                List<RecipeGroup> recipeGroups = new List<RecipeGroup>();
                List<ProcessEventLog> processEventLogs = new List<ProcessEventLog>();
                CurrentProcessInfo currentProcessInfo = currentProcessInfoService.Get();

                LogManager.Instance.Log("******************Transfer Missing BulkData DB to Cache**************************************\n", LogType.Information);

                foreach (var batch in batchs)
                {
                    var recipeDetailHistoriesByBatch = recipeDetailHistoryService.GetByBatch(batch.RecipeId, batch.id);
                    if (recipeDetailHistoriesByBatch != null)
                    {
                        recipeDetailHistories.AddRange(recipeDetailHistoriesByBatch);
                    }

                    var skippedIntegratedCheckResultsByBatch = skippedIntegratedCheckResultService.GetByBatchId(batch.id);
                    if (skippedIntegratedCheckResultsByBatch != null)
                    {
                        skippedIntegratedCheckResults.Add(skippedIntegratedCheckResultsByBatch);
                    }

                    var plcAlarms = plcAlarmService.GetByBatchId(batch.id);
                    List<string> serializedPlcAlarms = new List<string>();
                    foreach (var alarmItem in plcAlarms)
                    {
                        string plcAlarmSerialized = JsonConvert.SerializeObject(alarmItem);
                        serializedPlcAlarms.Add(plcAlarmSerialized);
                    }
                    if (plcAlarms != null)
                    {
                        _ = _serverCacheManager.ListLeftPushString(plcAlarmsCachedRelationKey, serializedPlcAlarms, 3);
                    }

                }

                if (missingBulkDataHeader.TransferType == TransferType.MissingBatchDataWithDataLogs)
                {
                    foreach (var batch in batchs)
                    {
                        var dataLogsByBatch = dataLogService.GetByBatch(batch.id);

                        if (dataLogsByBatch != null)
                        {
                            List<string> serializedDataLogs = new List<string>();
                            foreach (var dataLogItem in dataLogsByBatch)
                            {
                                string serializedDataLog = JsonConvert.SerializeObject(dataLogItem);
                                serializedDataLogs.Add(serializedDataLog);
                            }
                            _ = _serverCacheManager.ListLeftPushString(dataLogsCachedRelationKey, serializedDataLogs, 3);
                        }
                    }
                }

                LogManager.Instance.Log($"Total Datalogs: {dataLogs.Count}", LogType.Information);
                LogManager.Instance.Log($"Recipe Id List =  { string.Join(", ", recipes.Select(x => x.id).Select(_ => _))}\n\n", LogType.Information);
                LogManager.Instance.Log($"Total Batch Count:{batchs.Count()}", LogType.Information);
                LogManager.Instance.Log($"Batches:{ string.Join(", ", batchs.Select(x => x.id).Select(_ => _))}\n\n", LogType.Information);

                LogManager.Instance.Log("\n******************End ofTransfer Missing BulkData DB to Cache**************************************", LogType.Information);

                MissingBulkData missingBulkData = new MissingBulkData
                {
                    CreateDate = DateTime.Now,
                    CurrentProcessInfo = currentProcessInfo,
                    Batches = batchs,
                    //Bags = bags,
                    //IntegratedCheckResults = integratedCheckResults,
                    //LotProperties = lotProperties,
                    RecipeDetailHistories = recipeDetailHistories,
                    //RecipeDetails = recipeDetails,
                    //Recipes = recipes,
                    SkippedIntegratedCheckResults = skippedIntegratedCheckResults,
                    //LastLoadNumber = lastLoadNumber,
                    //ActiveTags = activeTags,
                    //RecipeGroups = recipeGroups,
                    DataLogsCachedRelationKey = dataLogsCachedRelationKey,
                    PlcAlarmsCachedRelationKey = plcAlarmsCachedRelationKey,
                    //ProcessEventLogs = processEventLogs
                };

                var serialized = JsonConvert.SerializeObject(missingBulkData);
                bool setResult = _serverCacheManager.Set($"{missingBulkDataHeader.MissingBulkDataKey}", serialized, TimeSpan.FromDays(60));

                foreach (var item in syncIssuesForBulkRequest)
                {
                    item.SyncStatus = SyncStatus.Completed;
                    syncIssueManager.UpdateSyncIssue(item);
                }

                missingBulkDataHeader.SyncState = SyncDataTransferState.ReadyToFetch;
            }
            else
            {
                missingBulkDataHeader.SyncState = SyncDataTransferState.FetchCompleted;
            }

            bool transferResult = SaveMissingBulkDataHeader(missingBulkDataHeader);


            return transferResult;
        }

        internal bool SaveMissingBulkDataHeader(MissingBulkDataHeader missingBulkDataHeader)
        {
            bool result = false;
            try
            {
                missingBulkDataHeader.CachedKey = $"MissingBulkDataHeader{missingBulkDataHeader.TransferType}PLC{missingBulkDataHeader.PlcDeviceId}";
                var synchronizationTransferSerialized = JsonConvert.SerializeObject(missingBulkDataHeader);
                _serverCacheManager.Set(missingBulkDataHeader.CachedKey, synchronizationTransferSerialized, TimeSpan.FromDays(60));
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        internal MissingBulkDataHeader GetMissingBulkDataHeader(TransferType transferType, int plcDeviceId)
        {
            MissingBulkDataHeader missingBulkDataHeader = null;
            try
            {
                var missingBulkDataHeaderSerialized = _serverCacheManager.GetString($"MissingBulkDataHeader{transferType}PLC{plcDeviceId}");
                missingBulkDataHeader = JsonConvert.DeserializeObject<MissingBulkDataHeader>(missingBulkDataHeaderSerialized);
            }
            catch
            {
                missingBulkDataHeader = null;
            }

            return missingBulkDataHeader;
        }

        internal bool DeleteMissingBulkDataHeaderFromServer(MissingBulkDataHeader missingBulkDataHeader)
        {
            bool resultData = _mainCacheManager.DeleteKey(missingBulkDataHeader.MissingBulkDataKey);
            bool resultDataHeader = _mainCacheManager.DeleteKey(missingBulkDataHeader.CachedKey);
            return resultData && resultDataHeader;
        }

        #endregion

        public bool SaveNonProcessEntityToDB(SyncIssue syncIssue)
        {
            bool commandResult = false;

            switch (syncIssue.EntityObjectType.Name)
            {
                case "Recipe":

                    Recipe recipe = JsonConvert.DeserializeObject<Recipe>(syncIssue.SerializedEntityObject);
                    var recipeService = new RecipeService(_syncConfiguration.PostgreSqlConnectionStrings[syncIssue.PlcDeviceId]);

                    switch (syncIssue.SyncDBCommand)
                    {
                        case SyncDBCommand.Insert:
                            commandResult = recipeService.InsertOrUpdate(recipe);
                            break;
                        case SyncDBCommand.Update:
                            commandResult = recipeService.Update(recipe);
                            break;
                        default:
                            break;
                    }

                    break;
                case "RecipeGroup":

                    RecipeGroup recipeGroup = JsonConvert.DeserializeObject<RecipeGroup>(syncIssue.SerializedEntityObject);
                    var recipeGroupService = new RecipeGroupService(_syncConfiguration.PostgreSqlConnectionStrings[syncIssue.PlcDeviceId]);

                    switch (syncIssue.SyncDBCommand)
                    {
                        case SyncDBCommand.Delete:
                            commandResult = recipeGroupService.Delete(recipeGroup);
                            break;
                        case SyncDBCommand.Insert:
                            commandResult = recipeGroupService.InsertOrUpdate(recipeGroup);
                            break;
                        case SyncDBCommand.Update:
                            commandResult = recipeGroupService.Update(recipeGroup);
                            break;
                    }

                    break;

                case "RecipeDetail":

                    RecipeDetail recipeDetail;
                    List<RecipeDetail> recipeDetails;

                    var recipeDetailService = new RecipeDetailService(_syncConfiguration.PostgreSqlConnectionStrings[syncIssue.PlcDeviceId]);

                    switch (syncIssue.SyncDBCommand)
                    {
                        case SyncDBCommand.Delete:
                            recipeDetail = JsonConvert.DeserializeObject<RecipeDetail>(syncIssue.SerializedEntityObject);
                            commandResult = recipeDetailService.Delete(recipeDetail);
                            break;
                        case SyncDBCommand.Insert:
                            recipeDetail = JsonConvert.DeserializeObject<RecipeDetail>(syncIssue.SerializedEntityObject);
                            commandResult = recipeDetailService.Insert(recipeDetail, false);
                            break;
                        case SyncDBCommand.InsertMany:
                            recipeDetails = JsonConvert.DeserializeObject<List<RecipeDetail>>(syncIssue.SerializedEntityObject);
                            commandResult = recipeDetailService.InsertOrUpdateMany(recipeDetails);
                            break;
                        case SyncDBCommand.Update:
                            recipeDetail = JsonConvert.DeserializeObject<RecipeDetail>(syncIssue.SerializedEntityObject);
                            commandResult = recipeDetailService.Update(recipeDetail);
                            break;
                    }

                    break;

                //for load number
                case "ApplicationProperty":

                    ApplicationProperty applicationProperty = JsonConvert.DeserializeObject<ApplicationProperty>(syncIssue.SerializedEntityObject);
                    var applicationPropertyService = new ApplicationPropertyService(_syncConfiguration.PostgreSqlConnectionStrings[syncIssue.PlcDeviceId]);

                    switch (syncIssue.SyncDBCommand)
                    {
                        case SyncDBCommand.Update:
                            commandResult = applicationPropertyService.Update(applicationProperty);
                            break;
                    }

                    break;

                case "Batch":

                    Batch batch = JsonConvert.DeserializeObject<Batch>(syncIssue.SerializedEntityObject);
                    var batchService = new BatchService(_syncConfiguration.PostgreSqlConnectionStrings[syncIssue.PlcDeviceId]);

                    switch (syncIssue.SyncDBCommand)
                    {
                        case SyncDBCommand.Delete:
                            commandResult = batchService.Delete(batch);
                            break;
                        case SyncDBCommand.Insert:
                            commandResult = batchService.InsertOrUpdate(batch);
                            break;
                        case SyncDBCommand.Update:
                            commandResult = batchService.Update(batch);
                            break;
                    }

                    break;

                case "Bag":

                    Bag bag = JsonConvert.DeserializeObject<Bag>(syncIssue.SerializedEntityObject);
                    var bagService = new BagService(_syncConfiguration.PostgreSqlConnectionStrings[syncIssue.PlcDeviceId]);

                    switch (syncIssue.SyncDBCommand)
                    {
                        case SyncDBCommand.Delete:
                            commandResult = bagService.Delete(bag);
                            break;
                        case SyncDBCommand.Insert:
                            commandResult = bagService.InsertOrUpdate(bag);
                            break;
                        case SyncDBCommand.Update:
                            commandResult = bagService.Update(bag);
                            break;
                    }

                    break;

                case "LotProperty":

                    LotProperty lotProperty = JsonConvert.DeserializeObject<LotProperty>(syncIssue.SerializedEntityObject);
                    var lotPropertyService = new LotPropertyService(_syncConfiguration.PostgreSqlConnectionStrings[syncIssue.PlcDeviceId]);

                    switch (syncIssue.SyncDBCommand)
                    {
                        case SyncDBCommand.Delete:
                            commandResult = lotPropertyService.Delete(lotProperty);
                            break;
                        case SyncDBCommand.Insert:
                            commandResult = lotPropertyService.InsertOrUpdate(lotProperty);
                            break;
                        case SyncDBCommand.Update:
                            commandResult = lotPropertyService.Update(lotProperty);
                            break;
                    }

                    break;

                case "CurrentProcessInfo":
                    CurrentProcessInfo currentProcessInfo = JsonConvert.DeserializeObject<CurrentProcessInfo>(syncIssue.SerializedEntityObject);
                    var currentProcessInfoService = new CurrentProcessInfoService(_syncConfiguration.PostgreSqlConnectionStrings[syncIssue.PlcDeviceId]);

                    switch (syncIssue.SyncDBCommand)
                    {
                        case SyncDBCommand.Update:
                            commandResult = currentProcessInfoService.Update(currentProcessInfo);
                            break;
                    }

                    break;

                case "ActiveTag":

                    ActiveTag activeTag = JsonConvert.DeserializeObject<ActiveTag>(syncIssue.SerializedEntityObject);
                    var activeTagService = new ActiveTagService(_syncConfiguration.PostgreSqlConnectionStrings[syncIssue.PlcDeviceId]);

                    switch (syncIssue.SyncDBCommand)
                    {
                        case SyncDBCommand.Update:
                            commandResult = activeTagService.Update(activeTag);
                            break;
                    }
                    break;

                case "ProcessEventLog":
                    ProcessEventLog processEventLog = JsonConvert.DeserializeObject<ProcessEventLog>(syncIssue.SerializedEntityObject);
                    var processEventLogService = new ProcessEventLogService(_syncConfiguration.PostgreSqlConnectionStrings[syncIssue.PlcDeviceId]);

                    switch (syncIssue.SyncDBCommand)
                    {
                        case SyncDBCommand.Insert:
                            commandResult = processEventLogService.Insert(processEventLog);
                            break;
                    }
                    break;
            }
            return commandResult;
        }
    }
}