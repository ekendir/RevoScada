using RevoScada.Entities;

namespace RevoScada.DataAccess.Concrete.Postgresql
{
    public class BagRepository : DapperGenericPostgreRepository<Bag>
    {
        public BagRepository(string connectionString) : base(connectionString)
        {
        }
    }
}