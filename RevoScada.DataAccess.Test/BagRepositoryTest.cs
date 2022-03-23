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
    public class BagRepositoryTest
    {
        BagRepository repository; int RandomNumber;

        [SetUp]
        public void Init()
        {
            repository = new BagRepository(TestStaticParameters.ApplicationConfiguration.PostgreSqlConnectionString);
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
            
           Bag entity = new Bag();
           entity.BatchId = 22;
           entity.SelectedPorts =new int[] {1,2,6,9,11};
            entity.BagName = "BagName A";
           bool insertResult = repository.Insert(entity);

            Assert.IsTrue(insertResult);

        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Delete()
        {
            // Do not forget to change the bag id value with the available one.
            int id = 41;
            Bag entity = repository.GetById(id);

            bool deleteResult = repository.Delete(entity);

            Assert.IsTrue(deleteResult);
        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Update()
        {
            RandomNumber = new Random().Next(0, 4000);

            var entity = repository.GetById(1);

            entity.BagName =  "value" + RandomNumber;

            bool insertResult = repository.Update(entity);

            Assert.IsTrue(insertResult);

        }


    }
}
