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
    public class DataLoggerServiceConfigurations
    {
        private static readonly Lazy<DataLoggerServiceConfigurations> lazy = new Lazy<DataLoggerServiceConfigurations>(() => new DataLoggerServiceConfigurations());
        public static DataLoggerServiceConfigurations Instance => lazy.Value;

        private DataLoggerServiceConfigurations()
        {
        }

        public void InitializeConfiguration(string startupConfigurationFile)
        {
            using (StreamReader reader = new StreamReader(startupConfigurationFile))
            {
                string json = reader.ReadToEnd();

                DataLoggerServiceConfiguration = JsonConvert.DeserializeObject<DataLoggerServiceConfiguration>(json);

                DataLoggerServiceConfiguration.PlcType = DataLoggerServiceConfiguration.PlcType.ToLower().Trim();

            }

            switch (DataLoggerServiceConfiguration.PlcType.ToLower())
            {
                case "siemens":

                    SiemensPlcConfigService siemensPlcConfigService = new SiemensPlcConfigService(DataLoggerServiceConfiguration.SqliteConnectionString);

                    PlcConfigs = siemensPlcConfigService.GetActiveFurnaceConfigurations().ToList();

                    Dictionary<int, ITagConfiguration> dataLogTagsKeyValuePair = new Dictionary<int, ITagConfiguration>();

                    SiemensTagConfigurationService siemensTagConfigurationService = new SiemensTagConfigurationService(DataLoggerServiceConfiguration.SqliteConnectionString);

                    var portTags = siemensTagConfigurationService.GetAllBySqlQuery("SELECT * FROM SiemensTagConfigurations WHERE IsActive = 1").ToList();
                   // var portTags = siemensTagConfigurationService.GetAllBySqlQuery("SELECT * FROM SiemensTagConfigurations WHERE (SiemensTagGroupId=2 OR SiemensTagGroupId=4) AND IsActive = 1").ToList();

                    foreach (SiemensTagConfiguration siemensTagConfigurationItem in portTags)
                    {
                        dataLogTagsKeyValuePair.Add(siemensTagConfigurationItem.Id, siemensTagConfigurationItem);
                    }

                    DataLogTags = dataLogTagsKeyValuePair;
                    BatchRunningInfoTags = new Dictionary<int, ITagConfiguration>();
                    BatchFinishInfoTags = new Dictionary<int, ITagConfiguration>();
                    ApplicationPropertyService applicationPropertyService;
                    int batchRunningInfoTagId;
                    int batchFinishInfoTagId;
                    SiemensTagConfiguration batchRunningInfoTag;
                    SiemensTagConfiguration batchFinishInfoTag;

                    
                    foreach (var connectionStringItem in DataLoggerServiceConfiguration.PostgreSqlConnectionStrings)
                    {
                        applicationPropertyService = new ApplicationPropertyService(connectionStringItem.Value);
                        batchRunningInfoTagId = Convert.ToInt32(applicationPropertyService.GetByName("BatchRunningInfoTagId").Value);
                        batchFinishInfoTagId = Convert.ToInt32(applicationPropertyService.GetByName("BatchFinishInfoTagId").Value);
                        batchRunningInfoTag = siemensTagConfigurationService.GetById(batchRunningInfoTagId);
                        batchFinishInfoTag = siemensTagConfigurationService.GetById(batchFinishInfoTagId);
                        BatchRunningInfoTags[connectionStringItem.Key] = batchRunningInfoTag;
                        BatchFinishInfoTags[connectionStringItem.Key] = batchFinishInfoTag;
                    }


                       
                    break;

                case "allenbradley":
                    //Allenbradley implementation
                    break;
                default:
                    throw new Exception("Check configuration file");
            }
        }

        public DataLoggerServiceConfiguration DataLoggerServiceConfiguration { get; private set; }

        public object PlcConfigs { get; private set; }

        public Dictionary<int, ITagConfiguration> DataLogTags { get; private set; }

        public Dictionary<int,ITagConfiguration> BatchRunningInfoTags { get; private set; }
        public Dictionary<int, ITagConfiguration>  BatchFinishInfoTags { get; private set; }

    }
}


/*
 
 
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
    public class DataLoggerServiceConfigurations
    {
        private static readonly Lazy<DataLoggerServiceConfigurations> lazy = new Lazy<DataLoggerServiceConfigurations>(() => new DataLoggerServiceConfigurations());
        public static DataLoggerServiceConfigurations Instance => lazy.Value;

        private DataLoggerServiceConfigurations()
        {
        }

        public void InitializeConfiguration(string startupConfigurationFile)
        {
            using (StreamReader reader = new StreamReader(startupConfigurationFile))
            {
                string json = reader.ReadToEnd();

                DataLoggerServiceConfiguration = JsonConvert.DeserializeObject<DataLoggerServiceConfiguration>(json);

                DataLoggerServiceConfiguration.PlcType = DataLoggerServiceConfiguration.PlcType.ToLower().Trim();

            }

            switch (DataLoggerServiceConfiguration.PlcType.ToLower())
            {
                case "siemens":

                    SiemensPlcConfigService siemensPlcConfigService = new SiemensPlcConfigService(DataLoggerServiceConfiguration.SqliteConnectionString);

                    PlcConfigs = siemensPlcConfigService.GetAll().ToList();


                    Dictionary<int, ITagConfiguration> dataLogTagsKeyValuePair = new Dictionary<int, ITagConfiguration>();

                    SiemensTagConfigurationService siemensTagConfigurationService = new SiemensTagConfigurationService(DataLoggerServiceConfiguration.SqliteConnectionString);

                    var portTags = siemensTagConfigurationService.GetAllBySqlQuery("SELECT * FROM SiemensTagConfigurations WHERE SiemensTagGroupId=2 OR SiemensTagGroupId=4").ToList();

                    foreach (SiemensTagConfiguration siemensTagConfigurationItem in portTags)
                    {
                        dataLogTagsKeyValuePair.Add(siemensTagConfigurationItem.Id, siemensTagConfigurationItem);
                    }

                    DataLogTags = dataLogTagsKeyValuePair;



                    ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(DataLoggerServiceConfigurations.Instance.DataLoggerServiceConfiguration.PostgreSqlConnectionString);

                    
                    int batchRunningInfoTagId = Convert.ToInt32(applicationPropertyService.GetByName("BatchRunningInfoTagId").Value);
                    int batchFinishInfoTagId = Convert.ToInt32(applicationPropertyService.GetByName("BatchFinishInfoTagId").Value);

                    SiemensTagConfiguration batchRunningInfoTag = siemensTagConfigurationService.GetById(batchRunningInfoTagId);
                    SiemensTagConfiguration batchFinishInfoTag = siemensTagConfigurationService.GetById(batchFinishInfoTagId);

                    BatchRunningInfoTag = batchRunningInfoTag;
                    BatchFinishInfoTag = batchFinishInfoTag;





                    break;

                case "allenbradley":
                    //Allenbradley implementation
                    break;
                default:
                    throw new Exception("Check configuration file");
            }
        }

        public DataLoggerServiceConfiguration DataLoggerServiceConfiguration { get; private set; }

        public object PlcConfigs { get; private set; }

        public Dictionary<int, ITagConfiguration> DataLogTags { get; private set; }

        public ITagConfiguration BatchRunningInfoTag { get; private set; }
        public ITagConfiguration BatchFinishInfoTag { get; private set; }


        public Dictionary<int,ITagConfiguration> BatchRunningInfoTags { get; private set; }
        public Dictionary<int, ITagConfiguration>  BatchFinishInfoTags { get; private set; }

    }
}

 */