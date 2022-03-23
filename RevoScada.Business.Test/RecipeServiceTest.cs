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
    public class RecipeServiceTest
    {
        private RecipeService _service;

        [SetUp]
        public void Init()
        {
           
            _service = new RecipeService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
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

            Assert.IsTrue(entity.id == 2);
        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Insert()
        {
            Recipe entity = new Recipe();
            entity.RecipeName = "recipename1";
            entity.Description= "value" + DateTime.Now.Millisecond.ToString();
            entity.RecipeGroupId = 1;
            entity.CreateDate = DateTime.Now;
            entity.ModifyDate = DateTime.Now;
            entity.LastRunDate = DateTime.Now;
            entity.IsActive = true;
                
            bool insertResult = _service.Insert(entity);

            Assert.IsTrue(insertResult);

        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Update()
        {
            var entity = _service.GetById(1);

            entity.Description = "value" + DateTime.Now.Millisecond.ToString();

            bool insertResult = _service.Update(entity);

            Assert.IsTrue(insertResult);

        }


        [TearDown]
        public void Closing()
        {
        }
    }


}
