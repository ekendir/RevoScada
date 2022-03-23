using System;
using System.Linq;
using Newtonsoft.Json;
using RevoScada.Cache;
using System.Collections.Generic;
using RevoScada.Synchronization.Types;
using RevoScada.Synchronization.Enums;
using RevoScada.Entities.Configuration.Service;
using RevoScada.Configurator;
using RevoScada.Entities.Configuration;
using RevoScada.PlcAccess;
using RevoScada.Entities.Complex;
using Revo.Core;
using Revo.Core.Data;
using System.Threading;
using System.Threading.Tasks;
using Revo.ServiceUtilities;

namespace RevoScada.Synchronization
{
    public class SyncStateManager
    {
        //private readonly string _connectionString;
        private readonly CacheManager _mainCacheManager;
        private LogFormatter _logFormatter;
        private readonly SyncConfiguration _syncConfiguration;
        public Dictionary<int, SiemensPlcConfig> PlcConfigs { get; set; }

        /// <summary>
        /// PlcDeviceId
        /// </summary>
        /// <param name="connectionStrings"></param>
        /// <param name="plcDeviceId">this parameter can change after initialized</param>
        /// <param name="batchId">this parameter can change after initialized</param>
        /// <param name="mainCacheServer"></param>
        /// <param name="serverCacheServer"></param>
        public SyncStateManager(SyncConfiguration syncConfiguration)
        {
            try
            {
                _syncConfiguration = syncConfiguration;
                _mainCacheManager = new CacheManager(CacheDBType.Main, _syncConfiguration.RedisServer);
                _logFormatter = new LogFormatter();
              
            }
            catch (Exception ex)
            {
                ex.Data["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().ToString();
                throw ex;
            }
        }

        public SyncStateManager(string localCacheServer)
        {
            try
            {
                _mainCacheManager = new CacheManager(CacheDBType.Main, localCacheServer);
            }
            catch (Exception ex)
            {
                ex.Data["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().ToString();
                throw ex;
            }
        }


        // this read and write operations intentionally detached from read-write service in order to not dependent to services while sync breaks services
        public SyncItem GetSyncItemFromPLC(int plcDeviceId, bool isServer)
        {
            SyncItem syncItem;
            var stringManipulation = new StringManipulation();
            string rawPlcSyncTagValue = null;
            try
            {
                SiemensTagConfiguration siemensTagConfiguration = (isServer ? (SiemensTagConfiguration)SyncServiceConfigurations.Instance.ScadaSyncServerStatusTags[plcDeviceId] : (SiemensTagConfiguration)SyncServiceConfigurations.Instance.ScadaSyncPCStatusTags[plcDeviceId]);

                SiemensPlcAccess siemensPlcAccess = new SiemensPlcAccess();
                ReadResult readResult = siemensPlcAccess.ReadDB(plcDeviceId, siemensTagConfiguration, 2);

                rawPlcSyncTagValue = siemensPlcAccess.GetFromReadResult<string>(siemensTagConfiguration, readResult);

                var rawPlcSyncTagValueSplitted = rawPlcSyncTagValue.Split(':').ToList();
                Enum.TryParse(rawPlcSyncTagValueSplitted[2], out UsagePriority usagePriority);
                Enum.TryParse(rawPlcSyncTagValueSplitted[6], out SyncStatus syncItemStatus);

                syncItem = new SyncItem
                {
                    MachineId = rawPlcSyncTagValueSplitted[0],
                    PlcDeviceId = Convert.ToInt32(rawPlcSyncTagValueSplitted[1]),
                    UsagePriority = usagePriority,
                    BatchId = Convert.ToInt32(rawPlcSyncTagValueSplitted[3]),
                    LastAccessDateToRemote = stringManipulation.StringToDateTime(rawPlcSyncTagValueSplitted[4]),
                    LastAccessDateToPLC = stringManipulation.StringToDateTime(rawPlcSyncTagValueSplitted[5]),
                    SyncItemStatus = syncItemStatus
                };
            }
            catch (Exception ex)
            {
                syncItem = null;
                LogManager.Instance.Log($"Get Sync Item From PLC{plcDeviceId}: Scada_Sync_Server_Status: [{(string.IsNullOrEmpty( rawPlcSyncTagValue)?"Empty":rawPlcSyncTagValue)}]", LogType.Error);
            }

            return syncItem;
        }

        public WriteResult SetSyncItemToPLC(SyncItem syncItem, bool isServer, bool logSetResult = true)
        {
            WriteResult writeResult;
            try
            {
                var stringManipulation = new StringManipulation();
                syncItem.LastAccessDateToPLC = DateTime.Now;
                SiemensTagConfiguration siemensTagConfiguration = isServer ? (SiemensTagConfiguration)SyncServiceConfigurations.Instance.ScadaSyncServerStatusTags[syncItem.PlcDeviceId] : (SiemensTagConfiguration)SyncServiceConfigurations.Instance.ScadaSyncPCStatusTags[syncItem.PlcDeviceId];
                string syncItemForPLC = $"{syncItem.MachineId}:{syncItem.PlcDeviceId}:{syncItem.UsagePriority}:{syncItem.BatchId}:{stringManipulation.FormatDateTimeToString(syncItem.LastAccessDateToPLC)}:{stringManipulation.FormatDateTimeToString(syncItem.LastAccessDateToRemote)}:{syncItem.SyncItemStatus}";
                SiemensPlcAccess siemensPlcAccess = new SiemensPlcAccess();
                SiemensWriteCommandItem siemensWriteCommandItem = siemensPlcAccess.GetSiemensWriteCommandItem(siemensTagConfiguration, syncItemForPLC);
                writeResult = siemensPlcAccess.WriteDB(siemensWriteCommandItem, 30);

                if (writeResult.IsSucceeded && logSetResult)
                {
                    _logFormatter.LogSingleSyncItem(syncItem, isServer);
                }
            }
            catch (Exception ex)
            {

                writeResult = new WriteResult
                {
                    S7Result = -1,
                };
                LogManager.Instance.Log($"Set Sync Item to PLC Error! Cannot connect to PLC{syncItem.PlcDeviceId} IP:{PlcConfigs[syncItem.PlcDeviceId].Ip} Detail:{ex.Message}", LogType.Error);
            }
            return writeResult;
        }

        /// <summary>
        /// Determines whether master or not. This method most important part of defining master in workflow
        /// </summary>
        /// <param name="plcDeviceId">plc device or furnace id defined in furnace tag configurations</param>
        /// <param name="isServer">true for server settings.</param>
        /// <returns></returns>
        public SyncItem CheckUsagePriority(int plcDeviceId, bool isServer)
        {
            SyncItem syncItemServer = GetSyncItemFromPLC(plcDeviceId, true);
            SyncItem syncItemServerPrevious = (SyncItem)syncItemServer.Clone();

            SyncItem syncItemLocalReturning = null;
            SyncItem syncItemLocalPC = GetSyncItemFromPLC(plcDeviceId, false);
            SyncItem syncItemLocalPCPrevious;

            if (syncItemLocalPC == null)
            {
                SyncItem firstTimeSavingSyncItemLocalPC = new SyncItem
                {
                    BatchId = syncItemServer.BatchId,
                    PlcDeviceId = syncItemServer.PlcDeviceId,
                    LastAccessDateToPLC = DateTime.MinValue,
                    LastAccessDateToRemote = DateTime.MinValue,
                    MachineId = _syncConfiguration.MachineId,
                    SyncItemStatus = SyncStatus.Stable,
                    UsagePriority = UsagePriority.Slave
                };

                SetSyncItemToPLC(firstTimeSavingSyncItemLocalPC, isServer: false, logSetResult: true);

                syncItemLocalPC = GetSyncItemFromPLC(plcDeviceId, false);
            }

            syncItemLocalPCPrevious = (SyncItem)syncItemLocalPC.Clone();

            bool isLocalPlcAccessSuccessful = ((DateTime.Now).Subtract(syncItemLocalPC?.LastAccessDateToPLC ?? DateTime.MinValue).TotalSeconds) < SyncServiceConfigurations.Instance.SyncConfiguration.TakeOverTimeDifferenceInSeconds;
            bool isServerPlcAccessSuccessful = ((DateTime.Now).Subtract(syncItemServer?.LastAccessDateToPLC ?? DateTime.MinValue).TotalSeconds) < SyncServiceConfigurations.Instance.SyncConfiguration.TakeOverTimeDifferenceInSeconds;

            WriteResult writeResultPC = new WriteResult();
            WriteResult writeResultServer = new WriteResult();

            if (isServer)
            {
                if (isLocalPlcAccessSuccessful == false && isServerPlcAccessSuccessful == true)
                {
                    if (syncItemServer.UsagePriority == UsagePriority.Slave)
                    {
                        // Server is master
                        syncItemServer.UsagePriority = UsagePriority.Master;
                        syncItemServer.SyncItemStatus = SyncStatus.Stable;
                        writeResultServer = SetSyncItemToPLC(syncItemServer, true);

                        //change pc usagepriority
                        syncItemLocalPC.UsagePriority = UsagePriority.Slave;
                        syncItemLocalPC.SyncItemStatus = SyncStatus.Stable;
                        writeResultPC = SetSyncItemToPLC(syncItemLocalPC, false);

                        //todo delete1 if (writeResultServer.IsSucceeded)
                        //{
                        //    _logFormatter.LogChangingPriority(syncItemServer, previousUsagePriority: UsagePriority.Slave);
                        //}

                        //if (writeResultPC.IsSucceeded)
                        //{
                        //    _logFormatter.LogChangingPriority(syncItemLocalPC, previousUsagePriority: UsagePriority.Master);
                        //}
                    }

                    if (syncItemServer.UsagePriority == UsagePriority.Master && syncItemLocalPC.UsagePriority == UsagePriority.Master)
                    {
                        //change pc usagepriority
                        syncItemLocalPC.UsagePriority = UsagePriority.Slave;
                        syncItemLocalPC.SyncItemStatus = SyncStatus.Stable;
                        writeResultPC = SetSyncItemToPLC(syncItemLocalPC, false);

                        if (writeResultServer.IsSucceeded)
                        {
                            _logFormatter.LogChangingPriority(syncItemServer, previousUsagePriority: UsagePriority.Slave);
                        }

                        if (writeResultPC.IsSucceeded)
                        {
                            _logFormatter.LogChangingPriority(syncItemLocalPC, previousUsagePriority: UsagePriority.Master);
                        }
                    }

                    syncItemLocalReturning = (writeResultPC.IsSucceeded && writeResultServer.IsSucceeded) ? syncItemServer : syncItemServerPrevious;
                }

                if (isLocalPlcAccessSuccessful == false && isServerPlcAccessSuccessful == true)
                {
                    if (syncItemServer.UsagePriority == UsagePriority.Master && syncItemLocalPC.UsagePriority == UsagePriority.Master)
                    {
                        //change pc usagepriority
                        syncItemLocalPC.UsagePriority = UsagePriority.Slave;
                        syncItemLocalPC.SyncItemStatus = SyncStatus.Stable;
                        writeResultPC = SetSyncItemToPLC(syncItemLocalPC, false);

                        if (writeResultServer.IsSucceeded)
                        {
                            _logFormatter.LogChangingPriority(syncItemServer, previousUsagePriority: UsagePriority.Slave);
                        }

                        if (writeResultPC.IsSucceeded)
                        {
                            _logFormatter.LogChangingPriority(syncItemLocalPC, previousUsagePriority: UsagePriority.Master);
                        }
                    }

                    syncItemLocalReturning = (writeResultPC.IsSucceeded && writeResultServer.IsSucceeded) ? syncItemServer : syncItemServerPrevious;
                }
            }
            else
            {

                if (isLocalPlcAccessSuccessful == true)
                {
                    // Im master again
                    if (syncItemLocalPC.UsagePriority == UsagePriority.Slave)
                    {
                        syncItemLocalPC.UsagePriority = UsagePriority.Master;
                        syncItemLocalPC.SyncItemStatus = SyncStatus.Pending;
                        writeResultPC = SetSyncItemToPLC(syncItemLocalPC, false);
#if DEBUG
                        _logFormatter.LogChangingPriority(syncItemLocalPC, UsagePriority.Slave);
#endif
                        //change remote
                        syncItemServer.UsagePriority = UsagePriority.Slave;
                        syncItemServer.SyncItemStatus = SyncStatus.Stable;
                        writeResultServer = SetSyncItemToPLC(syncItemServer, true);
                    }

                    if (syncItemLocalPC.UsagePriority == UsagePriority.Master && syncItemServer.UsagePriority == UsagePriority.Master)
                    {
                        syncItemServer.UsagePriority = UsagePriority.Slave;
                        syncItemServer.SyncItemStatus = SyncStatus.Stable;
                        writeResultServer = SetSyncItemToPLC(syncItemServer, true);
#if DEBUG
                        _logFormatter.LogChangingPriority(syncItemServer, UsagePriority.Master);
#endif
                    }
                    syncItemLocalReturning = (writeResultPC.IsSucceeded && writeResultServer.IsSucceeded) ? syncItemLocalPC : syncItemLocalPCPrevious;
                }
            }

            syncItemLocalReturning = syncItemLocalReturning ?? (isServer ? syncItemServerPrevious : syncItemLocalPCPrevious);
            return syncItemLocalReturning;
        }

        // check master or not
        public void RefreshPLCSyncItemstoCache()
        {
            Dictionary<string, SyncItem> syncItems = new Dictionary<string, SyncItem>();

            foreach (var plcConfigItem in PlcConfigs)
            { 
                try
                {
                    if (NetworkChecker.PingSucceeded(plcConfigItem.Value.Ip))
                    {
                        string pcKey = $"SyncItem{WorkingEnvironment.pc}PLC{plcConfigItem.Value.PlcDeviceId}";
                        string serverKey = $"SyncItem{WorkingEnvironment.server}PLC{plcConfigItem.Value.PlcDeviceId}";
                        syncItems[pcKey] = GetSyncItemFromPLC(plcConfigItem.Key, isServer: false);
                        syncItems[serverKey] = GetSyncItemFromPLC(plcConfigItem.Key, isServer: true);
                        Thread.Sleep(200);
                        continue;
                    }  
                }
                catch (Exception ex)
                {
                    LogManager.Instance.Log($"Refresh PLC Sync Status Error: {ex.Message} PLC:{plcConfigItem.Key} IP:{plcConfigItem.Value.Ip}", LogType.Fatal);
                }
            }

            string serialized = JsonConvert.SerializeObject(syncItems);
            _mainCacheManager.Set("CachedSyncItems", serialized, TimeSpan.FromDays(3));
        }

        public Dictionary<string, SyncItem> GetSyncItemsFromCache()
        {
            try
            {
                string serialized = _mainCacheManager.GetString("CachedSyncItems");
                Dictionary<string, SyncItem> keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, SyncItem>>(serialized);
                return keyValuePairs;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool IsValidMaster(int plcDeviceId, WorkingEnvironment workingEnvironment, short checkDurationInSeconds = 90,bool logError=true)
        {
            bool result = false;
            byte tryAmount = 50;
            
            do
            {
                if (tryAmount == 0)
                {
                    break;
                }

                tryAmount--;
                SyncItem syncItem = null;

                try
                {
                    syncItem = GetSyncItemsFromCache()[$"SyncItem{workingEnvironment}PLC{plcDeviceId}"];
                }
                catch (Exception ex)
                {
                    LogManager.Instance.Log($"PLC{plcDeviceId} IsValidMaster check error! Detail :{ex.Message}", LogType.Fatal);
                }


                if (syncItem != null)
                {
                    bool checkForMaster = syncItem.UsagePriority == UsagePriority.Master;
                    bool checkForValidDate = ((DateTime.Now).Subtract(syncItem.LastAccessDateToPLC).TotalSeconds) < checkDurationInSeconds;
                    bool checkForStableState = syncItem.SyncItemStatus == SyncStatus.Stable;
                    result = checkForMaster && checkForValidDate && checkForStableState;
                }
                else
                {
                    result = false;
                }

                Thread.Sleep(200);
                if (result)
                {
                    break;
                }


            } while (result == false);

            if (!result)
            {
                if (logError)
                {
                    LogManager.Instance.Log($"This machine is slave for PLC{plcDeviceId}. IsValidMaster: false", LogType.Fatal);
                }
            }

            return result;
        }
    }
}






























