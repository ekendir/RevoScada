using Revo.Core;
using Revo.ServiceUtilities;
using RevoScada.Cache;
using RevoScada.PlcAccess;
using RevoScada.PlcConnection.Siemens;
using RevoScada.Configurator;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RevoScada.ReadService
{
    class SiemensReadCycleStrategy : IPlcReadCycleStrategy
    {
        private readonly ManualResetEvent _stopEvent = new ManualResetEvent(false);

        private ConcurrentDictionary<int, int> _pingFailures;
        private CacheManager _readCacheManager;
        private CacheManager _mainCacheManager;
        private Dictionary<int, SiemensPlcConfig> _plcConfigs;

        /// <summary>
        /// Connection established for active devices
        /// </summary>
        public bool Connect()
        {
            SingleReadConnectionManager.Instance.SiemensPlcConfigs = (List<SiemensPlcConfig>)ReadConfigurations.Instance.PlcConfigs;
            try
            {
                SingleReadConnectionManager.Instance.InitializeConnections(20);
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
                _readCacheManager = new CacheManager(CacheDBType.ReadService, ReadConfigurations.Instance.ReadServiceConfiguration.RedisServer);
                _mainCacheManager = new CacheManager(CacheDBType.Main, ReadConfigurations.Instance.ReadServiceConfiguration.RedisServer);
                _plcConfigs = SingleReadConnectionManager.Instance.SiemensPlcConfigs.ToDictionary(x => x.PlcDeviceId, x => x);

                //reset readservice cache
                var keys = _readCacheManager.GetKeyNames("*");
                keys.ForEach(x => _readCacheManager.DeleteKey(x));
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
            while (!_stopEvent.WaitOne(ReadConfigurations.Instance.ReadServiceConfiguration.CycleWaitInMiliseconds))
            {
                Parallel.ForEach((List<SiemensPlcConfig>)ReadConfigurations.Instance.PlcConfigs, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount - 1 }, plcItem =>
                {
                    if (NetworkChecker.PingSucceeded(_plcConfigs[plcItem.PlcDeviceId].Ip))
                    {
                        _pingFailures[plcItem.PlcDeviceId] = 0;

                        try
                        {
                            bool setResult = _mainCacheManager.Set($"PingFailureReadServicePLC{plcItem.PlcDeviceId}", _pingFailures[plcItem.PlcDeviceId], null);
                        }
                        catch (Exception ex)
                        {
                            LogManager.Instance.Log($"PingFailure Check Redis service. {ex.Message}", LogType.Information);
                        }

                        try
                        {
                            IReadManager readManager = new SiemensReadManager(ReadConfigurations.Instance.ReadServiceConfiguration.RedisServer);
                            List<SiemensReadRequestItem> siemensReadRequestItems = ReadConfigurations.Instance.MultipleDeviceSiemensReadRequestItems[plcItem.PlcDeviceId];

                            foreach (var onDemandDBValue in ReadConfigurations.Instance.OnDemandDBNumberCollections[plcItem.PlcDeviceId])
                            {
                                bool isDemanded = _mainCacheManager.GetBool($"DemandRead:PLC{plcItem.PlcDeviceId}:DB{onDemandDBValue.Key}");
                                
                                if (isDemanded != siemensReadRequestItems.First(x=>x.DbNumber == onDemandDBValue.Key).IsDemanded)
                                {
                                    ReadConfigurations.Instance.ChangeDBNumberDemand(plcItem.PlcDeviceId, onDemandDBValue.Key, isDemanded);
                                }
                            }

                            bool readResult = readManager.Read(plcItem.PlcDeviceId, siemensReadRequestItems);
                        }
                        catch (Exception ex)
                        {
                            LogManager.Instance.Log($"ReadManager: Message:{ex.Message}", LogType.Information);
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
                                bool setResult = _mainCacheManager.Set($"PingFailureReadServicePLC{plcItem.PlcDeviceId}", _pingFailures[plcItem.PlcDeviceId], null);
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
