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
   public class ReportHeaderInfoService
    {
       private string _connectionString { get; set; }

       public ReportHeaderInfoService(string connectionString)
        {
            _connectionString = connectionString;
        }

       public ReportHeaderInfo ReportHeaderInfo(int batchId)
        {
            ReportHeaderInfo reportHeaderInfo = new ReportHeaderInfo();

            string query = $"SELECT bc.id \"BatchId\", bc.\"LoadNumber\", bc.\"StartDate\",bc.\"EndDate\",bc.\"RecipeId\",rc.\"RecipeName\", rc.\"Description\", rc.\"CreatedByUserId\" FROM public.\"Batches\" bc INNER JOIN public.\"Recipes\" rc ON bc.\"RecipeId\" = rc.id  WHERE bc.id={batchId}; ";

            IGenericRepository<ReportHeaderInfo> repository = new DataAccess.Concrete.Postgresql.DapperGenericPostgreRepository<ReportHeaderInfo>(_connectionString);
            
            reportHeaderInfo = repository.GetAllBySqlQuery(query).FirstOrDefault();

            return reportHeaderInfo;
        }

       public NumericReportHeaderInfo NumericReportHeaderInfo(int batchId)
        {
            NumericReportHeaderInfo numericReportHeaderInfo = new NumericReportHeaderInfo();

            string query = $"SELECT bc.id \"BatchId\", bc.\"LoadNumber\", bc.\"StartDate\",bc.\"EndDate\",bc.\"RecipeId\",rc.\"RecipeName\", rc.\"Description\" FROM public.\"Batches\" bc INNER JOIN public.\"Recipes\" rc ON bc.\"RecipeId\" = rc.id  WHERE bc.id={batchId}; ";

            IGenericRepository<NumericReportHeaderInfo> repository = new DataAccess.Concrete.Postgresql.DapperGenericPostgreRepository<NumericReportHeaderInfo>(_connectionString);
           
            numericReportHeaderInfo = repository.GetAllBySqlQuery(query).FirstOrDefault();

            BagService bagService = new BagService(_connectionString);
            
            LotPropertyService lotPropertyService = new LotPropertyService(_connectionString);

            IEnumerable<Bag> bags=  bagService.BagsByBatch(batchId);
            
            IEnumerable<LotProperty> LotProperties = lotPropertyService.GetByBagIdListProperties(bags.Select(x => x.id).ToList());

            if(LotProperties != null) {     numericReportHeaderInfo.PartNames = string.Join(", ", LotProperties.Where(s => !string.IsNullOrWhiteSpace(s.PartName)).Select(x => x.PartName));  
            numericReportHeaderInfo.SoirNames = string.Join(", ", LotProperties.Where(s => !string.IsNullOrWhiteSpace(s.SoirNumber)).Select(x => x.SoirNumber));  
            numericReportHeaderInfo.ToolNames = string.Join(", ", LotProperties.Where(s => !string.IsNullOrWhiteSpace(s.ToolName)).Select(x => x.ToolName));  
            numericReportHeaderInfo.BagNames  = string.Join(", ", bags.Select(x => x.BagName));}
          

            return numericReportHeaderInfo;
        }

       public NumericReportHeaderInfo NumericReportHeaderInfoByBag(int batchId ,int bagId)
       {
            NumericReportHeaderInfo numericReportHeaderInfo = new NumericReportHeaderInfo();

            string query = $"SELECT bc.id \"BatchId\", bc.\"LoadNumber\", bc.\"StartDate\",bc.\"EndDate\",bc.\"RecipeId\",rc.\"RecipeName\" FROM public.\"Batches\" bc INNER JOIN public.\"Recipes\" rc ON bc.\"RecipeId\" = rc.id  WHERE bc.id={batchId}; ";

            IGenericRepository<NumericReportHeaderInfo> repository = new DataAccess.Concrete.Postgresql.DapperGenericPostgreRepository<NumericReportHeaderInfo>(_connectionString);

            numericReportHeaderInfo = repository.GetAllBySqlQuery(query).FirstOrDefault();

            BagService bagService = new BagService(_connectionString);

            LotPropertyService lotPropertyService = new LotPropertyService(_connectionString);

            IEnumerable<Bag> bags = bagService.BagsByBatch(batchId).Where(x => x.id == bagId);

            IEnumerable<LotProperty> LotProperties = lotPropertyService.GetByBagIdListProperties(bags.Select(x => x.id).ToList());

            numericReportHeaderInfo.PartNames = string.Join(", ", LotProperties.Where(s => !string.IsNullOrWhiteSpace(s.PartName)).Select(x => x.PartName));
            numericReportHeaderInfo.SoirNames = string.Join(", ", LotProperties.Where(s => !string.IsNullOrWhiteSpace(s.SoirNumber)).Select(x => x.SoirNumber));
            numericReportHeaderInfo.ToolNames = string.Join(", ", LotProperties.Where(s => !string.IsNullOrWhiteSpace(s.ToolName)).Select(x => x.ToolName));
            numericReportHeaderInfo.BagNames = string.Join(", ", bags.Select(x => x.BagName));

            return numericReportHeaderInfo;
       }

    }
}
