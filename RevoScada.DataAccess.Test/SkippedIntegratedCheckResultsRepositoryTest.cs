using NUnit.Framework;
using RevoScada.Configurator;
using RevoScada.DataAccess.Concrete.Postgresql;
using RevoScada.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DataAccess.Test
{
    [TestFixture]
    public class SkippedIntegratedCheckResultsRepositoryTest
    {
        SkippedIntegratedCheckResultRepository repository;

        [SetUp]
        public void Init()
        {
          repository = new SkippedIntegratedCheckResultRepository(TestStaticParameters.ApplicationConfiguration.PostgreSqlConnectionString);
        }

        [Test]
        public void Get_by_date()
        {
            DateTime startDate = new DateTime(2020, 4, 1, 0, 0, 0);
            DateTime endDate = new DateTime(2020, 4, 1, 23, 59, 0);
            var filteredList = repository.GetByDate(startDate, endDate).ToList();
            DateTime d = filteredList[0].SkipDate;
            Assert.IsTrue(filteredList.Any(x => x.SkipDate == startDate));
        }

        [Test]
        public void Get_by_batchid_and_date()
        {
            DateTime startDate = new DateTime(2020, 4, 1, 0, 0, 0);
            DateTime endDate = new DateTime(2020, 4, 1, 23, 59, 0);

            var filteredList = repository.GetByBatchIdAndDate(0, startDate, endDate);

            Assert.IsTrue(filteredList.Any(x => x.BatchId == 0));
        } 
    }
}
