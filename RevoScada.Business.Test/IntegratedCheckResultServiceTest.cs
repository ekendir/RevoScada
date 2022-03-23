using NUnit.Framework;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.Entities;
using System;
using System.Linq;

namespace RevoScada.Business.Test
{

    [TestFixture]
    public class IntegratedCheckResultServiceTest
    {
        IntegratedCheckResultService _service;

        [SetUp]
        public void Init()
        {
           
            _service = new IntegratedCheckResultService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
        }
 

        [Test]
        public void Get_by_date()
        {
            DateTime startDate = new DateTime(2020, 4, 1, 0, 0, 0);
            DateTime endDate = new DateTime(2020, 4, 1, 23, 59, 0);

            var filteredList = _service.GetByDate(startDate, endDate).ToList();


            DateTime d = filteredList[0].CheckResultSaveDate;


            Assert.IsTrue(filteredList.Any(x => x.CheckResultSaveDate == startDate));

            //Assert.IsTrue(count >= 0);
        }

        [Test]
        public void Get_by_batchid_and_date()
        {
            DateTime startDate = new DateTime(2020, 4, 1, 0, 0, 0);
            DateTime endDate = new DateTime(2020, 4, 1, 23, 59, 0);

            var filteredList = _service.GetByBatchIdAndDate(1, startDate, endDate);


            Assert.IsTrue(filteredList.Any(x => x.BatchId == 1));
        }
 


        [TearDown]
        public void Closing()
        {
        }
    }


}
