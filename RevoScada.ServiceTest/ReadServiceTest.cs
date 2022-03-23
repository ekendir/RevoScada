using Newtonsoft.Json;
using NUnit.Framework;
using Revo.Core;
using Revo.SiemensDrivers.Sharp7;
using RevoScada.Cache;
using RevoScada.PlcAccess;
using RevoScada.PlcConnection.Siemens;
using RevoScada.TAI.Business.Configurations;
using RevoScada.TAI.Configurator;
using RevoScada.TAI.Entities.Complex;
using RevoScada.TAI.Entities.Configuration;
using RevoScada.TAI.Entities.Configuration.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RevoScada.TAI.ServiceTest
{

    [TestFixture]
    public class ReadServiceTest
    {
        CacheManager cacheManager;
        
        [SetUp]
        public void Init()
        {

            ReadConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\Test_ReadService.rsconfig");

            SingleReadConnectionManager.Instance.SiemensPlcConfigs = (List<SiemensPlcConfig>)ReadConfigurations.Instance.PlcConfigs;

            SingleReadConnectionManager.Instance.InitializeConnections(5);

            cacheManager = new CacheManager(CacheDBType.ReadService, ReadConfigurations.Instance.ReadServiceConfiguration.RedisServer);
            
            LogManager.Instance.InitializeConfiguration(ReadConfigurations.Instance.ReadServiceConfiguration.LogSettings);
        }

         
        [Test]
        public void Get_db_in_cache()
        {
         
            string dbKey = $"PLC1DB200";
            var readresult = cacheManager.GetString(dbKey);

            var readResultSerialized = JsonConvert.DeserializeObject<ReadResult>(readresult);

          


        }


 

        [Test]
        [TestCase(14792, 0)]
        public void compare_cache_int_value(int tagId,int expectedResult)
        {
            SiemensTagConfigurationService siemensTagConfigurationService= new SiemensTagConfigurationService(ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString);
            SiemensTagConfiguration siemensTagConfiguration = siemensTagConfigurationService.GetAll().First(x=>x.Id==tagId);
           

            // siemensTagConfiguration = new SiemensTagConfiguration { DBNumber = 1, DataType = "string[18]", Offset = 500 };

            string json = cacheManager.GetString($"PLC{siemensTagConfiguration.PlcId}DB{siemensTagConfiguration.DBNumber}");
            ReadResult readResult = JsonConvert.DeserializeObject<ReadResult>(json);

            int offsetIntegral = (Convert.ToInt32(Math.Floor(siemensTagConfiguration.Offset)));
            int offsetDecimal = Convert.ToInt32((siemensTagConfiguration.Offset - Math.Floor(siemensTagConfiguration.Offset)) * 10);


            int cacheIntResult = S7.GetIntAt(readResult.Result, offsetIntegral);

            Assert.IsTrue(cacheIntResult==expectedResult);
     

        }


        [Test]
        [TestCase(13176, 0)]
        public void compare_cache_float_value(int tagId, float expectedResult)
        {
            SiemensTagConfigurationService siemensTagConfigurationService = new SiemensTagConfigurationService(ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString);
            SiemensTagConfiguration siemensTagConfiguration = siemensTagConfigurationService.GetAll().First(x => x.Id == tagId);


            // siemensTagConfiguration = new SiemensTagConfiguration { DBNumber = 1, DataType = "string[18]", Offset = 500 };

            string json = cacheManager.GetString($"PLC{siemensTagConfiguration.PlcId}DB{siemensTagConfiguration.DBNumber}");
            ReadResult readResult = JsonConvert.DeserializeObject<ReadResult>(json);

            int offsetIntegral = (Convert.ToInt32(Math.Floor(siemensTagConfiguration.Offset)));
            int offsetDecimal = Convert.ToInt32((siemensTagConfiguration.Offset - Math.Floor(siemensTagConfiguration.Offset)) * 10);


            float cacheIntResult = S7.GetRealAt(readResult.Result, offsetIntegral);

            Assert.IsTrue(cacheIntResult == expectedResult);


        }




    }
}
