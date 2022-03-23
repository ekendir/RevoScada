using System;
using Newtonsoft.Json;
using NUnit.Framework;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.Configuration.Service;
using RevoScada.Entities.PageTagConfigurations;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RevoScada.Entities.Enums;

namespace RevoScada.DesktopApplication.Test
{
    [TestFixture]
    public class PageTagConfigurations
    {
        [SetUp]
        public void Init()
        {
            //   cacheManager = new CacheManager(CacheDBType.ReadService, ReadConfigurations.Instance.ReadServiceConfiguration.RedisServer);
            //   LogManager.Instance.InitializeConfiguration(ReadConfigurations.Instance.ReadServiceConfiguration.LogSettings);

            ApplicationConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\Test_DesktopApplication.rsconfig",true);

        }

        [Test]
        public void Get_all_tagconfigurations()
        {

            ApplicationConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\Test_DesktopApplication.rsconfig",true);

            var tagConfigurations = ApplicationConfigurations.Instance.TagConfigurations;


            /// sample usage tags
            var batchFinish = (SiemensTagConfiguration)tagConfigurations.First(x => ((SiemensTagConfiguration)x.Value).TagName == "Batch_Finish").Value;

            Dictionary<string, SiemensTagConfiguration> applicationButtonTagConfigurationMap = new Dictionary<string, SiemensTagConfiguration>();

            applicationButtonTagConfigurationMap.Add("btnBatch_Finish", batchFinish);

        }

        [Test]
        public void VacuumLines()
        {
            ApplicationConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\Test_DesktopApplication.rsconfig",true);

            var tagConfigurations = ApplicationConfigurations.Instance.TagConfigurations;


            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);

            var pageTagConfiguration = pageTagConfigurationService.GetByName("VacuumLines");

            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;

            var pageTagConfigurations = JsonConvert.DeserializeObject<VacuumLinesTagConfigurations>(jsonSerializedString);

            SiemensTagConfiguration vacPumpControlStateIsAuto = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[pageTagConfigurations.VacPumpControlStateIsAuto]);








            //  Assert.IsNotNull(pageTagConfigurations);

        }




        [Test]
        public void EnterParts()
        {

            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);

            var pageTagConfiguration = pageTagConfigurationService.GetByName("EnterParts");

            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;

            var pageTagConfigurations = JsonConvert.DeserializeObject<EnterPartsTagConfigurations>(jsonSerializedString);

            Assert.IsNotNull(pageTagConfigurations);

        }

    


        [Test]
        public void RecipeEditor()
        {
            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);

            var pageTagConfiguration = pageTagConfigurationService.GetByName("RecipeEditor");

            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;

            var pageTagConfigurations = JsonConvert.DeserializeObject<RecipeEditorTagConfigurations>(jsonSerializedString);

            Assert.IsNotNull(pageTagConfigurations);
        }

        [Test]
        public void IntegrityChecks()
        {

            
            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);

            var pageTagConfiguration = pageTagConfigurationService.GetByName("IntegrityChecks");

            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;

            var pageTagConfigurations = JsonConvert.DeserializeObject<IntegrityChecksTagConfigurations>(jsonSerializedString);


           
            Assert.IsNotNull(pageTagConfigurations);
        
        }

   


        [Test]
        public void SensorView()
        {
            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);

            var pageTagConfiguration = pageTagConfigurationService.GetByName("SensorView");

            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;

            var pageTagConfigurations = JsonConvert.DeserializeObject<SensorViewTagConfigurations>(jsonSerializedString);

            Assert.IsNotNull(pageTagConfigurations);
        }

        [Test]
        public void RunOperation()
        {
            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);

            var pageTagConfiguration = pageTagConfigurationService.GetByName("RunOperation");

            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;

            var pageTagConfigurations = JsonConvert.DeserializeObject<RunOperationTagConfigurations>(jsonSerializedString);

            Assert.IsNotNull(pageTagConfigurations);
        }

        [Test]
        public void ManuelOperation()
        {
            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);

            var pageTagConfiguration = pageTagConfigurationService.GetByName("ManualOperation");

            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;

            var pageTagConfigurations = JsonConvert.DeserializeObject<ManualOperationTagConfigurations>(jsonSerializedString);

            Assert.IsNotNull(pageTagConfigurations);
        }

        [Test]
        public void Alarm()
        {
            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);

            var pageTagConfiguration = pageTagConfigurationService.GetByName("Alarm");

            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;

            var pageTagConfigurations = JsonConvert.DeserializeObject<AlarmTagConfigurations>(jsonSerializedString);

            Assert.IsNotNull(pageTagConfigurations);
        }

        [Test]
        public void Recipe()
        {
            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);

            var pageTagConfiguration = pageTagConfigurationService.GetByName("Recipe");

            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;

            var pageTagConfigurations = JsonConvert.DeserializeObject<RecipeTagConfigurations>(jsonSerializedString);

            Assert.IsNotNull(pageTagConfigurations);
        }

        [Test]
        public void Quality()
        {
            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);

            var pageTagConfiguration = pageTagConfigurationService.GetByName("Quality");

            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;

            var pageTagConfigurations = JsonConvert.DeserializeObject<QualityTagConfigurations>(jsonSerializedString);

            Assert.IsNotNull(pageTagConfigurations);
        }

        [Test]
        public void Calibration()
        {
            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);

            var pageTagConfiguration = pageTagConfigurationService.GetByName("Calibration");

            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;

            var pageTagConfigurations = JsonConvert.DeserializeObject<CalibrationTagConfigurations>(jsonSerializedString);

            Assert.IsNotNull(pageTagConfigurations);
        }

        [Test]
        public void Oscillation()
        {
            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);

            var pageTagConfiguration = pageTagConfigurationService.GetByName("Oscillation");

            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;

            var pageTagConfigurations = JsonConvert.DeserializeObject<OscillationTagConfigurations>(jsonSerializedString);

            Assert.IsNotNull(pageTagConfigurations);
        }


        [Test]
        public void LeftHamburgerMenu()
        {
            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);

            var pageTagConfiguration = pageTagConfigurationService.GetByName("HamburgerMenuLeft");

            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;

            var pageTagConfigurations = JsonConvert.DeserializeObject<HamburgerMenuTagConfigurations>(jsonSerializedString);

            Assert.IsNotNull(pageTagConfigurations);
        }



        //[Test]
        //public void Reports()
        //{
        //    PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(DesktopApplicationConfigurations.Instance.DesktopApplicationConfiguration.PostgreSqlConnectionString);

        //    var pageTagConfiguration = pageTagConfigurationService.GetByName("Reports");

        //    string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;

        //    var integratedCheckTagConfigurations = JsonConvert.DeserializeObject<IntegratedChecksTagConfigurations>(jsonSerializedString);

        //    Assert.IsNotNull(integratedCheckTagConfigurations.FinishIncrementAmount);
        //}

        //[Test]
        //public void Trend()
        //{
        //    PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(DesktopApplicationConfigurations.Instance.DesktopApplicationConfiguration.PostgreSqlConnectionString);

        //    var pageTagConfiguration = pageTagConfigurationService.GetByName("Trend");

        //    string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;

        //    var integratedCheckTagConfigurations = JsonConvert.DeserializeObject<IntegratedChecksTagConfigurations>(jsonSerializedString);

        //    Assert.IsNotNull(integratedCheckTagConfigurations.FinishIncrementAmount);
        //}

    }
}
