using Newtonsoft.Json;
using RevoScada.Business;
using RevoScada.Business.Configurations;
using RevoScada.Entities;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.Configuration.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Configurator
{
    public class AlarmServiceConfigurations
    {
        private static readonly Lazy<AlarmServiceConfigurations> lazy = new Lazy<AlarmServiceConfigurations>(() => new AlarmServiceConfigurations());
        public static AlarmServiceConfigurations Instance => lazy.Value;

        private AlarmServiceConfigurations()
        {
        }

        public void InitializeConfiguration(string startupConfigurationFile)
        {
            using (StreamReader reader = new StreamReader(startupConfigurationFile))
            {
                string json = reader.ReadToEnd();

                AlarmServiceConfiguration = JsonConvert.DeserializeObject<AlarmServiceConfiguration>(json);

                AlarmServiceConfiguration.PlcType = AlarmServiceConfiguration.PlcType.ToLower().Trim();

            }

            switch (AlarmServiceConfiguration.PlcType.ToLower())
            {
                case "siemens":
                    
                    SiemensTagConfigurationService siemensTagConfigurationService = new SiemensTagConfigurationService(AlarmServiceConfiguration.SqliteConnectionString);
                    var alarmTagConfigurations =  siemensTagConfigurationService.GetAllBySqlQuery("SELECT * FROM SiemensTagConfigurations WHERE SiemensTagGroupId=3 AND IsActive=1").ToList();
                    SiemensPlcConfigService siemensPlcConfigService = new SiemensPlcConfigService(AlarmServiceConfiguration.SqliteConnectionString);
                    PlcConfigs = siemensPlcConfigService.GetActiveFurnaceConfigurations().ToList();
                    AlarmTagConfigurations = new Dictionary<int, Dictionary<int, ITagConfiguration>>();

                    int batchRunningInfoTagId;
                    int batchFinishInfoTagId;

                    SiemensTagConfiguration batchRunningInfoTag;
                    SiemensTagConfiguration batchFinishInfoTag;

                    BatchRunningInfoTags = new Dictionary<int, ITagConfiguration>();
                    BatchFinishInfoTags = new Dictionary<int, ITagConfiguration>();

                    ApplicationPropertyService applicationPropertyService;

                    foreach (var plcConfigItem in ( IEnumerable<SiemensPlcConfig>) PlcConfigs)
                    {
                        string connectionString = AlarmServiceConfiguration.PostgreSqlConnectionStrings.First(x => x.Key == plcConfigItem.PlcDeviceId).Value;
                        applicationPropertyService = new ApplicationPropertyService(connectionString);

                        Dictionary<int, ITagConfiguration> alarmTags = new Dictionary<int, ITagConfiguration>();
                        foreach (SiemensTagConfiguration siemensTagConfigurationItem in alarmTagConfigurations.Where(x=>x.PlcId==plcConfigItem.PlcDeviceId))
                        {
                            alarmTags.Add(siemensTagConfigurationItem.Id, siemensTagConfigurationItem);
                        }
                        AlarmTagConfigurations.Add(plcConfigItem.PlcDeviceId, alarmTags);

                        applicationPropertyService = new ApplicationPropertyService(connectionString);
                        batchRunningInfoTagId = Convert.ToInt32(applicationPropertyService.GetByName("BatchRunningInfoTagId").Value);
                        batchFinishInfoTagId = Convert.ToInt32(applicationPropertyService.GetByName("BatchFinishInfoTagId").Value);
                        batchRunningInfoTag = siemensTagConfigurationService.GetById(batchRunningInfoTagId);
                        batchFinishInfoTag = siemensTagConfigurationService.GetById(batchFinishInfoTagId);
                        BatchRunningInfoTags[plcConfigItem.PlcDeviceId] = batchRunningInfoTag;
                        BatchFinishInfoTags [plcConfigItem.PlcDeviceId] = batchFinishInfoTag;
                    }


                    break;
                case "allenbradley":
                    //Allenbradley implementation
                    break;
                default:
                    throw new Exception("Check configuration file");
            }
        }

        public AlarmServiceConfiguration AlarmServiceConfiguration { get; private set; }
        public object PlcConfigs { get; private set; }
        public Dictionary<int,Dictionary<int, ITagConfiguration>> AlarmTagConfigurations { get; private set; }
        public Dictionary<int, ITagConfiguration> BatchRunningInfoTags { get; private set; }
        public Dictionary<int, ITagConfiguration> BatchFinishInfoTags { get; private set; }
    }
}
