using RevoScada.Entities.Configuration;


namespace RevoScada.DataAccess.Concrete.SqLite
{
    public class PlcDeviceRepository : DapperGenericRepository<PlcDevice>
    {
        public PlcDeviceRepository(string connectionString) : base  (connectionString)
        {
        }
    }
}

