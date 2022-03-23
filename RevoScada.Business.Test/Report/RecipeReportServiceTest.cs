using NUnit.Framework;
using RevoScada.Business.Report;
using RevoScada.Business.Test;

namespace RevoScada.Business.Report.Test
{

    [TestFixture]
    public class RecipeReportServiceTest
    {
        RecipeReportService _service;

        [SetUp]
        public void Init()
        {
           
            _service = new RecipeReportService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
        }

        //[Test]
        //public void RecipeDetail()
        //{
        //    var list = _service.RecipeDetail(1).ToList();
        //}

        [Test]
        public void RecipeDetailFieldName()
        {
            var listFieldName = _service.RecipeDetailFieldName();
        }

        [Test]
        public void RecipeReport()
        {
            var listFieldName = _service.RecipeReport(350);
        }


        [TearDown]
        public void Closing()
        {
        }
    }
}
