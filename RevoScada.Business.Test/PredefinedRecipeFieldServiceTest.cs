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
    public class PredefinedRecipeFieldServiceTest
    {
        PredefinedRecipeFieldService _service;

        [SetUp]
        public void Init()
        {
           
            _service = new PredefinedRecipeFieldService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
        }

        [Test]
        public void Get_all()
        {
            var count = _service.GetAll().ToList().Count();
            Assert.IsTrue(count >= 2);
        }

        [Test]
        public void Get_all_byid()
        {
            var entity = _service.GetById(2);

            
        }


        [TearDown]
        public void Closing()
        {
        }
    }


}
