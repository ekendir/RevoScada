using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using RevoScada.Entities.Configuration.Service;
using RevoScada.Entities.Complex;
using RevoScada.Business.Configurations;
using RevoScada.Entities.Configuration;

namespace RevoScada.Configurator
{

    public class ReadConfigurations
    {
        private static readonly Lazy<ReadConfigurations> lazy = new Lazy<ReadConfigurations>(() => new ReadConfigurations());
        public static ReadConfigurations Instance => lazy.Value;

        private ReadConfigurations()
        {
        }

        public void InitializeConfiguration(string startupConfigurationFile)
        {
            using (StreamReader reader = new StreamReader(startupConfigurationFile))
            {
                string json = reader.ReadToEnd();

                ReadServiceConfiguration = JsonConvert.DeserializeObject<ReadServiceConfiguration>(json);
                ReadServiceConfiguration.PlcType = ReadServiceConfiguration.PlcType.ToLower().Trim();

            }

            switch (ReadServiceConfiguration.PlcType.ToLower())
            {
                case "siemens":
                    SiemensPlcConfigService siemensPlcConfigService = new SiemensPlcConfigService(ReadServiceConfiguration.SqliteConnectionString);
                    PlcConfigs = siemensPlcConfigService.GetActiveFurnaceConfigurations().ToList();
                    SiemensTagConfigurationService siemensTagConfigurationService = new SiemensTagConfigurationService(ReadServiceConfiguration.SqliteConnectionString);
                    int[] plcDeviceIds = ((List<SiemensPlcConfig>)PlcConfigs).Select(x => x.PlcDeviceId).ToArray();
                    MultipleDeviceSiemensReadRequestItems = siemensTagConfigurationService.MultiplePlcReadRequestItems(plcDeviceIds);
                    OnDemandDBNumberCollections = new Dictionary<int, Dictionary<int, bool>>();

                    foreach (var siemensReadRequestItems in MultipleDeviceSiemensReadRequestItems)
                    {
                        try
                        {
                           OnDemandDBNumberCollections[siemensReadRequestItems.Key] = siemensReadRequestItems.Value.Where(x => x.IsDemanded == false).ToDictionary(x => x.DbNumber, x => x.IsDemanded);             
                        }
                        catch (Exception ex)
                        {
                            ex.Data["ConfigDetail"] = "Check ondemand item in configuration db";
                            throw ex;
                        }
                    }

                    break;
                case "allenbradley":
                    //Allenbradley implementation
                    break;
                default:
                    throw new Exception("Check configuration file");
            }
        }


        public Dictionary<int, List<SiemensReadRequestItem>> MultipleDeviceSiemensReadRequestItems { get; private set; }


        /// <summary>
        /// Stores ondemand db numbers. It can change during execution.
        /// </summary>
        public Dictionary<int, Dictionary<int,bool>> OnDemandDBNumberCollections { get; set; }


        /// <summary>
        /// Changes Ondemand db numbers whether read or not. It depends on ondemand configuration in initial configuration file.
        /// </summary>
        public void ChangeDBNumberDemand(int plcDeviceId, int dbNumber, bool isDemanded)
        {
            MultipleDeviceSiemensReadRequestItems[plcDeviceId].Find(x => x.DbNumber == dbNumber).IsDemanded = isDemanded;
        }

        public ReadServiceConfiguration ReadServiceConfiguration { get; private set; }

        public object PlcConfigs { get; private set; }

    }
}