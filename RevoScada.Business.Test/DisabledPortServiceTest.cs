using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using RevoScada.Business;

namespace RevoScada.Business.Test
{
    [TestFixture]
    public class DisabledPortServiceTest
    {
        private DisabledPortService _service;

        [SetUp]
        public void Init()
        {
             _service = new DisabledPortService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
        }

        [Test]
        [TestCase(7)]
        public void GetByBatchGroupedByReceivedDate(int batchId)
        {
         
            var list = _service.GetByBatchGroupedByReceivedDate(batchId);
        }

        [Test]
        [TestCase(7, "2021-07-13 13:59:00")]
        public void GetPortListByBatchAndDate(int batchId,string date)
        {
            var list = _service.GetPortListByBatchAndDate(batchId, Convert.ToDateTime(date));
            var list2 = _service.GetPortListByBatchAndDateWithPortName(batchId, Convert.ToDateTime(date));

        }

        [TearDown]
        public void Closing()
        {
        }
    }
}
