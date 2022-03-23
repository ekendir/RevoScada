using RevoScada.Entities;

namespace RevoScada.DataAccess.Concrete.Postgresql
{
    public class DisabledPortRepository : DapperGenericPostgreRepository<DisabledPort>
    {
        public DisabledPortRepository(string connectionString) : base(connectionString)
        {
        }
    }
}   
