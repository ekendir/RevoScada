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
   public class IntegratedCheckResultService:GenericService<IntegratedCheckResult>
    {
        public IntegratedCheckResultService(string connectionString):base(connectionString)
        {
            _repository = new DataAccess.Concrete.Postgresql.IntegratedCheckResultRepository(_connectionString);
        }

        public IEnumerable<IntegratedCheckResult> GetByDate( DateTime startDate, DateTime endDate)
        {
            var repository = new DataAccess.Concrete.Postgresql.IntegratedCheckResultRepository(_connectionString);
            return repository.GetByDate(startDate, endDate);
        }

        public IEnumerable<IntegratedCheckResult> GetByBatchIdAndDate(int batchId, DateTime startDate, DateTime endDate)
        {
            var repository = new DataAccess.Concrete.Postgresql.IntegratedCheckResultRepository(_connectionString);
            return repository.GetByBatchIdAndDate(batchId, startDate, endDate);
        }

        public IEnumerable<IntegratedCheckResult> GetAllByBatchId(int batchId)
        {
            string query = $"SELECT * FROM public.\"IntegratedCheckResults\" WHERE \"BatchId\"={batchId}";
            IEnumerable<IntegratedCheckResult> queryResult = _repository.GetAllBySqlQuery(query);
            return queryResult;
        }

        public IEnumerable<IntegratedCheckResult> GetAllByBagId(int batchId, int bagId)
        {
            string query = $"SELECT * FROM public.\"IntegratedCheckResults\" WHERE \"BatchId\"={batchId} and \"BagId\" = {bagId}";
            IEnumerable<IntegratedCheckResult> queryResult = _repository.GetAllBySqlQuery(query);
            return queryResult;
        }
         
        public bool ResetCheckResult(int batchId)
        {
            IEnumerable<IntegratedCheckResult> listToDelete = GetAllByBatchId(batchId);
            bool result = _repository.DeleteMany(listToDelete);
            return result;
        }

        public bool InsertCheckResults(List<IntegratedCheckResult> integratedCheckResultList)
        {
            string sql = "INSERT INTO public.\"IntegratedCheckResults\" (id,\"ActualValue\", \"StartValue\", \"FinishValue\", \"Deviation\", \"RequirementValue\", \"BagId\", \"BatchId\", \"CheckResultSaveDate\", \"SensorTagId\") VALUES (@id,@ActualValue, @StartValue, @FinishValue, @Deviation, @RequirementValue, @BagId, @BatchId, @CheckResultSaveDate, @SensorTagId); ";
            bool insertResult = _repository.InsertMany(sql, integratedCheckResultList,true);
            return insertResult;
        }

        public bool InsertOrUpdateMany(List<IntegratedCheckResult> integratedCheckResultList)
        {
            string sql = "INSERT INTO public.\"IntegratedCheckResults\" (id,\"ActualValue\", \"StartValue\", \"FinishValue\", \"Deviation\", \"RequirementValue\", \"BagId\", \"BatchId\", \"CheckResultSaveDate\", \"SensorTagId\") VALUES (@id,@ActualValue, @StartValue, @FinishValue, @Deviation, @RequirementValue, @BagId, @BatchId, @CheckResultSaveDate, @SensorTagId); ";
            bool insertResult = _repository.InsertOrUpdateMany(sql, integratedCheckResultList);
            return insertResult;
        }

     
       

    }
}
