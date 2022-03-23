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
    public class BatchRepositoryTest
    {
       
        BatchRepository repository;
        int RandomNumber; 

        [SetUp]
        public void Init()
        {
           repository = new BatchRepository(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionStrings[1]);
        }

        [Test]
        public void Get_all()
        {
            var count = repository.GetAll().ToList().Count();
            Assert.IsTrue(count >= 0);
        }

        [Test]
        public void Get_all_by_id()
        {
            var entity = repository.GetById(2);

            Assert.IsTrue(entity.id == 2);
        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Insert()
        {
            Batch entity = new Batch();
            entity.RecipeId = 1;
            entity.StartDate = DateTime.Now;
            entity.EndDate = DateTime.Now;
            entity.Status = 0;
            entity.LoadNumber = "Co1923";
            entity.id = 98669;

            bool insertResult = repository.Insert(entity);

            Assert.IsTrue(insertResult);
        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Update()
        {
            RandomNumber = new Random().Next(0, 4000);

            Batch entity = new Batch();
            entity.RecipeId = 1;
            entity.StartDate = DateTime.Now;
            entity.EndDate = DateTime.Now;
            entity.Status = 0;
            entity.LoadNumber = "COCO"+RandomNumber;
            entity.id = 3;


            bool insertResult = repository.Update(entity);

            Assert.IsTrue(insertResult);

        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Delete()
        {
            // Do not forget to change the batch id value with the available one.
            int id = 52;
            Batch entity = repository.GetById(id);

            bool deleteResult = repository.Delete(entity);

            Assert.IsTrue(deleteResult);
        }


        [Test]
        public void GetNextId()
        {
            int d = (int)repository.GetNextId(); 
            
        }

    }
}
