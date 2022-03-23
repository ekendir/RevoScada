using System;
using System.Linq;
using NUnit.Framework;
using RevoScada.Business.Configurations;
using RevoScada.Configurator;
using RevoScada.Entities.Configuration;

namespace RevoScada.Business.Test
{
    [TestFixture]
    public class SiemensTagConfigurationServiceTest
    {
        private string _connectionString;
        [SetUp]
        public void Init()
        {
            ReadConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\Test_ReadService2.rsconfig");
            _connectionString = ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString;
        }

        [Test]

        public void Get_siemensTagConfig_all()
        {
            SiemensTagConfigurationService siemensTagConfigurationService = new SiemensTagConfigurationService(_connectionString);

            var d = siemensTagConfigurationService.GetAll();

            var count = siemensTagConfigurationService.GetAll()?.ToList()?.Count ?? 0;

            var getallResult = siemensTagConfigurationService.GetAll();


            Assert.IsTrue(count > 0);
        }

        [Test]
        public void Get_siemensTagConfig_readRequest_items()
        {
            SiemensTagConfigurationService siemensTagConfigurationService = new SiemensTagConfigurationService(_connectionString);

            var count = siemensTagConfigurationService.ReadRequestItems(1);
        }


   



        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Set_siemensTagConfig()
        {
           
            SiemensTagConfigurationService siemensTagConfigurationService = new SiemensTagConfigurationService(_connectionString);

            SiemensTagConfiguration siemensTagConfiguration = new SiemensTagConfiguration();
            siemensTagConfiguration.PlcId = 1;
            siemensTagConfiguration.TagName = "PLC1";
            siemensTagConfiguration.Address = "-";
            siemensTagConfiguration.DBNumber = 123;
            siemensTagConfiguration.Offset = 0;
            siemensTagConfiguration.DataType = "int";
            siemensTagConfiguration.SiemensTagGroupId = 0;
            siemensTagConfiguration.Description = "";
            siemensTagConfiguration.ModifiedDate = DateTime.Now;
            siemensTagConfiguration.IsActive = true;

            var control = siemensTagConfigurationService.Insert(siemensTagConfiguration);
            Assert.IsTrue(control == true);

        }


        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Update_siemensTagConfig()
        {
            SiemensTagConfigurationService siemensTagConfigurationService = new SiemensTagConfigurationService(_connectionString);

            SiemensTagConfiguration siemensTagConfiguration = new SiemensTagConfiguration();
            siemensTagConfiguration.Id= 
            siemensTagConfiguration.PlcId = 5692;
            siemensTagConfiguration.TagName = "PLC11111111";
            siemensTagConfiguration.Address = "-";
            siemensTagConfiguration.DBNumber = 123;
            siemensTagConfiguration.Offset = 0;
            siemensTagConfiguration.DataType = "int";
            siemensTagConfiguration.SiemensTagGroupId = 0;
            siemensTagConfiguration.Description = "";
            siemensTagConfiguration.ModifiedDate = DateTime.Now;
            siemensTagConfiguration.IsActive = true;

            var control = siemensTagConfigurationService.Update(siemensTagConfiguration);
            Assert.IsTrue(control == true);

        }
    }
}
