using Newtonsoft.Json;
using RevoScada.Business.Configurations;
using RevoScada.Entities.Configuration.Service;
using RevoScada.Entities.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Configurator
{
    public class WriteConfigurations
    {
        private static readonly Lazy<WriteConfigurations> lazy = new Lazy<WriteConfigurations>(() => new WriteConfigurations());
        public static WriteConfigurations Instance => lazy.Value;

        private WriteConfigurations()
        {
        }

        public void InitializeConfiguration(string startupConfigurationFile)
        {
            using (StreamReader reader = new StreamReader(startupConfigurationFile))
            {
                string json = reader.ReadToEnd();

                WriteServiceConfiguration = JsonConvert.DeserializeObject<WriteServiceConfiguration>(json);
                WriteServiceConfiguration.PlcType = WriteServiceConfiguration.PlcType.ToLower().Trim();

            }

            switch (WriteServiceConfiguration.PlcType.ToLower())
            {
                case "siemens":
                    SiemensPlcConfigService siemensPlcConfigService = new SiemensPlcConfigService(WriteServiceConfiguration.SqliteConnectionString);
                    PlcConfigs =  siemensPlcConfigService.GetActiveFurnaceConfigurations().ToList();
 
                    break;

                case "allenbradley":
                    //Allenbradley implementation
                    break;
                default:
                    throw new Exception("Check configuration file");
            }
        }

        
        public WriteServiceConfiguration WriteServiceConfiguration { get; private set; }


        public object PlcConfigs{ get; private set; }

}


}


 