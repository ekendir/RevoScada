using System;
using Newtonsoft.Json;
using NUnit.Framework;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.Entities.Complex;
using System.Diagnostics;
using System.Linq;
using RevoScada.Entities;

using RevoScada.Entities.Configuration;
using RevoScada.Entities.Configuration.Service;
using RevoScada.Entities.PageTagConfigurations;
using System.Collections.Generic;

namespace RevoScada.DesktopApplication.Test
{
    [TestFixture]
    public class EnterPartsTest
    {
        List<SiemensTagConfiguration> SiemensTagConfigurationsVacVentOff;

        EnterPartsTagConfigurations EnterPartsTagConfigurations;
        

        [SetUp]
        public void Init()
        {
            ApplicationConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\Test_DesktopApplication.rsconfig",true);

            InitializePageTagConfigurations();
        }


        public void InitializePageTagConfigurations()
        {
                     
            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
            var pageTagConfiguration = pageTagConfigurationService.GetByName("EnterParts");
            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
            EnterPartsTagConfigurations = JsonConvert.DeserializeObject<EnterPartsTagConfigurations>(jsonSerializedString);
        }

   

        //[Test]
        //public void set_to_plc()
        //{

        //    SiemensTagConfiguration enterPartsOk = ((SiemensTagConfiguration)DesktopApplicationConfigurations.Instance.TagConfigurations[EnterPartsTagConfigurations.EnterPartsOk]);
        //    SiemensTagConfiguration activeBatchName = ((SiemensTagConfiguration)DesktopApplicationConfigurations.Instance.TagConfigurations[EnterPartsTagConfigurations.ActiveBatchName]);
 
      
        //    PlcCommandManager plcCommandManager = new PlcCommandManager(_cacheServer);

        //    plcCommandManager.Set(enterPartsOk, true); 
        //    plcCommandManager.Set(activeBatchName, "active batch name");
 

        //}

    }
}