using RevoScada.Entities.Configuration.Service;

namespace RevoScada.Entities.Configuration
{
    public class BaseConfiguration
    {
        public WorkingEnvironment WorkingEnvironment { get; set; }
        public string MachineId { get; set; }
        public string SqliteConnectionString { get; set; }
        public LogSettings LogSettings { get; set; }
    }
}
