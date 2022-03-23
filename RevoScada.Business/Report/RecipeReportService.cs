using RevoScada.DataAccess.Abstract;
using RevoScada.DataAccess.Concrete.Postgresql;
using RevoScada.Entities;
using RevoScada.Entities.Complex.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Business.Report
{
    public class RecipeReportService
    {
        private string _connectionString { get; set; }
        public RecipeReportService(string connectionString)
        {
            _connectionString = connectionString;
             
        }
        public IEnumerable<RecipeDetailReportItem> RecipeDetail(int recipeId, int batchId)
        {
            IEnumerable<RecipeDetailReportItem> searchResult = new List<RecipeDetailReportItem>();
            IGenericRepository<RecipeDetailReportItem> repository = new DataAccess.Concrete.Postgresql.DapperGenericPostgreRepository<RecipeDetailReportItem>(_connectionString);
            var queryResult = repository.GetAllBySqlQuery($"SELECT re.\"RecipeName\", rd.\"RecipeFieldValue\",rd.\"SegmentNo\" FROM public.\"RecipeDetailHistories\" rd INNER JOIN public.\"Recipes\" re ON rd.\"RecipeId\"= re.id WHERE rd.\"BatchId\"={batchId}");
            return queryResult;
        }
        public IEnumerable<RecipeDetailFieldNameReportItem> RecipeDetailFieldName()
        {
            IEnumerable<RecipeDetailFieldNameReportItem> searchResult = new List<RecipeDetailFieldNameReportItem>();
            IGenericRepository<RecipeDetailFieldNameReportItem> repository = new DataAccess.Concrete.Postgresql.DapperGenericPostgreRepository<RecipeDetailFieldNameReportItem>(_connectionString);
            var queryResult = repository.GetAllBySqlQuery($"SELECT \"RecipeFieldName\", \"RecipeFieldOrder\" FROM public.\"RecipeFields\"");
            return queryResult;
        }
        public ReportHeaderInfo ReportHeaderInfo(int batchId)
        {
            ReportHeaderInfoService reportHeaderInfoService = new ReportHeaderInfoService(_connectionString);
            return reportHeaderInfoService.ReportHeaderInfo(batchId);

        }
        public DataTable RecipeReport(int batchId)
        {
            BatchService batchService = new BatchService(_connectionString);
            int recipeId = batchService.GetById(batchId).RecipeId;

            RecipeDetailHistoryService recipeDetailHistoryService = new RecipeDetailHistoryService(_connectionString);
            int segmentTotalCount = recipeDetailHistoryService.GetSegmentTotal(batchId);
            //int segmentTotalCount = recipeDetailHistoryService.GetSegmentTotal(recipeId);

            RecipeFieldService recipeFieldService = new RecipeFieldService(_connectionString);
            Dictionary<short,RecipeField> recipeFields = recipeFieldService.GetAll().Where(r => r.IsActive).ToList().ToDictionary(x => x.id, x => x);

            DataTable recipeReportTable = new DataTable();
            DataColumn dataColumn;

            dataColumn = new DataColumn();
            dataColumn.ColumnName = "Order";
            dataColumn.Caption = "Order";
            dataColumn.DataType = typeof(int);
            recipeReportTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.ColumnName = "Title";
            dataColumn.Caption = "Title";
            dataColumn.DataType = typeof(string);
            recipeReportTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.ColumnName = "Unit";
            dataColumn.Caption = "Title";
            dataColumn.DataType = typeof(string);
            recipeReportTable.Columns.Add(dataColumn);


            for (int i = 1; i <= segmentTotalCount; i++)
            {
                dataColumn = new DataColumn();
                dataColumn.ColumnName = "Segment" + i;
                dataColumn.Caption = "Segment" + i;
                dataColumn.DataType = typeof(string);
                recipeReportTable.Columns.Add(dataColumn);
            }

            DataTable recipeReportTableOrdered = recipeReportTable.Clone();


            IEnumerable<RecipeDetailHistory> recipeDetails = recipeDetailHistoryService.GetByBatch(recipeId, batchId);

            foreach (var collection in recipeDetails.GroupBy(x => x.RecipeFieldId))
            {
                DataRow row;

                row = recipeReportTable.NewRow();

                // todo:h Check if this logic is true
                if (!recipeFields.ContainsKey(collection.Key))
                    continue;

                row["Order"] = recipeFields[collection.Key].RecipeFieldOrder;
                row["Title"] = recipeFields[collection.Key].RecipeFieldName;

                foreach (var recipeDetailItem in collection.ToList())
                {
                    row["Segment"+recipeDetailItem.SegmentNo] =  recipeDetailItem.RecipeCellValue;
                }

                recipeReportTable.Rows.Add(row);

            }
          
            DataRow[] dataRows = recipeReportTable.Select().OrderBy(u => u["Order"]).ToArray();
             
            recipeReportTableOrdered = dataRows.CopyToDataTable();
            recipeReportTableOrdered.TableName = "RecipeReportTable";

            return recipeReportTableOrdered;
        }

        public DataTable RecipeReportByRecipe(int recipeId)
        {
            RecipeDetailService recipeDetailService = new RecipeDetailService(_connectionString);
            int segmentTotalCount = recipeDetailService.GetSegmentTotal(recipeId);

            RecipeFieldService recipeFieldService = new RecipeFieldService(_connectionString);
            Dictionary<short, RecipeField> recipeFields = recipeFieldService.GetAll().ToList().ToDictionary(x => x.id, x => x);

            DataTable recipeReportTable = new DataTable();
            DataColumn dataColumn;

            dataColumn = new DataColumn();
            dataColumn.ColumnName = "Order";
            dataColumn.Caption = "Order";
            dataColumn.DataType = typeof(int);
            recipeReportTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.ColumnName = "Title";
            dataColumn.Caption = "Title";
            dataColumn.DataType = typeof(string);
            recipeReportTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.ColumnName = "Unit";
            dataColumn.Caption = "Title";
            dataColumn.DataType = typeof(string);
            recipeReportTable.Columns.Add(dataColumn);


            for (int i = 1; i <= segmentTotalCount; i++)
            {
                dataColumn = new DataColumn();
                dataColumn.ColumnName = "Segment" + i;
                dataColumn.Caption = "Segment" + i;
                dataColumn.DataType = typeof(string);
                recipeReportTable.Columns.Add(dataColumn);
            }

            DataTable recipeReportTableOrdered = recipeReportTable.Clone();


            IEnumerable<RecipeDetail> recipeDetails = recipeDetailService.GetAllByRecipeId(recipeId);

            foreach (var collection in recipeDetails.GroupBy(x => x.RecipeFieldId))
            {
                DataRow row;

                row = recipeReportTable.NewRow();

                row["Order"] = recipeFields[collection.Key].RecipeFieldOrder;
                row["Title"] = recipeFields[collection.Key].RecipeFieldName;

                foreach (var recipeDetailItem in collection.ToList())
                {
                    row["Segment" + recipeDetailItem.SegmentNo] = recipeDetailItem.RecipeFieldValue;
                }

                recipeReportTable.Rows.Add(row);

            }

            DataRow[] dataRows = recipeReportTable.Select().OrderBy(u => u["Order"]).ToArray();

            recipeReportTableOrdered = dataRows.CopyToDataTable();
            recipeReportTableOrdered.TableName = "RecipeReportTable";




            return recipeReportTableOrdered;
        }
    }
}


/*
   public class RecipeReportService
    {
        private string _connectionString { get; set; }
        public RecipeReportService(string connectionString)
        {
            _connectionString = connectionString;
             
        }
        public IEnumerable<RecipeDetailReportItem> RecipeDetail(int recipeId)
        {
            IEnumerable<RecipeDetailReportItem> searchResult = new List<RecipeDetailReportItem>();
            IGenericRepository<RecipeDetailReportItem> repository = new DataAccess.Concrete.Postgresql.DapperGenericPostgreRepository<RecipeDetailReportItem>(_connectionString);
            var queryResult = repository.GetAllBySqlQuery($"SELECT re.\"RecipeName\", rd.\"RecipeFieldValue\",rd.\"SegmentNo\" FROM public.\"RecipeDetails\" rd INNER JOIN public.\"Recipes\" re ON rd.\"RecipeId\"= re.id WHERE re.id={recipeId}");
            return queryResult;
        }
        public IEnumerable<RecipeDetailFieldNameReportItem> RecipeDetailFieldName()
        {
            IEnumerable<RecipeDetailFieldNameReportItem> searchResult = new List<RecipeDetailFieldNameReportItem>();
            IGenericRepository<RecipeDetailFieldNameReportItem> repository = new DataAccess.Concrete.Postgresql.DapperGenericPostgreRepository<RecipeDetailFieldNameReportItem>(_connectionString);
            var queryResult = repository.GetAllBySqlQuery($"SELECT \"RecipeFieldName\", \"RecipeFieldOrder\" FROM public.\"RecipeFields\"");
            return queryResult;
        }
        public ReportHeaderInfo ReportHeaderInfo(int batchId)
        {
            ReportHeaderInfoService reportHeaderInfoService = new ReportHeaderInfoService(_connectionString);
            return reportHeaderInfoService.ReportHeaderInfo(batchId);

        }
        public DataTable RecipeReport(int batchId)
        {
            BatchService batchService = new BatchService(_connectionString);
            int recipeId = batchService.GetById(batchId).RecipeId;

            RecipeDetailService recipeDetailService = new RecipeDetailService(_connectionString);
            int segmentTotalCount = recipeDetailService.GetSegmentTotal(recipeId);

            RecipeFieldService recipeFieldService = new RecipeFieldService(_connectionString);
            Dictionary<short,RecipeField> recipeFields = recipeFieldService.GetAll().ToList().ToDictionary(x => x.id, x => x);

            DataTable recipeReportTable = new DataTable();
            DataColumn dataColumn;

            dataColumn = new DataColumn();
            dataColumn.ColumnName = "Order";
            dataColumn.Caption = "Order";
            dataColumn.DataType = typeof(int);
            recipeReportTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.ColumnName = "Title";
            dataColumn.Caption = "Title";
            dataColumn.DataType = typeof(string);
            recipeReportTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.ColumnName = "Unit";
            dataColumn.Caption = "Title";
            dataColumn.DataType = typeof(string);
            recipeReportTable.Columns.Add(dataColumn);


            for (int i = 1; i <= segmentTotalCount; i++)
            {
                dataColumn = new DataColumn();
                dataColumn.ColumnName = "Segment" + i;
                dataColumn.Caption = "Segment" + i;
                dataColumn.DataType = typeof(string);
                recipeReportTable.Columns.Add(dataColumn);
            }

            DataTable recipeReportTableOrdered = recipeReportTable.Clone();


            IEnumerable<RecipeDetail> recipeDetails = recipeDetailService.GetAllByRecipeId(recipeId);

            foreach (var collection in recipeDetails.GroupBy(x => x.RecipeFieldId))
            {
                DataRow row;

                row = recipeReportTable.NewRow();

                row["Order"] = recipeFields[collection.Key].RecipeFieldOrder;
                row["Title"] = recipeFields[collection.Key].RecipeFieldName;

                foreach (var recipeDetailItem in collection.ToList())
                {
                    row["Segment"+recipeDetailItem.SegmentNo] =  recipeDetailItem.RecipeFieldValue;
                }

                recipeReportTable.Rows.Add(row);

            }
          
            DataRow[] dataRows = recipeReportTable.Select().OrderBy(u => u["Order"]).ToArray();
             
            recipeReportTableOrdered = dataRows.CopyToDataTable();
            recipeReportTableOrdered.TableName = "RecipeReportTable";




            return recipeReportTableOrdered;
        }
        public DataTable RecipeReportByRecipe(int recipeId)
        {
            RecipeDetailService recipeDetailService = new RecipeDetailService(_connectionString);
            int segmentTotalCount = recipeDetailService.GetSegmentTotal(recipeId);

            RecipeFieldService recipeFieldService = new RecipeFieldService(_connectionString);
            Dictionary<short, RecipeField> recipeFields = recipeFieldService.GetAll().ToList().ToDictionary(x => x.id, x => x);

            DataTable recipeReportTable = new DataTable();
            DataColumn dataColumn;

            dataColumn = new DataColumn();
            dataColumn.ColumnName = "Order";
            dataColumn.Caption = "Order";
            dataColumn.DataType = typeof(int);
            recipeReportTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.ColumnName = "Title";
            dataColumn.Caption = "Title";
            dataColumn.DataType = typeof(string);
            recipeReportTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.ColumnName = "Unit";
            dataColumn.Caption = "Title";
            dataColumn.DataType = typeof(string);
            recipeReportTable.Columns.Add(dataColumn);


            for (int i = 1; i <= segmentTotalCount; i++)
            {
                dataColumn = new DataColumn();
                dataColumn.ColumnName = "Segment" + i;
                dataColumn.Caption = "Segment" + i;
                dataColumn.DataType = typeof(string);
                recipeReportTable.Columns.Add(dataColumn);
            }

            DataTable recipeReportTableOrdered = recipeReportTable.Clone();


            IEnumerable<RecipeDetail> recipeDetails = recipeDetailService.GetAllByRecipeId(recipeId);

            foreach (var collection in recipeDetails.GroupBy(x => x.RecipeFieldId))
            {
                DataRow row;

                row = recipeReportTable.NewRow();

                row["Order"] = recipeFields[collection.Key].RecipeFieldOrder;
                row["Title"] = recipeFields[collection.Key].RecipeFieldName;

                foreach (var recipeDetailItem in collection.ToList())
                {
                    row["Segment" + recipeDetailItem.SegmentNo] = recipeDetailItem.RecipeFieldValue;
                }

                recipeReportTable.Rows.Add(row);

            }

            DataRow[] dataRows = recipeReportTable.Select().OrderBy(u => u["Order"]).ToArray();

            recipeReportTableOrdered = dataRows.CopyToDataTable();
            recipeReportTableOrdered.TableName = "RecipeReportTable";




            return recipeReportTableOrdered;
        }
    } 


 
 
 */
