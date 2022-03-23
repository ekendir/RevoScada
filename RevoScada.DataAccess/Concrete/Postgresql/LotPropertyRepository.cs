using RevoScada.Entities;

namespace RevoScada.DataAccess.Concrete.Postgresql
{
    public class LotPropertyRepository : DapperGenericPostgreRepository<LotProperty>
    {
        public LotPropertyRepository(string connectionString) : base(connectionString)
        {
            TableName = "LotProperties";
        }
    }
}