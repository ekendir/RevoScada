using Newtonsoft.Json;
using Revo.Core;
using RevoScada.Business;
using RevoScada.Cache;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.Models.ModelEnums;
using RevoScada.DesktopApplication.Models.ModelTypes;
using RevoScada.DesktopApplication.Views.CalibrationViews;
using RevoScada.Entities;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.PageTagConfigurations;
using RevoScada.ProcessController;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RevoScada.DesktopApplication.ViewModels.CalibrationViewModels
{
    public abstract class CalibrationBase : UserControlBaseVM //ObservableObject
    {
        #region Commands
        public ICommand SetSourcetoLowSensorRangeCommand { get; set; }
        public ICommand SetSourcetoHighSensorRangeCommand { get; set; }
        public ICommand CalculateCommand { get; set; }
        public ICommand AcceptCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand ViewReportCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand OpenCalibrationCertificationWindowCommand { get; set; }

        #endregion

        public float TextBoxNewValue;
        protected CacheManager _mainCacheManager;
        protected CalibrationTagConfigurations CalibrationTagConfigurations;
        public CalibrationSettingsModel CalibrationSettings;
        protected readonly string _connectionString;
        protected readonly ApplicationConfiguration _applicationConfiguration;
        private ObservableCollection<CalibrationDataGrid> _calibrationListPTC;
        private ObservableCollection<CalibrationDataGrid> _calibrationListMON;
        private ObservableCollection<CalibrationDataGrid> _calibrationListMONCalibration;
        private ObservableCollection<CalibrationDataGrid> _calibrationListVACHeader;
        private ObservableCollection<CalibrationDataGrid> _calibrationListAIRTCLow;
        private ObservableCollection<CalibrationDataGrid> _calibrationListPressure;
        private ObservableCollection<CalibrationDataGrid> _calibrationListPressureSupport;
        private ObservableCollection<CalibrationDataGrid> _calibrationListVACMonitor;
        private ObservableCollection<CalibrationDataGrid> _calibrationListAIRTCHigh;
        private ObservableCollection<CalibrationDataGrid> _calibrationListAIRTCMediumHigh;
        private ObservableCollection<CalibrationDataGrid> _calibrationListVACHeaderRight;
        private ObservableCollection<CalibrationDataGrid> _calibrationListVACHeaderLeft;
        private ObservableCollection<CalibrationDataGrid> _calibrationListAIRTCMediumLeft;
        private ObservableCollection<CalibrationDataGrid> _calibrationListVACHeaderMonitor;
        private ObservableCollection<CalibrationDataGrid> _calibrationListAIRTCMediumLow;
        private CalibrationFormInput _calibrationFormInput;
        private short _sequenceOfSensorMinPTC;
        private short _sequenceOfSensorMaxPTC;
        private short _defaultSequenceOfSensorMinMON;
        private short _defaultSequenceOfSensorMaxMON;
        private short _sequenceOfSensorRangeStartSelectionPTC;
        private short _sequenceOfSensorRangeEndSelectionPTC;
        private short _sequenceOfSensorRangeStartSelectionMON;
        private short _sequenceOfSensorRangeEndSelectionMON;
        public bool InitSequenceOfSensorPTC;
        public bool InitSequenceOfSensorMON;
        public bool IsCycleRunnning;


        public short DefaultSequenceOfSensorMinPTC { get => _sequenceOfSensorMinPTC; set => OnPropertyChanged(ref _sequenceOfSensorMinPTC, value); }
        public short DefaultSequenceOfSensorMaxPTC { get => _sequenceOfSensorMaxPTC; set => OnPropertyChanged(ref _sequenceOfSensorMaxPTC, value); }
        public short DefaultSequenceOfSensorMinMON { get => _defaultSequenceOfSensorMinMON; set => OnPropertyChanged(ref _defaultSequenceOfSensorMinMON, value); }
        public short DefaultSequenceOfSensorMaxMON { get => _defaultSequenceOfSensorMaxMON; set => OnPropertyChanged(ref _defaultSequenceOfSensorMaxMON, value); }
        public short SequenceOfSensorRangeStartSelectionPTC { get => _sequenceOfSensorRangeStartSelectionPTC; set => OnPropertyChanged(ref _sequenceOfSensorRangeStartSelectionPTC, value); }
        public short SequenceOfSensorRangeEndSelectionPTC { get => _sequenceOfSensorRangeEndSelectionPTC; set => OnPropertyChanged(ref _sequenceOfSensorRangeEndSelectionPTC, value); }
        public short SequenceOfSensorRangeStartSelectionMON { get => _sequenceOfSensorRangeStartSelectionMON; set => OnPropertyChanged(ref _sequenceOfSensorRangeStartSelectionMON, value); }
        public short SequenceOfSensorRangeEndSelectionMON { get => _sequenceOfSensorRangeEndSelectionMON; set => OnPropertyChanged(ref _sequenceOfSensorRangeEndSelectionMON, value); }
        public CalibrationFormInput CalibrationFormInput { get => _calibrationFormInput; set => OnPropertyChanged(ref _calibrationFormInput, value); }


        public ObservableCollection<CalibrationDataGrid> CalibrationListPTC { get => _calibrationListPTC; set => OnPropertyChanged(ref _calibrationListPTC, value); }
        public ObservableCollection<CalibrationDataGrid> CalibrationListMON { get => _calibrationListMON; set => OnPropertyChanged(ref _calibrationListMON, value); }
        public ObservableCollection<CalibrationDataGrid> CalibrationListMONCalibration { get => _calibrationListMONCalibration; set => OnPropertyChanged(ref _calibrationListMONCalibration, value); }
        public ObservableCollection<CalibrationDataGrid> CalibrationListVACHeader { get => _calibrationListVACHeader; set => OnPropertyChanged(ref _calibrationListVACHeader, value); }
        public ObservableCollection<CalibrationDataGrid> CalibrationListAIRTCLow { get => _calibrationListAIRTCLow; set => OnPropertyChanged(ref _calibrationListAIRTCLow, value); }
        public ObservableCollection<CalibrationDataGrid> CalibrationListPressure { get => _calibrationListPressure; set => OnPropertyChanged(ref _calibrationListPressure, value); }
        public ObservableCollection<CalibrationDataGrid> CalibrationListPressureSupport { get => _calibrationListPressureSupport; set => OnPropertyChanged(ref _calibrationListPressureSupport, value); }
      
        public ObservableCollection<CalibrationDataGrid> CalibrationListVACMonitor { get => _calibrationListVACMonitor; set => OnPropertyChanged(ref _calibrationListVACMonitor, value); }
        public ObservableCollection<CalibrationDataGrid> CalibrationListAIRTCHigh { get => _calibrationListAIRTCHigh; set => OnPropertyChanged(ref _calibrationListAIRTCHigh, value); }
        public ObservableCollection<CalibrationDataGrid> CalibrationListAIRTCMediumHigh { get => _calibrationListAIRTCMediumHigh; set => OnPropertyChanged(ref _calibrationListAIRTCMediumHigh, value); }
        public ObservableCollection<CalibrationDataGrid> CalibrationListVACHeaderRight { get => _calibrationListVACHeaderRight; set => OnPropertyChanged(ref _calibrationListVACHeaderRight, value); }
        public ObservableCollection<CalibrationDataGrid> CalibrationListVACHeaderLeft { get => _calibrationListVACHeaderLeft; set => OnPropertyChanged(ref _calibrationListVACHeaderLeft, value); }
        public ObservableCollection<CalibrationDataGrid> CalibrationListAIRTCMediumLeft { get => _calibrationListAIRTCMediumLeft; set => OnPropertyChanged(ref _calibrationListAIRTCMediumLeft, value); }
        public ObservableCollection<CalibrationDataGrid> CalibrationListVACHeaderMonitor { get => _calibrationListVACHeaderMonitor; set => OnPropertyChanged(ref _calibrationListVACHeaderMonitor, value); }
        public ObservableCollection<CalibrationDataGrid> CalibrationListAIRTCMediumLow { get => _calibrationListAIRTCMediumLow; set => OnPropertyChanged(ref _calibrationListAIRTCMediumLow, value); }
       
        public CalibrationBase(ApplicationConfiguration applicationConfiguration)
        {
            _applicationConfiguration = applicationConfiguration;
            _connectionString = _applicationConfiguration.PostgreSqlConnectionString;
            _mainCacheManager = new CacheManager(CacheDBType.Main, _applicationConfiguration.RedisServer);
        }
        public CalibrationSettingsModel CalibrationSettingsSetter
        {
            get
            {
                CalibrationSettingsModel calibrationSettings;

                ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(_connectionString);
                ApplicationProperty applicationProperty = applicationPropertyService.GetByName("CalibrationSettings");

                if (!string.IsNullOrEmpty(applicationProperty?.Value))
                {
                    calibrationSettings = JsonConvert.DeserializeObject<CalibrationSettingsModel>(applicationProperty.Value);
                }
                else
                {
                    calibrationSettings = new CalibrationSettingsModel();
                }

                return calibrationSettings;
            }
            set
            {
                string calibrationSettingsString = JsonConvert.SerializeObject(value);
                ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(_connectionString);
                applicationPropertyService.UpdateByName("CalibrationSettings", calibrationSettingsString);
            }
        }
        public KeyValuePair<CalibrationSensorType, string>[] SensorTypeList { get; protected set; }
        protected Action UpdateCalibrationGridListAction { get; set; }
        public virtual void InitializePageTagConfigurations()
        {
            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(_applicationConfiguration.PostgreSqlConnectionString);
            var pageTagConfiguration = pageTagConfigurationService.GetByName("Calibration");
            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
            CalibrationTagConfigurations = JsonConvert.DeserializeObject<CalibrationTagConfigurations>(jsonSerializedString);

            SetCalibrationDatablock(true);

            foreach (var item in CalibrationTagConfigurations.CalibrationItemsPTC)
            {
                CalibrationTagConfigurations.CalibrationItemsPTC[item.Key].SensorNo = Convert.ToInt16(item.Key.Remove(0, 3));
                CalibrationTagConfigurations.CalibrationItemsPTC[item.Key].SensorValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(item.Value.SensorValue)];
                CalibrationTagConfigurations.CalibrationItemsPTC[item.Key].SensorRawValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(item.Value.SensorRawValue)];
                CalibrationTagConfigurations.CalibrationItemsPTC[item.Key].OldGain = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(item.Value.OldGain)];
                CalibrationTagConfigurations.CalibrationItemsPTC[item.Key].OldCallOffset = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(item.Value.OldCallOffset)];
                CalibrationTagConfigurations.CalibrationItemsPTC[item.Key].NewGain = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(item.Value.NewGain)];
                CalibrationTagConfigurations.CalibrationItemsPTC[item.Key].NewCallOffset = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(item.Value.NewCallOffset)];
            }

            foreach (var item in CalibrationTagConfigurations.CalibrationItemsMON)
            {
                CalibrationTagConfigurations.CalibrationItemsMON[item.Key].SensorNo = Convert.ToInt16(item.Key.Remove(0, 3));
                CalibrationTagConfigurations.CalibrationItemsMON[item.Key].SensorValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(item.Value.SensorValue)];
                CalibrationTagConfigurations.CalibrationItemsMON[item.Key].SensorRawValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(item.Value.SensorRawValue)];
                CalibrationTagConfigurations.CalibrationItemsMON[item.Key].OldGain = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(item.Value.OldGain)];
                CalibrationTagConfigurations.CalibrationItemsMON[item.Key].OldCallOffset = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(item.Value.OldCallOffset)];
                CalibrationTagConfigurations.CalibrationItemsMON[item.Key].NewGain = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(item.Value.NewGain)];
                CalibrationTagConfigurations.CalibrationItemsMON[item.Key].NewCallOffset = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(item.Value.NewCallOffset)];
            }

            foreach (var item in CalibrationTagConfigurations.CalibrationItems)
            {
                CalibrationTagConfigurations.CalibrationItems[item.Key].SensorNo = 0;
                CalibrationTagConfigurations.CalibrationItems[item.Key].SensorValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(item.Value.SensorValue)];
                CalibrationTagConfigurations.CalibrationItems[item.Key].SensorRawValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(item.Value.SensorRawValue)];
                CalibrationTagConfigurations.CalibrationItems[item.Key].OldGain = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(item.Value.OldGain)];
                CalibrationTagConfigurations.CalibrationItems[item.Key].OldCallOffset = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(item.Value.OldCallOffset)];
                CalibrationTagConfigurations.CalibrationItems[item.Key].NewGain = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(item.Value.NewGain)];
                CalibrationTagConfigurations.CalibrationItems[item.Key].NewCallOffset = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(item.Value.NewCallOffset)];
            }

            CalibrationTagConfigurations.CommandSetCalculateAcceptReset = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(CalibrationTagConfigurations.CommandSetCalculateAcceptReset)];
            CalibrationTagConfigurations.SensorType = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(CalibrationTagConfigurations.SensorType)];
            CalibrationTagConfigurations.SetSourcetoLowSensorValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(CalibrationTagConfigurations.SetSourcetoLowSensorValue)];
            CalibrationTagConfigurations.SetSourcetoHighSensorValue = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(CalibrationTagConfigurations.SetSourcetoHighSensorValue)];
            CalibrationTagConfigurations.SetSequenceFirst = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(CalibrationTagConfigurations.SetSequenceFirst)];
            CalibrationTagConfigurations.SetSequenceLast = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(CalibrationTagConfigurations.SetSequenceLast)];

            DefaultSequenceOfSensorMinPTC = 1;
            DefaultSequenceOfSensorMaxPTC = SensorCount(CalibrationSensorType.PTC);
            DefaultSequenceOfSensorMinMON = 1;
            DefaultSequenceOfSensorMaxMON = SensorCount(CalibrationSensorType.MON);
        }

        public async Task<bool> SetCalibrationDatablock(bool value)
        {
            bool result = false;

            await Task.Run(() =>
            {
                if (CalibrationTagConfigurations.DbNumbers != null)
                {
                    for (int i = 0; i < CalibrationTagConfigurations.DbNumbers.Count; i++)
                    {
                        result = ProcessManager.Instance.ChangeDemandReadStateOnCache(PlcDeviceId, CalibrationTagConfigurations.DbNumbers[i], value);

                    }
                }
            });
            await Task.Delay(3000);

            return result = false;
        }



        public short SensorCount(CalibrationSensorType calibrationSensorType)
        {
            short sensorCount = default;

            switch (calibrationSensorType)
            {
                case CalibrationSensorType.PTC:
                    sensorCount = Convert.ToInt16(ProcessManager.Instance.ApplicationProperties["PTCCount"].Value);
                    break;
                case CalibrationSensorType.MON:
                    sensorCount = Convert.ToInt16(ProcessManager.Instance.ApplicationProperties["MONCount"].Value);
                    break;
                case CalibrationSensorType.MONCalibration:
                case CalibrationSensorType.VACHeaderMonitor:
                case CalibrationSensorType.AIRTCHigh:
                case CalibrationSensorType.AIRTCMediumHigh:
                case CalibrationSensorType.AIRTCLow:
                case CalibrationSensorType.Pressure:
                case CalibrationSensorType.PressureSupport:
                case CalibrationSensorType.VacuumHeaderRight:
                case CalibrationSensorType.VacuumHeaderLeft:
                case CalibrationSensorType.AIRTCMediumLow:
                    sensorCount = 1;
                    break;
            }
            return sensorCount;
        }
        //public Dictionary<CalibrationSensorType, Dictionary<string, CalibrationSelectedValueInterval>> CalibrationSelectedValueIntervalBySensorTypes
        //{
        //    get
        //    {
        //        var intervalSerialied = _mainCacheManager.GetString($"CalibrationSelectedValueIntervalsPLC{_applicationConfiguration.PlcDevice.Id}");

        //        if (intervalSerialied != null)
        //        {
        //            _calibrationSelectedValueIntervalBySensorTypes = JsonConvert.DeserializeObject<Dictionary<CalibrationSensorType, Dictionary<string, CalibrationSelectedValueInterval>>>(intervalSerialied);
        //        }
        //        else
        //        {
        //            _calibrationSelectedValueIntervalBySensorTypes = new Dictionary<CalibrationSensorType, Dictionary<string, CalibrationSelectedValueInterval>>();
        //        }
        //        return _calibrationSelectedValueIntervalBySensorTypes;
        //    }
        //    set
        //    {
        //        _calibrationSelectedValueIntervalBySensorTypes = value;
        //        var intervalSerialied = JsonConvert.SerializeObject(_calibrationSelectedValueIntervalBySensorTypes);
        //        _mainCacheManager.Set($"CalibrationSelectedValueIntervalsPLC{_applicationConfiguration.PlcDevice.Id}", intervalSerialied);
        //    }
        //}
        public async void SetSensorType()
        {
            await Task.Run(() =>
            {
                PlcCommandManager plcCommandManager = new PlcCommandManager(_applicationConfiguration.RedisServer);
                Guid guid = Guid.NewGuid();
                plcCommandManager.Set((SiemensTagConfiguration)CalibrationTagConfigurations.SensorType, (int)CalibrationFormInput.SelectedCalibrationSensorType, guid);
                bool resultValue = plcCommandManager.IsUpdatedResult(guid, false, 1000);
            });

        }
        public virtual async Task SetSequenceOfSensorRangeSelection()
        {
            await Task.Run(() =>
            {
                PlcCommandManager plcCommandManager = new PlcCommandManager(_applicationConfiguration.RedisServer);
                Guid guidValueSetFirst = Guid.NewGuid();
                Guid guidValueSetLast = Guid.NewGuid();

                plcCommandManager.Set((SiemensTagConfiguration)CalibrationTagConfigurations.SetSequenceFirst, CalibrationFormInput.SequenceOfSensorRangeStartSelection, guidValueSetFirst);
                plcCommandManager.Set((SiemensTagConfiguration)CalibrationTagConfigurations.SetSequenceLast, CalibrationFormInput.SequenceOfSensorRangeEndSelection, guidValueSetLast);

                bool resultValueFirst = plcCommandManager.IsUpdatedResult(guidValueSetFirst, false, 1000);
                bool resultValueLast = plcCommandManager.IsUpdatedResult(guidValueSetLast, false, 1000);
            });
        }
        public virtual async Task SetSourcetoLowSensorRange()
        {
            PlcCommandManager plcCommandManager = new PlcCommandManager(_applicationConfiguration.RedisServer);
            Guid guidValueSet = Guid.NewGuid();
            Guid guidValueSetCommand = Guid.NewGuid();

            plcCommandManager.Set((SiemensTagConfiguration)CalibrationTagConfigurations.SetSourcetoLowSensorValue, CalibrationFormInput.SetSourceToLowSensorRangeValue, guidValueSet);
            plcCommandManager.Set((SiemensTagConfiguration)CalibrationTagConfigurations.CommandSetCalculateAcceptReset, 10, guidValueSetCommand);

            bool resultValueSet = await plcCommandManager.IsUpdatedResultAsync(guidValueSet, true, 100000);
            bool resultSetCommand = await plcCommandManager.IsUpdatedResultAsync(guidValueSetCommand, true, 100000);

            float value = CalibrationFormInput.SetSourceToLowSensorRangeValue;

            if (resultValueSet && resultSetCommand)
            {
                TextBoxNewValue = CalibrationFormInput.SetSourceToLowSensorRangeValue;
            }
            else
            {
                value = plcCommandManager.Get<float>((SiemensTagConfiguration)CalibrationTagConfigurations.SetSourcetoLowSensorValue, getAlwaysUpdatedResult: true);
            }
        }
        public virtual async Task SetSourcetoHighSensorRange()
        {
            PlcCommandManager plcCommandManager = new PlcCommandManager(_applicationConfiguration.RedisServer);

            Guid guidValueSet = Guid.NewGuid();
            Guid guidValueSetCommand = Guid.NewGuid();

            plcCommandManager.Set((SiemensTagConfiguration)CalibrationTagConfigurations.SetSourcetoHighSensorValue, CalibrationFormInput.SetSourceToHighSensorRangeValue, guidValueSet);
            plcCommandManager.Set((SiemensTagConfiguration)CalibrationTagConfigurations.CommandSetCalculateAcceptReset, 20, guidValueSetCommand);

            bool resultValueSet = await plcCommandManager.IsUpdatedResultAsync(guidValueSet, true, 50);
            bool resultSetCommand = await plcCommandManager.IsUpdatedResultAsync(guidValueSetCommand, true, 50);

            float value = CalibrationFormInput.SetSourceToHighSensorRangeValue;

            if (resultValueSet && resultSetCommand)
            {
                TextBoxNewValue = CalibrationFormInput.SetSourceToHighSensorRangeValue;
            }
            else
            {
                value = plcCommandManager.Get<float>((SiemensTagConfiguration)CalibrationTagConfigurations.SetSourcetoHighSensorValue, getAlwaysUpdatedResult: true);
            }

            TextBoxNewValue = value;
        }
        public virtual void Calculate()
        {
            PlcCommandManager plcCommandManager = new PlcCommandManager(_applicationConfiguration.RedisServer);
            Guid guid = Guid.NewGuid();
            plcCommandManager.Set<float>((SiemensTagConfiguration)CalibrationTagConfigurations.CommandSetCalculateAcceptReset, 30, guid);
            bool result = plcCommandManager.IsUpdatedResult(guid, false, 1000);
            UpdateCalibrationGridListAction.Invoke();
        }
        public virtual void Refresh()
        {
            UpdateCalibrationGridListAction.Invoke();
        }
        public virtual void Accept()
        {
            PlcCommandManager plcCommandManager = new PlcCommandManager(_applicationConfiguration.RedisServer);
            Guid guid = Guid.NewGuid();
            plcCommandManager.Set<float>((SiemensTagConfiguration)CalibrationTagConfigurations.CommandSetCalculateAcceptReset, 40, guid);
            bool result = plcCommandManager.IsUpdatedResult(guid, false, 1000);
        }
        public virtual void Reset(object parameter)
        {
            //todo:h view binding command parameters
            float commandSetCalculateAcceptResetParameter = parameter.ToString() == "ResetAccept" ? 50 : (parameter.ToString() == "ResetCalculate" ? 50 : 0);
            PlcCommandManager plcCommandManager = new PlcCommandManager(_applicationConfiguration.RedisServer);
            Guid guid = Guid.NewGuid();
            plcCommandManager.Set<float>((SiemensTagConfiguration)CalibrationTagConfigurations.CommandSetCalculateAcceptReset, 50, guid);
            bool result = plcCommandManager.IsUpdatedResult(guid, false, 1000);
            _mainCacheManager.DeleteManyKeys($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid*");
            _mainCacheManager.DeleteManyKeys($"CalibrationSensorValuesTableSensorType::PLC:{ProcessManager.Instance.PlcDeviceId}:*");


        }
        protected void OpenCalibrationCertificationWindow()
        {
            try
            {
                Dictionary<CalibrationSensorType, short> sensorCounts = new Dictionary<CalibrationSensorType, short>();
                foreach (var sensorTypeItem in SensorTypeList)
                {
                    sensorCounts.Add(sensorTypeItem.Key, SensorCount(sensorTypeItem.Key));
                }

               
                bool isWindowOpened= WindowsExtensions.IsWindowsOpen<CalibrationCertificationWindow>();
                if (!isWindowOpened)
                {
                    CalibrationCertificationWindow calibrationCertificationWindow = new CalibrationCertificationWindow(sensorCounts, SensorTypeList.ToList(), CalibrationTagConfigurations);
                    calibrationCertificationWindow.Owner = Application.Current.MainWindow;
                    calibrationCertificationWindow.Show();
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log($"There are any calculated set parameters! Detail:{ex.Message}", LogType.Warning);
            }
        }

      
    }
}
