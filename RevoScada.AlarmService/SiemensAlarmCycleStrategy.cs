using Revo.Core;
using Revo.ServiceUtilities;
using RevoScada.Cache;
using RevoScada.Configurator;
using RevoScada.Entities.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RevoScada.AlarmService
{
    class SiemensAlarmCycleStrategy : IPlcAlarmCycleStrategy
    {
        private readonly ManualResetEvent _stopEvent = new ManualResetEvent(false);
       
        private ConcurrentDictionary<int, int> _pingFailures;
      
        private Dictionary<int, SiemensPlcConfig> _plcConfigs;
        private  CacheManager _mainCacheManager;


        /// <summary>
        /// Initialize cycle cache values before run
        /// </summary>
        public void InitializeCycle()
        {
            try
            {
                _pingFailures = new ConcurrentDictionary<int, int>();
                _mainCacheManager = new CacheManager(CacheDBType.Main, AlarmServiceConfigurations.Instance.AlarmServiceConfiguration.RedisServer);
                _plcConfigs = ((IEnumerable<SiemensPlcConfig>)AlarmServiceConfigurations.Instance.PlcConfigs).ToDictionary(x => x.PlcDeviceId, x => x);
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
            while (!_stopEvent.WaitOne(AlarmServiceConfigurations.Instance.AlarmServiceConfiguration.CycleWaitInMiliseconds))
            {
                 
                Parallel.ForEach((List<SiemensPlcConfig>)AlarmServiceConfigurations.Instance.PlcConfigs, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount - 1 }, plcItem =>
                {
                    if (NetworkChecker.PingSucceeded(_plcConfigs[plcItem.PlcDeviceId].Ip))
                    {
                        _pingFailures[plcItem.PlcDeviceId] = 0;

                        try
                        {
                            bool setResult = _mainCacheManager.Set($"PingFailureAlarmServicePLC{plcItem.PlcDeviceId}", _pingFailures[plcItem.PlcDeviceId], null);
                        }
                        catch (Exception ex)
                        {
                            LogManager.Instance.Log($"PingFailure Check Redis service. {ex.Message}", LogType.Information);
                        }

                        try
                        {
                            IAlarmManager alarmManager = new SiemensAlarmManager(AlarmServiceConfigurations.Instance.AlarmServiceConfiguration.RedisServer);
                            bool writeResult = alarmManager.GetAlarms(plcItem.PlcDeviceId);
                        }
                        catch (Exception ex)
                        {
                            LogManager.Instance.Log($"AlarmManager: Message:{ex.Message}", LogType.Information);
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
                                bool setResult = _mainCacheManager.Set($"PingFailureAlarmServicePLC{plcItem.PlcDeviceId}", _pingFailures[plcItem.PlcDeviceId], null);
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
