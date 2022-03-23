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
    public class RecipeGroupServiceTest
    {
        private RecipeGroupService _service;

        [SetUp]
        public void Init()
        {
           
            _service = new RecipeGroupService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
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
            var entity = _service.GetById(2);

        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Insert()
        {
            RecipeGroup entity = new RecipeGroup();
            entity.GroupName = "groupname1";

            bool insertResult = _service.Insert(entity);

            Assert.IsTrue(insertResult);
        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Update()
        {
            var entity = _service.GetById(1);

            entity.GroupName = "groupname" + DateTime.Now.Millisecond;

            bool insertResult = _service.Update(entity);

            Assert.IsTrue(insertResult);
        }

        [TearDown]
        public void Closing()
        {
        }
    }

}
