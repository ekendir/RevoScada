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
    public class DisabledPortRepositoryTest
    {
        DisabledPortRepository repository; 
        int RandomNumber;

        [SetUp]
        public void Init()
        {

            repository = new DisabledPortRepository(TestStaticParameters.ApplicationConfiguration.PostgreSqlConnectionString);
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
        public void Get_all_by_batchId()
        {
            var entity = repository.GetById(2);

            Assert.IsTrue(entity.id == 2);
        }


        [Test]
        //[Ignore("Insert-update-delete tests ignored")]
        public void Insert()
        {
            RandomNumber = new Random().Next(0, 4000);

            DateTime receivedDate = DateTime.Now;
            receivedDate = new DateTime(receivedDate.Year, receivedDate.Month, receivedDate.Day, receivedDate.Hour, receivedDate.Minute, 0, 0);


            DisabledPort entity = new DisabledPort();
            entity.BatchId = 11;
            entity.ReceivedDate = receivedDate;
            entity.TagConfigurationList = new[] { 1, 2, 3, 4 };
         
            bool insertResult = repository.Insert(entity);

            Assert.IsTrue(insertResult);

        }

        

    }
}
