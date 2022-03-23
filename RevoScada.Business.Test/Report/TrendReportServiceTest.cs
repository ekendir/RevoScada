using System;
using NUnit.Framework;
using RevoScada.Business.Report;
using RevoScada.Configurator;

namespace RevoScada.Business.Test.Report
{
    [TestFixture]
    public class TrendReportServiceTest
    {

        TrendReportService _service;

        [SetUp]
        public void Init()
        {
           
            _service = new TrendReportService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
       
        }
        [Test]
        public void BatchNumericReport()
        { 
            var datatable = _service.BatchNumericReport(454);
        }

        [Test]
        public void BatchNumericReport_with_date()
        {
            DateTime startDate = DateTime.Now.AddDays(-1).AddHours(-4);
            DateTime endDate = DateTime.Now;
            
            var datatable=   _service.BatchNumericReport(317);


            foreach (var item in datatable.Rows)
            {

            }

        }







    }
}
