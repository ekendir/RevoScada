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
    public class ProcessEventLogService : GenericService<ProcessEventLog>
    {
        public ProcessEventLogService(string connectionString) : base(connectionString)
        {
            _repository = new DataAccess.Concrete.Postgresql.ProcessEventLogRepository(_connectionString);
        }
        public IEnumerable<ProcessEventLog> GetByBatchId(int batchId)
        {
            return _repository.GetAllBySqlQuery($"select * from  public.\"ProcessEventLogs\" where  \"BatchId\" ={batchId} order by id desc;");
        }

        public bool Insert(ProcessEventLog entity)
        {
            return _repository.Insert(entity);
        }

        public bool InsertOrUpdateMany(List<ProcessEventLog> processEventLogs)
        {
            string sql = "INSERT INTO public.\"ProcessEventLogs\"(id, \"EventText\", \"CreateDate\", \"BatchId\", \"Type\")	VALUES (@id, @EventText, @CreateDate, @BatchId, @Type);";
            bool result = _repository.InsertOrUpdateMany(sql, processEventLogs);
            return result;
        }
    }
}
