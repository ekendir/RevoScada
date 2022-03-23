using System;
using System.Linq;
using NUnit.Framework;
using RevoScada.Configurator;
using RevoScada.DataAccess.Concrete.SqLite;
using RevoScada.Entities.Configuration;

namespace RevoScada.DataAccess.Test
{
    [TestFixture]
    public class FurnaceTypeRepositoryTest
    {
       
        [SetUp]
        public void Init()
        {
         

            ReadConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.Files\Configuration\Test_ReadService.rsconfig");

            


        }

        [Test]
        public void Get_furnaceType_all()
        {
            FurnaceTypeRepository furnaceTypeRepository = new FurnaceTypeRepository(ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString);

            var furnaceTypes = furnaceTypeRepository.GetAll();

            var count = furnaceTypes.ToList().Count();

            Assert.IsTrue(count > 0);
        }

        [Test]
        public void Get_furnaceType_byId()
        {
            FurnaceTypeRepository furnaceTypeRepository = new FurnaceTypeRepository(ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString);

            var furnaceType = furnaceTypeRepository.GetById(1);

          

            Assert.IsTrue(furnaceType is null);
        }



        [Test]
        public void Set_furnaceType()
        {
            FurnaceTypeRepository furnaceTypeRepository = new FurnaceTypeRepository(ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString);

            FurnaceType furnaceType = new FurnaceType();
            // furnaceType.Id = 10;
            furnaceType.TypeName = "CO-2";
            furnaceType.Description = "Cure Oven".ToUpper();

            var control = furnaceTypeRepository.Insert(furnaceType);

            Assert.IsTrue(control == true);
        }

        [Test]
        public void Update_furnaceType()
        {
            FurnaceTypeRepository furnaceTypeRepository = new FurnaceTypeRepository(ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString);

            FurnaceType furnaceType = new FurnaceType();
            furnaceType.Id = 2;
            furnaceType.TypeName = "CO-33";
            furnaceType.Description = "Cure Oven".ToUpper();

            var control = furnaceTypeRepository.Update(furnaceType);

            Assert.IsTrue(control == true);

        }
    }
}
