using System;
using System.Linq;
using RevoScada.DesktopApplication.Models;
using System.Collections.Generic;
using RevoScada.DesktopApplication.Models.ModelEnums;
using RevoScada.Entities.PageTagConfigurations;
using RevoScada.Configurator;
using RevoScada.ProcessController;
using System.Globalization;
using System.Data;
using RevoScada.DesktopApplication.Models.ModelTypes;
using System.Threading;
using Revo.Core;
using RevoScada.Entities.Configuration;
using Revo.Core.Data;

namespace RevoScada.DesktopApplication.ViewModels.CalibrationViewModels
{
    public class CalibrationCertificationVM : ObservableObject
    {
        private CalibrationCertificationAdapter _calibrationCertificationAdapter;
        private CalibrationCertificationParameters _calibrationCertificationParameters;
        public DataTable CalibrationSensorValueIntervalDataTable;
        public CalibrationTagConfigurations CalibrationTagConfigurations;
        public bool IsRawValueCycleActivated = false;
        public bool IsUpdateCheckValueCycleActivated = false;
        private CalibrationCertificationPageControls _calibrationCertificationPageControls;
        private readonly PlcCommandManager _plcCommandManager;
        private List<decimal> _checkValueSets;
        private int _totalSetCount;
        private decimal Tolerance;
        private int StabilityCount;
        private Dictionary<string, CalibrationStabilityCheckValue> _stabilityCheckValues;
        
        public CalibrationCertificationParameters CalibrationCertificationParameters
        {
            get => _calibrationCertificationParameters;
            set => OnPropertyChanged(ref _calibrationCertificationParameters, value);
        }
        public CalibrationCertificationPageControls CalibrationCertificationPageControls { get => _calibrationCertificationPageControls; set => OnPropertyChanged(ref _calibrationCertificationPageControls, value); }
        public CalibrationCertificationVM()
        {
            _calibrationCertificationAdapter = new CalibrationCertificationAdapter(ApplicationConfigurations.Instance.Configuration.RedisServer, ApplicationConfigurations.Instance.Configuration.PlcDevice.Id);
            CalibrationSensorValueIntervalDataTable = new DataTable();
            CalibrationCertificationParameters = _calibrationCertificationAdapter.CachedCalibrationCertificationParameters;
            _plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
            CalibrationCertificationPageControls = new CalibrationCertificationPageControls();
        }
        internal void FirstTimeInitialization(Dictionary<CalibrationSensorType, short> sensorCounts, List<KeyValuePair<CalibrationSensorType, string>> sensorsWithName)
        {
            if (CalibrationCertificationParameters == null)
            {
                Dictionary<CalibrationSensorType, string> SensorUnitSymbols = new Dictionary<CalibrationSensorType, string>();
                foreach (var item in sensorsWithName)
                {
                    switch (item.Key)
                    {
                        case CalibrationSensorType.PTC:
                            SensorUnitSymbols[CalibrationSensorType.PTC] = "°C";
                            break;
                        case CalibrationSensorType.MON:
                            SensorUnitSymbols[CalibrationSensorType.MON] = "mmHg";
                            break;
                        case CalibrationSensorType.MONCalibration:
                            SensorUnitSymbols[CalibrationSensorType.MONCalibration] = "mmHg";
                            break;
                        case CalibrationSensorType.VACHeaderMonitor:
                            SensorUnitSymbols[CalibrationSensorType.VACHeaderMonitor] = "mmHg";
                            break;
                        case CalibrationSensorType.AIRTCHigh:
                            SensorUnitSymbols[CalibrationSensorType.AIRTCHigh] = "°C";
                            break;
                        case CalibrationSensorType.AIRTCMediumHigh:
                            SensorUnitSymbols[CalibrationSensorType.AIRTCMediumHigh] = "°C";
                            break;
                        case CalibrationSensorType.AIRTCLow:
                            SensorUnitSymbols[CalibrationSensorType.AIRTCLow] = "°C";
                            break;
                        case CalibrationSensorType.Pressure:
                            SensorUnitSymbols[CalibrationSensorType.Pressure] = "Bar";
                            break;
                        case CalibrationSensorType.PressureSupport:
                            SensorUnitSymbols[CalibrationSensorType.PressureSupport] = "Bar";
                            break;
                        case CalibrationSensorType.VacuumHeaderRight:
                            SensorUnitSymbols[CalibrationSensorType.VacuumHeaderRight] = "mmHg";
                            break;
                        case CalibrationSensorType.VacuumHeaderLeft:
                            SensorUnitSymbols[CalibrationSensorType.VacuumHeaderLeft] = "mmHg";
                            break;
                        case CalibrationSensorType.AIRTCMediumLow:
                            SensorUnitSymbols[CalibrationSensorType.AIRTCMediumLow] = "mmHg";
                            break;
                    }
                }

                CalibrationCertificationParameters = new CalibrationCertificationParameters
                {
                    SensorCounts = sensorCounts,

                    SelectedCalibrationSensorType = CalibrationSensorType.PTC,
                    SensorsWithName = sensorsWithName,
                    SensorUnitSymbols = SensorUnitSymbols,
                    CalibrationCertificationSettingsForAllSensorTypes = new Dictionary<CalibrationSensorType, CalibrationCertificationCheckSettings>()
                };

                foreach (var item in sensorCounts)
                {
                    CalibrationCertificationCheckSettings calibrationCertificationCheckSettings = new CalibrationCertificationCheckSettings
                    {
                        CheckValueList = new List<object> { 0m },
                        DefaultSequenceOfSensorMin = 1,
                        DefaultSequenceOfSensorMax = item.Value,
                        Tolerance = 0,
                        StabilityCount = 3,
                    };

                    CalibrationCertificationParameters.CalibrationCertificationSettingsForAllSensorTypes[item.Key] = calibrationCertificationCheckSettings;
                    _calibrationCertificationAdapter.CachedCalibrationCertificationParameters = CalibrationCertificationParameters;
                }
                
                ChangeSelectedCalibrationCertificationCheckSettings(CalibrationSensorType.PTC);
                CalibrationCertificationParameters.IsParametersSaved = false;
                CalibrationCertificationPageControls.IsStartButtonEnabled = false;
                CalibrationCertificationPageControls.IsStopButtonEnabled = false;
                CalibrationCertificationPageControls.IsStartButtonEnabled = false;
            }
            else
            {
                bool statusCheck = CalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.CalibrationCertificationCheckStatus == CalibrationCertificationCheckStatus.Reset;

                if (_calibrationCertificationAdapter.CheckTableAvaible())
                {
                    CalibrationCertificationPageControls.IsStartButtonEnabled = true;
                    CalibrationCertificationPageControls.IsResetAllByTypeEnabled = true;
                    CalibrationCertificationPageControls.IsResetAllEnabled = true;
                }
                else
                {
                    CalibrationCertificationPageControls.IsStartButtonEnabled = false;
                }

                ChangeSelectedCalibrationCertificationCheckSettings(CalibrationCertificationParameters.SelectedCalibrationSensorType);
            }
            CalibrationCertificationPageControls.IsGridResetSelectedVisible = true;
        }
        internal void StartChecking()
        {
            CreateEmptyTable();
            CalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.CalibrationCertificationCheckStatus = CalibrationCertificationCheckStatus.Running;
            IsUpdateCheckValueCycleActivated = true;
            CalibrationCertificationPageControls.IsSettingParametersLayoutEnabled = false;
            CalibrationCertificationPageControls.IsStartButtonEnabled = false;
            CalibrationCertificationPageControls.IsStopButtonEnabled = true;
            CalibrationCertificationPageControls.IsResetAllByTypeEnabled = false;
            CalibrationCertificationPageControls.IsResetAllEnabled = false;
            CalibrationCertificationPageControls.IsGridResetSelectedVisible = false;
        }
        internal void StopChecking()
        {
            CalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.CalibrationCertificationCheckStatus = CalibrationCertificationCheckStatus.Paused;
            IsUpdateCheckValueCycleActivated = false;
            CalibrationCertificationPageControls.IsSettingParametersLayoutEnabled = true;
            CalibrationCertificationPageControls.IsStartButtonEnabled = true;
            CalibrationCertificationPageControls.IsStopButtonEnabled = false;
            CalibrationCertificationPageControls.IsResetAllByTypeEnabled = true;
            CalibrationCertificationPageControls.IsResetAllEnabled = true;
            CalibrationCertificationPageControls.IsGridResetSelectedVisible = true;
        }
        internal void SaveSelectedCalibrationCertificationCheckSettings()
        {
            NumberStyles style;
            CultureInfo provider;
            if (CalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.CheckValueList != null && CalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.CheckValueList.Count > 0)
            {
                for (int i = 0; i < CalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.CheckValueList.Count(); i++)
                {
                    var value = CalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.CheckValueList[i].ToString();
                    decimal result;
                    style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
                    provider = new CultureInfo("en-US");

                    try
                    {
                        result = Decimal.Parse(value, style, provider);
                        CalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.CheckValueList[i] = result;
                    }
                    catch (FormatException)
                    {
                        CalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.CheckValueList.RemoveAt(i);
                    }
                }
            }
            CalibrationCertificationParameters.IsParametersSaved = true;
            CalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.IsSettingsSaved = true;
            CalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.IsSaveEnabled = false;
            CalibrationCertificationPageControls.IsStartButtonEnabled = true;
            CalibrationCertificationPageControls.IsStopButtonEnabled = false;

            CreateEmptyTable();
            FillSensorValues();

            _calibrationCertificationAdapter.CachedCalibrationCertificationParameters = CalibrationCertificationParameters;


        }
        internal void ChangeSelectedCalibrationCertificationCheckSettings(CalibrationSensorType calibrationSensorType)
        {
            CalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings = CalibrationCertificationParameters.CalibrationCertificationSettingsForAllSensorTypes[calibrationSensorType];
            _calibrationCertificationAdapter.CachedCalibrationCertificationParameters = CalibrationCertificationParameters;
            CalibrationCertificationPageControls.IsSettingParametersLayoutBeforeResetEnabled = CalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.CalibrationCertificationCheckStatus == CalibrationCertificationCheckStatus.Reset;
        }
        internal void CreateEmptyTable()
        {
            if (CalibrationCertificationParameters?.SelectedCalibrationCertificationCheckSettings?.CheckValueList != null && CalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.CheckValueList.Count > 0)
            {
                _calibrationCertificationAdapter.CachedCalibrationCertificationParameters = CalibrationCertificationParameters;
                if (CalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.CalibrationCertificationCheckStatus == CalibrationCertificationCheckStatus.Reset)
                {
                    _calibrationCertificationAdapter.DeleteTable();
                }
                _calibrationCertificationAdapter.CreateTable();
            }
        }
        internal void FillSensorValues()
        {
            try
            {
                if (CalibrationCertificationParameters?.SelectedCalibrationCertificationCheckSettings?.CheckValueList != null && CalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.CheckValueList.Count > 0)
                {
                    CalibrationSensorValueIntervalDataTable = _calibrationCertificationAdapter.SavedSensorValuesTable();
                }
            }
            catch (Exception)
            {

            }
        }
        internal void UpdateCalibrationSensorRawValues()
        {
            for (int sensorNo = 1; sensorNo <= CalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.DefaultSequenceOfSensorMax; sensorNo++)
            {
                try
                {
                    decimal newValue = SensorValue(CalibrationCertificationParameters.SelectedCalibrationSensorType, sensorNo, true);
                    GridUpdater(sensorNo - 1, 1, newValue.ToString());
                }
                catch (Exception ex)
                {
                }
              
            }
        }
        internal void InitializeBeforeCheckStart()
        {
            IsUpdateCheckValueCycleActivated = true;
            CalibrationCertificationPageControls.IsSettingParametersLayoutBeforeResetEnabled = false;
            _checkValueSets = CalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.CheckValueListAsDecimal;
            _totalSetCount = _checkValueSets.Count();
            Tolerance = CalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.Tolerance;
            StabilityCount = CalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.StabilityCount;
            _stabilityCheckValues = _calibrationCertificationAdapter.StabilityCheckValues;

        }
        internal void UpdateCalibrationSensorValues()
        {

            for (int sensorNo = CalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.DefaultSequenceOfSensorMin; sensorNo <= CalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.DefaultSequenceOfSensorMax; sensorNo++)
            {
                int valueNearToIndex = 0;
                decimal newValue = Convert.ToDecimal(CalibrationSensorValueIntervalDataTable.Rows[sensorNo - 1][1] != DBNull.Value ? CalibrationSensorValueIntervalDataTable.Rows[sensorNo - 1][1] : 0m);

                do
                {
                    if (_checkValueSets[valueNearToIndex] + CalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.Tolerance >= newValue && _checkValueSets[valueNearToIndex] - Tolerance <= newValue)
                    {
                        var stabilityCheckValue = new CalibrationStabilityCheckValue { MappedDataRowIndex = sensorNo - 1, MappedDataColumnIndex = valueNearToIndex + 2, MappedDataErrorColumnIndex = (valueNearToIndex + 2) + _totalSetCount, CalibrationSensorType = CalibrationCertificationParameters.SelectedCalibrationSensorType };

                        if (_stabilityCheckValues.ContainsKey(stabilityCheckValue.MapKey))
                        {
                            stabilityCheckValue = _stabilityCheckValues[stabilityCheckValue.MapKey];
                        }
                        else
                        {
                            stabilityCheckValue.SensorValue = newValue;
                        }

                        if (stabilityCheckValue.IsStable)
                        {
                            break;
                        }

                        if (stabilityCheckValue.SensorValue == newValue)
                        {
                            stabilityCheckValue.OccurenceCount = ++stabilityCheckValue.OccurenceCount;
                            GridUpdater(stabilityCheckValue.MappedDataRowIndex, stabilityCheckValue.MappedDataColumnIndex, newValue + $" ({stabilityCheckValue.OccurenceCount})?");
                        }
                        else
                        {
                            GridUpdater(stabilityCheckValue.MappedDataRowIndex, stabilityCheckValue.MappedDataColumnIndex, "Fail");
                            stabilityCheckValue.OccurenceCount = 0;
                            stabilityCheckValue.SensorValue = newValue;
                        }

                        if (stabilityCheckValue.OccurenceCount >= StabilityCount)
                        {
                            stabilityCheckValue.IsStable = true;
                            GridUpdater(stabilityCheckValue.MappedDataRowIndex, stabilityCheckValue.MappedDataColumnIndex, newValue);
                            GridUpdater(stabilityCheckValue.MappedDataRowIndex, stabilityCheckValue.MappedDataErrorColumnIndex, Math.Abs(newValue - _checkValueSets[valueNearToIndex]));
                            _calibrationCertificationAdapter.UpdateTable(CalibrationSensorValueIntervalDataTable);
                        }
                        _stabilityCheckValues[stabilityCheckValue.MapKey] = stabilityCheckValue;
                        break;
                    }
                    valueNearToIndex++;
                } while (valueNearToIndex < _checkValueSets.Count());
            }

            lock (this)
            {
                _calibrationCertificationAdapter.StabilityCheckValues = _stabilityCheckValues;
            }
        }
        internal void ResetSuccessUpdateCalibrationSensorValues()
        {
            foreach (var item in _stabilityCheckValues.Values.Where(x => x.MapKey.StartsWith(CalibrationCertificationParameters.SelectedCalibrationSensorType.ToString())))
            {

                if (item.IsStable == false)
                {
                    item.OccurenceCount = 0;
                }
            }
            lock (this)
            {
                _calibrationCertificationAdapter.StabilityCheckValues = _stabilityCheckValues;
            }

        }
        private void GridUpdater(int rowIndex, int columnIndex, object value)
        {
            lock (this)
            {
                try
                {
                    if (CalibrationSensorValueIntervalDataTable != null && CalibrationSensorValueIntervalDataTable.Columns.Count > 0 && CalibrationSensorValueIntervalDataTable.Rows[rowIndex][columnIndex] is object)
                    {
                        CalibrationSensorValueIntervalDataTable.Rows[rowIndex][columnIndex] = value;
                        _calibrationCertificationAdapter.UpdateTable(CalibrationSensorValueIntervalDataTable);
                    }
                //    LogManager.Instance.Log($"GridUpdate:[{rowIndex}][{columnIndex}] ", LogType.Error);
                }
                catch (Exception ex)
                {
                    LogManager.Instance.Log($"GridUpdate: {ex.Message}", LogType.Error);
                }
            }
        }
        public void ResetStabilityChecks(Dictionary<int, List<int>> selectedSensorList)
        {
            _stabilityCheckValues = _calibrationCertificationAdapter.StabilityCheckValues;

            foreach (var item in selectedSensorList)
            {
                //todo:l refactor...
                try
                {
                    foreach (int  columnIndex in item.Value)
                    {
                        
                        string key = $"{CalibrationCertificationParameters.SelectedCalibrationSensorType}::[{item.Key}][{columnIndex}]";

                        if (_stabilityCheckValues.ContainsKey(key))
                        {
                            CalibrationStabilityCheckValue checkValue = _stabilityCheckValues[key];
                            checkValue.IsStable = false;
                            checkValue.OccurenceCount = 0;
                            GridUpdater(checkValue.MappedDataRowIndex, checkValue.MappedDataColumnIndex, "Fail");
                            GridUpdater(checkValue.MappedDataRowIndex, checkValue.MappedDataErrorColumnIndex, string.Empty);
                            _stabilityCheckValues[key] = checkValue;
                        }
                       
                    }
                }
                catch (Exception ex)
                {
                    LogManager.Instance.Log($"ResetStabilityChecks: {ex.Message}", LogType.Error);
                }
            }
            _calibrationCertificationAdapter.StabilityCheckValues = _stabilityCheckValues;
        }
        internal void ResetBySelectedType()
        {
            IsRawValueCycleActivated = false;
            _calibrationCertificationAdapter.DeleteTable();
            CalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.CalibrationCertificationCheckStatus = CalibrationCertificationCheckStatus.Reset;
            _calibrationCertificationAdapter.CachedCalibrationCertificationParameters = CalibrationCertificationParameters;
            CalibrationSensorValueIntervalDataTable = null;
            CalibrationCertificationPageControls.IsStartButtonEnabled = false;
            CalibrationCertificationPageControls.IsStopButtonEnabled = false;
            _calibrationCertificationAdapter.ResetStabilityCheckParameters(CalibrationCertificationParameters.SelectedCalibrationSensorType);
            CalibrationCertificationPageControls.IsSettingParametersLayoutBeforeResetEnabled = true;
            Thread.Sleep(100);
            IsRawValueCycleActivated = true;

        }
        internal void ResetAll()
        {
            IsRawValueCycleActivated = false;
            CalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.CalibrationCertificationCheckStatus = CalibrationCertificationCheckStatus.Reset;
            _calibrationCertificationAdapter.DeleteAllTables();
            CalibrationSensorValueIntervalDataTable = null;
            _calibrationCertificationAdapter.CachedCalibrationCertificationParameters = CalibrationCertificationParameters;
            CalibrationCertificationPageControls.IsStartButtonEnabled = false;
            CalibrationCertificationPageControls.IsStopButtonEnabled = false;
            _calibrationCertificationAdapter.ResetStabilityCheckParameters();
            CalibrationCertificationPageControls.IsSettingParametersLayoutBeforeResetEnabled = true;
            Thread.Sleep(100);
            IsRawValueCycleActivated = true;

        }

        private decimal SensorValue(CalibrationSensorType calibrationSensorType, int sensorNo, bool getAlwaysUpdatedDB)
        {
            float value = 0.00f;
            SiemensTagConfiguration siemensTagConfiguration;
            switch (calibrationSensorType)
            {
                case CalibrationSensorType.PTC:
                    siemensTagConfiguration = CalibrationTagConfigurations.CalibrationItemsPTC[$"PTC{sensorNo}"].SensorValue as SiemensTagConfiguration;
                    value = _plcCommandManager.Get<float>(siemensTagConfiguration, getAlwaysUpdatedDB);
                    break;
                case CalibrationSensorType.MON:
                    siemensTagConfiguration = CalibrationTagConfigurations.CalibrationItemsMON[$"MON{sensorNo}"].SensorValue as SiemensTagConfiguration;
                    value = _plcCommandManager.Get<float>(siemensTagConfiguration, getAlwaysUpdatedDB);
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
                    siemensTagConfiguration = CalibrationTagConfigurations.CalibrationItems[calibrationSensorType.ToString()].SensorValue as SiemensTagConfiguration;
                    value = _plcCommandManager.Get<float>(siemensTagConfiguration, getAlwaysUpdatedDB);
                    break;
            }
            
            return NumericManipulation.TruncateDecimalWithExceptionalValues(value, precision: 1);
        }
        //public decimal TruncateDecimal(decimal value, int precision)
        //{
        //    decimal step = (decimal)Math.Pow(10, precision);
        //    decimal tmp = Math.Truncate(step * value);
        //    return tmp / step;
        //}

        //private decimal SensorValue(CalibrationSensorType calibrationSensorType, int sensorNo, bool getAlwaysUpdatedDB)
        //{
        //    decimal d = 0.0m;
        //    int randomBase = DateTime.Now.Second;

        //    if (randomBase > 0 && randomBase < 11)
        //    {
        //        d = (randomBase % 11 == 0 || randomBase % 7 == 0) ? 50.2m : 50.1m;
        //    }
        //    else if (randomBase > 20 && randomBase < 40)
        //    {
        //        d = (randomBase % 11 == 0 || randomBase % 7 == 0) ? 100.2m : 100.1m;
        //    }
        //    else if (randomBase > 40)
        //    {
        //        d = (randomBase % 11 == 0 || randomBase % 7 == 0) ? 150.2m : 150.1m;
        //    }
        //    return d;
        //}

    }
}
 