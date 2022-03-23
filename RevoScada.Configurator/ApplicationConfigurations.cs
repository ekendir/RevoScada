using Newtonsoft.Json;
using RevoScada.Business.Configurations;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.Configuration.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace RevoScada.Configurator
{

    public class ApplicationConfigurations
    {
        private static readonly Lazy<ApplicationConfigurations> lazy = new Lazy<ApplicationConfigurations>(() => new ApplicationConfigurations());
        public static ApplicationConfigurations Instance => lazy.Value;

        private Dictionary<int, ITagConfiguration> _tagConfigurations;

        private ApplicationConfigurations()
        {
        }

       public void InitializeConfiguration(string startupConfigurationFile, bool isServiceConfig)
       {
            _tagConfigurations = new Dictionary<int, ITagConfiguration>();

           using (StreamReader reader = new StreamReader(startupConfigurationFile))
           {
               string json = reader.ReadToEnd();
                Configuration = JsonConvert.DeserializeObject<ApplicationConfiguration>(json);

                if (isServiceConfig)
                {
                    PlcDeviceService plcDeviceService = new PlcDeviceService(Configuration.SqliteConnectionString);
                    FurnaceService furnaceService = new FurnaceService(Configuration.SqliteConnectionString);
                    Configuration.PlcDevices = new Dictionary<int, PlcDevice>();
                    Configuration.Furnaces = new Dictionary<int, Furnace>();

                    //Plcdevices
                    foreach (var item in furnaceService.GetAll().Where(x => x.IsActive == true))
                    {
                        Configuration.Furnaces.Add(item.Id, item);
                        PlcDevice plcDevice = plcDeviceService.GetAll().FirstOrDefault(x => x.Id == item.Id);
                        Configuration.PlcDevices.Add(item.Id, plcDevice);
                    }

                    LoadTagConfigurations();
                }
                else
                {

                    FurnaceService furnaceService = new FurnaceService(Configuration.SqliteConnectionString);
                    PlcDeviceService plcDeviceService = new PlcDeviceService(Configuration.SqliteConnectionString);
                    Configuration.PlcType = Configuration.PlcType.ToLower().Trim();
                    Furnace furnace = furnaceService.GetAll().First(x => x.IsActive == true);
                    PlcDevice plcDevice;// = plcDeviceService.GetAll().Where(x => x.FurnaceId == furnace.Id).First();

                    if (Configuration.WorkingEnvironment== WorkingEnvironment.pc)
                    {
                        Configuration.PlcDevice = plcDeviceService.GetAll().Where(x => x.FurnaceId == furnace.Id).FirstOrDefault();
                        Configuration.Furnace = furnace;
                    }
                  
                    Configuration.Furnaces = new Dictionary<int, Furnace>();
                    Configuration.PlcDevices = new Dictionary<int, PlcDevice>();
                    
                    Configuration.PostgreSqlConnectionString = Configuration.PostgreSqlConnectionStrings[furnace.Id];

                    //Plcdevices
                    foreach (var item in furnaceService.GetAll().Where(x => x.IsActive == true))
                    {
                        Configuration.Furnaces.Add(item.Id, item);
                        plcDevice = plcDeviceService.GetAll().FirstOrDefault(x => x.Id == item.Id);
                        Configuration.PlcDevices.Add(item.Id, plcDevice);
                    }


                    switch (Configuration.PlcTypes[furnace.Id].ToLower())
                    {
                        case "siemens":
                            _tagConfigurations = new Dictionary<int, ITagConfiguration>();
                            LoadTagConfigurations();
                            break;
                        case "allenbradley":
                            //Allenbradley implementation
                            break;
                        default:
                            throw new Exception("Check configuration file");
                    }
                }
            }
       }

        public void RedefineSelectedFurnace(int plcDeviceId) {

            if (Configuration.WorkingEnvironment == WorkingEnvironment.server)
            {
                Configuration.PlcDevice = Configuration.PlcDevices[plcDeviceId]; 
                Configuration.Furnace = Configuration.Furnaces[plcDeviceId]; 
            }
        }

        public ApplicationConfiguration Configuration { get; private set; }

        public Dictionary<int, ITagConfiguration> TagConfigurations {

            get
            {
                if (_tagConfigurations == null)
                {
                    LoadTagConfigurations();
                }
                 
                return _tagConfigurations;

            }
            set {

             _tagConfigurations=   value;
            }
          
        }

        private void LoadTagConfigurations() {
            Dictionary<int, ITagConfiguration> tagKeyValuePair = new Dictionary<int, ITagConfiguration>();
          //  SiemensTagConfigurationService siemensTagConfigurationService = (AllApplicationConfigurations.Count>0) ? new SiemensTagConfigurationService(AllApplicationConfigurations.First().Value.SqliteConnectionString) : new SiemensTagConfigurationService(Configuration.SqliteConnectionString);
            SiemensTagConfigurationService siemensTagConfigurationService = new SiemensTagConfigurationService(Configuration.SqliteConnectionString);
            var tagConfigurations = siemensTagConfigurationService.GetAll().ToList();
            foreach (var item in tagConfigurations)
            { 
                tagKeyValuePair.Add(item.Id, item);
            }
            _tagConfigurations = tagKeyValuePair;
        }
    }
}


 