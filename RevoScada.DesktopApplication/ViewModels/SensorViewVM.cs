using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.WindowsUI;
using Newtonsoft.Json;
using RevoScada.ProcessController;
using RevoScada.Synchronization.Enums;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.Models.ModelEnums;
using RevoScada.DesktopApplication.Views;
using RevoScada.DesktopApplication.Views.Popups;
using RevoScada.Entities;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.Configuration.Service;
using RevoScada.Entities.Enums;
using RevoScada.Entities.PageTagConfigurations;
using DevExpress.Xpf.Grid;
using RevoScada.DesktopApplication.Models.SettingModels;
using System.Drawing;
using System.Windows.Media;
using Revo.Core.Data;
using Revo.Core;
using System.Threading;


namespace RevoScada.DesktopApplication.ViewModels
{
    public class SensorViewVM : UserControlBaseVM
    {
        #region Collections
        private ObservableCollection<SensorViewItemsTableRow> _PTCSensorViewItemsTableRows;
        private ObservableCollection<SensorViewItemsTableRow> _MONSensorViewItemsTableRows;
        public ObservableCollection<SensorViewItemsTableRow> PTCSensorViewItemsTableRows
        {
            get => _PTCSensorViewItemsTableRows;
            set => OnPropertyChanged(ref _PTCSensorViewItemsTableRows, value);
        }
        public ObservableCollection<SensorViewItemsTableRow> MONSensorViewItemsTableRows
        {
            get => _MONSensorViewItemsTableRows;
            set => OnPropertyChanged(ref _MONSensorViewItemsTableRows, value);
        }
        private Dictionary<string, BagDetailDto> BagDetails;
        private Dictionary<int, SolidColorBrush> _rowColorContainer = new Dictionary<int, SolidColorBrush>();
        #endregion


        #region Fields
        private PlcCommandManager _plcCommandManager;
        Dictionary<int, SensorViewPorts> _PTCPortTagConfiguration = new Dictionary<int, SensorViewPorts>();
        Dictionary<int, SensorViewPorts> _MONPortTagConfiguration = new Dictionary<int, SensorViewPorts>();
        private readonly string _connectionString;
        private readonly ApplicationColors _applicationColors;
        private ProcessEventLogService _processEventLogService;
        public SensorViewTagConfigurations _sensorViewTagConfigurations;
        #endregion
        #region Properties
        private string _ptcGridViewFilterCriteria;
        public string PTCGridViewFilterCriteria
        {
            get => _ptcGridViewFilterCriteria;
            set => OnPropertyChanged(ref _ptcGridViewFilterCriteria, value);
        }

        private string _monGridViewFilterCriteria;
        public string MONGridViewFilterCriteria
        {
            get => _monGridViewFilterCriteria;
            set => OnPropertyChanged(ref _monGridViewFilterCriteria, value);
        }

        private SensorViewFilterSettingsModel _sensorViewFilterSettings;
        public SensorViewFilterSettingsModel SensorViewFilterSettings
        {
            get
            {
                ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(_connectionString);
                ApplicationProperty applicationProperty = applicationPropertyService.GetByName("SensorViewFilterSettings");

                if (!string.IsNullOrEmpty(applicationProperty?.Value))
                {
                    _sensorViewFilterSettings = JsonConvert.DeserializeObject<SensorViewFilterSettingsModel>(applicationProperty.Value);
                }
                else
                {
                    _sensorViewFilterSettings = new SensorViewFilterSettingsModel
                    {
                        PTCGridViewFilterCriteria = string.Empty,
                        MONGridViewFilterCriteria = string.Empty,
                    };
                }
                return _sensorViewFilterSettings;
            }
            set
            {
                _sensorViewFilterSettings.PTCGridViewFilterCriteria = PTCGridViewFilterCriteria;
                _sensorViewFilterSettings.MONGridViewFilterCriteria = MONGridViewFilterCriteria;

                string settingsSerialized = JsonConvert.SerializeObject(_sensorViewFilterSettings);
                ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(_connectionString);
                applicationPropertyService.UpdateByName("SensorViewFilterSettings", settingsSerialized);
            }
        }
        #endregion

        public SensorViewVM(WaitIndicatorControl waitIndicatorControl, Dictionary<string, bool> permissions, UserGridModel activeUser)
        {
            InitializeSensorViewTagConfigurations();
            _connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;
            _processEventLogService = new ProcessEventLogService(_connectionString);
            Permissions = permissions;
            ActiveUser = activeUser;
            _plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
            WaitIndicatorControl = waitIndicatorControl;
            WaitIndicatorControl.IsWaitIndicatorVisible = false;


            _applicationColors = JsonConvert.DeserializeObject<ApplicationColors>(ProcessManager.Instance.ApplicationProperties["ApplicationColors"].Value);
            _rowColorContainer = new Dictionary<int, SolidColorBrush>();
            _rowColorContainer.Add(0, (SolidColorBrush)new BrushConverter().ConvertFromString(_applicationColors.SensorViewRegularColor));
            _rowColorContainer.Add(1, (SolidColorBrush)new BrushConverter().ConvertFromString(_applicationColors.SensorViewEnterPartsEnable));
            _rowColorContainer.Add(2, (SolidColorBrush)new BrushConverter().ConvertFromString(_applicationColors.SensorViewOperatorDisable));
            _rowColorContainer.Add(3, (SolidColorBrush)new BrushConverter().ConvertFromString(_applicationColors.SensorViewOscillationDisable));
            _rowColorContainer.Add(4, (SolidColorBrush)new BrushConverter().ConvertFromString(_applicationColors.SensorViewOperatorEnable));

            CreatePTCTable();

            CreateVACTable();

            UpdateSensorData();

            // Get PTC and MON grid filter values on page load
            if (!string.IsNullOrEmpty(SensorViewFilterSettings.PTCGridViewFilterCriteria))
                PTCGridViewFilterCriteria = SensorViewFilterSettings.PTCGridViewFilterCriteria;

            if (!string.IsNullOrEmpty(SensorViewFilterSettings.MONGridViewFilterCriteria))
                MONGridViewFilterCriteria = SensorViewFilterSettings.MONGridViewFilterCriteria;
        }


        private void InitializeSensorViewTagConfigurations()
        {
            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);

            var pageTagConfiguration = pageTagConfigurationService.GetByName("SensorView");

            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;

            _sensorViewTagConfigurations = JsonConvert.DeserializeObject<SensorViewTagConfigurations>(jsonSerializedString);

            SetSensorViewDatablock(true);

            int ptcCount = Convert.ToInt32(ProcessManager.Instance.ApplicationProperties["PTCCount"].Value);
            int monCount = Convert.ToInt32(ProcessManager.Instance.ApplicationProperties["MONCount"].Value);

            BagDetails = ProcessManager.Instance.ProcessBagDetails;

            foreach (var partTemperaturePortItem in _sensorViewTagConfigurations.PartTemperaturePorts.Take(ptcCount))
            {
                SensorViewPorts sensorViewItemsTagConfiguration = new SensorViewPorts();
                int portNumeric = Convert.ToInt32(partTemperaturePortItem.Key.TrimStart('P', 'T', 'C'));
                sensorViewItemsTagConfiguration.Value = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(partTemperaturePortItem.Value.Value)];
                sensorViewItemsTagConfiguration.EnableStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(partTemperaturePortItem.Value.EnableStatus)];
                sensorViewItemsTagConfiguration.Rate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(partTemperaturePortItem.Value.Rate)];
                _PTCPortTagConfiguration.Add(portNumeric, sensorViewItemsTagConfiguration);
            }

            foreach (var partTemperaturePortItem in _sensorViewTagConfigurations.PartVacuumDatas.Take(monCount))
            {
                SensorViewPorts sensorViewItemsTagConfiguration = new SensorViewPorts();
                int portNumeric = Convert.ToInt32(partTemperaturePortItem.Key.TrimStart('M', 'O', 'N'));
                sensorViewItemsTagConfiguration.Value = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(partTemperaturePortItem.Value.Value)];
                sensorViewItemsTagConfiguration.EnableStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(partTemperaturePortItem.Value.EnableStatus)];
                sensorViewItemsTagConfiguration.Rate = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(partTemperaturePortItem.Value.Rate)];
                _MONPortTagConfiguration.Add(portNumeric, sensorViewItemsTagConfiguration);
            }
        }
        public async Task<bool> SetSensorViewDatablock(bool value)
        {
            bool result = false;

            await Task.Run(() =>
                {
                    if (_sensorViewTagConfigurations.DbNumbers != null)
                    {
                        for (int i = 0; i < _sensorViewTagConfigurations.DbNumbers.Count; i++)
                        {
                            result = ProcessManager.Instance.ChangeDemandReadStateOnCache(PlcDeviceId, _sensorViewTagConfigurations.DbNumbers[i], value);

                        }
                    }
                });
            await Task.Delay(3000);

            return result = false;
        }

        private void CreatePTCTable()
        {           
            PTCSensorViewItemsTableRows = new ObservableCollection<SensorViewItemsTableRow>();
            bool getAlwaysUpdatedResult = true;
            foreach (var sensorViewItem in _PTCPortTagConfiguration)
            {
                SensorViewItemsTableRow sensorViewItemsTableRow = new SensorViewItemsTableRow();
                sensorViewItemsTableRow.PortName = "PTC" + sensorViewItem.Key;
                sensorViewItemsTableRow.BagName = BagDetails.ContainsKey("PTC" + sensorViewItem.Key) ? BagDetails["PTC" + sensorViewItem.Key].BagName : "-";
                sensorViewItemsTableRow.EnableDisableToggleViewVisibility = sensorViewItemsTableRow.BagName.Length > 1 ? Visibility.Visible : Visibility.Collapsed;

                Task getPlcCommand = Task.Factory.StartNew(() =>
                {
                    sensorViewItemsTableRow.EnableDisableCommand = _plcCommandManager.Get<int>((SiemensTagConfiguration)_PTCPortTagConfiguration[sensorViewItemsTableRow.PortNumeric].EnableStatus, getAlwaysUpdatedResult);
                    sensorViewItemsTableRow.RowColor = _rowColorContainer[sensorViewItemsTableRow.EnableDisableCommand];
                    sensorViewItemsTableRow.EnableDisableButtonText = (sensorViewItemsTableRow.EnableDisableCommand == 1 || sensorViewItemsTableRow.EnableDisableCommand == 4) ? "Disable" : "Enable";
                    sensorViewItemsTableRow.EnableDisableStatus = (sensorViewItemsTableRow.EnableDisableCommand == 1 || sensorViewItemsTableRow.EnableDisableCommand == 4) ? "Enabled" : "Disabled";
                });
                PTCSensorViewItemsTableRows.Add(sensorViewItemsTableRow);
                getAlwaysUpdatedResult = false;
                getPlcCommand.Wait();
            }
        }

        public void CreateVACTable()
        {           
            MONSensorViewItemsTableRows = new ObservableCollection<SensorViewItemsTableRow>();
            bool getAlwaysUpdatedResult = true;
            foreach (var sensorViewItem in _MONPortTagConfiguration)
            {
                SensorViewItemsTableRow sensorViewItemsTableRow = new SensorViewItemsTableRow();
                sensorViewItemsTableRow.PortName = "MON" + sensorViewItem.Key;
                sensorViewItemsTableRow.BagName = BagDetails.ContainsKey("MON" + sensorViewItem.Key) ? BagDetails["MON" + sensorViewItem.Key].BagName : "-";
                sensorViewItemsTableRow.EnableDisableToggleViewVisibility = sensorViewItemsTableRow.BagName.Length > 1 ? Visibility.Visible : Visibility.Collapsed;

                Task getPlcCommand = Task.Factory.StartNew(() =>
                {
                    sensorViewItemsTableRow.EnableDisableCommand = _plcCommandManager.Get<int>((SiemensTagConfiguration)_MONPortTagConfiguration[sensorViewItemsTableRow.PortNumeric].EnableStatus, getAlwaysUpdatedResult);
                    sensorViewItemsTableRow.RowColor = _rowColorContainer[sensorViewItemsTableRow.EnableDisableCommand];
                    sensorViewItemsTableRow.EnableDisableButtonText = (sensorViewItemsTableRow.EnableDisableCommand == 1 || sensorViewItemsTableRow.EnableDisableCommand == 4) ? "Disable" : "Enable";
                    sensorViewItemsTableRow.EnableDisableStatus = (sensorViewItemsTableRow.EnableDisableCommand == 1 || sensorViewItemsTableRow.EnableDisableCommand == 4) ? "Enabled" : "Disabled";
                });
                MONSensorViewItemsTableRows.Add(sensorViewItemsTableRow);
                getAlwaysUpdatedResult = false;
                getPlcCommand.Wait();
            }
        }

        public void UpdateSensorData()
        {

            // for the first time take updated datablock then use it for others
            bool getAlwaysUpdatedResult = true;

            for (int i = 0; i < PTCSensorViewItemsTableRows.Count; i++)
            {
                int portNumeric = PTCSensorViewItemsTableRows[i].PortNumeric;
                // getAlwaysUpdatedResult = false;
                PTCSensorViewItemsTableRows[i].EnableDisableCommand = _plcCommandManager.Get<int>((SiemensTagConfiguration)(_PTCPortTagConfiguration[portNumeric].EnableStatus), getAlwaysUpdatedResult);
                PTCSensorViewItemsTableRows[i].PortValue = NumericManipulation.TryRoundFloat(_plcCommandManager.Get<float>((SiemensTagConfiguration)(_PTCPortTagConfiguration[portNumeric].Value), getAlwaysUpdatedResult), 2);
                PTCSensorViewItemsTableRows[i].RateValue = NumericManipulation.TryRoundFloat(_plcCommandManager.Get<float>((SiemensTagConfiguration)(_PTCPortTagConfiguration[portNumeric].Rate), getAlwaysUpdatedResult), 2);
                PTCSensorViewItemsTableRows[i].RowColor = _rowColorContainer[PTCSensorViewItemsTableRows[i].EnableDisableCommand];
                PTCSensorViewItemsTableRows[i].EnableDisableStatus = (PTCSensorViewItemsTableRows[i].EnableDisableCommand == 1 || PTCSensorViewItemsTableRows[i].EnableDisableCommand == 4) ? "Enabled" : "Disabled";
            }

            for (int i = 0; i < MONSensorViewItemsTableRows.Count; i++)
            {
                int portNumeric = MONSensorViewItemsTableRows[i].PortNumeric;
                // getAlwaysUpdatedResult = false;
                MONSensorViewItemsTableRows[i].EnableDisableCommand = _plcCommandManager.Get<int>((SiemensTagConfiguration)(_MONPortTagConfiguration[portNumeric].EnableStatus), getAlwaysUpdatedResult);
                MONSensorViewItemsTableRows[i].PortValue = NumericManipulation.TryRoundFloat(_plcCommandManager.Get<float>((SiemensTagConfiguration)(_MONPortTagConfiguration[portNumeric].Value), getAlwaysUpdatedResult), 2);
                MONSensorViewItemsTableRows[i].RateValue = NumericManipulation.TryRoundFloat(_plcCommandManager.Get<float>((SiemensTagConfiguration)(_MONPortTagConfiguration[portNumeric].Rate), getAlwaysUpdatedResult), 2);
                MONSensorViewItemsTableRows[i].RowColor = _rowColorContainer[MONSensorViewItemsTableRows[i].EnableDisableCommand];
                MONSensorViewItemsTableRows[i].EnableDisableStatus = (MONSensorViewItemsTableRows[i].EnableDisableCommand == 1 || MONSensorViewItemsTableRows[i].EnableDisableCommand == 4) ? "Enabled" : "Disabled";
            }
        }

        public async Task<bool> TogglePortUsageButtonEnable(SensorViewItemsTableRow sensorViewItemsTableRow)
        {
            if (Permissions.ContainsKey("enableDisablePort"))
            {
                // If permission had not been granted, return
                if (!Permissions["enableDisablePort"])
                    return false;
            }

            var enableDisableCommandTagConfig = new SiemensTagConfiguration();
            bool isEnabledCommandRequest = !(sensorViewItemsTableRow.EnableDisableCommand == 1 || sensorViewItemsTableRow.EnableDisableCommand == 4);
            switch (sensorViewItemsTableRow.PortType)
            {
                case "PTC":
                    enableDisableCommandTagConfig = (SiemensTagConfiguration)_PTCPortTagConfiguration[sensorViewItemsTableRow.PortNumeric].EnableStatus;
                    break;
                case "MON":
                    enableDisableCommandTagConfig = (SiemensTagConfiguration)_MONPortTagConfiguration[sensorViewItemsTableRow.PortNumeric].EnableStatus;
                    break;
            }

            WaitIndicatorControl.IsWaitIndicatorVisible = true;
            int setValue = 0;
            bool result = false;

            if (isEnabledCommandRequest)
            {
                setValue = 4; // Sends operator enable command
                Guid guid = Guid.NewGuid();
                _plcCommandManager.Set(enableDisableCommandTagConfig, setValue, guid);
                result = await _plcCommandManager.IsUpdatedResultAsync(guid, true, 1500);
            }
            else
            {
                setValue = 2; //  Sends disable command
                Guid guid = Guid.NewGuid();
                _plcCommandManager.Set(enableDisableCommandTagConfig, setValue, guid);
                result = await _plcCommandManager.IsUpdatedResultAsync(guid, true, 1500);
            }

            if (!result)
            {
                WaitIndicatorControl.IsWaitIndicatorVisible = false;
                return false; // Do not insert process event log to db if setting value to PLC failed.
            }
            else
            {
                sensorViewItemsTableRow.EnableDisableButtonText = isEnabledCommandRequest ? "Disable" : "Enable";
            }

            // Save process event log to DB
            ProcessEventLog sensorProcessEventLog = new ProcessEventLog();

            if (ProcessManager.Instance.CurrentProcess.IsBatchLoaded)
                sensorProcessEventLog.BatchId = ProcessManager.Instance.CurrentProcess.BatchId;

            sensorProcessEventLog.CreateDate = DateTime.Now;
            sensorProcessEventLog.Type = ProcessEventLogType.Manual.ToString();

            string eventText = $"Sensor View {sensorViewItemsTableRow.PortType} {sensorViewItemsTableRow.PortNumeric} port {((!isEnabledCommandRequest ? "disabled" : "enabled" ))}";
            sensorProcessEventLog.EventText = eventText;
            sensorProcessEventLog.ModifiedByUserId = ActiveUser.id;
            _processEventLogService.Insert(sensorProcessEventLog);

            var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
            ProcessEventLogAdapter processEventLogAdapter = new ProcessEventLogAdapter(ApplicationConfigurations.Instance.Configuration.RedisServer);
            processEventLogAdapter.CreateProcessEventLogSyncIssue(sensorProcessEventLog, fromToDirection, ApplicationConfigurations.Instance.Configuration.PlcDevice.Id);
            WaitIndicatorControl.IsWaitIndicatorVisible = false;

            return result;
        }
        public void SaveSensorViewFilterSettings()
        {
            try
            {
                SensorViewFilterSettings = _sensorViewFilterSettings;
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log($"SaveSensorViewFilterSettings Detail: {ex.Message}\n\n", LogType.Error);
            }
        }
    }
}