using NUnit.Framework;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Business.Test
{
   

    [TestFixture]
    public class ProcessEventLogServiceTest
    {
        private ProcessEventLogService _service;

        [SetUp]
        public void Init()
        {
           
            _service = new ProcessEventLogService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
        }
       

        [Test]
        public void Get_byBatchId()
        {
            var events = _service.GetByBatchId(332)?.ToList();

            if (events != null && events.Count > 0)
            {
                Assert.True(events[0].BatchId == 67);
            }
        }


        [Test]
        public void Get_byBatchIdb()
        {
            var events = _service.GetByBatchId(1)?.ToList();

            //if (events != null && events.Count > 0)
            //{
            //    Assert.True(events[0].BatchId == 67);
            //}
        }


        [Test]
        //[Ignore("Insert-update-delete tests ignored")]
        public void Insert()
        {
            ProcessEventLog entity = new ProcessEventLog();
            entity.EventText = "PTC6 Disable";
            entity.CreateDate = DateTime.Now;
            entity.BatchId = 67;
            entity.Type = "Manual";
            bool insertResult = _service.Insert(entity);

            Assert.IsTrue(insertResult);
        }


        [TearDown]
        public void Closing()
        {
        }
    }


}
