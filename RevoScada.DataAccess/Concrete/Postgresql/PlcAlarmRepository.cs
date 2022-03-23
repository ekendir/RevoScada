using RevoScada.Entities;

namespace RevoScada.DataAccess.Concrete.Postgresql
{
    public class PlcAlarmRepository : DapperGenericPostgreRepository<PlcAlarm>
    {
        public PlcAlarmRepository(string connectionString) : base(connectionString)
        {
        }
    }
}
