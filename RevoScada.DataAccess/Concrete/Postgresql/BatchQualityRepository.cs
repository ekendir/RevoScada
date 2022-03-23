using RevoScada.Entities;

namespace RevoScada.DataAccess.Concrete.Postgresql
{
    public class BatchQualityRepository : DapperGenericPostgreRepository<BatchQuality>
    {
        public BatchQualityRepository(string connectionString) : base(connectionString)
        {
            TableName = "BatchQualities";
        }
    }
}