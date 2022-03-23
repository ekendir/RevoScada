using RevoScada.Entities;

namespace RevoScada.DataAccess.Concrete.Postgresql
{
    public class PageTagConfigurationRepository : DapperGenericPostgreRepository<PageTagConfiguration>
    {
        public PageTagConfigurationRepository(string connectionString) : base(connectionString)
        {
        }
    }
}
