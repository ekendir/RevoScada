using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RevoScada.ProcessController;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.Models.SettingModels;
using RevoScada.Entities;
using RevoScada.Entities.Complex.Alarm;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.PageTagConfigurations;

namespace RevoScada.DesktopApplication.ViewModels
{
    public class ScadaSettingsVM : ObservableObject
    {
        public string ActionResultMessage { get; set; }
        private SettingsTagConfigurations _settingsTagConfigurations { get; set; }

        private ObservableCollection<SettingsLastDBStatusGridModel> _settingsLastDBStatusGridModels;
        public ObservableCollection<SettingsLastDBStatusGridModel> SettingsLastDBStatusGridModels
        {
            get => _settingsLastDBStatusGridModels;
            set => OnPropertyChanged(ref _settingsLastDBStatusGridModels, value);
        }

        private string _readServiceStateInfo;
        public string ReadServiceStateInfo
        {
            get => _readServiceStateInfo;
            set => OnPropertyChanged(ref _readServiceStateInfo, value);
        }

        private int _minLoadNumberSerial;
        public int MinLoadNumberSerial
        {
            get => _minLoadNumberSerial;
            set => OnPropertyChanged(ref _minLoadNumberSerial, value);
        }

        public ReportExportSettings ReportExportSettings { get; set; }
        public List<AirTcSelection> AirTcSelections { get; set; }
        public bool OscillationEnabled { get; set; }
        public bool CascadeControlEnabled { get; set; }

        public ScadaSettingsVM()
        {
            try
            {
                ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
                ApplicationProperty applicationProperty = applicationPropertyService.GetByName("AirTcSelections");
                var airTcSelectionsSerialized = applicationProperty.Value;
                AirTcSelections = JsonConvert.DeserializeObject<List<AirTcSelection>>(airTcSelectionsSerialized);

                PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
                var pageTagConfiguration = pageTagConfigurationService.GetByName("Settings");
                string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
                _settingsTagConfigurations = JsonConvert.DeserializeObject<SettingsTagConfigurations>(jsonSerializedString);
                _settingsTagConfigurations.CascadeControlEnabled = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(_settingsTagConfigurations.CascadeControlEnabled)];
                _settingsTagConfigurations.OscillationEnabled = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(_settingsTagConfigurations.OscillationEnabled)];
               
                UpdateCheckStateFromPLC();
            }
            catch
            {
            }
        }

        public void UpdateStatusGridAsync()
        {


            Dictionary<int, LastDBStatus> statuses = new Dictionary<int, LastDBStatus>();
            try
            {
                statuses = ProcessManager.Instance.LastDbReadStatuses();

            }
            catch (Exception)
            {
                return;
            }

            List<LastDBStatus> lastDBStatuses = new List<LastDBStatus>();

            lastDBStatuses = statuses.Select(x => x.Value).ToList();

            SettingsLastDBStatusGridModels = SettingsLastDBStatusGridModels is null ? new ObservableCollection<SettingsLastDBStatusGridModel>() : SettingsLastDBStatusGridModels;

            foreach (LastDBStatus item in statuses.Values.OrderBy(x => x.DBNumber))
            {
                SettingsLastDBStatusGridModel settingsLastDBStatusGridModel = SettingsLastDBStatusGridModels.FirstOrDefault(x => x.LastDBStatus.DBNumber == item.DBNumber);

                if (settingsLastDBStatusGridModel == null)
                {
                    settingsLastDBStatusGridModel = new SettingsLastDBStatusGridModel
                    {
                        LastDBStatus = item
                    };

                    SettingsLastDBStatusGridModels.Add(settingsLastDBStatusGridModel);
                }
                else
                {
                    int index = _settingsLastDBStatusGridModels.IndexOf(SettingsLastDBStatusGridModels.FirstOrDefault(x => x.LastDBStatus.DBNumber == item.DBNumber));

                    settingsLastDBStatusGridModel = new SettingsLastDBStatusGridModel
                    {
                        LastDBStatus = item
                    };


                    SettingsLastDBStatusGridModels[index] = settingsLastDBStatusGridModel;
                }
            }


            var readServiceState = ProcessManager.Instance.LastReadStatus();
            ReadServiceStateInfo = $"Last Read: {readServiceState.LastCycleRunTime:dd:MM:yyyy HH:mm:ss.ff} Total DB: {readServiceState.GetAllDBCount}";


        }


        public void SaveApplicationInitialSettings()
        {
            try
            {
                var serializedApplicationInitialSettings = JsonConvert.SerializeObject(ReportExportSettings);
                ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
                applicationPropertyService.UpdateByName("ReportExportSettings", serializedApplicationInitialSettings);
                ActionResultMessage = "Saved!";
            }
            catch (Exception)
            {
                ActionResultMessage = "Error!";
            }
        }

        //todo:l refactor name
        public ReportExportSettings GetApplicationInitialSettings()
        {
            ReportExportSettings reportSettings;
            try
            {
                ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
                ApplicationProperty applicationProperty = applicationPropertyService.GetByName("ReportExportSettings");
                reportSettings = JsonConvert.DeserializeObject<ReportExportSettings>(applicationProperty.Value);
                reportSettings = reportSettings ?? new ReportExportSettings();
            }
            catch
            {
                reportSettings = new ReportExportSettings();
            }
            ReportExportSettings = reportSettings;
            return reportSettings;
        }

        public void SaveAirTcSelections(AirTcSelection airTcSelection)
        {
            try
            {
                foreach (var item in AirTcSelections)
                {
                    if (item.Value == airTcSelection.Value)
                    {
                        item.IsSelected = true;
                    }
                    else
                    {
                        item.IsSelected = false;
                    }
                }

                var serializedJson = JsonConvert.SerializeObject(AirTcSelections, new JsonSerializerSettings { Formatting = Formatting.Indented });
                ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
                applicationPropertyService.UpdateByName("AirTcSelections", serializedJson);

                PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
                var siemensTagConfiguration = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[airTcSelection.TagId];
                plcCommandManager.Set(siemensTagConfiguration, airTcSelection.Value);
            }
            catch
            {
            }
        }

        public void UpdateCheckStateFromPLC()
        {
            PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
            OscillationEnabled = plcCommandManager.Get<bool>((SiemensTagConfiguration)_settingsTagConfigurations.OscillationEnabled, true);
            CascadeControlEnabled = plcCommandManager.Get<bool>((SiemensTagConfiguration)_settingsTagConfigurations.CascadeControlEnabled, true);
        }

        public void SaveFurnaceProcessSettingstoPLC(string typeOfSettings, int newValue)
        {
            PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);

            switch (typeOfSettings)
            {
                case "cascadeControlEnabled":
                    plcCommandManager.Set((SiemensTagConfiguration)_settingsTagConfigurations.CascadeControlEnabled, newValue);
                    break;
                case "oscillationEnabled":
                    plcCommandManager.Set((SiemensTagConfiguration)_settingsTagConfigurations.OscillationEnabled, newValue);
                    break;
            }
        }
    }
}
