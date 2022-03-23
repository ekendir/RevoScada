using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using RevoScada.Business;

namespace RevoScada.Business.Test
{
    [TestFixture]
    public class DataLogServiceTest
    {
        private DataLogService _service;

        [SetUp]
        public void Init()
        {
             _service = new DataLogService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
        }

        [Test]
        public void Get_all_byid()
        {
            var entity = _service.GetById(1140943);
            Assert.IsTrue(entity.id == 1140943);
        }

        [Test]
        public void Get_all_by_batchid()
        {
            for (int i = 0; i < 10; i++)
            {
                Stopwatch stopwatch = new Stopwatch(); ;
                stopwatch.Start();
                var list = _service.GetByBatch(67).ToList();
                Console.WriteLine(stopwatch.Elapsed.Seconds);
            }
        }

        [Test]
        public void Get_all_by_batchid_paged()
        {
          var list = _service.GetByBatchPaged(67,10,1).ToList();
        }

        [Test]
        public void Get_all_paged()
        {
            var list = _service.GetAllPaged(10, 1).ToList();
        }

        [Test]
        public void Get_()
        {
            long list = _service.GetMaxIdByBatchId(22400);
        }

        [TearDown]
        public void Closing()
        {
        }
    }
}
