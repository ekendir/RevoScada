using NUnit.Framework;
using NUnit.Framework.Constraints;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp.RuntimeBinder;

namespace RevoScada.Business.Test
{
   

    [TestFixture]
    public class BagServiceTest
    {
        private BagService _service;
        int RandomNumber;

        [SetUp]
        public void Init()
        {
            _service = new BagService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
        }

        [Test]
        public void Get_all_byid()
        {
            var entity = _service.GetById(2);

            Assert.IsTrue(entity.id == 2);
        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Insert()
        {

            Bag entity = new Bag();
            entity.BatchId = 22;
            entity.SelectedPorts = new int[] { 1, 2, 6, 9, 11 };
            entity.BagName = "BagName A";
            bool insertResult = _service.Insert(entity);

            Assert.IsTrue(insertResult);

        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Update()
        {
            RandomNumber = new Random().Next(0, 4000);

            var entity = _service.GetById(1);

            entity.BagName = "value" + RandomNumber;

            bool insertResult = _service.Update(entity);

            Assert.IsTrue(insertResult);

        }

        [Test]
        public void Delete()
        {
            // Do not forget to change the bag id value with the available one.
            int id = 36;
            Bag entity = _service.GetById(id);

            bool deleteResult = _service.Delete(entity);

            Assert.IsTrue(deleteResult);
        }


        [Test]
        [TestCase(120)]
        public void BagsByBatch(int batchId)
        {
            var  bags = _service.BagsByBatch(batchId)?.ToList();

            if (bags != null && bags.Count > 0)
            {
                Assert.True(bags[0].BatchId == batchId);

            }
        }
         


        [TearDown]
        public void Closing()
        {
        }
    }


}
