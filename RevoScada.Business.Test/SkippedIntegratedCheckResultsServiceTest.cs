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
    public class SkippedIntegratedCheckResultsServiceTest
    {
        SkippedIntegratedCheckResultService _service;

        [SetUp]
        public void Init()
        {
           
            _service = new SkippedIntegratedCheckResultService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
        }


        [Test]
        public void Get_by_date()
        {
            DateTime startDate = new DateTime(2020, 4, 1, 0, 0, 0);
            DateTime endDate = new DateTime(2020, 4, 1, 23, 59, 0);
            var filteredList = _service.GetByDate(startDate, endDate).ToList();
            DateTime d = filteredList[0].SkipDate;
            Assert.IsTrue(filteredList.Any(x => x.SkipDate == startDate));
        }

        [Test]
        public void Get_by_batchid_and_date()
        {
            DateTime startDate = new DateTime(2020, 4, 1, 0, 0, 0);
            DateTime endDate = new DateTime(2020, 4, 1, 23, 59, 0);

            var filteredList = _service.GetByBatchIdAndDate(0, startDate, endDate);

            Assert.IsTrue(filteredList.Any(x => x.BatchId == 0));
        }


        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Insert()
        {

            var entity = new SkippedIntegratedCheckResult
            {
                BatchId = 313,
                SkipDate = DateTime.Now
            };
            _service.Insert(entity);
        }



        [TearDown]
        public void Closing()
        {
        }
    }


}
