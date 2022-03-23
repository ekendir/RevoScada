using RevoScada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevoScada.Business
{
    public class DataLogService : GenericService<DataLog>
    {
        public DataLogService(string connectionString) : base(connectionString)
        {
            _repository = new DataAccess.Concrete.Postgresql.DataLogRepository(_connectionString);
        }

        public IEnumerable<DataLog> GetAllBySqlQuery(string queryText)
        {
            return _repository.GetAllBySqlQuery(queryText);
        }

        public DataLog GetById(long id)
        {
            return _repository.GetById(id);
        }

        public IEnumerable<DataLog> GetByBatch(int batchId)
        {
            return _repository.GetAllBySqlQuery($"select * from  public.\"DataLogs\" where  \"BatchId\" = {batchId} ORDER BY \"ReceivedDate\";");
        }
        public IEnumerable<DataLog> GetByBatch(int batchId, DateTime startDate, DateTime endDate)
        {
            return _repository.GetAllBySqlQuery($"SELECT * FROM  public.\"DataLogs\" WHERE  \"BatchId\" = {batchId} AND (\"ReceivedDate\">= '{ startDate:yyyy-MM-dd HH:mm:ss}'   AND \"ReceivedDate\"<= '{endDate:yyyy-MM-dd HH:mm:ss}'  ) ORDER BY \"ReceivedDate\";");
        }

        public IEnumerable<DataLog> GetByBatchPaged(int batchId, int pageSize, int page)
        {
            page = pageSize * (page - 1);
            return _repository.GetAllBySqlQuery($"SELECT * FROM public.\"DataLogs\" WHERE  \"BatchId\" = {batchId} ORDER BY \"ReceivedDate\" LIMIT {pageSize} OFFSET {page} ;");
        }

        public IEnumerable<DataLog> GetByBagSensorsPaged(int batchId, List<int> bagSensors, int pageSize, int page)
        {
            string bagSensorsLiteral = string.Join(",", bagSensors);
            page = pageSize * (page - 1);
            return _repository.GetAllBySqlQuery($"SELECT * FROM public.\"DataLogs\" WHERE  \"BatchId\" = {batchId} AND \"TagConfigurationId\" IN ( { bagSensorsLiteral} ) ORDER BY \"ReceivedDate\" LIMIT {pageSize} OFFSET {page} ;");
        }

        public IEnumerable<DataLog> GetAllPaged(int pageSize, int page)
        {
            page = pageSize * (page - 1);
            return _repository.GetAllBySqlQuery($"select * from  public.\"DataLogs\" LIMIT {pageSize} OFFSET {page};");
        }
        public bool Insert(DataLog entity)
        {
            return _repository.Insert(entity);
        }

        public bool InsertMany(List<DataLog> dataLogs)
        {
            string sql = "INSERT INTO public.\"DataLogs\"(id,\"BatchId\", \"ReceivedDate\", \"TagConfigurationId\",  \"TagValue\") VALUES (@id,@BatchId, @ReceivedDate, @TagConfigurationId,  @TagValue);";
            bool insertResult = _repository.InsertMany(sql, dataLogs, true);
            return insertResult;
        }

        public bool InsertOrUpdateMany(List<DataLog> entities)
        {
            string sql = "INSERT INTO public.\"DataLogs\"(id,\"BatchId\", \"ReceivedDate\", \"TagConfigurationId\",  \"TagValue\") VALUES (@id,@BatchId, @ReceivedDate, @TagConfigurationId,  @TagValue);";
            return _repository.InsertOrUpdateMany(sql,entities);
        }

        public bool Update(DataLog entity)
        {
            return _repository.Update(entity);
        }

        public long GetMaxIdByBatchId(long batch)
        {
         var entity = _repository.GetAllBySqlQuery($"SELECT Max(id) as id FROM public.\"DataLogs\" WHERE \"BatchId\"={batch}; ").FirstOrDefault();
         return entity.id;
        }
    }
}
