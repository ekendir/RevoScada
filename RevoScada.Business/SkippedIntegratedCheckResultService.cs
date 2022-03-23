using RevoScada.DataAccess.Abstract;

using RevoScada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Business
{
   public class SkippedIntegratedCheckResultService:GenericService<SkippedIntegratedCheckResult>
    {      
        public SkippedIntegratedCheckResultService(string connectionString) : base(connectionString)
        {
            _repository = new DataAccess.Concrete.Postgresql.SkippedIntegratedCheckResultRepository(_connectionString);
        }
        
        public IEnumerable<SkippedIntegratedCheckResult> GetByDate( DateTime startDate, DateTime endDate)
        {
            var repository = new DataAccess.Concrete.Postgresql.SkippedIntegratedCheckResultRepository(_connectionString);
            return repository.GetByDate(startDate, endDate);
        }

        public IEnumerable<SkippedIntegratedCheckResult> GetByBatchIdAndDate(int batchId, DateTime startDate, DateTime endDate)
        {
            var repository = new DataAccess.Concrete.Postgresql.SkippedIntegratedCheckResultRepository(_connectionString);
            return repository.GetByBatchIdAndDate(batchId, startDate, endDate);
        }

        public SkippedIntegratedCheckResult GetByBatchId(int batchId)
        {
            string query = $"SELECT * FROM public.\"SkippedIntegratedCheckResults\" WHERE \"BatchId\"={batchId}";
            SkippedIntegratedCheckResult queryResult = _repository.GetAllBySqlQuery(query).FirstOrDefault();
            return queryResult;
        }

        public bool Insert(SkippedIntegratedCheckResult entity)
        {
            return _repository.Insert(entity);
        }

        public bool InsertOrUpdate(SkippedIntegratedCheckResult skippedIntegratedCheckResult)
        {
            return _repository.InsertOrUpdate(skippedIntegratedCheckResult);
        }

        public bool InsertOrUpdateMany(List<SkippedIntegratedCheckResult> skippedIntegratedCheckResults)
        {
            string sql = "INSERT INTO public.\"SkippedIntegratedCheckResults\"( id, \"BatchId\", \"SkipDate\") VALUES (@id, @BatchId,@SkipDate);";
            return _repository.InsertOrUpdateMany(sql, skippedIntegratedCheckResults);
        }
    }
}
