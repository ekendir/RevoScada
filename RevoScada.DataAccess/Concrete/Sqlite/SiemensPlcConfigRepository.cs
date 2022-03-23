using RevoScada.Entities.Configuration;


namespace RevoScada.DataAccess.Concrete.SqLite
{
    public class SiemensPlcConfigRepository : DapperGenericRepository<SiemensPlcConfig>
    {
        public SiemensPlcConfigRepository(string connectionString) : base  (connectionString)
        {

        }

    }
}

