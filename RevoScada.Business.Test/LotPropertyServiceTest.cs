using NUnit.Framework;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Business.Test
{
   

    [TestFixture]
    public class LotPropertyServiceTest
    {
        LotPropertyService _service;
        int RandomNumber;

        [SetUp]
        public void Init()
        {
           
            _service = new LotPropertyService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
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
            RandomNumber = new Random().Next(0, 4000);

            LotProperty entity = new LotProperty();
            entity.BagId = 22;
            entity.SoirNumber = "SoirNumber_" + RandomNumber;
            entity.PartName = "PartName_" + RandomNumber;
            entity.ToolName = "ToolName_" + RandomNumber;

            bool insertResult = _service.Insert(entity);

            Assert.IsTrue(insertResult);

        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Update()
        {
            RandomNumber = new Random().Next(0, 4000);

            var entity = _service.GetById(1);

            entity.SoirNumber = "SoirNumber_" + RandomNumber;
            entity.PartName = "PartName_" + RandomNumber;
            entity.ToolName = "ToolName_" + RandomNumber;

            bool insertResult = _service.Update(entity);

            Assert.IsTrue(insertResult);

        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Delete()
        {
            var entity = new LotProperty();
            entity.id = 13;

            bool deleteResult = _service.Delete(entity);
            Assert.IsTrue(deleteResult);
        }



        [Test]
        public void GetByBatchId()
        {
            
            IEnumerable<LotProperty> result = _service.GetByBagId(9);

        }

        [Test]
        public void GetByBagId()
        {

            IEnumerable<LotProperty> result = _service.GetByBagIdListProperties(new List<int> { 9, 11 });

        }



        [TearDown]
        public void Closing()
        {
        }
    }


}
