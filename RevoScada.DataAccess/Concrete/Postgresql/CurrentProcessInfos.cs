using RevoScada.Entities;

namespace RevoScada.DataAccess.Concrete.Postgresql
{
    public class CurrentProcessInfoRepository : DapperGenericPostgreRepository<CurrentProcessInfo>
    {
        public CurrentProcessInfoRepository(string connectionString) : base(connectionString)
        {
        }
    }
} 