
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
using RevoScada.Entities;
using RevoScada.ProcessController;

namespace RevoScada.DesktopApplication.Test
{
    [TestFixture]
    class VacuumLinesTest
    {
        List<SiemensTagConfiguration> SiemensTagConfigurationsVacuumValue;
        List<SiemensTagConfiguration> SiemensTagConfigurationsVacuumPortIsAuto;
        List<SiemensTagConfiguration> SiemensTagConfigurationsVacVentOff;
        VacuumLinesTagConfigurations VacuumLinesTagConfigurations;
        string _cacheServer;

        [SetUp]
        public void Init()
        {
            ApplicationConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\Test_DesktopApplication.rsconfig",true);
            _cacheServer = ApplicationConfigurations.Instance.Configuration.RedisServer;
            InitializePageTagConfigurations();
           
        }

        public void InitializePageTagConfigurations()
        {
            SiemensTagConfigurationsVacuumValue = new List<SiemensTagConfiguration>();

            SiemensTagConfigurationsVacuumPortIsAuto = new List<SiemensTagConfiguration>();

            SiemensTagConfigurationsVacVentOff = new List<SiemensTagConfiguration>();


            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
            var pageTagConfiguration = pageTagConfigurationService.GetByName("VacuumLines");
            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
            VacuumLinesTagConfigurations = JsonConvert.DeserializeObject<VacuumLinesTagConfigurations>(jsonSerializedString);

            foreach (var vacuumPort in VacuumLinesTagConfigurations.VacuumPorts.Values)
            {
                SiemensTagConfiguration mon = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[vacuumPort.Value]);
                SiemensTagConfigurationsVacuumValue.Add(mon);
            }

            foreach (var vacuumPort in VacuumLinesTagConfigurations.VacuumPorts.Values)
            {
                SiemensTagConfiguration mon = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[vacuumPort.IsAuto]);
                SiemensTagConfigurationsVacuumPortIsAuto.Add(mon);
            }

 
            foreach (var vacuumPort in VacuumLinesTagConfigurations.VacuumPorts.Values)
            {
                SiemensTagConfiguration mon = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[vacuumPort.VacVentOff]);
                SiemensTagConfigurationsVacVentOff.Add(mon);
            }


        }

        [Test]
        public void Get_vacuum_mon_value()
        {
 
            Dictionary<string, float> vacuumPortValues = new Dictionary<string, float>();

            PlcCommandManager plcCommandManager = new PlcCommandManager(_cacheServer);
            ;
            foreach (var vacuumPort in SiemensTagConfigurationsVacuumValue.Take(100))
            {
            float value = plcCommandManager.Get<float>(vacuumPort, false);
            vacuumPortValues[vacuumPort.TagName] = value;
            }


        }
     




        [Test]
        public void Set_vacuum_mon_isauto()
        {

            var setlenecekPort = SiemensTagConfigurationsVacuumPortIsAuto.First(x => x.TagName == "MON1");
            int neyisetlicez = 0;  

            PlcCommandManager plcCommandManager = new PlcCommandManager(_cacheServer);
            plcCommandManager.Set(setlenecekPort, neyisetlicez);

        }

        [Test]
        public void Get_vacuum_mon_vacventoff()
        {

            Dictionary<string, int> vacuumPortVacVentOff = new Dictionary<string, int>();

            PlcCommandManager plcCommandManager = new PlcCommandManager(_cacheServer);

            foreach (var vacuumPort in SiemensTagConfigurationsVacVentOff)
            {
                int value = plcCommandManager.Get<int>(vacuumPort, false);
                vacuumPortVacVentOff[vacuumPort.TagName] = value;
            }


        }

        [Test]
        public void Set_vacuum_mon_vacventoff()
        {

            var setlenecekPort =SiemensTagConfigurationsVacVentOff.First(x => x.TagName == "MON1");

            int neyisetlicez = 0;  


            PlcCommandManager plcCommandManager = new PlcCommandManager(_cacheServer);
            plcCommandManager.Set(setlenecekPort, neyisetlicez);

        }


        [Test]
        public void set_VacPumpControlStateIsAuto()
        {

            SiemensTagConfiguration tagConfiguration = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.VacPumpControlStateIsAuto]);

            PlcCommandManager plcCommandManager = new PlcCommandManager(_cacheServer);
            
            plcCommandManager.Set(tagConfiguration, 0);

        }


        [Test]
        public void set_VacPumpControlStateOnOff()
        {

            SiemensTagConfiguration tagConfiguration = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.VacPumpControlStateOnOff]);

            PlcCommandManager plcCommandManager = new PlcCommandManager(_cacheServer);
            
            plcCommandManager.Set(tagConfiguration, 0);

        }


        #region  VacSetControlStatus

        [Test]
        public void set_VacSetControlStatusIsAuto()
        {

            SiemensTagConfiguration tagConfiguration = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.VacSetControlStatusIsAuto]);

            PlcCommandManager plcCommandManager = new PlcCommandManager(_cacheServer);
             
            plcCommandManager.Set(tagConfiguration, 0);

        }

        [Test]
        public void set_VacSetControlStatusPid()
        {

            SiemensTagConfiguration tagConfiguration = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.VacSetControlStatusPid]);

            PlcCommandManager plcCommandManager = new PlcCommandManager(_cacheServer);
            
            plcCommandManager.Set(tagConfiguration, 0);

        }

        [Test]
        public void set_VacSetControlStatusSp()
        {

            SiemensTagConfiguration tagConfiguration = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.VacSetControlStatusSp]);

            PlcCommandManager plcCommandManager = new PlcCommandManager(_cacheServer);
            
            plcCommandManager.Set(tagConfiguration, 0);

        }



        #endregion


        #region SystemVacuumValues

        [Test]
        public void get_SystemVacuumValuesPv()
        {

            SiemensTagConfiguration tagConfigurationPv = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.SystemVacuumValuesPv]);

            SiemensTagConfiguration tagConfigurationRate = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.SystemVacuumValuesRate]);

            PlcCommandManager plcCommandManager = new PlcCommandManager(_cacheServer);

            float valuePv=   plcCommandManager.Get<float>(tagConfigurationPv, true);
            float valueRate = plcCommandManager.Get<float>(tagConfigurationRate, true);

        }

        #endregion


        [Test]
        public void get_monitoring_lines()
        {
            SiemensTagConfiguration t1high = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.MonitoringLinesHighMonPort]);
            SiemensTagConfiguration t2high = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.MonitoringLinesHighMonVacuumValue]);
            SiemensTagConfiguration t3high = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.MonitoringLinesHighMonVacuumValueInTime]);
            SiemensTagConfiguration t1low = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.MonitoringLinesLowMonPort]);
            SiemensTagConfiguration t2low = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.MonitoringLinesLowMonVacuumValue]);
            SiemensTagConfiguration t3low = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.MonitoringLinesLowMonVacuumValueInTime]);

            PlcCommandManager plcCommandManager = new PlcCommandManager(_cacheServer);
            float v1high = plcCommandManager.Get<float>(t1high, true);
            float v2high = plcCommandManager.Get<float>(t2high, true);
            float v3high = plcCommandManager.Get<float>(t3high, true);
            float v1low = plcCommandManager.Get<float>(t1low, true);
            float v2low = plcCommandManager.Get<float>(t2low, true);
            float v3low = plcCommandManager.Get<float>(t3low, true);



        }

        [Test]
        public void set_all_vacuum_line_control_action_auto()
        {
            SiemensTagConfiguration tagConfiguration = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.AllVacuumLineControlActionAuto]);

            PlcCommandManager plcCommandManager = new PlcCommandManager(_cacheServer);
            
            plcCommandManager.Set(tagConfiguration, 0);

        }

        [Test]
        public void set_all_vacuum_line_control_action_manuel()
        {
            SiemensTagConfiguration tagConfiguration = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.AllVacuumLineControlActionManuel]);

            PlcCommandManager plcCommandManager = new PlcCommandManager(_cacheServer);
             
            plcCommandManager.Set(tagConfiguration, 0);

        }

        [Test]
        public void set_all_vacuum_line_control_action_off()
        {
            SiemensTagConfiguration tagConfiguration = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.AllVacuumLineControlActionOff]);

            PlcCommandManager plcCommandManager = new PlcCommandManager(_cacheServer);

            
            plcCommandManager.Set(tagConfiguration, 0);

        }
      
        [Test]
        public void set_all_vacuum_line_control_action_vac()
        {
            SiemensTagConfiguration tagConfiguration = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.AllVacuumLineControlActionVac]);

            PlcCommandManager plcCommandManager = new PlcCommandManager(_cacheServer);
             
            plcCommandManager.Set(tagConfiguration, 0);

        }

        [Test]
        public void set_all_vacuum_line_control_action_vent()
        {
 
            SiemensTagConfiguration tagConfiguration = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[VacuumLinesTagConfigurations.AllVacuumLineControlActionVent]);

            PlcCommandManager plcCommandManager = new PlcCommandManager(_cacheServer);
            
            plcCommandManager.Set(tagConfiguration, 0);

        }

    }
}