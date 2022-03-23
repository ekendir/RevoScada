using Revo.Core;
using Revo.ServiceUtilities;
using RevoScada.Cache;
using RevoScada.PlcConnection.Siemens;
using RevoScada.Synchronization;
using RevoScada.Configurator;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.Configuration.Service;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RevoScada.WriteService
{
    class SiemensWriteCycleStrategy : IPlcWriteCycleStrategy
    {
        private readonly ManualResetEvent _stopEvent = new ManualResetEvent(false);
       
        private ConcurrentDictionary<int, int> _pingFailures;
        private CacheManager _writeCacheManager;
        private CacheManager _mainCacheManager;
        private Dictionary<int, SiemensPlcConfig> _plcConfigs;
        
        /// <summary>
        /// Connection established for active devices
        /// </summary>
        public bool Connect()
        {
            SingleWriteConnectionManager.Instance.SiemensPlcConfigs = (List<SiemensPlcConfig>)WriteConfigurations.Instance.PlcConfigs;
            try
            {
                SingleWriteConnectionManager.Instance.InitializeConnections(20);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Initialize cycle cache values before run
        /// </summary>
        public void InitializeCycle()
        {
            try
            {
                _pingFailures = new ConcurrentDictionary<int, int>();
                _writeCacheManager = new CacheManager(CacheDBType.WriteService, WriteConfigurations.Instance.WriteServiceConfiguration.RedisServer);
                _mainCacheManager = new CacheManager(CacheDBType.Main, WriteConfigurations.Instance.WriteServiceConfiguration.RedisServer);
                _plcConfigs = SingleWriteConnectionManager.Instance.SiemensPlcConfigs.ToDictionary(x => x.PlcDeviceId, x => x);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Infinite cycle with wait time. It runs write query for each device concurrently. Beware! Minimum cpu count should be two.
        /// </summary>
        public void RunCycle()
        {
            SyncStateManager syncStateManager = new SyncStateManager(WriteConfigurations.Instance.WriteServiceConfiguration.RedisServer);
             
            while (!_stopEvent.WaitOne(WriteConfigurations.Instance.WriteServiceConfiguration.CycleWaitInMiliseconds))
            {
                Parallel.ForEach((List<SiemensPlcConfig>)WriteConfigurations.Instance.PlcConfigs, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount - 1 }, plcItem =>
                {
                    if (NetworkChecker.PingSucceeded(_plcConfigs[plcItem.PlcDeviceId].Ip))
                    {
                        _pingFailures[plcItem.PlcDeviceId] = 0;

                        try
                        {
                            bool setResult = _mainCacheManager.Set($"PingFailureWriteServicePLC{plcItem.PlcDeviceId}", _pingFailures[plcItem.PlcDeviceId], null);
                        }
                        catch (Exception ex)
                        {
                            LogManager.Instance.Log($"PingFailure Check Redis service. {ex.Message}", LogType.Information);
                        }

                        try
                        {
                            if (syncStateManager.IsValidMaster(plcItem.PlcDeviceId, WriteConfigurations.Instance.WriteServiceConfiguration.WorkingEnvironment,90,false))
                            {
                                IWriteManager writeManager = new SiemensWriteManager(WriteConfigurations.Instance.WriteServiceConfiguration.RedisServer);
                                bool writeResult = writeManager.Write(plcItem.PlcDeviceId);
                            }
                            else
                            {
                                Thread.Sleep(3000);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogManager.Instance.Log($"WriteManager: Message:{ex.Message}", LogType.Information);
                        }
                    }
                    else
                    {
                        _pingFailures[plcItem.PlcDeviceId] = _pingFailures.ContainsKey(plcItem.PlcDeviceId) ? _pingFailures[plcItem.PlcDeviceId] + 1 : 0;

                        if (_pingFailures[plcItem.PlcDeviceId] > 10)
                        {
                            Thread.Sleep(10 * 1000);

                            try
                            {
                                bool setResult = _mainCacheManager.Set($"PingFailureWriteServicePLC{plcItem.PlcDeviceId}", _pingFailures[plcItem.PlcDeviceId], null);
                                LogManager.Instance.Log($"Check {_plcConfigs[plcItem.PlcDeviceId].Ip} connection for plc device: {plcItem.PlcDeviceId}. Ping Failure Count: {_pingFailures[plcItem.PlcDeviceId]}", LogType.Error);
                            }
                            catch (Exception ex)
                            {
                                LogManager.Instance.Log($"PingFailure Check Redis service! {ex.Message}", LogType.Information);
                            }
                        }
                        else
                        {
                            LogManager.Instance.Log($"Check {_plcConfigs[plcItem.PlcDeviceId].Ip} connection for plc device: {plcItem.PlcDeviceId}. Ping Failure Count: {_pingFailures[plcItem.PlcDeviceId]}", LogType.Error);
                        }

                    }
                });
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
