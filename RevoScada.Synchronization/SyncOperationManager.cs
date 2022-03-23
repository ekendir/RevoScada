using Revo.Core;
using Revo.ServiceUtilities;
using RevoScada.Synchronization.Enums;
using RevoScada.Synchronization.Types;
using RevoScada.Business;
using RevoScada.Entities;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.Configuration.Service;
using RevoScada.Entities.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RevoScada.Synchronization
{
    public class SyncOperationManager
    {
        private readonly SyncStateManager _syncStateManager;
        private readonly SyncDataManager _syncDataManager;
        private readonly SyncConfiguration _syncConfiguration;
        private readonly SyncIssueManager _syncIssueManager;

        /// <summary>
        /// Remote check done once after restart of service. once it complated it neednt to sync server issue related pc
        /// </summary>
        private bool _isRemoteCheckCompleted = false;

        public SyncOperationManager(SyncConfiguration syncConfiguration, Dictionary<int, SiemensPlcConfig> plcConfigs)
        {
            _syncConfiguration = syncConfiguration;

            _syncStateManager = new SyncStateManager(syncConfiguration);
            _syncStateManager.PlcConfigs = plcConfigs;

            _syncDataManager = new SyncDataManager(syncConfiguration);

            syncConfiguration.RemoteRedisServer = syncConfiguration.WorkingEnvironment == WorkingEnvironment.server ? string.Empty :syncConfiguration.RemoteRedisServer;
            _syncIssueManager = new SyncIssueManager(syncConfiguration.RedisServer,syncConfiguration.RemoteRedisServer);
           
        }

        public ConcurrentDictionary<int, int> PingFailures;
        public Dictionary<int, SiemensPlcConfig> PlcConfigs { get; set; }

        public void CheckStateAndSynchronizeBulkDataForPC(SiemensPlcConfig plcConfigForPC)
        {
            try
            {
                SyncItem syncItemLocalPC;
                bool plcAccessCheckResult = NetworkChecker.PingSucceeded(plcConfigForPC.Ip, 10);
                LogManager.Instance.Log($"{plcConfigForPC.Ip} Plc access  check result Is succeeded: {plcAccessCheckResult}", LogType.Information);

                if (plcAccessCheckResult)
                {
                    var currentProcessInfoFromCache = _syncDataManager.GetCurrentProcessInfo(plcConfigForPC.PlcDeviceId);
                    syncItemLocalPC = _syncStateManager.GetSyncItemFromPLC(plcConfigForPC.PlcDeviceId, isServer: false);
                    syncItemLocalPC.BatchId = currentProcessInfoFromCache.BatchId;
                    syncItemLocalPC.MachineId = _syncConfiguration.MachineId;
                    _syncStateManager.SetSyncItemToPLC(syncItemLocalPC, false);

                    if (PingFailures.ContainsKey(plcConfigForPC.PlcDeviceId))
                    {
                       if (PingFailures[plcConfigForPC.PlcDeviceId] > 0)
                        {
                            LogManager.Instance.Log($"Network connection established for {plcConfigForPC.Ip} PLC{plcConfigForPC.PlcDeviceId}", LogType.Information);
                        }
                    }

                    PingFailures[plcConfigForPC.PlcDeviceId] = 0;
                    bool remoteServerCheckResult = NetworkChecker.PingSucceeded(_syncConfiguration.ScadaServer, 5);
                    LogManager.Instance.Log($"{_syncConfiguration.ScadaServer} Scada server access check result Is succeeded: {remoteServerCheckResult}", LogType.Information);

                    if (remoteServerCheckResult)
                    {
                        syncItemLocalPC = _syncStateManager.GetSyncItemFromPLC(plcConfigForPC.PlcDeviceId, isServer: false);
                        syncItemLocalPC.LastAccessDateToRemote = DateTime.Now;
                        syncItemLocalPC.SyncItemStatus = SyncStatus.Stable;
                        syncItemLocalPC.MachineId = _syncConfiguration.MachineId;
                        WriteResult writeResult = _syncStateManager.SetSyncItemToPLC(syncItemLocalPC, isServer: false, logSetResult: false);

                        if (writeResult.IsSucceeded)
                        {
                            SyncItem syncItemCheckResultTemp = _syncStateManager.CheckUsagePriority(plcConfigForPC.PlcDeviceId, false);

                            /// sync stuation occurs
                            if (syncItemCheckResultTemp != null && syncItemCheckResultTemp.UsagePriority != syncItemLocalPC.UsagePriority)
                            {
                                syncItemLocalPC = syncItemCheckResultTemp;
                                SyncBulkDataForPC(syncItemLocalPC);
                            }
                        }
                    }
                    else
                    {
                        LogManager.Instance.Log($"==> FIX IT !!! Remote server access error! IP: {_syncConfiguration.ScadaServer}", LogType.Fatal);
                    }
                }
                else
                {
                    PingFailures[plcConfigForPC.PlcDeviceId]++;
                    if (PingFailures[plcConfigForPC.PlcDeviceId] > 100)
                    {
                        bool remoteServerCheckResult = NetworkChecker.PingSucceeded(_syncConfiguration.ScadaServer, 30);
                        LogManager.Instance.Log($"==> FIX IT !!! PLC access error! IP: {plcConfigForPC.Ip}", LogType.Fatal);
                        LogManager.Instance.Log($"PC to Server Access for IP: {_syncConfiguration.ScadaServer} Is Successfull:{remoteServerCheckResult}", LogType.Error);
                        Thread.Sleep(2000);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log($"Error in CheckStateAndSynchronizeBulkDataForPC. Detail: {ex.Message}", LogType.Fatal);
            }
        }

        public void SyncBulkDataForPC(SyncItem syncItemLocalPC)
        {
            WriteResult writeResult;
            try
            {
                SyncDataManager syncDataManager = new SyncDataManager(_syncConfiguration);
                bool requestResult = syncDataManager.RequestMissingData(FromToDirection.PCtoServer, TransferType.MissingBatchDataWithDataLogs, syncItemLocalPC.PlcDeviceId);
                syncItemLocalPC.SyncItemStatus = (requestResult) ? SyncStatus.Pending : syncItemLocalPC.SyncItemStatus;
                _syncStateManager.SetSyncItemToPLC(syncItemLocalPC, false);

                Thread.Sleep(5000);

                if (requestResult)
                {
                    if (syncItemLocalPC.SyncItemStatus == SyncStatus.Pending)
                    {

                        byte tryAmount = 4;
                        do
                        {
                            if (tryAmount == 0)
                            {
                                break;
                            }
                            tryAmount--;

                            try
                            {
                                MissingBulkDataHeader missingBulkDataHeader = syncDataManager.GetMissingBulkDataHeader(TransferType.MissingBatchDataWithDataLogs, syncItemLocalPC.PlcDeviceId);

                                if (missingBulkDataHeader != null)
                                {
                                    switch (missingBulkDataHeader.SyncState)
                                    {
                                        case SyncDataTransferState.NotStarted:
                                            new LogFormatter().LogObject(missingBulkDataHeader, $"Bulk Data Operation {missingBulkDataHeader.CachedKey}");
                                            break;
                                        case SyncDataTransferState.PreparingData:
                                            new LogFormatter().LogObject(missingBulkDataHeader, $"Bulk Data Operation {missingBulkDataHeader.CachedKey}");
                                            break;
                                        case SyncDataTransferState.ReadyToFetch:

                                            LogManager.Instance.Log($" PLC Device  {missingBulkDataHeader.PlcDeviceId} changed its state!", LogType.Information);

                                            bool writeMissingBulkDataToLocalDBResult = syncDataManager.WriteMissingBulkDataToLocalDB(missingBulkDataHeader);

                                            missingBulkDataHeader.SyncState = writeMissingBulkDataToLocalDBResult ? SyncDataTransferState.FetchCompleted : missingBulkDataHeader.SyncState;

                                            bool saveMissingBulkDataHeaderResult = syncDataManager.SaveMissingBulkDataHeader(missingBulkDataHeader);

                                            if (saveMissingBulkDataHeaderResult)
                                            {
                                                syncItemLocalPC.SyncItemStatus = SyncStatus.Stable;

                                                writeResult = _syncStateManager.SetSyncItemToPLC(syncItemLocalPC, false);

                                                if (writeResult.IsSucceeded)
                                                {
                                                    LogManager.Instance.Log($" {missingBulkDataHeader.MissingBulkDataKey} bulk copy operation completed successfully!", LogType.Information);
                                                    tryAmount = 0;
                                                    break;
                                                }
                                                else
                                                {
                                                    LogManager.Instance.Log($" {missingBulkDataHeader.MissingBulkDataKey} bulk copy operation failed!", LogType.Information);
                                                }
                                            }
                                            break;
                                    }
                                }
                                _syncStateManager.SetSyncItemToPLC(syncItemLocalPC, false);
                                Thread.Sleep(2000);
                            }
                            catch (Exception ex)
                            {
                                LogManager.Instance.Log($"Error in CheckStateAndSynchronizeBulkDataForPC! Detail:{ex}", LogType.Information);
                                Thread.Sleep(1000);
                            }
                        } while (true);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log($"Error in CheckStateAndSynchronizeBulkDataForPC! Detail:{ex}", LogType.Information);
                Thread.Sleep(1000);
            }
        }

        public void CheckStateForServer()
        {

            foreach (var plcConfigItem in PlcConfigs)
            {
                try
                {
                    bool plcNetworkCheckResult = NetworkChecker.PingSucceeded(plcConfigItem.Value.Ip);
                    bool pcAccessCheckResult = NetworkChecker.PingSucceeded(_syncConfiguration.RemoteComputers[plcConfigItem.Key]);

                    SyncItem syncItemRemotePc = new SyncItem();
                    SyncItem syncItemServer = new SyncItem();

                    if (plcNetworkCheckResult)
                    {
                        if (PingFailures.ContainsKey(plcConfigItem.Value.PlcDeviceId))
                        {
                            if (PingFailures[plcConfigItem.Value.PlcDeviceId] > 0)
                            {
                                LogManager.Instance.Log($"Network connection established for {plcConfigItem.Value.Ip} PLC{plcConfigItem.Value.PlcDeviceId}", LogType.Information);
                            }
                        }

                        PingFailures[plcConfigItem.Value.PlcDeviceId] = 0;

                        var currentProcessInfoFromCache = _syncDataManager.GetCurrentProcessInfo(plcConfigItem.Value.PlcDeviceId);

                        if (currentProcessInfoFromCache != null)
                        {
                            syncItemServer = _syncStateManager.GetSyncItemFromPLC(plcConfigItem.Value.PlcDeviceId, isServer: true);
                            syncItemServer.MachineId = _syncConfiguration.MachineId;
                            syncItemServer.LastAccessDateToRemote = pcAccessCheckResult ? DateTime.Now : syncItemServer.LastAccessDateToRemote;
                            syncItemServer.BatchId = currentProcessInfoFromCache.BatchId;
                            WriteResult writeResult = _syncStateManager.SetSyncItemToPLC(syncItemServer, isServer: true, logSetResult: false);

                            if (writeResult.IsSucceeded)
                            {
                                SyncItem syncItemBeforeCheck = (SyncItem)syncItemServer.Clone();
                                SyncItem checkUsagePriorityResult = _syncStateManager.CheckUsagePriority(plcConfigItem.Value.PlcDeviceId, isServer: true);

                                if (syncItemBeforeCheck.UsagePriority == UsagePriority.Slave && checkUsagePriorityResult.UsagePriority == UsagePriority.Master)
                                {
                                    try
                                    {
                                        CurrentProcessInfoService currentProcessInfoService = new CurrentProcessInfoService(_syncConfiguration.PostgreSqlConnectionStrings[plcConfigItem.Key]);
                                        CurrentProcessInfo currentProcessInfo = currentProcessInfoService.Get();
                                        if (currentProcessInfo.BatchId > 0 && (currentProcessInfo.BatchCurrentState == BatchCurrentState.Running || currentProcessInfo.BatchCurrentState == BatchCurrentState.Hold))
                                        {
                                            SyncIssue syncIssue = new SyncIssue
                                            {
                                                FromToDirection = FromToDirection.ServerToPC,
                                                TransferType = TransferType.AfterStart,
                                                SyncStatus = SyncStatus.BatchStartDataPending,
                                                PlcDeviceId = currentProcessInfo.PlcDeviceId,
                                                BatchId = currentProcessInfo.BatchId
                                            };

                                            bool createResult = _syncIssueManager.CreateNewSyncIssue(syncIssue);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        LogManager.Instance.Log($"After priority change issue for afterstart couldn't be created! PLC{plcConfigItem.Key}. Detail:{ex.Message} ", LogType.Fatal);
                                    }
                                }

                            }
                        }
                        else
                        {
                            throw new Exception($"CheckStateForServer: Check plc connection! PLC{plcConfigItem.Value.PlcDeviceId} IP:{plcConfigItem.Value.Ip} ");
                        }
                    }
                    else
                    {
                        PingFailures[plcConfigItem.Value.PlcDeviceId]++;
                        if (PingFailures[plcConfigItem.Value.PlcDeviceId] > 100)
                        {
                            LogManager.Instance.Log($"Network Error PLC{plcConfigItem.Value.PlcDeviceId}. Ping Failure Count: {PingFailures[plcConfigItem.Value.PlcDeviceId]}  ", LogType.Error);
                            Thread.Sleep(2000);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.Instance.Log($"Error in CheckStateForServer for PLC {plcConfigItem.Value.PlcDeviceId}! Detail: {ex.Message}", LogType.Fatal);
                }
            }
        }

        public void CheckForSyncIssueForPC()
        {
            var syncIssuesLocal = _syncIssueManager.SyncIssues();

            foreach (var syncIssueItem in syncIssuesLocal.Where(x => x.FromToDirection == FromToDirection.PCtoServer))
            {
                switch (syncIssueItem.SyncStatus)
                {
                    case SyncStatus.NoneProcessChangesPending:

                        bool noneProcessIssueSendResult = _syncDataManager.TransferIssueDataToServer(syncIssueItem);

                        if (noneProcessIssueSendResult)
                        {
                            syncIssueItem.SyncStatus = SyncStatus.NoneProcessChangesReadyToFetch;
                            _syncIssueManager.UpdateSyncIssue(syncIssueItem);
                        }

                        break;

                    case SyncStatus.BatchStartDataPending:
                        bool sendBatchInfoPCtoServerAfterStartResult = _syncDataManager.SendSingleBatchDataPCtoServerAfterStart(plcDeviceId: syncIssueItem.PlcDeviceId, batchId: syncIssueItem.BatchId);

                        if (sendBatchInfoPCtoServerAfterStartResult)
                        {
                            syncIssueItem.SyncStatus = SyncStatus.BatchStartDataReadytoFetch;
                            _syncIssueManager.UpdateSyncIssue(syncIssueItem);
                        }
                        break;
                    case SyncStatus.BatchHoldDataPending:
                        bool sendBatchInfoPCtoServerAfterHoldResult = _syncDataManager.SendSingleBatchDataPCtoServerAfterHold(plcDeviceId: syncIssueItem.PlcDeviceId, batchId: syncIssueItem.BatchId);
                        if (sendBatchInfoPCtoServerAfterHoldResult)
                        {
                            syncIssueItem.SyncStatus = SyncStatus.BatchHoldDataReadytoFetch;
                            _syncIssueManager.UpdateSyncIssue(syncIssueItem);
                        }
                        break;
                    case SyncStatus.BatchFinishDataPending:
                        bool sendBatchInfoPCtoServerAfterFinishResult = _syncDataManager.SendSingleBatchDataPCtoServerAfterFinish(plcDeviceId: syncIssueItem.PlcDeviceId, batchId: syncIssueItem.BatchId);

                        if (sendBatchInfoPCtoServerAfterFinishResult)
                        {
                            syncIssueItem.SyncStatus = SyncStatus.BatchFinishDataReadytoFetch;
                            _syncIssueManager.UpdateSyncIssue(syncIssueItem);
                        }
                        break;
                    case SyncStatus.BatchStartDataReadytoFetch:
                    case SyncStatus.BatchFinishDataReadytoFetch:
                    case SyncStatus.BatchHoldDataReadytoFetch:
                    case SyncStatus.NoneProcessChangesReadyToFetch:
                    case SyncStatus.Completed:
                        var syncItem = _syncStateManager.GetSyncItemFromPLC(syncIssueItem.PlcDeviceId, isServer: false);
                        syncItem.SyncItemStatus = SyncStatus.Stable;
                        _syncStateManager.SetSyncItemToPLC(syncItem, false);
                        _syncIssueManager.DeleteSyncIssue(syncIssueItem);
                        break;

                }
            }

            List<SyncIssue> syncIssuesRemote = new List<SyncIssue>();


            if (!_isRemoteCheckCompleted)
            {
                try
                {
                    byte tryAmount = 10;
                    do
                    {
                        tryAmount--;

                        try
                        {
                            syncIssuesRemote = _syncIssueManager.RemoteSyncIssues();
                            break;
                        }
                        catch(Exception ex)
                        {
                            LogManager.Instance.Log($"Remote Connection Error: couldnt connect to redis server. IP: {_syncConfiguration.RemoteRedisServer}   {tryAmount} try amount left! Message: {ex.Message}", LogType.Error);
                            Thread.Sleep(1000);
                            if (tryAmount == 0)
                            {
                                throw;
                            }
                        }

                    } while (tryAmount>0);

                    LogFormatter logFormatter = new LogFormatter();

                    foreach (var syncIssueItem in syncIssuesRemote.Where(x => x.FromToDirection == FromToDirection.ServerToPC && x.TransferType == TransferType.NonProcessChanges))
                    {
                        bool commandResult = false;

                        switch (syncIssueItem.SyncStatus)
                        {
                            case SyncStatus.NoneProcessChangesPending:

                                commandResult = _syncDataManager.SaveNonProcessEntityToDB(syncIssueItem);

                                if (commandResult)
                                {
                                    syncIssueItem.SyncStatus = SyncStatus.Completed;
                                    _syncIssueManager.UpdateRemoteSyncIssue(syncIssueItem);
                                    logFormatter.LogObject(syncIssueItem.SerializedEntityObject, "Non Process Entity Sync");
                                }

                                break;
                        }
                    }

                    _isRemoteCheckCompleted = true;


                }
                catch (Exception ex)
                {
                    LogManager.Instance.Log($"FIX:  Remote SyncIssue couldnt connect to redis server. \n" +
                                            $"> Possible Solutions\n" +
                                            $">> Check access to IP: {_syncConfiguration.RemoteRedisServer}\n" +
                                            $">> Check {_syncConfiguration.RemoteRedisServer} server connected to network\n" +
                                            $">> Check whether pc ip is static\n\n"+
                                            $"Error Message{ex.Message}\n"
                                            , LogType.Fatal);
                    throw;
                }

            
            }



        }

        public void CheckForSyncIssueForServer()
        {
            List<SyncIssue> syncIssues = null;

            if (_syncConfiguration.IsSyncActive)
            {
                syncIssues =_syncIssueManager.SyncIssues();
            }
            else
            {
                _syncIssueManager.DeleteAllSyncIssues();
                return;
            }

            foreach (var syncIssueItem in syncIssues.Where(x => x.FromToDirection == FromToDirection.ServerToPC))
            {

                switch (syncIssueItem.SyncStatus)
                {

                    //todo:m refactor with interfaces 
                    case SyncStatus.Completed:
                        var syncItem = _syncStateManager.GetSyncItemFromPLC(syncIssueItem.PlcDeviceId, isServer: true);
                        syncItem.SyncItemStatus = SyncStatus.Stable;
                        _syncStateManager.SetSyncItemToPLC(syncItem, true);
                        _syncIssueManager.DeleteSyncIssue(syncIssueItem);
                        break;
                }
            }

            LogFormatter logFormatter = new LogFormatter();

            foreach (var syncIssueItem in syncIssues.Where(x => x.FromToDirection == FromToDirection.PCtoServer))
            {
                bool commandResult = false;

                switch (syncIssueItem.SyncStatus)
                {

                    //todo:m refactor with interfaces 
                    case SyncStatus.NoneProcessChangesPending:
                        try
                        {
                            commandResult = _syncDataManager.SaveNonProcessEntityToDB(syncIssueItem);

                            if (commandResult)
                            {
                                syncIssueItem.SyncStatus = SyncStatus.Completed;
                                _syncIssueManager.UpdateSyncIssue(syncIssueItem);
                                logFormatter.LogObject(syncIssueItem.SerializedEntityObject, "Non Process Entity Sync");
                            }
                        }
                        catch (Exception ex)
                        {
                            logFormatter.LogObject(syncIssueItem.SerializedEntityObject, $"Non Process Entity Sync Error! (exception:{ex.Message})");
                        }
                     

                        break;
                    case SyncStatus.Completed:

                        try
                        {
                            var syncItem = _syncStateManager.GetSyncItemFromPLC(syncIssueItem.PlcDeviceId, isServer: true);
                            syncItem.SyncItemStatus = SyncStatus.Stable;
                            _syncStateManager.SetSyncItemToPLC(syncItem, true);
                            _syncIssueManager.DeleteSyncIssue(syncIssueItem);
                        }
                        catch (Exception ex)
                        {
                            logFormatter.LogObject(syncIssueItem.SerializedEntityObject, $"CheckForSyncIssueForServer Completed Error! (exception:{ex.Message})");
                        }
                       
                        break;
                }
            }
        }

        public void CheckForNewBatchInfo()
        {
            _ = _syncDataManager.CheckSingleBatchDataForServer();
        }

        /// <summary>
        ///  Cached SyncItems are used for accessing from other devices. 
        /// </summary>
        public void RefreshPLCSyncItemstoCache()
        {
            _syncStateManager.RefreshPLCSyncItemstoCache();
        }

        public void CheckForMissingBulkDataHeaderRequest()
        {
            foreach (var plcConfigItem in PlcConfigs)
            {
                try
                {

                    var missingBulkDataHeader = _syncDataManager.GetMissingBulkDataHeader(TransferType.MissingBatchDataWithDataLogs, plcConfigItem.Value.PlcDeviceId);

                    if (missingBulkDataHeader != null && missingBulkDataHeader.SyncState == SyncDataTransferState.NotStarted)
                    {
                        _ = _syncDataManager.TransferMissingBulkDataFromDBtoCache(TransferType.MissingBatchDataWithDataLogs, plcConfigItem.Value.PlcDeviceId);
                    }
                    if (missingBulkDataHeader != null && missingBulkDataHeader.SyncState == SyncDataTransferState.FetchCompleted)
                    {
                        _ = _syncDataManager.DeleteMissingBulkDataHeaderFromServer(missingBulkDataHeader);
                    }


                }
                catch (Exception ex)
                {
                    LogManager.Instance.Log($"Error in CheckStateForServer for PLC {plcConfigItem.Value.PlcDeviceId}! Detail: {ex.Message}", LogType.Fatal);
                }
            }

        }
    }

}
