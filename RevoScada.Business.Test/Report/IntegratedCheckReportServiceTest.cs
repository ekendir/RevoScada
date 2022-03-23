using System;
using NUnit.Framework;
using RevoScada.Business.Report;
using RevoScada.Configurator;

namespace RevoScada.Business.Test.Report
{
    [TestFixture]
    public class IntegratedCheckReportServiceTest
    {

        IntegratedCheckReportService _service;

        [SetUp]
        public void Init()
        {
           
            _service = new IntegratedCheckReportService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
        }


        [Test]
        public void CheckDetails()
        {
            var batchList = _service.IntegratedChecksByBatch(305);

        }

    }
}
