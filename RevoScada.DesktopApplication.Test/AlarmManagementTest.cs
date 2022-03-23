using Newtonsoft.Json;
using NUnit.Framework;

using RevoScada.Cache;
using RevoScada.ProcessController;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.Entities;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.Configuration.Service;
using RevoScada.Entities.PageTagConfigurations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
 
namespace RevoScada.DesktopApplication.Test
{

    [TestFixture]
    public class AlarmManagementTest
    {
        private  CacheManager _mainCacheManager;
        private  CacheManager _readCacheManager;
        private  CacheManager _writeCacheManager;


        [SetUp]
        public void Init()
        {
         //   AlarmServiceConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\SingleConfigurations\AlarmService.rsconfig");
            ApplicationConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\SingleConfigurations\Application.rsconfig",false);
            
            _mainCacheManager = new CacheManager(CacheDBType.Main, ApplicationConfigurations.Instance.Configuration.RedisServer);
            _readCacheManager = new CacheManager(CacheDBType.ReadService, ApplicationConfigurations.Instance.Configuration.RedisServer);
            _writeCacheManager = new CacheManager(CacheDBType.WriteService, ApplicationConfigurations.Instance.Configuration.RedisServer);
           
        }

        [Test]
        public void delete_324982094092384908230423()
        {
            ProcessManager.Instance.Initialize(ApplicationConfigurations.Instance.Configuration, ApplicationConfigurations.Instance.TagConfigurations);
            ProcessManager.Instance.InitializeSelectedDevice( plcDeviceId:1);
            bool isBatchRunnigsagad = ProcessManager.Instance.IsBatchRunning();
        }


            [Test]
        public void Get_alarm_List()
        {
            List<string> alarmKeys = _mainCacheManager.GetKeyNames("alarm_*");


            
            List<PlcAlarm> plcAlarms = new List<PlcAlarm>();

            foreach (string alarmKey in alarmKeys)
            {
                string serializedPlcAlarmItem = _mainCacheManager.GetString(alarmKey);
                PlcAlarm alarmItem = JsonConvert.DeserializeObject<PlcAlarm>(serializedPlcAlarmItem);
                plcAlarms.Add(alarmItem);
            }
        
        }

        [Test]
      //[Ignore("critical part for testing alarm")]
        public void Reset_alarm()
        {
            List<string> alarmKeys = _mainCacheManager.GetKeyNames("alarm_*");



            List<PlcAlarm> plcAlarms = new List<PlcAlarm>();

            foreach (string alarmKey in alarmKeys)
            {
                //alarmKeys.Remove(alarmKeys[alarmKeyIteration]);
                _mainCacheManager.DeleteKey(alarmKey);
            }

        }
    
       

    }








}
