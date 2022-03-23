using Newtonsoft.Json;
using Revo.Core;
using RevoScada.Cache;
using RevoScada.ProcessController;
using RevoScada.Synchronization;
using RevoScada.Configurator;
using RevoScada.Entities.Configuration.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RevoScada.ProcessManagerService
{
    class OperationCycle 
    {
        private readonly ManualResetEvent _stopEvent = new ManualResetEvent(false);

        private CacheManager _readCacheManager;
        private CacheManager _mainCacheManager;

        /// <summary>
        /// Initialize cycle cache values before run
        /// </summary>
        public void InitializeCycle(ApplicationConfigurations applicationConfigurations)
        {

            
            try
            {
                _readCacheManager = new CacheManager(CacheDBType.ReadService, applicationConfigurations.Configuration.RedisServer);
                _mainCacheManager = new CacheManager(CacheDBType.Main, applicationConfigurations.Configuration.RedisServer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Infinite cycle with wait time. It runs read query for each device concurrently. Beware! Minimum cpu count should be two.
        /// </summary>
        public void RunCycle()
        {
            var configuration = ApplicationConfigurations.Instance.Configuration;
            var plcDevices = ApplicationConfigurations.Instance.Configuration.PlcDevices;
            ProcessManager.Instance.Initialize(configuration, ApplicationConfigurations.Instance.TagConfigurations);
            AlarmManager.Instance.Initialize(configuration);
            SyncStateManager syncStateManager = new SyncStateManager(ApplicationConfigurations.Instance.Configuration.RedisServer);

            while (!_stopEvent.WaitOne(2000))
            {
                //todo:m paralele dönüştür.
                foreach (var device in plcDevices)
                {
                    try
                    {
                        if (syncStateManager.IsValidMaster(device.Value.FurnaceId, configuration.WorkingEnvironment,90,false))
                        {

                            ProcessManager.Instance.InitializeSelectedDevice(device.Value.FurnaceId);
                            AlarmManager.Instance.InitializeSelectedDevice(device.Value.FurnaceId);
                            ProcessManager.Instance.InitializeRunOperationTags();
                            ProcessManager.Instance.CheckRunningProcess();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.Instance.Log($"Error occured in part of cycle plc{device.Value.FurnaceId} Detail:{ex.Message}", LogType.Information);
                    }
                }
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
