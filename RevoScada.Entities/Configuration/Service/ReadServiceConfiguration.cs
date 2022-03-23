 
namespace RevoScada.Entities.Configuration.Service
{
    public class ReadServiceConfiguration    {
        public string PlcType { get; set; }
        public string RedisServer { get; set; }
        public int CycleWaitInMiliseconds { get; set; }
        public string SqliteConnectionString { get; set; }
        public LogSettings LogSettings { get; set; }
    }
}
