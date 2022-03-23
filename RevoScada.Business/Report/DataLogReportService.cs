using Revo.Core.Helper;
using RevoScada.DataAccess.Abstract;
using RevoScada.Entities;
using RevoScada.Entities.Complex.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Business.Report
{
    public class DataLogReportService
    {
        private readonly string _connectionString;

        private Dictionary<int, ActiveTag> _activeTags;

        private List<int> _dataLogTagConfigurations;


        public DataLogReportService(string connectionString)
        {
            _connectionString = connectionString;

            ActiveTagService activeTagService = new ActiveTagService(_connectionString);

           //_activeTags = activeTagService.GetAllBySqlQuery("SELECT * FROM public.\"ActiveTags\" WHERE \"ActiveTagGroupId\" != 3").ToDictionary(x => x.id, x => x);
           _activeTags = activeTagService.GetAll().ToDictionary(x => x.id, x => x);

        }


        public IEnumerable<DataLogReportItem> DataLogDetail(int batchId)
        {
            IEnumerable<DataLogReportItem> searchResult = new List<DataLogReportItem>();
            IGenericRepository<DataLogReportItem> repository = new DataAccess.Concrete.Postgresql.DapperGenericPostgreRepository<DataLogReportItem>(_connectionString);
            var queryResult = repository.GetAllBySqlQuery($"SELECT \"ReceivedDate\", \"TagValue\" from public.\"DataLogs\" WHERE \"BatchId\" = {batchId}");
            return queryResult;
        }

        private void InitNumericReportParameters(int batchId)
        {
            if (_dataLogTagConfigurations ==null)
            {
                DataLogService dataLogService = new DataLogService(_connectionString);

                _dataLogTagConfigurations = dataLogService.GetAllBySqlQuery($"SELECT DISTINCT  \"TagConfigurationId\"	FROM public.\"DataLogs\" WHERE  \"BatchId\"={batchId} ").Select(x => x.TagConfigurationId).OrderBy(x => x).ToList();
            }
        }



        public DataTable GetAllNumericReportByBatch(int batchId, int pageSize, int page)
        {

            InitNumericReportParameters(batchId);

            //BagService bagService = new BagService(_connectionString);

            //IEnumerable<Bag> bags = bagService.BagsByBatch(batchId);

            //Dictionary<string, int> selectedSensors = bags.Select(x => x.SelectedPorts).SelectMany(x => x).ToDictionary(x => _activeTags[x].TagName, x => x);

            int loggedRowCountInBatch = _dataLogTagConfigurations.Count();

            //  IEnumerable<DataLog> dataLogs = dataLogService.GetByBatchPaged(batchId, loggedRowCountInBatch, page);

            DataTable numericReportTable = new DataTable();

            DataColumn dataColumn;

            dataColumn = new DataColumn();
            dataColumn.ColumnName = "Mins";
            dataColumn.Caption = "Mins";
            dataColumn.DataType = typeof(double);
            numericReportTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.ColumnName = "Time";
            dataColumn.Caption = "Time";
            dataColumn.DataType = typeof(DateTime);
            numericReportTable.Columns.Add(dataColumn);


            foreach (int item in _dataLogTagConfigurations)
            {
                dataColumn = new DataColumn();
                dataColumn.ColumnName = _activeTags[item].TagName;
                dataColumn.Caption = _activeTags[item].TagName;
                dataColumn.DataType = typeof(float);
                numericReportTable.Columns.Add(dataColumn);
            }

            DataLogService dataLogService = new DataLogService(_connectionString);

            IEnumerable<DataLog> dataLogs = dataLogService.GetByBatchPaged(batchId, pageSize * loggedRowCountInBatch, page);

            if (dataLogs.Count() > 0)
            {
                DateTime firstDate = dataLogs.First().ReceivedDate;
                foreach (var collection in dataLogs.GroupBy(x => x.ReceivedDate))
                {
                    TimeSpan diff = collection.Key - firstDate;
                    var minsAndSeconds= $"{(int)diff.TotalMinutes}.{diff.Seconds:00}";

                    DataRow row;
                    row = numericReportTable.NewRow();
                    row["Mins"] = Convert.ToDouble(minsAndSeconds);
                    row["Time"] = collection.Key;

                    foreach (var dataLogItem in collection.ToList())
                    {
                        if (_activeTags[dataLogItem.TagConfigurationId].TagName.Contains("Press"))
                        {
                            row[_activeTags[dataLogItem.TagConfigurationId].TagName] = String.Format("{0:0.00}", dataLogItem.TagValue);
                        }
                        else { row[_activeTags[dataLogItem.TagConfigurationId].TagName] = String.Format("{0:0.0}", dataLogItem.TagValue);}


                        
                    }

                    numericReportTable.Rows.Add(row);

                }

            }
            else
            {
                numericReportTable = null;
            }
            return numericReportTable;
        }

        public DataTable GetAllNumericReportByBatch(int batchId, DateTime processStartDate, DateTime startDate, DateTime endDate)
        {

            InitNumericReportParameters(batchId);
             
            int loggedRowCountInBatch = _dataLogTagConfigurations.Count();
             
            DataTable numericReportTable = new DataTable();

            DataColumn dataColumn;

            dataColumn = new DataColumn();
            dataColumn.ColumnName = "Mins";
            dataColumn.Caption = "Mins";
            dataColumn.DataType = typeof(ushort);
            numericReportTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.ColumnName = "Time";
            dataColumn.Caption = "Time";
            dataColumn.DataType = typeof(DateTime);
            numericReportTable.Columns.Add(dataColumn);


            foreach (int item in _dataLogTagConfigurations)
            {
                dataColumn = new DataColumn();
                dataColumn.ColumnName = _activeTags[item].TagName;
                dataColumn.Caption = _activeTags[item].TagName;
                dataColumn.DataType = typeof(float);
                numericReportTable.Columns.Add(dataColumn);
            }

            DataLogService dataLogService = new DataLogService(_connectionString);

            IEnumerable<DataLog> dataLogs = dataLogService.GetByBatch(batchId,startDate,endDate);

            if (dataLogs.Count()==0)
            {
                return null;
            }
            
            foreach (var collection in dataLogs.GroupBy(x => x.ReceivedDate))
            {
                TimeSpan diff = collection.Key - processStartDate;

                DataRow row;
                row = numericReportTable.NewRow();
                row["Mins"] = diff.TotalMinutes;
                row["Time"] = collection.Key;

                foreach (var dataLogItem in collection.ToList())
                {
                    row[_activeTags[dataLogItem.TagConfigurationId].TagName] = String.Format("{0:0.0}", dataLogItem.TagValue); 
                }

                numericReportTable.Rows.Add(row);

            }

            return numericReportTable;
        }



        public DataTable GetAllNumericReportByBag(int bagId, int pageSize, int page)
        {
            BagService bagService = new BagService(_connectionString);

            Bag bag = bagService.GetById(bagId);

            Dictionary<int, ActiveTag> defaultSensors = _activeTags.Where(x => x.Value.ActiveTagGroupId == 0 && x.Value.IsLogData==true).ToDictionary(x=>x.Key,x => x.Value);

            // todo:h This section causes exception
            Dictionary<int, ActiveTag> selectedSensors = bag.SelectedPorts.Select(x => x).ToDictionary(x => x, x => _activeTags[x]);
             
            int loggedRowCountInBatch = defaultSensors.Count()+selectedSensors.Count();
         
            DataTable numericReportTable = new DataTable();

            DataColumn dataColumn;

            dataColumn = new DataColumn();
            dataColumn.ColumnName = "Mins";
            dataColumn.Caption = "Mins";
            dataColumn.DataType = typeof(ushort);
            numericReportTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.ColumnName = "Time";
            dataColumn.Caption = "Time";
            dataColumn.DataType = typeof(DateTime);
            numericReportTable.Columns.Add(dataColumn);

            foreach (var item in defaultSensors)
            {
                dataColumn = new DataColumn();
                dataColumn.ColumnName = item.Value.TagName;
                dataColumn.Caption = item.Value.TagName;
                dataColumn.DataType = typeof(float);

               
                numericReportTable.Columns.Add(dataColumn);
            }
            
            foreach (var item in selectedSensors)
            {
                dataColumn = new DataColumn();
                dataColumn.ColumnName = item.Value.TagName;
                dataColumn.Caption = item.Value.TagName;
                dataColumn.DataType = typeof(float);

                if (!item.Value.TagName.Contains("VAC"))
                    numericReportTable.Columns.Add(dataColumn);
            }

            List<int> queryArray = defaultSensors.Select(x => x.Key).Concat(selectedSensors.Select(x => x.Key)).ToList();

            DataLogService dataLogService = new DataLogService(_connectionString);

            IEnumerable<DataLog> dataLogs = dataLogService.GetByBagSensorsPaged(bag.BatchId, queryArray, pageSize * loggedRowCountInBatch, page);

            if (dataLogs == null || !dataLogs.Any())
                return null;

            var firstDate = dataLogs.First().ReceivedDate;

            foreach (var collection in dataLogs.GroupBy(x => x.ReceivedDate))
            {
                TimeSpan diff = collection.Key - firstDate;

                DataRow row;
                row = numericReportTable.NewRow();
                row["Mins"] = diff.TotalMinutes;
                row["Time"] = collection.Key;

                foreach (var dataLogItem in collection.ToList())
                {
                    if (_activeTags[dataLogItem.TagConfigurationId].TagName.Contains("Press")) {
                        row[_activeTags[dataLogItem.TagConfigurationId].TagName] = String.Format("{0:0.0#}", dataLogItem.TagValue);
                    } else {
                        row[_activeTags[dataLogItem.TagConfigurationId].TagName] = String.Format("{0:0.0}", dataLogItem.TagValue);
                    }

                    
                }

                numericReportTable.Rows.Add(row);

            }

          


            return numericReportTable;
        }

    
    }
}