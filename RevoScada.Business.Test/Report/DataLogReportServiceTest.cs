using System;
using System.Data;
using NUnit.Framework;
using RevoScada.Business.Report;
using RevoScada.Configurator;


namespace RevoScada.Business.Test.Report
{
    [TestFixture]
    public class DataLogReportServiceTest
    {

        DataLogReportService _service;

        [SetUp]
        public void Init()
        {
           
            _service = new DataLogReportService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
        }


        [Test]
        public void DataLogDetails()
        {
            var logList = _service.DataLogDetail(305);

        }

        [Test]
        public void GetAllNumericReportByBatch()
        {

            DataTable logList = _service.GetAllNumericReportByBatch(313, 20, 1);

            Assert.IsTrue(logList.Rows.Count == 20);

        }

        [Test]
        public void GetAllNumericReportByBag()
        {

            DataTable logList = _service.GetAllNumericReportByBag(417, 20, 1);

            Assert.IsTrue(logList.Rows.Count == 20);

        }

    }
}
