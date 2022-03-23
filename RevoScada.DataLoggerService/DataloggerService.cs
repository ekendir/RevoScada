using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.AdoJobStore;
using Revo.Core;
using RevoScada.Cache;
using RevoScada.DataLoggerService.Jobs;
using RevoScada.Configurator;
using RevoScada.Entities;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Enums;
using RevoScada.Business;
using RevoScada.Entities.PageTagConfigurations;
using RevoScada.ProcessController;
using RevoScada.Entities.Configuration;

namespace RevoScada.DataLoggerService
{
    public partial class DataloggerService : ServiceBase
    {
        private static ManualResetEvent stopEvent = new ManualResetEvent(false);
        private static Thread serviceInfiniteLoop;
        private string _startupConfigurationFile;
        private const string _serviceName = "RevoScadaDataLoggerService";
        public static  CacheManager MainCacheManager;
        public static CacheManager ReadCacheManager;
        static IScheduler Scheduler;

        private static Dictionary<int,DataLoggerInfo> _dataLoggerInfos;
        public DataloggerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs != null && commandLineArgs.Length > 2 && commandLineArgs[2] == "debug")
            {
#if DEBUG
                if (Debugger.IsAttached == false) Debugger.Launch();
#endif
            }

            try
            {
                _startupConfigurationFile = Environment.GetCommandLineArgs()[1];
                DataLoggerServiceConfigurations.Instance.InitializeConfiguration(_startupConfigurationFile);
                MainCacheManager = new CacheManager(CacheDBType.Main, DataLoggerServiceConfigurations.Instance.DataLoggerServiceConfiguration.RedisServer);
                ReadCacheManager = new CacheManager(CacheDBType.ReadService, DataLoggerServiceConfigurations.Instance.DataLoggerServiceConfiguration.RedisServer);
                LogManager.Instance.InitializeConfiguration(DataLoggerServiceConfigurations.Instance.DataLoggerServiceConfiguration.LogSettings);
                LogManager.Instance.Log("service running...", LogType.Information);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(_serviceName, $"Service stopped. Check configuration file location or format for Logging. {ex.Message}", EventLogEntryType.Error);
                Environment.Exit(0);
            }

            serviceInfiniteLoop = new Thread(DoWork)
            {
                IsBackground = true
            };

            serviceInfiniteLoop.Start();
        }
        private void DoWork() {

            Schedule().GetAwaiter().GetResult();
        }

        /// <summary>
        /// It schedules cycled jobs defined in current info list.
        /// </summary>
        private static async Task Schedule()
        {
            CacheManager cacheManager = new CacheManager(CacheDBType.Main, DataLoggerServiceConfigurations.Instance.DataLoggerServiceConfiguration.RedisServer);
            NameValueCollection props = new NameValueCollection { { "quartz.serializer.type", "binary" } };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);
            Scheduler =  await factory.GetScheduler();
            await Scheduler.Start();
            
            _dataLoggerInfos = new Dictionary<int, DataLoggerInfo>();

            do
            {
                GetLatestDataLoggerInfos();

                foreach (var dataloggerItem in _dataLoggerInfos.Where(x=>x.Value.AssignedJobId== string.Empty && (x.Value.CurrentProcessInfo.BatchCurrentState == BatchCurrentState.Running || x.Value.CurrentProcessInfo.BatchCurrentState == BatchCurrentState.Hold)))
                {
                    string guid= Guid.NewGuid().ToString("N");
                    dataloggerItem.Value.AssignedJobId = $"Job_{guid}";

                    Dictionary<int, SensorViewPorts> pTCPortTagConfiguration = new Dictionary<int, SensorViewPorts>();
                    Dictionary<int, SensorViewPorts> mONPortTagConfiguration = new Dictionary<int, SensorViewPorts>();

                    PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(DataLoggerServiceConfigurations.Instance.DataLoggerServiceConfiguration.PostgreSqlConnectionStrings[dataloggerItem.Key]);
                    var pageTagConfiguration = pageTagConfigurationService.GetByName("SensorView");
                    string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
                    SensorViewTagConfigurations sensorViewTagConfigurations = JsonConvert.DeserializeObject<SensorViewTagConfigurations>(jsonSerializedString);

                    ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(DataLoggerServiceConfigurations.Instance.DataLoggerServiceConfiguration.PostgreSqlConnectionStrings[dataloggerItem.Key]);
                    int ptcCount = Convert.ToInt32(applicationPropertyService.GetByName("PTCCount").Value);
                    int monCount = Convert.ToInt32(applicationPropertyService.GetByName("MONCount").Value);
 
                    foreach (var partTemperaturePortItem in sensorViewTagConfigurations.PartTemperaturePorts.Take(ptcCount))
                    {
                        SensorViewPorts sensorViewItemsTagConfiguration = new SensorViewPorts();
                        int portNumeric = Convert.ToInt32(partTemperaturePortItem.Key.TrimStart('P', 'T', 'C'));
                        sensorViewItemsTagConfiguration.Value = (SiemensTagConfiguration)DataLoggerServiceConfigurations.Instance.DataLogTags[Convert.ToInt32(partTemperaturePortItem.Value.Value)];
                        sensorViewItemsTagConfiguration.EnableStatus = (SiemensTagConfiguration)DataLoggerServiceConfigurations.Instance.DataLogTags[Convert.ToInt32(partTemperaturePortItem.Value.EnableStatus)];
                        sensorViewItemsTagConfiguration.Rate = (SiemensTagConfiguration)DataLoggerServiceConfigurations.Instance.DataLogTags[Convert.ToInt32(partTemperaturePortItem.Value.Rate)];
                        pTCPortTagConfiguration.Add(portNumeric, sensorViewItemsTagConfiguration);
                    }

                    foreach (var partTemperaturePortItem in sensorViewTagConfigurations.PartVacuumDatas.Take(monCount))
                    {
                        SensorViewPorts sensorViewItemsTagConfiguration = new SensorViewPorts();
                        int portNumeric = Convert.ToInt32(partTemperaturePortItem.Key.TrimStart('M', 'O', 'N'));
                        sensorViewItemsTagConfiguration.Value = (SiemensTagConfiguration)DataLoggerServiceConfigurations.Instance.DataLogTags[Convert.ToInt32(partTemperaturePortItem.Value.Value)];
                        sensorViewItemsTagConfiguration.EnableStatus = (SiemensTagConfiguration)DataLoggerServiceConfigurations.Instance.DataLogTags[Convert.ToInt32(partTemperaturePortItem.Value.EnableStatus)];
                        sensorViewItemsTagConfiguration.Rate = (SiemensTagConfiguration)DataLoggerServiceConfigurations.Instance.DataLogTags[Convert.ToInt32(partTemperaturePortItem.Value.Rate)];
                        mONPortTagConfiguration.Add(portNumeric, sensorViewItemsTagConfiguration);
                    }

                    var pTCPortTagConfigurationSerialized = JsonConvert.SerializeObject(pTCPortTagConfiguration);
                    var mONPortTagConfigurationSerialized = JsonConvert.SerializeObject(mONPortTagConfiguration);

                    dataloggerItem.Value.JobDetail = JobBuilder.Create<LogDataJob>()
                        .WithIdentity($"Job_{guid}", "DataloggerCycleGroup")
                        .WithDescription($"description  {guid}")
                        .UsingJobData("PlcDeviceId", dataloggerItem.Key)
                        .UsingJobData("PTCPortTagConfigurationSerialized", pTCPortTagConfigurationSerialized)
                        .UsingJobData("MONPortTagConfigurationSerialized", mONPortTagConfigurationSerialized)
                        .UsingJobData("guid", guid)
                        .Build();
                         
                        dataloggerItem.Value.Trigger = TriggerBuilder.Create()
                        .WithIdentity($"Trigger_{guid}", "DataloggerCycleGroup")
                        .StartNow()
                        .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromMilliseconds(DataLoggerServiceConfigurations.Instance.DataLoggerServiceConfiguration.CycleWaitInMiliseconds))
                        .RepeatForever()).Build();
                }

                foreach (var dataloggerItem in _dataLoggerInfos.Where(x => x.Value.IsJobCycleStarted ==false && (x.Value.CurrentProcessInfo.BatchCurrentState == BatchCurrentState.Running|| x.Value.CurrentProcessInfo.BatchCurrentState== BatchCurrentState.Hold)))
                {
                    int plcDeviceId = dataloggerItem.Value.CurrentProcessInfo.PlcDeviceId;

                    try
                    {
                        LogManager.Instance.Log($"{_dataLoggerInfos[plcDeviceId].AssignedJobId} Cycle is Starting! Plc DeviceID: {dataloggerItem.Value.CurrentProcessInfo.PlcDeviceId}", LogType.Information);
                        await Task.Delay(200);
                        await Scheduler.ScheduleJob(dataloggerItem.Value.JobDetail, dataloggerItem.Value.Trigger);
                        await Task.Delay(200);
                        dataloggerItem.Value.IsJobCycleStarted = true;
                    }
                    catch (Exception ex)
                    {
                        _dataLoggerInfos[plcDeviceId].AssignedJobId = string.Empty;
                        LogManager.Instance.Log($"{_dataLoggerInfos[plcDeviceId].AssignedJobId} Cycle Failed to Start! Plc DeviceID: {dataloggerItem.Value.CurrentProcessInfo.PlcDeviceId}. Exception: {ex}", LogType.Information);
                    }
                }

                var removalList = new Dictionary<int, DataLoggerInfo>();
                foreach (var dataloggerItem in _dataLoggerInfos.Where(x=>x.Value.IsJobCycleStarted == true && (x.Value.CurrentProcessInfo.BatchCurrentState!= BatchCurrentState.Running && x.Value.CurrentProcessInfo.BatchCurrentState != BatchCurrentState.Hold)))
                {
                    try
                    {
                        await Scheduler.PauseJob(dataloggerItem.Value.JobDetail.Key);
                        await Task.Delay(200);
                        await Scheduler.DeleteJob(dataloggerItem.Value.JobDetail.Key);
                        removalList.Add(dataloggerItem.Key, dataloggerItem.Value);
                        await Task.Delay(200);
                        LogManager.Instance.Log($"{dataloggerItem.Value.JobDetail.Key.Name} Stopped!", LogType.Information);
                    }
                    catch (Exception ex)
                    {
                      LogManager.Instance.Log($"Error occured while removing job {dataloggerItem.Value.JobDetail.Key}. Detail: { ex}", LogType.Information);
                    }
                }

                try
                {
                    _dataLoggerInfos = _dataLoggerInfos.Except(removalList).ToDictionary(x => x.Key, x => x.Value);
                }
                catch (Exception ex)
                {
                    LogManager.Instance.Log($"Error occured while removing job list. Detail {ex}", LogType.Information);
                }
            }
            while (!stopEvent.WaitOne(5000));
        }

        /// <summary>
        /// Updates current process info data in cycled list
        /// </summary>
        private static void GetLatestDataLoggerInfos()
        {
            try
            {
                List<string> currentProcessInfoKeyNames = MainCacheManager.GetKeyNames("CurrentProcessInfo*");
                foreach (var keyName in currentProcessInfoKeyNames)
                {
                    string serializedString = MainCacheManager.GetString(keyName);
                    CurrentProcessInfo currentProcessInfo = JsonConvert.DeserializeObject<CurrentProcessInfo>(serializedString);

                    if (_dataLoggerInfos.ContainsKey(currentProcessInfo.PlcDeviceId))
                    {
                        _dataLoggerInfos[currentProcessInfo.PlcDeviceId].CurrentProcessInfo = currentProcessInfo;
                    }
                    else
                    {
                        DataLoggerInfo dataLoggerInfo = new DataLoggerInfo();
                        dataLoggerInfo.CurrentProcessInfo = currentProcessInfo;
                        dataLoggerInfo.AssignedJobId = string.Empty;
                        _dataLoggerInfos.Add(currentProcessInfo.PlcDeviceId, dataLoggerInfo);
                    }

                    _dataLoggerInfos[currentProcessInfo.PlcDeviceId].CurrentProcessInfo = currentProcessInfo;
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log($"GetLatestDataLoggerInfos error! Detail: {ex}", LogType.Information);
            }
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry(_serviceName, $"Abort Started", EventLogEntryType.Information);

            stopEvent.Set();
            if (!serviceInfiniteLoop.Join(10000))
                serviceInfiniteLoop.Abort();

            Scheduler.Shutdown();
            Thread.Sleep(500);


            EventLog.WriteEntry(_serviceName, $"Abort Completed", EventLogEntryType.Information);

        }
    }
}
