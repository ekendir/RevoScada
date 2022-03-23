using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Newtonsoft.Json;
using RevoScada.Cache;
using RevoScada.DesktopApplication.Models.ModelEnums;
using RevoScada.DesktopApplication.Models.ModelTypes;

namespace RevoScada.DesktopApplication.ViewModels.CalibrationViewModels
{
    public class CalibrationCertificationAdapter
    {
        private Dictionary<string, CalibrationStabilityCheckValue> _stabilityCheckValues = new Dictionary<string, CalibrationStabilityCheckValue>();
        private CalibrationCertificationParameters _calibrationCertificationParameters = new CalibrationCertificationParameters();
        private readonly CacheManager _mainCacheManager;
        private readonly int _plcDeviceId;
        private string CalibrationSensorValuesTableSensorTypeKey { get { return $"CalibrationSensorValuesTable::PLC:{_plcDeviceId}::SensorType:{CachedCalibrationCertificationParameters.SelectedCalibrationSensorType}"; } }
        public CalibrationCertificationParameters CachedCalibrationCertificationParameters
        {
            get
            {
                var intervalSerialied = _mainCacheManager.GetString($"CalibrationCertificationSelectedParametersPLC{_plcDeviceId}");

                if (intervalSerialied != null && intervalSerialied != "{}")
                {
                    _calibrationCertificationParameters = JsonConvert.DeserializeObject<CalibrationCertificationParameters>(intervalSerialied);
                }
                else
                {
                    _calibrationCertificationParameters = null;
                }
                return _calibrationCertificationParameters;
            }
            set
            {
                _calibrationCertificationParameters = value;
                var intervalSerialied = JsonConvert.SerializeObject(_calibrationCertificationParameters);
                _mainCacheManager.Set($"CalibrationCertificationSelectedParametersPLC{_plcDeviceId}", intervalSerialied);
            }
        }
        public Dictionary<string, CalibrationStabilityCheckValue> StabilityCheckValues
        {
            get
            {
                var intervalSerialied = _mainCacheManager.GetString($"CalibrationCertificationStabilityCheckValuesSelectedParametersPLC{_plcDeviceId}");

                if (intervalSerialied != null && intervalSerialied != "{}")
                {
                    _stabilityCheckValues = JsonConvert.DeserializeObject<Dictionary<string, CalibrationStabilityCheckValue>>(intervalSerialied);
                }
                else
                {
                    _stabilityCheckValues = new Dictionary<string, CalibrationStabilityCheckValue>();
                }
                return _stabilityCheckValues;
            }
            set
            {
                var intervalSerialied = JsonConvert.SerializeObject(value);
                _mainCacheManager.Set($"CalibrationCertificationStabilityCheckValuesSelectedParametersPLC{_plcDeviceId}", intervalSerialied);
            }
        }
        public void ResetStabilityCheckParameters(CalibrationSensorType? calibrationSensorType = null)
        {
            var filteredForDeletion = StabilityCheckValues.Where(x => x.Value.CalibrationSensorType == calibrationSensorType);
            var tempStabilityCheckValues = StabilityCheckValues;
            foreach (var item in filteredForDeletion)
            {
                tempStabilityCheckValues.Remove(item.Key);
            }
            StabilityCheckValues = tempStabilityCheckValues;
        }
        public CalibrationCertificationAdapter(string cacheServer, int plcDeviceId)
        {
            _plcDeviceId = plcDeviceId;
            _mainCacheManager = new CacheManager(CacheDBType.Main, cacheServer);
        }
        public DataTable SavedSensorValuesTable()
        {
            DataTable sensorCheckValues = new DataTable();
            var dataTableSerialized = _mainCacheManager.GetString(CalibrationSensorValuesTableSensorTypeKey);
            
            if (dataTableSerialized != null)
            {
                sensorCheckValues = JsonConvert.DeserializeObject<DataTable>(dataTableSerialized);
            }
            else
            {
                dataTableSerialized = JsonConvert.SerializeObject(sensorCheckValues);
                _mainCacheManager.Set(CalibrationSensorValuesTableSensorTypeKey, dataTableSerialized);
            }
            return sensorCheckValues;
        }
        public void UpdateTable(DataTable dt)
        {
            var dataTableSerialized = JsonConvert.SerializeObject(dt);
            _mainCacheManager.Set(CalibrationSensorValuesTableSensorTypeKey, dataTableSerialized);
        }
        public void DeleteTable()
        {
            _mainCacheManager.DeleteKey(CalibrationSensorValuesTableSensorTypeKey);
        }
        public void DeleteAllTables()
        {
            foreach (var item in CachedCalibrationCertificationParameters.SensorsWithName)
            {
                _mainCacheManager.DeleteKey($"CalibrationSensorValuesTable::PLC:{_plcDeviceId}::SensorType:{item.Key}");
            }
        }
        public void CreateTable()
        {

            DataTable sensorValues = SavedSensorValuesTable();
            DataColumn dataColumn;
            if (sensorValues != null && sensorValues.Columns.Count == 0)
            {
                // first row sensor name column
                dataColumn = new DataColumn();
                dataColumn.ColumnName = $"Sensor";
                dataColumn.DataType = typeof(string);
                sensorValues.Columns.Add(dataColumn);

                dataColumn = new DataColumn();
                dataColumn.ColumnName = $"Values";
                dataColumn.DataType = typeof(string);
                sensorValues.Columns.Add(dataColumn);

                sensorValues.Rows.Clear();

                for (int i = 1; i <= CachedCalibrationCertificationParameters.SensorCounts[CachedCalibrationCertificationParameters.SelectedCalibrationSensorType]; i++)
                {
                    sensorValues.Rows.Add($"{CachedCalibrationCertificationParameters.SelectedCalibrationSensorType} {i}");
                }

                foreach (var item in CachedCalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.CheckValueList)
                {
                    try
                    {
                        dataColumn = new DataColumn();
                        dataColumn.ColumnName = $"{item} {CachedCalibrationCertificationParameters.SensorUnitSymbols[CachedCalibrationCertificationParameters.SelectedCalibrationSensorType]}";
                        dataColumn.Caption = $"{item } {CachedCalibrationCertificationParameters.SensorUnitSymbols[CachedCalibrationCertificationParameters.SelectedCalibrationSensorType]}";
                        dataColumn.DefaultValue = "Fail";
                        sensorValues.Columns.Add(dataColumn);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                foreach (var item in CachedCalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.CheckValueList)
                {
                    try
                    {
                        dataColumn = new DataColumn();
                        dataColumn.ColumnName = $"Errors at {item} {CachedCalibrationCertificationParameters.SensorUnitSymbols[CachedCalibrationCertificationParameters.SelectedCalibrationSensorType]}";
                        dataColumn.Caption = $"{item} {CachedCalibrationCertificationParameters.SensorUnitSymbols[CachedCalibrationCertificationParameters.SelectedCalibrationSensorType]}";
                        sensorValues.Columns.Add(dataColumn);
                    }
                    catch (Exception ex)
                    {
                    }
                }

                dataColumn = new DataColumn();
                dataColumn.ColumnName = $"TOLERANCE (±) {CachedCalibrationCertificationParameters.SensorUnitSymbols[CachedCalibrationCertificationParameters.SelectedCalibrationSensorType]}";
                dataColumn.Caption = $"TOLERANCE (±) {CachedCalibrationCertificationParameters.SensorUnitSymbols[CachedCalibrationCertificationParameters.SelectedCalibrationSensorType]}";
                dataColumn.DefaultValue = CachedCalibrationCertificationParameters.SelectedCalibrationCertificationCheckSettings.Tolerance;
                sensorValues.Columns.Add(dataColumn);

                var dataTableSerialized = JsonConvert.SerializeObject(sensorValues);
                _mainCacheManager.Set(CalibrationSensorValuesTableSensorTypeKey, dataTableSerialized);
            }
        }
        public bool CheckTableAvaible()
        {
            var keyNames = _mainCacheManager.GetKeyNames(CalibrationSensorValuesTableSensorTypeKey);
            return keyNames.Count()== 1;
        }
    }
}
