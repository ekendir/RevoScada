using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Newtonsoft.Json;
using NUnit.Framework;
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
using RevoScada.TAI.Entities.Configuration.Service;

namespace RevoScada.TAI.ServiceTest
{
    //--
    [TestFixture]
    public class DataLoggerTest
    {
        private   CacheManager _readCacheManager;
        private   CacheManager _mainCacheManager;

        int _plcDeviceId;

        [SetUp]
        public void Init()
        {
            DataLoggerServiceConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\Test_DataLoggerService.rsconfig");

            _readCacheManager = new CacheManager(CacheDBType.ReadService, DataLoggerServiceConfigurations.Instance.DataLoggerServiceConfiguration.RedisServer);
            _mainCacheManager = new CacheManager(CacheDBType.Main, DataLoggerServiceConfigurations.Instance.DataLoggerServiceConfiguration.RedisServer);
          
            
            _plcDeviceId = 1;
        }

        [Test]
        public void set_logdata_port_tags()
        {
            SiemensPlcConfigService siemensPlcConfigService = new SiemensPlcConfigService(WriteConfigurations.Instance.WriteServiceConfiguration.SqliteConnectionString);
            var result = siemensPlcConfigService.GetAllBySqlQuery("SELECT TOP 400 * FROM  SiemensPlcConfigs");

        }

        [Test]
        public void set_logdata_port_tags_1()
        {
            SiemensPlcConfigService siemensPlcConfigService = new SiemensPlcConfigService(WriteConfigurations.Instance.WriteServiceConfiguration.SqliteConnectionString);
            var result = siemensPlcConfigService.GetAllBySqlQuery("SELECT TOP 400 * FROM  SiemensPlcConfigs");

        }



      
    }
}