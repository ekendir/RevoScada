using NUnit.Framework;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.Entities;
using System;
using System.Linq;

namespace RevoScada.Business.Test
{
    [TestFixture]
    public class BatchQualityRepositoryTest
    {
        BatchQualityService _service;
        

        [SetUp]
        public void Init()
        {
           _service = new BatchQualityService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
        }

        [Test]
        public void Get_all()
        {
            var entity = _service.GetAll().FirstOrDefault();

            Assert.IsTrue(entity.id == 1);
        }

        [Test]
        public void Get_all_byid()
        {
            var entity = _service.GetById(1);

            Assert.IsTrue(entity.id == 1);
        }

        [Test]
        [Ignore("Insert test")] 
        public void Insert()
        {

            BatchQuality entity = new BatchQuality
            {
                CardName = "test card name 1",
                Description = "test description",
                LastModified = DateTime.Now,
                SortOrder = 1
            };


           bool insertResult = _service.Insert(entity);
           
           Assert.IsTrue(insertResult);

        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Update()
        {
            DateTime modifiedDate = DateTime.Now;

            var entity=_service.GetById(1);

            entity.LastModified = modifiedDate;

            bool updateResult = _service.Update(entity);

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


            bool insertResult = _service.Insert(entity);
            bool deleteResult = _service.Delete(entity);

            Assert.IsTrue(insertResult);
            Assert.IsTrue(deleteResult);

        }

    }
}
