using System;
using System.Linq;
using NUnit.Framework;
using RevoScada.Business.Configurations;
using RevoScada.Configurator;
using RevoScada.Entities.Configuration;

namespace RevoScada.Business.Test
{
    [TestFixture]
    public class SiemensPlcConfigServiceTest
    {
        private string _connectionString;

        [SetUp]
        public void Init()
        {
            ReadConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\Test_ReadService1.rsconfig");
            _connectionString = ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString;
        }

        [Test]
        public void Get_siemensPlcConfig_all()
        {
            SiemensPlcConfigService siemensPlcConfigService = new SiemensPlcConfigService(_connectionString);

            //var count = siemensPlcConfigService.GetById(1)?.ToList()?.Count ?? 0;
            var count = siemensPlcConfigService.GetAll()?.ToList()?.Count ?? 0;

            Assert.IsTrue(count>0);
        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Set_siemensPlcConfig()
        {
            SiemensPlcConfigService siemensPlcConfigService = new SiemensPlcConfigService(_connectionString);

            SiemensPlcConfig siemensPlcConfig = new SiemensPlcConfig();
           // siemensPlcConfig.PlcDeviceId = 40; 
            siemensPlcConfig.Ip = "192.168.1.203";
            siemensPlcConfig.Rack = 0;
            siemensPlcConfig.Slot = 0;
            siemensPlcConfig.ModifiedDate = DateTime.Now;

            var control = siemensPlcConfigService.Insert(siemensPlcConfig);

            Assert.IsTrue(control == true);
        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Update_siemensPlcConfig()
        {
            SiemensPlcConfigService siemensPlcConfigService = new SiemensPlcConfigService(_connectionString);
            SiemensPlcConfig siemensPlcConfig = new SiemensPlcConfig();
            siemensPlcConfig.PlcDeviceId = 7;
            siemensPlcConfig.Ip = "192.168.1.203";
            siemensPlcConfig.Rack = 0;
            siemensPlcConfig.Slot = 1;
            siemensPlcConfig.ModifiedDate = DateTime.Now;

            var control = siemensPlcConfigService.Update(siemensPlcConfig);

            Assert.IsTrue(control == true);
        }
 
    }
}
