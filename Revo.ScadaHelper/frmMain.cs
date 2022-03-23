using Newtonsoft.Json;
using Revo.Core;
using RevoScada.Cache;
using RevoScada.PlcConnection.Siemens;
using RevoScada.Synchronization;
using RevoScada.Synchronization.Enums;
using RevoScada.Synchronization.Types;
using RevoScada.TAI.Configurator;
using RevoScada.TAI.Entities.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Revo.ScadaHelper
{
    public partial class frmMain : Form
    {
        const string syncconfigpath = @"C:\RevoScada.TAI.Files\Configuration\SingleConfigurations\SyncService.rsconfig";

        public frmMain()
        {
            InitializeComponent();
        }

        private async void btnClearCache_Click(object sender, EventArgs e)
        {
            
            List<string> allKeys = new List<string>();

            bool result = await Task.Run(() =>
            {
                CacheManager _cacheManager;
                List<string> keys;

                _cacheManager = new CacheManager(CacheDBType.Main, "localhost");
                keys = _cacheManager.GetKeyNames("*");
                allKeys.AddRange(keys);
                keys.ForEach(x => _cacheManager.DeleteKey(x));
                
                _cacheManager = new CacheManager(CacheDBType.ReadService, "localhost");
                keys = _cacheManager.GetKeyNames("*");
                allKeys.AddRange(keys);
                keys.ForEach(x => _cacheManager.DeleteKey(x));

                _cacheManager = new CacheManager(CacheDBType.WriteService, "localhost");
                keys = _cacheManager.GetKeyNames("*");
                allKeys.AddRange(keys);
                keys.ForEach(x => _cacheManager.DeleteKey(x));

                return true;
            });

            string removedKeys = string.Join(Environment.NewLine, allKeys);

            txtOutput.Text = $"{removedKeys} {Environment.NewLine}--------------------{ Environment.NewLine} items removed from cache!";

        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            /*
             konfigrasyon klonlama
postgre sql sıfırlama
load resetleme

             */
            try
            {
                
              
            }
            catch (Exception ex)
            {
                 
            }
 


        }

        private void BtnResetLoadNumber_Click(object sender, EventArgs e)
        {

        }

        private void BtnInitializeSyncState_Click(object sender, EventArgs e)
        {
            try
            {

                SyncServiceConfigurations.Instance.InitializeConfiguration(syncconfigpath);
                LogManager.Instance.InitializeConfiguration(SyncServiceConfigurations.Instance.SyncConfiguration.LogSettings);
                LogManager.Instance.Log("service running...", LogType.Information);

                var siemensPlcConfigs = (List<SiemensPlcConfig>)SyncServiceConfigurations.Instance.PlcConfigs;
                SingleReadConnectionManager.Instance.SiemensPlcConfigs = siemensPlcConfigs;
                SingleReadConnectionManager.Instance.InitializeConnections(10);
                SingleWriteConnectionManager.Instance.SiemensPlcConfigs = siemensPlcConfigs;
                SingleWriteConnectionManager.Instance.InitializeConnections(10);

                int plcDeviceId = Convert.ToInt32(txtPlcDeviceIdInitSync.Text);

            SyncStateManager syncStateManager = new SyncStateManager(SyncServiceConfigurations.Instance.SyncConfiguration);
            SyncItem syncItem;

            
                 
                syncItem = new SyncItem
                {
                    MachineId = SyncServiceConfigurations.Instance.SyncConfiguration.MachineId,
                    PlcDeviceId =plcDeviceId,
                    UsagePriority = UsagePriority.Master,
                    BatchId = -1,
                    LastAccessDateToRemote = DateTime.Now,
                    LastAccessDateToPLC = DateTime.Now,
                    SyncItemStatus = SyncStatus.Stable,
                };


                syncStateManager.SetSyncItemToPLC(syncItem, false);


                syncItem = new SyncItem
                {
                    MachineId = txtServerId.Text,
                    PlcDeviceId = plcDeviceId,
                    UsagePriority = UsagePriority.Slave,
                    BatchId = -1,
                    LastAccessDateToRemote = DateTime.Now,
                    LastAccessDateToPLC = DateTime.Now,
                    SyncItemStatus = SyncStatus.Stable,
                };

                syncStateManager.SetSyncItemToPLC(syncItem, true);

                
              var s1=  syncStateManager.GetSyncItemFromPLC(plcDeviceId, false);
              var s2=  syncStateManager.GetSyncItemFromPLC(plcDeviceId, true);

             
               

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(JsonConvert.SerializeObject(s1, new JsonSerializerSettings { Formatting = Formatting.Indented }));
                stringBuilder.AppendLine(JsonConvert.SerializeObject(s2, new JsonSerializerSettings { Formatting = Formatting.Indented }));

                txtOutput.Text = stringBuilder.ToString();

            }
            catch (Exception)
            {
            }
        }

        private void BtnMonitorPLCSyncItem_Click(object sender, EventArgs e)
        {

            SyncServiceConfigurations.Instance.InitializeConfiguration(syncconfigpath);
            LogManager.Instance.InitializeConfiguration(SyncServiceConfigurations.Instance.SyncConfiguration.LogSettings);
            LogManager.Instance.Log("service running...", LogType.Information);

            var siemensPlcConfigs = (List<SiemensPlcConfig>)SyncServiceConfigurations.Instance.PlcConfigs;
            SingleReadConnectionManager.Instance.SiemensPlcConfigs = siemensPlcConfigs;
            SingleReadConnectionManager.Instance.InitializeConnections(10);
            SingleWriteConnectionManager.Instance.SiemensPlcConfigs = siemensPlcConfigs;
            SingleWriteConnectionManager.Instance.InitializeConnections(10);

            int plcDeviceId = Convert.ToInt32(txtPlcDeviceIdInitSync.Text);
            SyncStateManager syncStateManager = new SyncStateManager(SyncServiceConfigurations.Instance.SyncConfiguration);
            SyncItem syncItem;
        
            
              var s1 = syncStateManager.GetSyncItemFromPLC(plcDeviceId, false);
            var s2 = syncStateManager.GetSyncItemFromPLC(plcDeviceId, true);

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(JsonConvert.SerializeObject(s1, new JsonSerializerSettings { Formatting = Formatting.Indented }));
            stringBuilder.AppendLine(JsonConvert.SerializeObject(s2, new JsonSerializerSettings { Formatting = Formatting.Indented }));

            txtOutput.Text = stringBuilder.ToString();
            
          

         
        }
    }
}