using System;
using System.Linq;
using NUnit.Framework;
using RevoScada.Configurator;
using RevoScada.DataAccess.Concrete.SqLite;
using RevoScada.Entities.Configuration;

namespace RevoScada.DataAccess.Test
{
    [TestFixture]
    public class PlcTypeRepositoryTest
    {
         
        [SetUp]
        public void Init()
        {
            ReadConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.Files\Configuration\Test_ReadService.rsconfig");

        }

        [Test]
        public void Get_plctype_byId()
        {
            PlcTypeRepository plcTypeRepository = new PlcTypeRepository(ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString);

            var entity = plcTypeRepository.GetById(1);

            Assert.IsTrue(entity != null);
        }

        [Test]
        public void Set_plctype()
        {
            PlcTypeRepository plcTypeRepository = new PlcTypeRepository(ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString);

            PlcType plcType = new PlcType();
            plcType.TypeName = "Rockwell";

            var control = plcTypeRepository.Insert(plcType);

            Assert.IsTrue(control == true);
        }

        [Test]
        public void Update_plctype()
        {
            PlcTypeRepository plcTypeRepository = new PlcTypeRepository(ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString);

            PlcType plcType = new PlcType();
            plcType.Id = 2;
            plcType.TypeName = "Rockwellll";

            var updateControl = plcTypeRepository.Update(plcType);

            Assert.IsTrue(updateControl == true);
        }
    }
}
