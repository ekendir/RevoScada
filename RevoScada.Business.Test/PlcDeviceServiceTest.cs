using System;
using System.Linq;
using NUnit.Framework;
using RevoScada.Business.Configurations;
using RevoScada.Configurator;
using RevoScada.Entities.Configuration;

namespace RevoScada.Business.Test
{
    [TestFixture]
    public class PlcDeviceServiceTest
    {
        private string _connectionString;

        //private string _configurationfile;

        [SetUp]
        public void Init()
        {
            
            _connectionString = ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString;
        }

        [Test]
        public void Get_plcDevice_byId()
        {
            PlcDeviceService plcDeviceService = new PlcDeviceService(_connectionString);
            var count = plcDeviceService.GetById(1);

        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Set_plcDevice()
        {
            PlcDeviceService plcDeviceService = new PlcDeviceService(_connectionString);
            PlcDevice plcDevice = new PlcDevice();
            plcDevice.PlcType = 1;
            plcDevice.FurnaceId = 1;
            plcDevice.ModifiedDate = DateTime.Now;
            plcDevice.IsActive = true;

            var control = plcDeviceService.Insert(plcDevice);
            Assert.IsTrue(control == true);
        }
        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Update_plcDevice()
        {
            PlcDeviceService plcDeviceService = new PlcDeviceService(_connectionString);
            PlcDevice plcDevice = new PlcDevice();
            plcDevice.Id = 2;
            plcDevice.PlcType = 11;
            plcDevice.FurnaceId = 11;
            plcDevice.ModifiedDate = DateTime.Now;
            plcDevice.IsActive = true;

            var control = plcDeviceService.Update(plcDevice);
            Assert.IsTrue(control == true);
        }


        [TearDown]
        public void Closing()
        {
        }
    }
}
