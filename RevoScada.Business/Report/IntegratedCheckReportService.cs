using RevoScada.DataAccess.Abstract;
using RevoScada.Entities;
using RevoScada.Entities.Complex.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Business.Report
{
   public class IntegratedCheckReportService
    {
        private string _connectionString { get; set; }

        public IntegratedCheckReportService(string connectionString)
        {
            _connectionString = connectionString;
        }
        public List<IntegratedCheckReportItem> IntegratedChecksByBatch(int batchId)
        {
            //IEnumerable<IntegratedCheckResult> searchResult = new List<IntegratedCheckResult>();
            //IGenericRepository<IntegratedCheckReportItem> repository = new DataAccess.Concrete.Postgresql.DapperGenericPostgreRepository<IntegratedCheckReportItem>(_connectionString);
            List<IntegratedCheckReportItem> integratedCheckReportItems = new List<IntegratedCheckReportItem>();

         
            IntegratedCheckResultService integratedCheckResultService = new IntegratedCheckResultService(_connectionString);
            
            ActiveTagService activeTagService = new ActiveTagService(_connectionString);

            BagService bagService = new BagService(_connectionString);
            var bags = bagService.BagsByBatch(batchId).ToDictionary(x => x.id, x => x);


            var activeTags = activeTagService.ActiveTagsByTagIdKey();


            IEnumerable<IntegratedCheckResult> integratedCheckResults = integratedCheckResultService.GetAllByBatchId(batchId);

            if (integratedCheckResults.Count()==0)
            {
                return null;
            }

            foreach (var integratedCheckResult in integratedCheckResults)
            {
                IntegratedCheckReportItem integratedCheckReportItem = new IntegratedCheckReportItem();
                integratedCheckReportItem.BagName = bags[integratedCheckResult.BagId].BagName;

                if(activeTags.ContainsKey(integratedCheckResult.SensorTagId))
                    integratedCheckReportItem.SelectedSensorName = activeTags[integratedCheckResult.SensorTagId].TagName;

                integratedCheckReportItem.ActualValue = integratedCheckResult.ActualValue;
                integratedCheckReportItem.Deviation = integratedCheckResult.Deviation;
                integratedCheckReportItem.FinishValue = integratedCheckResult.FinishValue;
                integratedCheckReportItem.RequirementValue = integratedCheckResult.RequirementValue;
                integratedCheckReportItem.StartValue = integratedCheckResult.StartValue;

                integratedCheckReportItems.Add(integratedCheckReportItem);
            }


              return integratedCheckReportItems;
        }

        public List<IntegratedCheckReportItem> IntegratedChecksByBag(int batchId, int bagId)
        {
            //IEnumerable<IntegratedCheckResult> searchResult = new List<IntegratedCheckResult>();
            //IGenericRepository<IntegratedCheckReportItem> repository = new DataAccess.Concrete.Postgresql.DapperGenericPostgreRepository<IntegratedCheckReportItem>(_connectionString);
            List<IntegratedCheckReportItem> integratedCheckReportItems = new List<IntegratedCheckReportItem>();


            IntegratedCheckResultService integratedCheckResultService = new IntegratedCheckResultService(_connectionString);

            ActiveTagService activeTagService = new ActiveTagService(_connectionString);

            BagService bagService = new BagService(_connectionString);
            var bags = bagService.BagsByBatch(batchId).ToDictionary(x => x.id, x => x);


            var activeTags = activeTagService.ActiveTagsByTagIdKey();



            IEnumerable<IntegratedCheckResult> integratedCheckResults = integratedCheckResultService.GetAllByBagId(batchId, bagId);

            if (integratedCheckResults.Count() == 0)
            {
                return null;
            }

            foreach (var integratedCheckResult in integratedCheckResults)
            {
                IntegratedCheckReportItem integratedCheckReportItem = new IntegratedCheckReportItem();
                integratedCheckReportItem.BagName = bags[integratedCheckResult.BagId].BagName;
                integratedCheckReportItem.SelectedSensorName = activeTags[integratedCheckResult.SensorTagId].TagName;
                integratedCheckReportItem.ActualValue = integratedCheckResult.ActualValue;
                integratedCheckReportItem.Deviation = integratedCheckResult.Deviation;
                integratedCheckReportItem.FinishValue = integratedCheckResult.FinishValue;
                integratedCheckReportItem.RequirementValue = integratedCheckResult.RequirementValue;
                integratedCheckReportItem.StartValue = integratedCheckResult.StartValue;

                integratedCheckReportItems.Add(integratedCheckReportItem);
            }


            return integratedCheckReportItems;
        }


        public  SkippedIntegratedCheckReportItem SkipIntegratedCheckDetail(int batchId)
        {
            SkippedIntegratedCheckReportItem queryResult;
            try
            {
                IEnumerable< SkippedIntegratedCheckReportItem> searchResult = new List<SkippedIntegratedCheckReportItem>();
                IGenericRepository< SkippedIntegratedCheckReportItem> repository = new DataAccess.Concrete.Postgresql.DapperGenericPostgreRepository<Entities.Complex.Report.SkippedIntegratedCheckReportItem>(_connectionString);
            queryResult = repository.GetAllBySqlQuery($"SELECT \"SkipDate\" FROM public.\"SkippedIntegratedCheckResults\" WHERE \"BatchId\" = {batchId}").First();
           
            }
            catch 
            {

                return null;
            }
        
            
            return queryResult;
        }

    }
}
