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
    public class RecipeFieldRepositoryTest
    {

        RecipeFieldRepository repository; int RandomNumber;

        [SetUp]
        public void Init()
        {
            repository = new RecipeFieldRepository(TestStaticParameters.ApplicationConfiguration.PostgreSqlConnectionString);
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
            var entity = repository.GetById(1);

            Assert.IsTrue(entity.id == 1);
        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Insert()
        {

            RecipeField entity = new RecipeField();
            entity.RecipeFieldName = "name1";
            entity.RecipeFieldOrder = 1;
            entity.IsActive = false;

            bool insertResult = repository.Insert(entity);

            Assert.IsTrue(insertResult);
        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Update()
        {
            RandomNumber = new Random().Next(0, 240);

            var entity = repository.GetById(1);

            entity.RecipeFieldOrder = Convert.ToInt16(RandomNumber);

            bool insertResult = repository.Update(entity);

            Assert.IsTrue(insertResult);
        }
    }
}