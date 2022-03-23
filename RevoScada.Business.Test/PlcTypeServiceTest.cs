using System;
using System.Linq;
using NUnit.Framework;
using RevoScada.Business.Configurations;
using RevoScada.Configurator;
using RevoScada.Entities.Configuration;

namespace RevoScada.Business.Test
{
    [TestFixture]
    public class PlcTypeServiceTest
    {
        private string _connectionString;

        [SetUp]
        public void Init()
        {
            
            _connectionString = ReadConfigurations.Instance.ReadServiceConfiguration.SqliteConnectionString;
        }

        [Test]
        public void Get_plctype_byId()
        {
            PlcTypeService plcTypeService = new PlcTypeService(_connectionString);

            var count = plcTypeService.GetById(1);

        }

        [Test]
        public void Set_plctype()
        {
            PlcTypeService plcTypeService = new PlcTypeService(_connectionString);

            PlcType plcType = new PlcType();
            plcType.TypeName = "Rockwell";

            var control = plcTypeService.Insert(plcType);

            Assert.IsTrue(control == true);
        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Update_plctype()
        {
            PlcTypeService plcTypeService = new PlcTypeService(_connectionString);

            PlcType plcType = new PlcType();
            plcType.Id = 2;
            plcType.TypeName = "Rockwellll";

            var updateControl = plcTypeService.Update(plcType);

            Assert.IsTrue(updateControl == true);
        }



        [TearDown]
        public void Closing()
        {
        }

    }
}
