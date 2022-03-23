using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revo.Core;
using RevoScada.Entities;
using RevoScada.Business;

namespace RevoScada.Business.Test
{
    [TestFixture]
    public class ProcessDataIntegrationTest
    {

        #region  
        const string localPostgre = "Server=127.0.0.1;Port=5432;Database=TaiScada;User Id=postgres;Password=0,369*/;Timeout=20;";
        const string serverPostgre = "Server=192.168.1.50;Port=5432;Database=TaiScada;User Id=postgres;Password=0,369*/;Timeout=20;";

        ActiveTagService lActiveTagService;
        BagService lBagService;
        BatchService lBatchService;
        CurrentProcessInfoService lCurrentProcessInfoService;
        DataLogService lDataLogService;
        IntegratedCheckResultService lIntegratedCheckResultService;
        LotPropertyService lLotPropertyService;
        RecipeDetailHistoryService lRecipeDetailHistoryService;
        RecipeDetailService lRecipeDetailService;
        SkippedIntegratedCheckResultService lSkippedIntegratedCheckResultService;
        ProcessEventLogService lProcessEventLogService;
        RecipeService lRecipeService;
        PlcAlarmService lPlcAlarmService;
        ActiveTagService sActiveTagService;
        BagService sBagService;
        BatchService sBatchService;
        CurrentProcessInfoService sCurrentProcessInfoService;
        DataLogService sDataLogService;
        IntegratedCheckResultService sIntegratedCheckResultService;
        LotPropertyService sLotPropertyService;
        RecipeDetailHistoryService sRecipeDetailHistoryService;
        RecipeDetailService sRecipeDetailService;
        SkippedIntegratedCheckResultService sSkippedIntegratedCheckResultService;
        ProcessEventLogService sProcessEventLogService;
        RecipeService sRecipeService;
        PlcAlarmService sPlcAlarmService;


        #endregion

        [SetUp]
        public void Init()
        {
            lActiveTagService = new ActiveTagService(localPostgre);
            lBagService = new BagService(localPostgre);
            lBatchService = new BatchService(localPostgre);
            lCurrentProcessInfoService = new CurrentProcessInfoService(localPostgre);
            lDataLogService = new DataLogService(localPostgre);
            lIntegratedCheckResultService = new IntegratedCheckResultService(localPostgre);
            lLotPropertyService = new LotPropertyService(localPostgre);
            lRecipeDetailHistoryService = new RecipeDetailHistoryService(localPostgre);
            lRecipeDetailService = new RecipeDetailService(localPostgre);
            lSkippedIntegratedCheckResultService = new SkippedIntegratedCheckResultService(localPostgre);
            lProcessEventLogService = new ProcessEventLogService(localPostgre);
            lRecipeService = new RecipeService(localPostgre);
            lPlcAlarmService = new PlcAlarmService(localPostgre);

            sActiveTagService = new ActiveTagService(serverPostgre);
            sBagService = new BagService(serverPostgre);
            sBatchService = new BatchService(serverPostgre);
            sCurrentProcessInfoService = new CurrentProcessInfoService(serverPostgre);
            sDataLogService = new DataLogService(serverPostgre);
            sIntegratedCheckResultService = new IntegratedCheckResultService(serverPostgre);
            sLotPropertyService = new LotPropertyService(serverPostgre);
            sRecipeDetailHistoryService = new RecipeDetailHistoryService(serverPostgre);
            sRecipeDetailService = new RecipeDetailService(serverPostgre);
            sSkippedIntegratedCheckResultService = new SkippedIntegratedCheckResultService(serverPostgre);
            sProcessEventLogService = new ProcessEventLogService(serverPostgre);
            sRecipeService = new RecipeService(serverPostgre);
            sPlcAlarmService = new PlcAlarmService(serverPostgre);


        }

        [Test]
        [TestCase(1, 9)]
        public void CheckDataOnlyAfterFinish(int plcId, int batchId)
        {
            var resultLActiveTagService = lActiveTagService.GetAll().ToArray();
            var resultLBagService = lBagService.BagsByBatch(batchId);
            var resultLBatchService = lBatchService.GetById(batchId);
            var resultLCurrentProcessInfoService = lCurrentProcessInfoService.Get();
            var resultLDataLogService = lDataLogService.GetByBatch(batchId);
            var resultLIntegratedCheckResultService = lIntegratedCheckResultService.GetAllByBatchId(batchId);
            var resultLLotPropertyService = lLotPropertyService.GetByBagIdListProperties(resultLBagService.Select(x => x.id).ToList());
            var resultLRecipeDetailHistoryService = lRecipeDetailHistoryService.GetByBatch(resultLBatchService.RecipeId, resultLBatchService.id);
            var resultLRecipeDetailService = lRecipeDetailService.GetAllByRecipeId(resultLBatchService.RecipeId);
            var resultLSkippedIntegratedCheckResultService = lSkippedIntegratedCheckResultService.GetByBatchId(batchId);
            var resultLProcessEventLogService = lProcessEventLogService.GetByBatchId(batchId);
            var resultLRecipeService = lRecipeService.GetById(resultLBatchService.RecipeId);
            var resultLPlcAlarmService = lPlcAlarmService.GetByBatchId(batchId);

            var resultSActiveTagService = sActiveTagService.GetAll().ToArray();
            var resultSBagService = sBagService.BagsByBatch(batchId);
            var resultSBatchService = sBatchService.GetById(batchId);
            var resultSCurrentProcessInfoService = sCurrentProcessInfoService.Get();
            var resultSDataLogService = sDataLogService.GetByBatch(batchId);
            var resultSIntegratedCheckResultService = sIntegratedCheckResultService.GetAllByBatchId(batchId);
            var resultSLotPropertyService = sLotPropertyService.GetByBagIdListProperties(resultSBagService.Select(x => x.id).ToList());
            var resultSRecipeDetailHistoryService = sRecipeDetailHistoryService.GetByBatch(resultSBatchService.RecipeId, resultSBatchService.id);
            var resultSRecipeDetailService = sRecipeDetailService.GetAllByRecipeId(resultSBatchService.RecipeId);
            var resultSSkippedIntegratedCheckResultService = sSkippedIntegratedCheckResultService.GetByBatchId(batchId);
            var resultSProcessEventLogService = sProcessEventLogService.GetByBatchId(batchId);
            var resultSRecipeService = sRecipeService.GetById(resultSBatchService.RecipeId);
            var resultSPlcAlarmService = sPlcAlarmService.GetByBatchId(batchId);

            resultLActiveTagService = resultLActiveTagService.OrderBy(x => x.id).ToArray();
            resultSActiveTagService = resultSActiveTagService.OrderBy(x => x.id).ToArray();

            // Assert.IsTrue(resultLActiveTagService.EqualTo(resultSActiveTagService));
            Assert.IsTrue(resultLBagService.EqualTo(resultSBagService));
            resultLBatchService.CreatedByUserId = resultSBatchService.CreatedByUserId;
            resultLBatchService.ModifiedByUserId = resultSBatchService.ModifiedByUserId;

            Assert.IsTrue(resultLBatchService.EqualTo(resultSBatchService));
            //  Assert.IsTrue(resultLCurrentProcessInfoService.EqualTo(resultSCurrentProcessInfoService));


            DateTime date = DateTime.MinValue;
            var revisedLDataLog = new List<DataLog>();

            foreach (var dataLogItem in resultLDataLogService)
            {
                dataLogItem.id = 0;
                dataLogItem.ReceivedDate = date;
                dataLogItem.TagValue = 0;
                revisedLDataLog.Add(dataLogItem);
            }
            var revisedSDataLog = new List<DataLog>();

            foreach (var dataLogItem in resultSDataLogService)
            {
                dataLogItem.id = 0;
                dataLogItem.ReceivedDate = date;
                dataLogItem.TagValue = 0;
                revisedSDataLog.Add(dataLogItem);
            }

            revisedLDataLog = revisedLDataLog.OrderBy(x => x.TagConfigurationId).ToList();
            revisedSDataLog = revisedSDataLog.OrderBy(x => x.TagConfigurationId).ToList();

            Assert.IsTrue(resultLDataLogService.Count() == resultSDataLogService.Count());
            Assert.IsTrue(revisedLDataLog.EqualTo(revisedSDataLog));
            Assert.IsTrue(resultLIntegratedCheckResultService.EqualTo(resultSIntegratedCheckResultService));
          
            Assert.IsTrue(resultLLotPropertyService.EqualTo(resultSLotPropertyService));

            resultLRecipeDetailHistoryService = resultLRecipeDetailHistoryService.OrderBy(x => x.id).ToList();
            resultSRecipeDetailHistoryService = resultSRecipeDetailHistoryService.OrderBy(x => x.id).ToList();
            Assert.IsTrue(resultLRecipeDetailHistoryService.EqualTo(resultSRecipeDetailHistoryService));

            resultLRecipeDetailService = resultLRecipeDetailService.OrderBy(x => x.id).ToList();
            resultSRecipeDetailService = resultSRecipeDetailService.OrderBy(x => x.id).ToList();
            Assert.IsTrue(resultLRecipeDetailService.EqualTo(resultSRecipeDetailService));
            Assert.IsTrue((resultLSkippedIntegratedCheckResultService ?? new SkippedIntegratedCheckResult()).EqualTo(resultSSkippedIntegratedCheckResultService?? new SkippedIntegratedCheckResult()));

            resultLRecipeService.LastRunDate = DateTime.MinValue;
            resultSRecipeService.LastRunDate = DateTime.MinValue;
            resultLRecipeService.ModifyDate = DateTime.MinValue;
            resultSRecipeService.ModifyDate = DateTime.MinValue;
            Assert.IsTrue(resultLRecipeService.EqualTo(resultSRecipeService));
            
            resultLPlcAlarmService = resultLPlcAlarmService.OrderBy(x => x.id).ToList();
            resultSPlcAlarmService = resultSPlcAlarmService.OrderBy(x => x.id).ToList();
            Assert.IsTrue(resultLPlcAlarmService.EqualTo(resultSPlcAlarmService));

            var resultLProcessEventLogServiceArray = resultLProcessEventLogService.OrderBy(x => x.id).Select(x => x.EventText).ToArray();
            var resultSProcessEventLogServiceArray = resultSProcessEventLogService.OrderBy(x => x.id).Select(x => x.EventText).ToArray();
            Assert.IsTrue(resultLProcessEventLogServiceArray.EqualTo(resultSProcessEventLogServiceArray));
        }


        [Test]
        [TestCase(1,  9)]
        public void CheckDataAfterStart(int plcId, int batchId)
        {

            var resultLActiveTagService = lActiveTagService.GetAll().ToArray();
            var resultLBagService = lBagService.BagsByBatch(batchId);
            var resultLBatchService = lBatchService.GetById(batchId);
            var resultLCurrentProcessInfoService = lCurrentProcessInfoService.Get();
            var resultLIntegratedCheckResultService = lIntegratedCheckResultService.GetAllByBatchId(batchId);
            var resultLLotPropertyService = lLotPropertyService.GetByBagIdListProperties(resultLBagService.Select(x => x.id).ToList());
            var resultLRecipeDetailHistoryService = lRecipeDetailHistoryService.GetByBatch(resultLBatchService.RecipeId, resultLBatchService.id);
            var resultLRecipeDetailService = lRecipeDetailService.GetAllByRecipeId(resultLBatchService.RecipeId);
            var resultLSkippedIntegratedCheckResultService = lSkippedIntegratedCheckResultService.GetByBatchId(batchId);
            var resultLRecipeService = lRecipeService.GetById(resultLBatchService.RecipeId);

            var resultSActiveTagService = sActiveTagService.GetAll().ToArray();
            var resultSBagService = sBagService.BagsByBatch(batchId);
            var resultSBatchService = sBatchService.GetById(batchId);
            var resultSCurrentProcessInfoService = sCurrentProcessInfoService.Get();
            var resultSIntegratedCheckResultService = sIntegratedCheckResultService.GetAllByBatchId(batchId);
            var resultSLotPropertyService = sLotPropertyService.GetByBagIdListProperties(resultSBagService.Select(x => x.id).ToList());
            var resultSRecipeDetailHistoryService = sRecipeDetailHistoryService.GetByBatch(resultSBatchService.RecipeId, resultSBatchService.id);
            var resultSRecipeDetailService = sRecipeDetailService.GetAllByRecipeId(resultSBatchService.RecipeId);
            var resultSSkippedIntegratedCheckResultService = sSkippedIntegratedCheckResultService.GetByBatchId(batchId);
            var resultSProcessEventLogService = sProcessEventLogService.GetByBatchId(batchId);
            var resultSRecipeService = sRecipeService.GetById(resultSBatchService.RecipeId);

            resultLActiveTagService = resultLActiveTagService.OrderBy(x => x.id).ToArray();
            resultSActiveTagService = resultSActiveTagService.OrderBy(x => x.id).ToArray();

            Assert.IsTrue(resultLActiveTagService.EqualTo(resultSActiveTagService));
            Assert.IsTrue(resultLBagService.EqualTo(resultSBagService));
            resultLBatchService.CreatedByUserId = resultSBatchService.CreatedByUserId;
            resultLBatchService.ModifiedByUserId = resultSBatchService.ModifiedByUserId;
          //  Assert.IsTrue(resultLBatchService.EqualTo(resultSBatchService));
            Assert.IsTrue(resultLCurrentProcessInfoService.EqualTo(resultSCurrentProcessInfoService));
            Assert.IsTrue(resultLIntegratedCheckResultService.EqualTo(resultSIntegratedCheckResultService));
            Assert.IsTrue(resultLLotPropertyService.EqualTo(resultSLotPropertyService));
            Assert.IsTrue(resultLRecipeDetailHistoryService.EqualTo(resultSRecipeDetailHistoryService));
            resultLRecipeDetailService = resultLRecipeDetailService.OrderBy(x => x.id).ToList();
            resultSRecipeDetailService = resultSRecipeDetailService.OrderBy(x => x.id).ToList();
            Assert.IsTrue(resultLRecipeDetailService.EqualTo(resultSRecipeDetailService));
            Assert.IsTrue((resultLSkippedIntegratedCheckResultService ?? new SkippedIntegratedCheckResult()).EqualTo(resultSSkippedIntegratedCheckResultService ?? new SkippedIntegratedCheckResult()));
            Assert.IsTrue(resultLRecipeService.EqualTo(resultSRecipeService));

        }


        [Test]
        [TestCase(1, 2330)]
        public void CheckDataAfterHold(int plcId, int batchId)
        {
            var resultLBatchService = lBatchService.GetById(batchId);
            var resultLCurrentProcessInfoService = lCurrentProcessInfoService.Get();
            var resultSBatchService = sBatchService.GetById(batchId);
            var resultSCurrentProcessInfoService = sCurrentProcessInfoService.Get();
            var resultLDataLogService = sDataLogService.GetByBatch(batchId);
            var resultSDataLogService = sDataLogService.GetByBatch(batchId);

            Assert.IsTrue(resultLBatchService.EqualTo(resultSBatchService));
            Assert.IsTrue(resultLCurrentProcessInfoService.EqualTo(resultSCurrentProcessInfoService));

            DateTime date = new DateTime(2021, 1, 14, 12, 0, 0);
            var revisedLDataLog = new List<DataLog>();

            foreach (var dataLogItem in resultLDataLogService)
            {
                dataLogItem.id = 0;
                dataLogItem.ReceivedDate = date;
                dataLogItem.TagValue = 0;
                revisedLDataLog.Add(dataLogItem);
            }
            var revisedSDataLog = new List<DataLog>();

            foreach (var dataLogItem in resultSDataLogService)
            {
                dataLogItem.id = 0;
                dataLogItem.ReceivedDate = date;
                dataLogItem.TagValue = 0;
                revisedSDataLog.Add(dataLogItem);
            }

            Assert.IsTrue(resultLDataLogService.Count() == resultSDataLogService.Count());
        }



        [Test]
        [TestCase(275,1231321)]
        [Ignore("")]
        public void DeleteRecipeDetailAndHistories(int recipeId,int batchId)
        {
            //var lRecipeDetails = lRecipeDetailService.GetAllByRecipeId(recipeId);
            //var sRecipeDetails = sRecipeDetailService.GetAllByRecipeId(recipeId);

            //foreach (var item in lRecipeDetails)
            //{
            //    lRecipeDetailService.Delete(item);
            //}
            //foreach (var item in sRecipeDetails)
            //{
            //    sRecipeDetailService.Delete(item);
            //}



            var lRecipeDetailHistories = lRecipeDetailHistoryService.GetByBatch(recipeId, batchId);
            var sRecipeDetailHistories = sRecipeDetailHistoryService.GetByBatch(recipeId, batchId);




        }





        [Test]
        [TestCase(276)]
        public void Compare(int recipeId)
        {
            var lRecipeDetails = lRecipeDetailService.GetAllByRecipeId(recipeId).OrderBy(x => x.id).ThenBy(x => x.RecipeFieldId).ThenBy(x => x.id).ToList();
            var sRecipeDetails = sRecipeDetailService.GetAllByRecipeId(recipeId).OrderBy(x => x.id).ThenBy(x => x.RecipeFieldId).ThenBy(x => x.id).ToList();

            string lid =string.Join(" ",lRecipeDetails.Select(x=>x.id));
            string sid =string.Join(" ",sRecipeDetails.Select(x=>x.id));

            Assert.IsTrue(lid == sid);

            Assert.IsTrue(sRecipeDetails.EqualTo(lRecipeDetails));
        }

        [Test]
        [TestCase(9)]
        public void DatalogCheck(int batchId)
        {
            var resultLDataLogService = lDataLogService.GetByBatch(batchId);
            var resultSDataLogService = sDataLogService.GetByBatch(batchId);

            DateTime date = DateTime.MinValue;
            var revisedLDataLog = new List<DataLog>();

            foreach (var dataLogItem in resultLDataLogService)
            {
                dataLogItem.id = 0;
                dataLogItem.ReceivedDate = date;
                dataLogItem.TagValue = 0;
                revisedLDataLog.Add(dataLogItem);
            }
            
            var revisedSDataLog = new List<DataLog>();
            foreach (var dataLogItem in resultSDataLogService)
            {
                dataLogItem.id = 0;
                dataLogItem.ReceivedDate = date;
                dataLogItem.TagValue = 0;
                revisedSDataLog.Add(dataLogItem);
            }

            revisedLDataLog = revisedLDataLog.OrderBy(x => x.TagConfigurationId).ToList();
            revisedSDataLog = revisedSDataLog.OrderBy(x => x.TagConfigurationId).ToList();


            int lcount = resultLDataLogService.Count();
            int scount = resultSDataLogService.Count();

            Assert.IsTrue(lcount == scount);
            Assert.IsTrue(revisedLDataLog.EqualTo(revisedSDataLog));
        }
    }
}
