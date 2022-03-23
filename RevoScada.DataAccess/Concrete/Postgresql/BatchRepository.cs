using RevoScada.Entities;

namespace RevoScada.DataAccess.Concrete.Postgresql
{
    public class BatchRepository : DapperGenericPostgreRepository<Batch>
    {
        public BatchRepository(string connectionString) : base(connectionString)
        {
            TableName = "Batches";
        }
    }
}
