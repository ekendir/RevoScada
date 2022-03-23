using RevoScada.Entities.Complex.Report;


namespace RevoScada.Business.Report
{
    public class NumericReportService
    {
        private string _connectionString { get; set; }

        public NumericReportService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public BatchNumericReportModel BatchNumericReport(int batchId,int numericReportPageSize, int numericReportPage)
        {
            ReportHeaderInfoService reportHeaderInfoService = new ReportHeaderInfoService(_connectionString);

            IntegratedCheckReportService integratedCheckReportService = new IntegratedCheckReportService(_connectionString);
 
            DataLogReportService dataLogService = new DataLogReportService(_connectionString);

            BatchNumericReportModel batchNumericReportModel = new BatchNumericReportModel();

            BagService bagService = new BagService(_connectionString);


            batchNumericReportModel.NumericReportHeaderInfo = reportHeaderInfoService.NumericReportHeaderInfo(batchId);
            batchNumericReportModel.IntegratedCheckReportItems = integratedCheckReportService.IntegratedChecksByBatch(batchId);
            batchNumericReportModel.SkippedIntegratedCheckReportItem = integratedCheckReportService.SkipIntegratedCheckDetail(batchId);
            
            batchNumericReportModel.NumericDataTable = dataLogService.GetAllNumericReportByBatch(batchId, numericReportPageSize, numericReportPage);
            batchNumericReportModel.Bags = bagService.BagsByBatch(batchId);

            if(batchNumericReportModel.NumericDataTable != null)
                batchNumericReportModel.NumericDataTable.TableName = "BatchNumericReportTable";

            return batchNumericReportModel;
        }

        public BatchNumericReportModel BatchNumericReport(int batchId)
        {
            BatchNumericReportModel batchNumericReportModel = new BatchNumericReportModel();

            batchNumericReportModel = BatchNumericReport(batchId, 1000000, 1);
            return batchNumericReportModel;
        }


        public BagNumericReportModel NumericReportByBag(int batchId, int bagId, int numericReportPageSize, int numericReportPage)
        {
            ReportHeaderInfoService reportHeaderInfoService = new ReportHeaderInfoService(_connectionString);

            IntegratedCheckReportService integratedCheckReportService = new IntegratedCheckReportService(_connectionString);


            DataLogReportService dataLogService = new DataLogReportService(_connectionString);


            BagNumericReportModel bagNumericReportModel = new BagNumericReportModel();

            bagNumericReportModel.NumericReportHeaderInfo = reportHeaderInfoService.NumericReportHeaderInfoByBag(batchId, bagId);
            bagNumericReportModel.IntegratedCheckReportItems = integratedCheckReportService.IntegratedChecksByBag(batchId, bagId);
            bagNumericReportModel.SkippedIntegratedCheckReportItem = integratedCheckReportService.SkipIntegratedCheckDetail(batchId);

            bagNumericReportModel.NumericDataTable = dataLogService.GetAllNumericReportByBag(bagId, numericReportPageSize, numericReportPage);

            return bagNumericReportModel;
        }

        public BagNumericReportModel NumericReportByBag(int batchId,int bagId)
        {
         

            BagNumericReportModel bagNumericReportModel = new BagNumericReportModel();
            bagNumericReportModel = NumericReportByBag(batchId,bagId, 1000000, 1);
            return bagNumericReportModel;
        }

    }
}