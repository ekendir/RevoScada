using Newtonsoft.Json;
using Revo.Core;
using Revo.ServiceUtilities;
using RevoScada.Cache;
using RevoScada.PlcAccess;
using RevoScada.PlcConnection.Siemens;
using RevoScada.ProcessController;
using RevoScada.Synchronization;
using RevoScada.Synchronization.Enums;
using RevoScada.Synchronization.Types;
using RevoScada.Configurator;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.Configuration.Service;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RevoScada.SynchronizationService
{
    class OperationCycle
    {
        private readonly ManualResetEvent _stopEvent = new ManualResetEvent(false);
        private ConcurrentDictionary<int, int> _pingFailures;
        private SyncStateManager _syncStateManager;

        /// <summary>
        /// Initialize cycle cache values before run
        /// </summary>
        public void InitializeCycle()
        {
            try
            {
                _syncStateManager = new SyncStateManager(SyncServiceConfigurations.Instance.SyncConfiguration);
                _syncStateManager.PlcConfigs = PlcConfigs;

                SingleWriteConnectionManager.Instance.SiemensPlcConfigs = PlcConfigs.Values.ToList();
                SingleReadConnectionManager.Instance.SiemensPlcConfigs = PlcConfigs.Values.ToList();
                SingleWriteConnectionManager.Instance.InitializeConnections(10);
                SingleReadConnectionManager.Instance.InitializeConnections(10);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dictionary<int, SiemensPlcConfig> PlcConfigs
        {
            get
            {
                return ((IEnumerable<SiemensPlcConfig>)SyncServiceConfigurations.Instance.PlcConfigs).ToDictionary(x => x.PlcDeviceId, x => x);
            }
        }

        
        /// <summary>
        /// Infinite cycle with wait time. No need to parallel so it has long cycle duration.
        /// </summary>
        public void RunCycle()
        {
            SyncOperationManager syncOperationManager = new SyncOperationManager(SyncServiceConfigurations.Instance.SyncConfiguration, PlcConfigs);
            SyncItem syncItem;
            switch (SyncServiceConfigurations.Instance.SyncConfiguration.WorkingEnvironment)
            {
                case WorkingEnvironment.pc:

                    var plcConfigForPC = PlcConfigs.Values.First();

                    do
                    {

                        if (!NetworkChecker.PingSucceeded(SyncServiceConfigurations.Instance.SyncConfiguration.ScadaServer))
                        {
                            Thread.Sleep(5000);
                            continue;
                        }


                        syncOperationManager.PingFailures = new ConcurrentDictionary<int, int>();
                        syncOperationManager.PingFailures[plcConfigForPC.PlcDeviceId] = 0;
                        syncItem = _syncStateManager.GetSyncItemFromPLC(plcConfigForPC.PlcDeviceId, isServer: false);

                        if (syncItem == null)
                        {
                            syncItem = new SyncItem
                            {
                                MachineId = SyncServiceConfigurations.Instance.SyncConfiguration.MachineId,
                                PlcDeviceId = plcConfigForPC.PlcDeviceId,
                                UsagePriority = UsagePriority.Master,
                                BatchId = -1,
                                LastAccessDateToRemote = DateTime.Now,
                                LastAccessDateToPLC = DateTime.Now,
                                SyncItemStatus = SyncStatus.Stable,
                            };
                        }
                        else
                        {
                            syncItem.MachineId = SyncServiceConfigurations.Instance.SyncConfiguration.MachineId;
                        }

                        try
                        {
                            _syncStateManager.SetSyncItemToPLC(syncItem, isServer: false);
                            break;
                        }
                        catch (Exception)
                        {
                            LogManager.Instance.Log($"PLC Access error. Check cable or network access for PLC{plcConfigForPC.PlcDeviceId}  IP:{plcConfigForPC.Ip}", LogType.Information);
                            Thread.Sleep(1000);
                            continue;
                        }
                    } while (true);
                  
                    do
                    {
                        syncOperationManager.CheckForSyncIssueForPC();
                        syncOperationManager.CheckStateAndSynchronizeBulkDataForPC(plcConfigForPC);
                        syncOperationManager.RefreshPLCSyncItemstoCache();
                    }
                    while (!_stopEvent.WaitOne(SyncServiceConfigurations.Instance.SyncConfiguration.SyncCycleTimeInSeconds));


                       break;
                case WorkingEnvironment.server:
                    syncOperationManager.PingFailures = new ConcurrentDictionary<int, int>();

                    //reset connection error count
                    foreach (var plcConfigItem in PlcConfigs.Values)
                    {
                        syncOperationManager.PingFailures[plcConfigItem.PlcDeviceId] = 0;

                        if (NetworkChecker.PingSucceeded(plcConfigItem.Ip))
                        {

                            syncItem = _syncStateManager.GetSyncItemFromPLC(plcConfigItem.PlcDeviceId, isServer: true);

                            if (syncItem == null)
                            {
                                // bu kısımın oluşturduğu düzensizliğe bakılacak.
                                syncItem = new SyncItem
                                {
                                    MachineId = SyncServiceConfigurations.Instance.SyncConfiguration.MachineId,
                                    PlcDeviceId = plcConfigItem.PlcDeviceId,
                                    UsagePriority = UsagePriority.Slave,
                                    BatchId = -1,
                                    LastAccessDateToRemote = DateTime.Now,
                                    SyncItemStatus = SyncStatus.Stable,
                                };
                                _syncStateManager.SetSyncItemToPLC(syncItem, isServer: true);
                            }
                            else
                            {
                                syncItem.MachineId = SyncServiceConfigurations.Instance.SyncConfiguration.MachineId;
                                syncItem.SyncItemStatus = SyncStatus.Stable;
                                _syncStateManager.SetSyncItemToPLC(syncItem, isServer: true);
                            }
                        }
                        else
                        {
                            LogManager.Instance.Log($"PLC{plcConfigItem.PlcDeviceId} connection failure before run!  IP:{plcConfigItem.Ip} ", LogType.Error);
                        }
                    }

                        syncOperationManager.PlcConfigs = PlcConfigs;

                    do
                    {
                        syncOperationManager.CheckForSyncIssueForServer();
                        syncOperationManager.CheckForNewBatchInfo();
                        syncOperationManager.CheckStateForServer();
                        syncOperationManager.CheckForMissingBulkDataHeaderRequest();
                        syncOperationManager.RefreshPLCSyncItemstoCache();
                    }
                    while (!_stopEvent.WaitOne(SyncServiceConfigurations.Instance.SyncConfiguration.SyncCycleTimeInSeconds));

                    break;
            }
        }

        /// <summary>
        /// It aborts ManualResetEvent infinite cycle.
        /// </summary>
        public void AbortCycle()
        {
            _stopEvent.Set();
        }
    }
}
