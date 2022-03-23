using NUnit.Framework;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.Entities;
using System;
using System.Linq;


namespace RevoScada.Business.Test
{
   

    [TestFixture]
    public class BatchServiceTest
    {
        private BatchService _service;

        [SetUp]
        public void Init()
        {
            _service = new BatchService(TestStaticParameters.ApplicationConfigurations.Configuration.PostgreSqlConnectionString);
        }

      

        [Test]
        public void Get_all_by_id()
        {
            var entity = _service.GetById(2299);

          bool d=  entity.StartDate == DateTime.MinValue;


            Assert.IsTrue(entity.id == 2);
        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Insert()
        {
            Batch entity = new Batch();
            entity.RecipeId = 1;
            entity.StartDate = DateTime.Now;
            entity.EndDate = DateTime.Now;
            entity.Status = 0;
            entity.LoadNumber = "Co1923";
            entity.BatchGroupId = 99;


            bool insertResult =   _service.Insert(entity);

            Assert.IsTrue(insertResult);
        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Update()
        {
             
            Batch entity = new Batch();
            entity.RecipeId = 1;
            entity.StartDate = DateTime.Now;
            entity.EndDate = DateTime.Now;
            entity.Status = 0;
            entity.LoadNumber = "COCOCO";
            entity.id = 3;
            entity.BatchGroupId = 99;


            bool insertResult = _service.Update(entity);

            Assert.IsTrue(insertResult);
 
        }

        [Test]
        [Ignore("Insert-update-delete tests ignored")]
        public void Delete()
        {
            // Do not forget to change the batch id value with the available one.
            int id = 53;
            Batch entity = _service.GetById(id);

            bool deleteResult = _service.Delete(entity);

            Assert.IsTrue(deleteResult);
        }


     

        [Test]
        public void Search_batch_and_recipename()
        {
            var searchResult = _service.SearchBatchAndRecipeName("CO1", 120).ToList();
            var searchResult2 = _service.SearchBatchAndRecipeName("JSF", 120).ToList();

            Assert.IsTrue(searchResult[0].LoadNumber.Contains("CO1"));
            Assert.IsTrue(searchResult2[0].RecipeName.Contains("JSF"));

        }


        [Test]
        public void Search_batch_and_recipename_empty_search()
        {
            var searchResult = _service.SearchBatchAndRecipeName("calib", 120).ToList();

            Assert.IsTrue(searchResult[0].LoadNumber.Contains("CO1"));

        }



        [Test]
        public void GetBatches()
        {
            var list = _service.GetBatches(5);
        }

        
        [Test]
        public void Get_last_completed_process()
        {
            Batch batch = _service.GetLastCompleted();
        }

        [Test]
        [TestCase("AC7-12")]
        [TestCase("AC7-12-R433")]
        public void Get_max_revision(string formattedLoadNumber)
        {
            int revision = _service.GetMaxRevisionNumber(formattedLoadNumber);
            Assert.IsTrue(revision >= 0);
        }

        [Test]
        [TestCase("AC7",21,1)]
        public void Rename_load_number(string furnaceName, int newLoadNumberSerial, int batchId)
        {
            Batch existingBatch = _service.GetById(1);
             _service.RenameLoadNumber(furnaceName,newLoadNumberSerial, existingBatch);
        }

        [TearDown]
        public void Closing()
        {
        }
    }


}
