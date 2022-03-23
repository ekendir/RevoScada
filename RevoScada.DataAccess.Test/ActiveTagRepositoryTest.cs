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
    public class ActiveTagRepositoryTest
    {
        ActiveTagRepository repository;
        int RandomNumber;

        [SetUp]
        public void Init()
        {
          ApplicationConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.Files\Configuration\Test_DesktopApplication.rsconfig",true);
          repository = new ActiveTagRepository(TestStaticParameters.ApplicationConfiguration.PostgreSqlConnectionString);
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
           ActiveTag entity = new ActiveTag();
            entity.id = 1;
            entity.TagName = "PTC1";
           entity.IsLogData=false;
           
           bool insertResult = repository.Insert(entity);
           Assert.IsTrue(insertResult);

        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Update()
        {
            RandomNumber = new Random().Next(0, 4000);

            var entity = repository.GetById(1);

            entity.TagName =  "PTC" + RandomNumber;

            bool insertResult = repository.Update(entity);

            Assert.IsTrue(insertResult);

        }


    }
}
