using System.Collections.Generic;

namespace RevoScada.Entities.Configuration
{
    public class ApplicationConfiguration: BaseConfiguration
    {
        public string PostgreSqlConnectionString { get; set; }
        public Dictionary<int,string> PostgreSqlConnectionStrings { get; set; }
        public string PlcType { get; set; }
        public Dictionary<int, string> PlcTypes { get; set; }
        public string RedisServer { get; set; }
        public Dictionary<string,string> ServiceNames { get; set; }
        public Furnace Furnace { get; set; }
        public PlcDevice PlcDevice { get; set; }
        public Dictionary<int,Furnace> Furnaces { get; set; }
        public Dictionary<int, PlcDevice> PlcDevices { get; set; }
        public string ApplicationLogoFullPath { get; set; }
        public string RunOperationImageFullPath { get; set; }
        public string ReportHeaderTitleRoot { get; set; }
        public string ReportHeaderLogoImage { get; set; }
        public string ApplicationFullPath { get; set; }
    }
}

