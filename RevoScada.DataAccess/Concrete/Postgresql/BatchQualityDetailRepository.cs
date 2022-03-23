using RevoScada.Entities;

namespace RevoScada.DataAccess.Concrete.Postgresql
{
    public class BatchQualityDetailRepository : DapperGenericPostgreRepository<BatchQualityDetail>
    {
        public BatchQualityDetailRepository(string connectionString) : base(connectionString)
        {
            
        }
    }
}