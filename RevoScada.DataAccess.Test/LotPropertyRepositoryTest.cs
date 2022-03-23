using NUnit.Framework;
using RevoScada.Configurator;
using RevoScada.DataAccess.Concrete.Postgresql;
using RevoScada.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DataAccess.Test
{
    [TestFixture]
    public class LotPropertyRepositoryTest
    {
        LotPropertyRepository repository; 
        int RandomNumber;

        [SetUp]
        public void Init()
        {
            repository = new LotPropertyRepository(TestStaticParameters.ApplicationConfiguration.PostgreSqlConnectionString);
        }

        [Test]
        public void Get_all()
        {
            var count = repository.GetAll().ToList().Count();
            Assert.IsTrue(count >= 0);
        }

        [Test]
        public void Get_all_byid()
        {
            var entity = repository.GetById(2);

            Assert.IsTrue(entity.id == 2);
        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Insert()
        {
           RandomNumber = new Random().Next(0, 4000);
         
           LotProperty entity = new LotProperty();
           entity.BagId = 22;
           entity.SoirNumber = "SoirNumber_" + RandomNumber;
           entity.PartName = "PartName_" + RandomNumber;
           entity.ToolName = "ToolName_" + RandomNumber;

            bool insertResult = repository.Insert(entity);

           Assert.IsTrue(insertResult);

        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Update()
        {
            RandomNumber = new Random().Next(0, 4000);

            var entity = repository.GetById(1);

            entity.SoirNumber = "SoirNumber_" + RandomNumber;
            entity.PartName = "PartName_" + RandomNumber;
            entity.ToolName = "ToolName_" + RandomNumber;

            bool insertResult = repository.Update(entity);

            Assert.IsTrue(insertResult);

        }


    }
}
