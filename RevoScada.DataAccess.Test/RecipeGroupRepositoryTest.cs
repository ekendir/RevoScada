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
    public class RecipeGroupRepositoryTest
    {
        RecipeGroupRepository repository; int RandomNumber;

        [SetUp]
        public void Init()
        {
            repository = new RecipeGroupRepository(TestStaticParameters.ApplicationConfiguration.PostgreSqlConnectionString);
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
            RecipeGroup entity = new RecipeGroup();
            entity.GroupName = "groupname1";
            bool insertResult = repository.Insert(entity);
            Assert.IsTrue(insertResult);
        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Update()
        {
            RandomNumber = new Random().Next(0, 4000);
            var entity = repository.GetById(1);
            entity.GroupName = "groupname" + RandomNumber;
            bool insertResult = repository.Update(entity);
            Assert.IsTrue(insertResult);
        }
    }
}
