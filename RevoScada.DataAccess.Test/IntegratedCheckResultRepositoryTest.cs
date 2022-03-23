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
    public class IntegratedCheckResultRepositoryTest
    {
        IntegratedCheckResultRepository repository;

        [SetUp]
        public void Init()
        {
          repository = new IntegratedCheckResultRepository( TestStaticParameters.ApplicationConfiguration.PostgreSqlConnectionString);
        }

        [Test]
        public void Get_all()
        {
            var count = repository.GetAll().ToList().Count();
            Assert.IsTrue(count >= 0);
        }

        [Test]
        public void Get_by_date()
        {
            DateTime startDate = new DateTime(2020, 4, 1, 0, 0, 0);
            DateTime endDate = new DateTime(2020, 4, 1, 23, 59, 0);

            var filteredList = repository.GetByDate(startDate, endDate).ToList();


            DateTime d = filteredList[0].CheckResultSaveDate;


            Assert.IsTrue(filteredList.Any(x => x.CheckResultSaveDate== startDate));

             
        }

        [Test]
        public void Get_by_batchid_and_date()
        {
            DateTime startDate = new DateTime(2020, 4, 1, 0, 0, 0);
            DateTime endDate = new DateTime(2020, 4, 1, 23, 59, 0);

            var filteredList = repository.GetByBatchIdAndDate(1,startDate, endDate);


            Assert.IsTrue(filteredList.Any(x=>x.BatchId==1));
        }


        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Insert()
        {
            var entity = new IntegratedCheckResult {
                ActualValue = 0,
                StartValue = 0,
                FinishValue = 0,
                Deviation = 0,
                RequirementValue = 0,
                BagId = 1,
                BatchId = -5
            };
          
            repository.Insert(entity);
      
        }

    }
}
