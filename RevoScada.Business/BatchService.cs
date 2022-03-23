using RevoScada.DataAccess.Abstract;
using RevoScada.Entities;
using RevoScada.Entities.Complex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Business
{
   public class BatchService: GenericService<Batch>
    {
        public BatchService(string connectionString):base(connectionString)
        {
            _repository = new DataAccess.Concrete.Postgresql.BatchRepository(_connectionString);
        }

        public IEnumerable<Batch> GetBatches(int amount)
        {
            IEnumerable<Batch> result = _repository.GetAllBySqlQuery($"(SELECT * FROM public.\"Batches\" WHERE \"BatchGroupId\"= 1 OR \"BatchGroupId\"= 2)  UNION ALL (SELECT * FROM public.\"Batches\" WHERE  \"Status\"= 2 OR \"Status\"= 3  ORDER BY \"EndDate\" DESC LIMIT {amount}) ");
            return result;
        }

        public IEnumerable<Batch> GetPrepareBatches()
        {
            IEnumerable<Batch> result = _repository.GetAllBySqlQuery($"SELECT * FROM public.\"Batches\" WHERE \"BatchGroupId\"= 1 ORDER BY \"id\" DESC");
            return result;
        }
        public IEnumerable<Batch> GetCompletedBatches(int amount)
        {
            IEnumerable<Batch> result = _repository.GetAllBySqlQuery($"SELECT * FROM public.\"Batches\" WHERE \"Status\"= 2 AND \"IsEnterPartsSkip\" = false ORDER BY \"EndDate\" DESC LIMIT {amount} ");
            return result;
        }


        public int GetDailyProcessOrder(int batchId,out Batch batch)
        {
            int order = 0;
            Batch resultBatch = GetById(batchId);

            if (resultBatch != null)
            {
                string startDateMin = resultBatch.StartDate.ToString("yyyy-MM-dd 00:00:00.000");
                string startDateMax = resultBatch.StartDate.ToString("yyyy-MM-dd 23:59:59.999");
                List<Batch> list = _repository.GetAllBySqlQuery($"SELECT * FROM public.\"Batches\" WHERE \"StartDate\" <= '{startDateMax}' AND \"StartDate\" >= '{startDateMin}'  ORDER BY \"StartDate\" ").ToList();
                order = list.FindIndex(i => EqualityComparer<Batch>.Default.Equals(resultBatch, i));
                batch = resultBatch;
            }
            else
            {
                batch = null;
            }

            return order;
        }


        public Batch GetLastCompleted()
        {
            //Enson çalışan batch, Last 5 batchten gelirse qry doğru sonuç vermiyor. Bu sebeple end date'e göre sıralandı
            //Batch result = _repository.GetAllBySqlQuery($"SELECT * FROM public.\"Batches\" WHERE \"BatchGroupId\"= 0  ORDER BY \"id\" DESC LIMIT 1;").FirstOrDefault();
            Batch result = _repository.GetAllBySqlQuery($"SELECT * FROM public.\"Batches\" WHERE \"BatchGroupId\"= 0  ORDER BY \"EndDate\" DESC LIMIT 1;").FirstOrDefault();
            return result;
        }

        public Batch GetActiveCurrentBatch()
        {
            CurrentProcessInfoService currentProcessInfoService = new CurrentProcessInfoService(_connectionString);
            CurrentProcessInfo currentProcessInfo = currentProcessInfoService.Get();

            Batch batch;

            try
            {
                batch = (currentProcessInfo.BatchId != 0) ? GetById(currentProcessInfo.BatchId):null;
            }
            catch (Exception)
            {
                batch = null;
            } 

            return batch;
        }

        public IEnumerable<Batch> GetAllBySqlQuery(string queryText)
        {
            return _repository.GetAllBySqlQuery(queryText);
        }

        public short GetMaxRevisionNumber(string formattedLoadNumber)
        {
            string realLoadNumber = formattedLoadNumber.Where(x => x == '-').Count() > 1 ? formattedLoadNumber.Remove(formattedLoadNumber.LastIndexOf("-"), formattedLoadNumber.Length - formattedLoadNumber.LastIndexOf("-")) : formattedLoadNumber;
            var queryResult = _repository.GetAllBySqlQuery($"SELECT * FROM public.\"Batches\" WHERE  \"LoadNumber\"='{realLoadNumber}'");
            short maxRevision = queryResult.Max(x => x.Revision);
            return maxRevision;
        }

        /// <summary>
        /// Renames batch's load number. If it is avaible in db with same name this operation cannot update.
        /// </summary>
        /// <param name="furnaceName">Furnace name defined in configuration file</param>
        /// <param name="newLoadNumberSerial">new number to set loadnumber</param>
        /// <param name="batch">the batch to modify</param>
        public bool RenameLoadNumber(string furnaceName, int newLoadNumberSerial, Batch batch)
        {
            bool result = false;
            string loadNumber = $"{furnaceName}-{newLoadNumberSerial}";
            int loadNumberOccurenceCount = _repository.GetAllBySqlQuery($"SELECT * FROM public.\"Batches\" WHERE  \"LoadNumber\"='{loadNumber}'").Count();

            if (loadNumberOccurenceCount == 0)
            {
                batch.LoadNumber = loadNumber;
                result = _repository.Update(batch);
            }
            return result;
        }

        public IEnumerable<BatchSearchDto> SearchBatchAndRecipeName(string searchText, int count)
        {
            IEnumerable<BatchSearchDto> searchResult =new List<BatchSearchDto>();

            IGenericRepository<BatchSearchDto> repository = new DataAccess.Concrete.Postgresql.DapperGenericPostgreRepository<BatchSearchDto>(_connectionString);

            if (!string.IsNullOrEmpty(searchText))
            {
                if (searchText.Length >= 2)
                {
                    searchResult= repository.GetAllBySqlQuery($"SELECT bc.id, bc.\"LoadNumber\", re.\"RecipeName\", bc.\"StartDate\", bc.\"EndDate\",bc.\"Revision\" FROM public.\"Batches\" bc INNER JOIN public.\"Recipes\" re ON  bc.\"RecipeId\"= re.id WHERE (bc.\"Status\"= 2 OR bc.\"Status\"= 3 OR bc.\"Status\"= 1)  AND CONCAT(\"LoadNumber\",' ',\"RecipeName\") LIKE '%{searchText}%' ORDER BY bc.\"StartDate\" DESC LIMIT {count};");
                }
            }
            else
            {
                //searchResult = repository.GetAllBySqlQuery($"SELECT bc.id, bc.\"LoadNumber\", re.\"RecipeName\", bc.\"StartDate\",bc.\"Revision\" FROM public.\"Batches\" bc INNER JOIN public.\"Recipes\" re ON  bc.\"RecipeId\" = re.id WHERE  (\"Status\"= 2 OR \"Status\"= 3 OR bc.\"Status\"= 1)  ORDER BY bc.\"StartDate\" DESC LIMIT {count}");
                searchResult = repository.GetAllBySqlQuery($"SELECT bc.id, bc.\"LoadNumber\", re.\"RecipeName\", bc.\"StartDate\", bc.\"EndDate\", bc.\"Revision\" FROM public.\"Batches\" bc INNER JOIN public.\"Recipes\" re ON  bc.\"RecipeId\" = re.id WHERE  (\"Status\"= 2 OR \"Status\"= 3 OR bc.\"Status\"= 1)  ORDER BY bc.\"StartDate\" DESC ");
            }

            foreach (var item in searchResult)
            {
                item.LoadNumber = item.Revision > 0 ? $"{item.LoadNumber}-R{item.Revision}" : item.LoadNumber;
            }

            return searchResult;
        }

        public Batch GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool Insert(Batch entity)
        {
            return _repository.Insert(entity);
        }

        public bool InsertOrUpdate(Batch entity)
        {
            return _repository.InsertOrUpdate(entity);
        }


        public bool InsertOrUpdateMany(List<Batch> batches)
        {
            string sql = "INSERT INTO public.\"Batches\"(id, \"LoadNumber\", \"StartDate\", \"EndDate\", \"RecipeId\", \"Status\", \"BatchGroupId\", \"Revision\", \"IsEnterPartsSkip\")	VALUES(@id, @LoadNumber, @StartDate, @EndDate, @RecipeId, @Status, @BatchGroupId, @Revision, @IsEnterPartsSkip)";
            return _repository.InsertOrUpdateMany(sql,batches);
        }




        public bool Update(Batch entity)
        {
            return _repository.Update(entity);
        }

        public bool Delete(Batch entity)
        {
            return _repository.Delete(entity);
        }

     
        //public IEnumerable<DataLog> GetAllMissingDataLog(int pageSize, int page, long minRequestedId)
        //{
        //    page = pageSize * (page - 1);
        //    return _repository.GetAllBySqlQuery($"SELECT * FROM  public.\"DataLogs\" LIMIT {pageSize} OFFSET {page} WHERE id >{minRequestedId};");
        //}
    }
}
