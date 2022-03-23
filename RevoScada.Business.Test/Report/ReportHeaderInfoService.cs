using System;
using NUnit.Framework;
using RevoScada.Business.Report;
using RevoScada.Configurator;

namespace RevoScada.Business.Test.Report
{
    [TestFixture]
    public class ReportHeaderInfoServiceTest
    {

        ReportHeaderInfoService _service;

        [SetUp]
        public void Init()
        {
           
            _service = new ReportHeaderInfoService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
        }

        [Test]
        public void ReportHeaderInfo()
        {
            var result = _service.ReportHeaderInfo(67);

            Assert.IsTrue(result.BatchId == 67);

        }

        [Test]
        public void NumericReportHeaderInfo()
        {
            var result = _service.NumericReportHeaderInfo(67);

            Assert.IsTrue(result.BatchId == 67);

        }

        [Test]
        public void NumericReportHeaderInfoByBag()
        {
            var result = _service.NumericReportHeaderInfoByBag(67,55);

            Assert.IsTrue(result.BatchId == 67);

        }

    }
}
