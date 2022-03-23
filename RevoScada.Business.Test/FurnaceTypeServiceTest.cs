using System;
using System.Linq;
using NUnit.Framework;
using RevoScada.Business.Configurations;
using RevoScada.Configurator;
using RevoScada.Entities.Configuration;

namespace RevoScada.Business.Test
{
    [TestFixture]
    public class FurnaceTypeServiceTest
    {
        private string _connectionString;
        //private string _configurationfile;

        [SetUp]
        public void Init()
        {
            
            _connectionString = ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString;
        }


        [Test]
        public void Get_furnacetype_byId()
        {
            FurnaceTypeService furnaceTypeService = new FurnaceTypeService(_connectionString);
            var count = furnaceTypeService.GetById(1);

        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Set_furnaceType()
        {
            FurnaceTypeService furnaceTypeService = new FurnaceTypeService(_connectionString);

            FurnaceType furnaceType = new FurnaceType();
           // furnaceType.Id = 10;
            furnaceType.TypeName = "CO-2";
            furnaceType.Description = "Cure Oven".ToUpper();

            var control = furnaceTypeService.Insert(furnaceType);

            Assert.IsTrue(control == true);

        }
        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Update_furnaceType()
        {
            FurnaceTypeService furnaceTypeService = new FurnaceTypeService(_connectionString);

            FurnaceType furnaceType = new FurnaceType();
            furnaceType.Id = 2;
            furnaceType.TypeName = "CO-33";
            furnaceType.Description = "Cure Oven".ToUpper();

            var control = furnaceTypeService.Update(furnaceType);

            Assert.IsTrue(control == true);

        }



        [TearDown]
        public void Closing()
        {
        }
    }
}
