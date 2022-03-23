using RevoScada.Entities;

namespace RevoScada.DataAccess.Concrete.Postgresql
{
    public class ApplicationPropertyRepository : DapperGenericPostgreRepository<ApplicationProperty>
    {
        public ApplicationPropertyRepository(string connectionString) : base(connectionString)
        {
            TableName = "ApplicationProperties";
        }
    }
}