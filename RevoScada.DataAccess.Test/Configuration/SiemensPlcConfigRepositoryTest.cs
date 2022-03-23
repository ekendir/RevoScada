using System;
using System.Linq;
using NUnit.Framework;
using RevoScada.Configurator;
using RevoScada.DataAccess.Concrete.SqLite;
using RevoScada.Entities.Configuration;

namespace RevoScada.DataAccess.Test
{
    [TestFixture]
    public class SiemensPlcConfigRepositoryTest
    {
       
        [SetUp]
        public void Init()
        {
            ReadConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.Files\Configuration\Test_ReadService.rsconfig");

        }

        [Test]
        public void Get_siemensPlcConfig_all()
        {
            SiemensPlcConfigRepository siemensPlcConfigRepository = new SiemensPlcConfigRepository(ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString);

            //var count = siemensPlcConfigService.GetById(1)?.ToList()?.Count ?? 0;
            var count = siemensPlcConfigRepository.GetAll()?.ToList()?.Count ?? 0;

            Assert.IsTrue(count > 0);
        }

        [Test]
        public void Get_siemensPlcConfig_byId()
        {
            SiemensPlcConfigRepository siemensPlcConfigRepository = new SiemensPlcConfigRepository(ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString);

            var count = siemensPlcConfigRepository.GetById(1);
            ///var count = siemensPlcConfigRepository.Get_all()?.ToList()?.Count ?? 0;

        }



        [Test]
        public void Set_siemensPlcConfig()
        {
            SiemensPlcConfigRepository siemensPlcConfigRepository = new SiemensPlcConfigRepository(ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString);

            SiemensPlcConfig siemensPlcConfig = new SiemensPlcConfig();
            // siemensPlcConfig.PlcDeviceId = 40; 
            siemensPlcConfig.Ip = "192.168.1.203";
            siemensPlcConfig.Rack = 0;
            siemensPlcConfig.Slot = 0;
            siemensPlcConfig.ModifiedDate = DateTime.Now;

            var control = siemensPlcConfigRepository.Insert(siemensPlcConfig);

            Assert.IsTrue(control == true);
        }

        [Test]
        public void Update_siemensPlcConfig()
        {
            SiemensPlcConfigRepository siemensPlcConfigRepository = new SiemensPlcConfigRepository(ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString);
            SiemensPlcConfig siemensPlcConfig = new SiemensPlcConfig();
            siemensPlcConfig.PlcDeviceId = 7;
            siemensPlcConfig.Ip = "192.168.1.203";
            siemensPlcConfig.Rack = 0;
            siemensPlcConfig.Slot = 1;
            siemensPlcConfig.ModifiedDate = DateTime.Now;

            var control = siemensPlcConfigRepository.Update(siemensPlcConfig);

            Assert.IsTrue(control == true);
        }
    }
}
