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
    public class PredefinedRecipeFieldRepositoryTest
    {
        PredefinedRecipeFieldRepository repository;
        

        [SetUp]
        public void Init()
        {
          repository = new PredefinedRecipeFieldRepository(TestStaticParameters.ApplicationConfiguration.PostgreSqlConnectionString);
        }

        [Test]
        public void Get_all_byid()
        {
            var entity = repository.GetById(2);

            Assert.IsTrue(entity.id == 2);
        }

        [Test]
        public void Get_all()
        {
            int count = repository.GetAll().Count();

            Assert.IsTrue(count > 2);
        }




    }
}
