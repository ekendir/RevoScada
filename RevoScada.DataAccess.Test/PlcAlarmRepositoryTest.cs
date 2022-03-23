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
    public class PlcAlarmRepositoryTest
    {
        PlcAlarmRepository repository; 

        [SetUp]
        public void Init()
        {
            repository = new PlcAlarmRepository(TestStaticParameters.ApplicationConfiguration.PostgreSqlConnectionString);
        }


        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Insert()
        {
            PlcAlarm entity = new PlcAlarm();
            entity.TagConfigurationId = 2938;
            entity.Status = "I";
            entity.InDateTime = DateTime.Now;
            //entity.OutDateTime =;
            //entity.AcknowledgedDateTime =;
            entity.BatchId = -5;
            bool insertResult = repository.Insert(entity);

            Assert.IsTrue(insertResult);

        }
        [Test]
        [TestCase(67)]
        [Ignore("Insert-update-delete tests ignored")]
        public void Get_byBatchId(int batchId)
        {
            var entity = repository.GetAllBySqlQuery($"SELECT * FROM  public.\"PlcAlarms\" WHERE  \"BatchId\" ={batchId};");

            Assert.IsTrue(entity.Count() > 0);
        }

    }
}
