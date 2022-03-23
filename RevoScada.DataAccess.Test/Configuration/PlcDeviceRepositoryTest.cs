using System;
using System.Linq;
using NUnit.Framework;
using RevoScada.Configurator;
using RevoScada.DataAccess.Concrete.SqLite;
using RevoScada.Entities.Configuration;

namespace RevoScada.DataAccess.Test
{
    [TestFixture]
    public class PlcDeviceRepositoryTest
    {
        
        [SetUp]
        public void Init()
        {
            ReadConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.Files\Configuration\Test_ReadService.rsconfig");

        }

        [Test]
        public void Get_plcDevice_byId()
        {
            PlcDeviceRepository plcDeviceRepository = new PlcDeviceRepository(ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString);
            

        }

        [Test]
        public void Set_plcDevice()
        {
            PlcDeviceRepository plcDeviceRepository = new PlcDeviceRepository(ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString);
            PlcDevice plcDevice = new PlcDevice();
            plcDevice.PlcType = 1;
            plcDevice.FurnaceId = 1;
            plcDevice.ModifiedDate = DateTime.Now;
            plcDevice.IsActive = true;

            var control = plcDeviceRepository.Insert(plcDevice);
            Assert.IsTrue(control == true);
        }
        [Test]
        public void Update_plcDevice()
        {
            PlcDeviceRepository plcDeviceRepository = new PlcDeviceRepository(ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString);
            PlcDevice plcDevice = new PlcDevice();
            plcDevice.Id = 2;
            plcDevice.PlcType = 11;
            plcDevice.FurnaceId = 11;
            plcDevice.ModifiedDate = DateTime.Now;
            plcDevice.IsActive = true;

            var control = plcDeviceRepository.Update(plcDevice);
            Assert.IsTrue(control == true);
        }

    }
}
