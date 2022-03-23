using RevoScada.Entities.Configuration;

namespace RevoScada.DataAccess.Concrete.SqLite
{
    public class FurnaceRepository : DapperGenericRepository<Furnace>
    {
        public FurnaceRepository(string connectionString) : base  (connectionString)
        {

        }

    }
}

