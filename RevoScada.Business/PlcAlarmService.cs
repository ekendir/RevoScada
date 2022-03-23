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
    public class PlcAlarmService : GenericService<PlcAlarm>
    {
        public PlcAlarmService(string connectionString) : base(connectionString)
        {
            _repository = new DataAccess.Concrete.Postgresql.PlcAlarmRepository(_connectionString);
        }
        public IEnumerable<PlcAlarm> GetByBatchId(int batchId)
        {
            return _repository.GetAllBySqlQuery($"select * from  public.\"PlcAlarms\" where  \"BatchId\" ={batchId};");
        }

        public bool Insert(PlcAlarm entity)
        {
            return _repository.Insert(entity);
        }

        public bool InsertOrUpdate(List<PlcAlarm> plcAlarms)
        {
            string sql = "INSERT INTO public.\"PlcAlarms\" (id, \"TagConfigurationId\", \"Status\", \"InDateTime\", \"OutDateTime\", \"AcknowledgedDateTime\", \"BatchId\", \"PlcValue\") VALUES (@id, @TagConfigurationId, @Status, @InDateTime, @OutDateTime, @AcknowledgedDateTime, @BatchId, @PlcValue);";
            bool result = _repository.InsertOrUpdateMany(sql, plcAlarms);
            return result;
        }

    }
}
