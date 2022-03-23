using RevoScada.Entities;

namespace RevoScada.DataAccess.Concrete.Postgresql
{
    public class ProcessEventLogRepository : DapperGenericPostgreRepository<ProcessEventLog>
    {
        public ProcessEventLogRepository(string connectionString) : base(connectionString)
        {
        }
    }
}
