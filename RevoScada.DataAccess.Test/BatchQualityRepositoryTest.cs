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
    public class BatchQualityRepositoryTest
    {
        BatchQualityRepository repository;
        

        [SetUp]
        public void Init()
        {
          repository = new BatchQualityRepository(TestStaticParameters.ApplicationConfiguration.PostgreSqlConnectionString);
        }

        [Test]
        public void Get_all()
        {
            var entity = repository.GetAll().FirstOrDefault();

            Assert.IsTrue(entity.id == 1);
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

            BatchQuality entity = new BatchQuality
            {
                CardName = "test card name 1",
                Description = "test description",
                LastModified = DateTime.Now,
                SortOrder = 1
            };


           bool insertResult = repository.Insert(entity);
           
           Assert.IsTrue(insertResult);

        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Update()
        {
            DateTime modifiedDate = DateTime.Now;

            var entity=repository.GetById(1);

            entity.LastModified = modifiedDate;

            bool updateResult = repository.Update(entity);

            Assert.IsTrue(updateResult);

        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Delete()
        {
            BatchQuality entity = new BatchQuality
            {
                CardName = "test card name 1",
                Description = "test description",
                LastModified = DateTime.Now,
                SortOrder = 1,
                id=0
            };


            bool insertResult = repository.Insert(entity);
            bool deleteResult = repository.Delete(entity);

            Assert.IsTrue(insertResult);
            Assert.IsTrue(deleteResult);

        }

    }
}
