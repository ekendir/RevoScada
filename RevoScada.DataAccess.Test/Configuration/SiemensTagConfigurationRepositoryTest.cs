using System;
using System.Linq;
using System.Linq.Expressions;
 
using NUnit.Framework;
using RevoScada.Configurator;
using RevoScada.DataAccess.Concrete.SqLite;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Configuration;

namespace RevoScada.DataAccess.Test
{
    [TestFixture]
    public class SiemensTagConfigurationRepositoryTest
    {
        
        [SetUp]
        public void Init()
        {
  
        }

        [Test]
        public void Get_siemensTagConfig_all()
        {
            SiemensTagConfigurationRepository siemensTagConfigurationRepository = new SiemensTagConfigurationRepository(ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString);

            var count = siemensTagConfigurationRepository.GetAll()?.ToList()?.Count ?? 0;

            Assert.IsTrue(count > 0);
        }


        [Test]
        public void Get_read_request_items()
        {
            var siemensTagConfigurationRepository = new DataAccess.Concrete.SqLite.SiemensTagConfigurationRepository(ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString);

            var readRequestItems = siemensTagConfigurationRepository.ReadRequestList(1);

        }


        [Test]
        public void Set_siemensTagConfig()
        {
            SiemensTagConfigurationRepository siemensTagConfigurationRepository = new SiemensTagConfigurationRepository(ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString);

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

            var control = siemensTagConfigurationRepository.Insert(siemensTagConfiguration);
            Assert.IsTrue(control == true);

        }


        [Test]
        public void Update_siemensTagConfig()
        {
            SiemensTagConfigurationRepository siemensTagConfigurationRepository = new SiemensTagConfigurationRepository(ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString);

            SiemensTagConfiguration siemensTagConfiguration = new SiemensTagConfiguration();
            siemensTagConfiguration.Id =
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

            var control = siemensTagConfigurationRepository.Update(siemensTagConfiguration);
            Assert.IsTrue(control == true);

        }
    }
}
