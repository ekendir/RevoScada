using RevoScada.Entities.Configuration;



namespace RevoScada.DataAccess.Concrete.SqLite
{
    public class PlcTypeRepository : DapperGenericRepository<PlcType>
    {
        public PlcTypeRepository(string connectionString) : base  (connectionString)
        {

        }
    }
}
