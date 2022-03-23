using NUnit.Framework;
using Revo.Core;
using Revo.Core.Data;
using RevoScada.Cache;
using RevoScada.PlcConnection.Siemens;
using RevoScada.Synchronization.Enums;
using RevoScada.Synchronization.Types;
using RevoScada.Configurator;
using RevoScada.Entities;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.Configuration.Service;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RevoScada.Synchronization.Test
{
    [TestFixture]
    public class SyncTest
    {
        const string syncconfigpath = @"C:\RevoScada.Files\Configuration\MultipleConfigurations\SyncService.rsconfig";
        ConcurrentDictionary<int, int> _pingFailures;

        SyncOperationManager _syncOperationManager;
        SyncStateManager _syncStateManager;
        [SetUp]
        public void Init()
        {
            SyncServiceConfigurations.Instance.InitializeConfiguration(syncconfigpath);
            LogManager.Instance.InitializeConfiguration(SyncServiceConfigurations.Instance.SyncConfiguration.LogSettings);
            LogManager.Instance.Log("service running...", LogType.Information);

            //var siemensPlcConfigs = (List<SiemensPlcConfig>)SyncServiceConfigurations.Instance.PlcConfigs;

            //SingleReadConnectionManager.Instance.SiemensPlcConfigs = siemensPlcConfigs;
            //SingleReadConnectionManager.Instance.InitializeConnections(10);
            //SingleWriteConnectionManager.Instance.SiemensPlcConfigs = siemensPlcConfigs;
            //SingleWriteConnectionManager.Instance.InitializeConnections(10);
            //_pingFailures = new ConcurrentDictionary<int, int>();
            //_syncOperationManager = new SyncOperationManager(null,null);
            //_syncStateManager = new SyncStateManager(SyncServiceConfigurations.Instance.SyncConfiguration);

            //_syncStateManager.PlcConfigs = ((List<SiemensPlcConfig>)SyncServiceConfigurations.Instance.PlcConfigs).ToDictionary(x => x.PlcDeviceId, x => x);
        }

        [Test]
        [TestCase(1, "SRV1")]
        [Ignore("critical")]
        public void Reset_syncItem(int plcDeviceId, string serverMachineId)
        {
            SyncStateManager syncStateManager = new SyncStateManager(SyncServiceConfigurations.Instance.SyncConfiguration);
            SyncItem syncItem1;
            SyncItem syncItem2;

            syncItem1 = new SyncItem
            {
                MachineId = SyncServiceConfigurations.Instance.SyncConfiguration.MachineId,
                PlcDeviceId = plcDeviceId,
                UsagePriority = UsagePriority.Master,
                BatchId = -1,
                LastAccessDateToRemote = DateTime.Now,
                LastAccessDateToPLC = DateTime.Now,
                SyncItemStatus = SyncStatus.Stable,
            };
            syncStateManager.SetSyncItemToPLC(syncItem1, false);

            syncItem2 = new SyncItem
            {
                MachineId = serverMachineId,
                PlcDeviceId = plcDeviceId,
                UsagePriority = UsagePriority.Slave,
                BatchId = -1,
                LastAccessDateToRemote = DateTime.Now,
                LastAccessDateToPLC = DateTime.Now,
                SyncItemStatus = SyncStatus.Stable,
            };
            syncStateManager.SetSyncItemToPLC(syncItem2, true);

            var syncItemRead1 = syncStateManager.GetSyncItemFromPLC(plcDeviceId, false);
            var syncItemRead2 = syncStateManager.GetSyncItemFromPLC(plcDeviceId, true);

            Assert.IsTrue(syncItemRead1.MachineId == syncItem1.MachineId);
            Assert.IsTrue(syncItemRead2.MachineId == syncItem2.MachineId);
            Assert.IsTrue(syncItemRead1.UsagePriority == syncItem1.UsagePriority);
            Assert.IsTrue(syncItemRead2.UsagePriority == syncItem2.UsagePriority);

        }
        
    }
}
