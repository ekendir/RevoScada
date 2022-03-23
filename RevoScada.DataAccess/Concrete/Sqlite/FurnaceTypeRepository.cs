using RevoScada.Entities.Configuration;



namespace RevoScada.DataAccess.Concrete.SqLite
{
    public class FurnaceTypeRepository : DapperGenericRepository<FurnaceType>
    {
        public FurnaceTypeRepository(string connectionString) : base  (connectionString)
        {
        }
    }
}

