using System;
using NUnit.Framework;
using RevoScada.Business.Report;
using RevoScada.Configurator;

namespace RevoScada.Business.Test.Report
{
    [TestFixture]
    public class NumericReportServiceTest
    {

        NumericReportService _service;

        [SetUp]
        public void Init()
        {
           
            _service = new NumericReportService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
        }


        [Test]
        public void BatchNumericReport()
        {
       // _service.BatchNumericReport(313, 5, 1);
         var d=   _service.BatchNumericReport(313);


        }

        [Test]
        public void NumericReportByBag()
        {
            _service.NumericReportByBag(313, 417, 1000, 1);

        }



    }
}
