using RevoScada.Entities.Complex.Report;
using System;
using System.Data;

namespace RevoScada.Business.Report
{
    public class TrendReportService
    {
        private string _connectionString { get; set; }

        public TrendReportService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable BatchNumericReport(int batchId)
        {
         
            DataLogReportService dataLogService = new DataLogReportService(_connectionString);

            DataTable dataTable= dataLogService.GetAllNumericReportByBatch(batchId, 1000000, 1);

            if (dataTable != null)
            {
                dataTable.TableName = "TrendTable";
            }
           
            return dataTable;
        }

        public DataTable BatchNumericReport(int batchId,DateTime processStartDate, DateTime startDate, DateTime endDate)
        {

            DataLogReportService dataLogService = new DataLogReportService(_connectionString);

            DataTable dataTable = dataLogService.GetAllNumericReportByBatch(batchId, processStartDate, startDate,endDate);

            if (dataTable != null)
            {
                dataTable.TableName = "TrendTable";
            }
 
            return dataTable;
        }

    }
}