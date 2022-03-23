


using System;
using System.Linq;
using NUnit.Framework;
using RevoScada.Business.Configurations;
using RevoScada.Configurator;
using RevoScada.Entities.Configuration;

namespace RevoScada.Business.Test
{
    [TestFixture]
    public class FurnaceServiceTest
    {
        private string _connectionString;

       // private string _configurationfile;

        [SetUp]
        public void Init()
        {
            
            _connectionString = ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString;
        }
      
        [Test]
        public void Get_furnace_all()
        {
            FurnaceService furnaceService=  new  FurnaceService(_connectionString);

            var count = furnaceService.GetAll()?.ToList()?.Count() ?? 0;

            Assert.IsTrue(count > 0);

        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Set_furnace()
        {
            FurnaceService furnaceService = new FurnaceService(_connectionString);
            Furnace furnace = new Furnace();
            furnace.CustomerId = 1;
            furnace.FurnaceName = "CO-2";
            furnace.FurnaceTypeId = 1;
            furnace.ModifiedDate = DateTime.Now;
            furnace.IsActive = true;

            var control = furnaceService.Insert(furnace);

            Assert.IsTrue(control == true);
        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Update_furnace()
        {
            FurnaceService furnaceService = new FurnaceService(_connectionString);

            Furnace furnace = new Furnace();
            furnace.Id = 1;
            furnace.CustomerId = 1;
            furnace.FurnaceName = "CO-4";
            furnace.FurnaceTypeId = 1;
            furnace.ModifiedDate = DateTime.Now;
            furnace.IsActive = true;

           var updateControl = furnaceService.Update(furnace);
        }

        [TearDown]
        public void Closing()
        {
        }
    }
}
