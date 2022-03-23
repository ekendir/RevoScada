using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using NUnit.Framework;
using Revo.Core;
using Revo.SiemensDrivers.Sharp7;
using RevoScada.Cache;
using RevoScada.PlcConnection.Siemens;
using RevoScada.TAI.Business;
using RevoScada.TAI.Business.Configurations;
using RevoScada.TAI.Configurator;
using RevoScada.TAI.Entities;
using RevoScada.TAI.Entities.Complex;
using RevoScada.TAI.Entities.Complex.Alarm;
using RevoScada.TAI.Entities.Configuration;
using RevoScada.TAI.Entities.Enums;

namespace RevoScada.TAI.ServiceTest
{
    
    [TestFixture]
    public class AlarmServiceTest
    {
        private CacheManager _readCacheManager;

        private CacheManager _writeCacheManager;

        private CacheManager _mainCacheManager;

        private int _plcDeviceId;

        [SetUp]
        public void Init()
        {

            AlarmServiceConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\Test_AlarmService.rsconfig");
            _readCacheManager = new CacheManager(CacheDBType.ReadService, AlarmServiceConfigurations.Instance.AlarmServiceConfiguration.RedisServer);
            _writeCacheManager = new CacheManager(CacheDBType.WriteService, AlarmServiceConfigurations.Instance.AlarmServiceConfiguration.RedisServer);
            _mainCacheManager = new CacheManager(CacheDBType.Main, AlarmServiceConfigurations.Instance.AlarmServiceConfiguration.RedisServer);
            _plcDeviceId = ((List<SiemensPlcConfig>)AlarmServiceConfigurations.Instance.PlcConfigs)[0].PlcDeviceId;

            LogManager.Instance.InitializeConfiguration(AlarmServiceConfigurations.Instance.AlarmServiceConfiguration.LogSettings);


        }

      
        [Test]
        public void processrun_save_cache_value_to_db()
        {
            //save dummy alarm
            PlcAlarm plcAlarm = new PlcAlarm
            {
                PlcValue = true,
                TagConfigurationId = 0,
                Status = "I",
                BatchId = 0,
                InDateTime=DateTime.Now
            };
             

            string serializedUpdatedPlcAlarm = JsonConvert.SerializeObject(plcAlarm);
            _mainCacheManager.Set("alarm_plc0_tagid_1", serializedUpdatedPlcAlarm);

             
           
            // setting CurrentBatchInfo cache is alarm unsaved and finished batch for test
            //CurrentBatchInfo currentBatchInfo = new CurrentBatchInfo
            //{
            //    BatchId = 1,
            //    CurrentState = BatchCurrentState.Finished,
            //    IsAlarmSaved = false
            //};

            //string  currentBatchInfoSerialized = JsonConvert.SerializeObject(currentBatchInfo);


            //_mainCacheManager.Set("CurrentBatchInfo", currentBatchInfoSerialized);

           

        }

        //[Test]
        //public void Update_CurrentBatchInfo()
        //{
          
        //    CurrentBatchInfo currentBatchInfo = new CurrentBatchInfo
        //    {
        //        BatchId = 1,
        //        CurrentState = BatchCurrentState.Running,
        //        IsAlarmSaved = false
        //    };


        //    string serializedCurrentBatchInfo = JsonConvert.SerializeObject(currentBatchInfo);


        //    _mainCacheManager.Set($"CurrentBatchInfo", serializedCurrentBatchInfo);
  
        //}

        
        //public bool BeginUpdateCache(string lockedKey,byte tryAmount)
        //{
        //    bool beginResult = false;

        //   string value = _mainCacheManager.GetString(lockedKey);

        //    if (value == null) {
        //        TimeSpan span = TimeSpan.FromMinutes(5.0);
        //        bool setCacheResult = _mainCacheManager.Set(lockedKey, "true", span);
        //        beginResult = true;
        //    }
        //    else
        //    {
        //        do
        //        {
                    
        //            if (tryAmount == 0)
        //            {
        //                break;
        //            }
                    
        //            tryAmount--;

        //            beginResult = ! Convert.ToBoolean(value);
        //            Thread.Sleep(1000);

        //        } while (Convert.ToBoolean(value) == true);
        //    }


        //    return beginResult;
        //}
        //public bool EndUpdateCache(string lockedKey, byte tryAmount)
        //{
        //    bool endResult = false;

        //    string alarmSetLocked = _mainCacheManager.GetString(lockedKey);

        //    if (alarmSetLocked == null)
        //    {
        //        endResult = true;
        //    }
        //    else if (Convert.ToBoolean(alarmSetLocked)==true)
        //    {
        //        TimeSpan span = TimeSpan.FromMinutes(5.0);
        //        bool setCacheResult = _mainCacheManager.Set(lockedKey, "false", span);
        //        endResult = setCacheResult;

        //    }

        //   return endResult;
        //}





    }

  

  

}