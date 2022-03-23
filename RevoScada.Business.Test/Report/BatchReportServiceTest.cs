using System;
using NUnit.Framework;
using RevoScada.Business.Report;
using RevoScada.Configurator;

namespace RevoScada.Business.Test.Report
{
    [TestFixture]
    public class BatchReportServiceTest
    {

        BatchReportService _service;

        [SetUp]
        public void Init()
        {
           
            _service = new BatchReportService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
        }


        [Test]
        public void BatchDetails()
        {
            var batchList = _service.BatchDetail(28);

        }

        [Test]
        public void BatchLotDetails()
        {
            var batchLotList = _service.BatchDetailLotProperties(9);
        }


        [Test]
        public void BatchReport()
        {
            var batchLotList = _service.BatchReport(67);
        }




    }
}
