using RevoScada.DataAccess.Abstract;
using RevoScada.Entities;
using RevoScada.Entities.Complex.Report;
using RevoScada.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Business.Report
{
    public class BatchReportService
    {
        private string _connectionString { get; set; }

        public BatchReportService(string connectionString)
        {
            _connectionString = connectionString;

        }
        public IEnumerable<BatchDetailReportItem> BatchDetail(int batchId)
        {
            IEnumerable<BatchDetailReportItem> searchResult = new List<BatchDetailReportItem>();
            IGenericRepository<BatchDetailReportItem> repository = new DataAccess.Concrete.Postgresql.DapperGenericPostgreRepository<BatchDetailReportItem>(_connectionString);
            var queryResult = repository.GetAllBySqlQuery($"SELECT \"LoadNumber\", \"StartDate\", \"EndDate\", \"RecipeId\",\"BagName\", \"SelectedPorts\", bag.\"id\" FROM public.\"Batches\" batch INNER JOIN public.\"Bags\" bag ON batch.\"id\" = bag.\"BatchId\" WHERE batch.\"id\" = {batchId}"); return queryResult;
        }

        public IEnumerable<BatchDetailLotPropertiesReportItem> BatchDetailLotProperties(int bagId)
        {
            IEnumerable<BatchDetailLotPropertiesReportItem> searchResult = new List<BatchDetailLotPropertiesReportItem>();
            IGenericRepository<BatchDetailLotPropertiesReportItem> repository = new DataAccess.Concrete.Postgresql.DapperGenericPostgreRepository<BatchDetailLotPropertiesReportItem>(_connectionString);
            var queryResult = repository.GetAllBySqlQuery($"SELECT \"BagId\",\"SoirNumber\",\"PartName\",\"ToolName\" FROM public.\"LotProperties\" WHERE \"BagId\" ={bagId}");
            return queryResult;
        }

        public BatchReportModel BatchReport(int batchId)
        {
            BatchReportModel batchReportModel = new BatchReportModel();

            IntegratedCheckReportService integratedCheckReportService = new IntegratedCheckReportService(_connectionString);
           
            ReportHeaderInfoService reportHeaderInfoService = new ReportHeaderInfoService(_connectionString);
            BagService bagService = new BagService(_connectionString);
            LotPropertyService lotPropertyService = new LotPropertyService(_connectionString);
            ActiveTagService activeTagService = new ActiveTagService(_connectionString);
            Dictionary<int, ActiveTag> activeTagNames = activeTagService.ActiveTagsByTagIdKey();



            IEnumerable<Bag> bags = bagService.BagsByBatch(batchId);

            List<BagSensorAndPartDetail> bagSensorAndPartDetails = new List<BagSensorAndPartDetail>();

            foreach (Bag bagItem in bags)
            {

                BagSensorAndPartDetail bagSensorAndPartDetail = new BagSensorAndPartDetail();
                bagSensorAndPartDetail.BagId = bagItem.id;
                bagSensorAndPartDetail.BagName = bagItem.BagName;
                bagSensorAndPartDetail.LotProperties = lotPropertyService.GetByBagId(bagItem.id).ToList();

                BagSensors bagSensors = new BagSensors();
                bagSensors.PTCs = bagItem.SelectedPorts.Select(x => activeTagNames[x]).Where(x => x.ActiveTagGroupId ==  ActiveTagGroups.PTC).OrderBy(x => x.id).Select(x => x.TagName).ToList();
                bagSensors.MONs = bagItem.SelectedPorts.Select(x => activeTagNames[x]).Where(x => x.ActiveTagGroupId ==  ActiveTagGroups.MON).OrderBy(x => x.id).Select(x => x.TagName).ToList();
                bagSensors.VACs = bagSensors.MONs.Select(x => x.Replace("MON", "VAC")).ToList();

                bagSensorAndPartDetail.BagSensors = bagSensors;

                bagSensorAndPartDetails.Add(bagSensorAndPartDetail);
            }


            batchReportModel.IntegratedCheckReportItems = integratedCheckReportService.IntegratedChecksByBatch(batchId);
            batchReportModel.SkippedIntegratedCheckReportItem = integratedCheckReportService.SkipIntegratedCheckDetail(batchId);
                        
            batchReportModel.ReportHeaderInfo = reportHeaderInfoService.ReportHeaderInfo(batchId);

            batchReportModel.BagSensorAndPartDetails = bagSensorAndPartDetails;


            return batchReportModel;
        }



    }
}