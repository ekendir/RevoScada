using NUnit.Framework;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevoScada.Business.Test
{

    [TestFixture]
    public class ActiveTagServiceTest
    {
        ActiveTagService _service;
        int RandomNumber;

        [SetUp]
        public void Init()
        {
              _service = new ActiveTagService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
        }

        [Test]
        public void Get_all()
        {
            var count = _service.GetAll().ToList().Count();
            Assert.IsTrue(count >= 0);
        }

        [Test]
        public void Get_all_byid()
        {
            var entity = _service.GetById(1647);

            Assert.IsTrue(entity.id == 1647);
        }


      
        [Test]
        public void ActiveTagsByTagNameKey()
        {
            Dictionary<string, ActiveTag> dictionaryResult = _service.GetAll().ToDictionary(x => x.TagName, x => x);

            
        }
        [Test]
        public void ActiveTagsByTagIdKey()
        {
            Dictionary<int, ActiveTag> dictionaryResult = _service.GetAll().ToDictionary(x => x.id, x => x);

            
        }




        //[Test]
        //        public void Insert()
        //        {

        //            ActiveTag entity = new ActiveTag();
        //            entity.TagName = "PTC1555";
        //            entity.IsLogData = false;
        //            entity.id = 9999991;

        //            bool insertResult = _service.Insert(entity);
        //            Assert.IsTrue(insertResult);

        //        }

        [Test]
        public void Update()
        {
            RandomNumber = new Random().Next(0, 4000);

            var entity = _service.GetById(13365);

            entity.IsLogData = false;

            bool insertResult = _service.Update(entity);

            _service.GetAllBySqlQuery("CLUSTER employees USING employees_ind;");

            Assert.IsTrue(insertResult);

        }

 

        [TearDown]
        public void Closing()
        {
        }
    }


}
