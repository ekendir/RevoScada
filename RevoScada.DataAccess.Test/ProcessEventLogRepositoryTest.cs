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
    public class ProcessEventLogRepositoryTest
    {
        ProcessEventLogRepository repository; 

        [SetUp]
        public void Init()
        {
            repository = new ProcessEventLogRepository(TestStaticParameters.ApplicationConfiguration.PostgreSqlConnectionString);
        }


        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Insert()
        {
            ProcessEventLog entity = new ProcessEventLog();
            entity.EventText = "PTC6 Disable";
            entity.CreateDate = DateTime.Now;
            entity.BatchId = 67;
            entity.Type = "Manual";
            bool insertResult = repository.Insert(entity);

            Assert.IsTrue(insertResult);

        }

        [Test]
        [TestCase(67)]
        public void Get_byBatchId(int batchId)
        {
            var entity = repository.GetAllBySqlQuery($"SELECT * FROM  public.\"ProcessEventLogs\" WHERE  \"BatchId\" ={batchId};");

            Assert.IsTrue(entity.Count() > 0);
        }
    }
}
