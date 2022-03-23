using NUnit.Framework;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.Entities;
using RevoScada.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Business.Test
{
   

    [TestFixture]
    public class PlcAlarmServiceTest
    {
        private PlcAlarmService _service;

        [SetUp]
        public void Init()
        {
           
            _service = new PlcAlarmService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
        }
       

        [Test]
        public void Get_byBatchId()
        {
            var alarms = _service.GetByBatchId(470)?.ToList();

            if (alarms != null && alarms.Count > 0)
            {
                Assert.True(alarms[0].BatchId == 67);
            }
        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Insert()
        {
            
            PlcAlarm plcAlarm = new PlcAlarm {
                AcknowledgedDateTime = DateTime.Now,
                BatchId = -1,
                TagConfigurationId = 0,
                Status =  PlcAlarmStatusType.AIO.ToString(),
                 PlcValue=false,
                  InDateTime = DateTime.Now,
                   OutDateTime = DateTime.Now



            };

            _service.Insert(plcAlarm);


        }


        [TearDown]
        public void Closing()
        {
        }
    }


}
