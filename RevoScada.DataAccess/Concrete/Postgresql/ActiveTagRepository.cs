using RevoScada.Entities;

namespace RevoScada.DataAccess.Concrete.Postgresql
{
    public class ActiveTagRepository : DapperGenericPostgreRepository<ActiveTag>
    {
        public ActiveTagRepository(string connectionString) : base(connectionString)
        {
        }
    }
}