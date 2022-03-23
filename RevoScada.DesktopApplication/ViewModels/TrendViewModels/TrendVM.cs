using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
using System.Windows;
using Newtonsoft.Json;
using RevoScada.ProcessController;
using RevoScada.Business;
using RevoScada.Business.Report;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.Views;
using RevoScada.Entities;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Enums;
using RevoScada.DesktopApplication.Models.SettingModels;
using RevoScada.DesktopApplication.Views.TrendViews;

namespace RevoScada.DesktopApplication.ViewModels
{
    public class TrendVM : UserControlBaseVM
    {
        #region Services
        private readonly string _connectionString;
        private TrendReportService _trendReportService;
        private ActiveTagService _activeTagService;
        private ApplicationPropertyService _applicationPropertyService;
        #endregion

        #region Collections
        public List<string> LiveTrendPrimaryPorts;
        public List<string> VacPortNames;
        public List<string> TempPortNames;
        public List<string> PressPortNames;
        public List<double> Mins;
        public List<DateTime> DateTimeValues;
        public List<double> AxisXMins;
        public Dictionary<string, List<float>> ValuesByTagNamesSensorValue;
        public Dictionary<string, List<float>> UpdatedValuesByTagNamesSensorValue;
        public Dictionary<string, TrendSelectedPortUIProperty> TrendSelectedPortUIProperties;
        #endregion

        #region Fields
        private Trend_View _trendView;
        private Trend_View_Type_20 _trendViewType20;
        private bool _isItInitiallyLoaded;
        public bool HasBatchFound;
        public TrendChartYAxisParamaters TrendChartYAxisParamaters;
        private DateTime _dataLogInitialDate;
        private DateTime _trendStartDate;
        private int _batchId;
        public double LastMin;
        public double LastAxisLimitVal;
        
        public bool IsMarkerVisible;
        private bool _isPrimaryPortsAdded;
        #endregion

        #region Properties
        public Dictionary<string, TrendSelectedPortUIProperty> TrendSelectedPortUIPropertiesSetter
        {
            get
            {
                Dictionary<string, TrendSelectedPortUIProperty> trendSelectedPortUIProperties = new Dictionary<string, TrendSelectedPortUIProperty>();


                ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(_connectionString);
                ApplicationProperty applicationProperty= applicationPropertyService.GetByName("TrendSelectedPortUIProperties");

                if (!string.IsNullOrEmpty(applicationProperty?.Value))
                {
                    try
                    {
                        trendSelectedPortUIProperties = JsonConvert.DeserializeObject<Dictionary<string, TrendSelectedPortUIProperty>>(applicationProperty.Value);
                    }
                    catch (Exception)
                    {
                        applicationPropertyService.UpdateByName("TrendSelectedPortUIProperties", "");
                    }
                }
                else
                {
                    trendSelectedPortUIProperties = new Dictionary<string, TrendSelectedPortUIProperty>();
                }
                return trendSelectedPortUIProperties;
            }
            set
            {
                string trendSelectedPortUIPropertiesSerialized = JsonConvert.SerializeObject(value);
                ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(_connectionString);
                applicationPropertyService.UpdateByName("TrendSelectedPortUIProperties", trendSelectedPortUIPropertiesSerialized);             
            }
        }

        private Visibility _isBatchInfoSecVisible;
        public Visibility IsBatchInfoSecVisible 
        {
            get => _isBatchInfoSecVisible;
            set => OnPropertyChanged(ref _isBatchInfoSecVisible, value);
        }

        private string _batchLoadNumber;
        public string BatchLoadNumber
        {
            get => _batchLoadNumber;
            set => OnPropertyChanged(ref _batchLoadNumber, value);
        }
        #endregion

        public TrendVM(WaitIndicatorControl waitIndicatorControl, Trend_View trendView = null, Trend_View_Type_20 trendViewType20 = null)
        {
            WaitIndicatorControl = waitIndicatorControl;
            WaitIndicatorControl.IsWaitIndicatorVisible = true;
            _connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;
            _trendReportService = new TrendReportService(_connectionString);
            _activeTagService = new ActiveTagService(_connectionString);
            _applicationPropertyService = new ApplicationPropertyService(_connectionString);
            LiveTrendPrimaryPorts = new List<string>();
            TrendSelectedPortUIProperties = TrendSelectedPortUIPropertiesSetter;
            Mins = new List<double>();
            DateTimeValues = new List<DateTime>();
            AxisXMins = new List<double>();
            ValuesByTagNamesSensorValue = new Dictionary<string, List<float>>();
            UpdatedValuesByTagNamesSensorValue = new Dictionary<string, List<float>>();
            GetChartAxisYParamaters();
            GetChartPortNamesByGroups();

            if (trendView != null)
                _trendView = trendView;
            else if (trendViewType20 != null)
                _trendViewType20 = trendViewType20;

            BatchService batchService = new BatchService(_connectionString);
            Batch batch = new Batch();

            if(!ProcessManager.Instance.IsBatchRunning())
            {
                batch = batchService.GetLastCompleted();
                if (batch == null)
                {
                    WaitIndicatorControl.IsWaitIndicatorVisible = false;
                    //  return;
                }
            }
          
            if(batch != null)
                _batchId = ProcessManager.Instance.CurrentProcess.BatchId == 0 ? batch.id : ((ProcessManager.Instance.CurrentProcess.BatchCurrentState == BatchCurrentState.Running) ? ProcessManager.Instance.CurrentProcess.BatchId : batch.id);
           


            //Todo:h batch null gelme durumu yönetilecek.Db boşken! EK
            //if (batch == null)
            //{
            //    WaitIndicatorControl.IsWaitIndicatorVisible = false;
            //  //  return;
            //}
            //else            
            //_batchId = ProcessManager.Instance.CurrentProcess.BatchId == 0 ? batch.id : ((ProcessManager.Instance.CurrentProcess.BatchCurrentState == BatchCurrentState.Running) ? ProcessManager.Instance.CurrentProcess.BatchId : batch.id);
            //_batchId = 0;

            if (_batchId > 0)
                HasBatchFound = true;
            else
            {
                WaitIndicatorControl.IsWaitIndicatorVisible = false;
                return;
            }

            if (ProcessManager.Instance.CurrentProcess.BatchCurrentState == BatchCurrentState.Running)
                IsBatchInfoSecVisible = Visibility.Collapsed;
            else
            {
                IsBatchInfoSecVisible = Visibility.Visible;
                BatchLoadNumber = batchService.GetById(_batchId).LoadNumber;
            }
        }

        private void GetChartAxisYParamaters()
        {
            var trendChartYAxisParamVal = _applicationPropertyService.GetByName("TrendChartYAxisParamaters")?.Value ?? string.Empty;

            if(!string.IsNullOrEmpty(trendChartYAxisParamVal))
                TrendChartYAxisParamaters = JsonConvert.DeserializeObject<TrendChartYAxisParamaters>(trendChartYAxisParamVal);
        }

        private void GetChartPortNamesByGroups()
        {
            var trendChartPortGroupsVal = _applicationPropertyService.GetByName("TrendChartPortGroups")?.Value ?? string.Empty;
            Dictionary<string, int> trendChartPortGroups = new Dictionary<string, int>();
            VacPortNames = new List<string>();
            TempPortNames = new List<string>();
            PressPortNames = new List<string>();

            if (!string.IsNullOrEmpty(trendChartPortGroupsVal))
            {
                trendChartPortGroups = JsonConvert.DeserializeObject<Dictionary<string, int>>(trendChartPortGroupsVal).ToDictionary(t => t.Key, t => t.Value);

                VacPortNames = trendChartPortGroups.Where(t => t.Value == 1).Select(t => t.Key).ToList();
                TempPortNames = trendChartPortGroups.Where(t => t.Value == 3).Select(t => t.Key).ToList();
                PressPortNames = trendChartPortGroups.Where(t => t.Value == 2).Select(t => t.Key).ToList();
            }
        }

        public async void InitializePageData()
        {
            await CreateDataLogGridModel();

            if(_trendView != null)
                _trendView.CreateChartData();
            else if(_trendViewType20 != null)
                _trendViewType20.CreateChartData();
        }

        public void ContinuousUpdate()
        {
            if (!_isItInitiallyLoaded)
            {
                if (_trendView == null && _trendViewType20 == null)
                    return;

                _isItInitiallyLoaded = true;

                if (_trendView != null)
                    _trendView.Timer.Stop();
                else if (_trendViewType20 != null)
                    _trendViewType20.Timer.Stop();

                InitializePageData();
            }
            else
            {
                if (ProcessManager.Instance.CurrentProcess.BatchCurrentState == BatchCurrentState.Running)
                {
                    CreateUpdatedDataLogGridModel();
                }
            }
        }

        private List<double> ExpandMinValues(double startIndex)
        {
            var newMins = new List<double>();
            double limit =  (startIndex + LastAxisLimitVal);
            // Update LastAxisLimitVal
            LastAxisLimitVal = limit;

            for (double i = startIndex; i <= limit; i++)
            {
                newMins.Add(i);
            }
            return newMins;
        }

        private async Task CreateDataLogGridModel()
        {
            await Task.Run(() =>
            {
                DataTable trendDataTable = _trendReportService.BatchNumericReport(_batchId);
                // Get only ports which have group id 0
                LiveTrendPrimaryPorts = _activeTagService.GetAll().Where(a => a.ActiveTagGroupId == 0 && a.IsLogData == true).OrderBy(a => a.id)
                                                                  .Select(a => a.TagName).ToList();

                if (trendDataTable == null)
                {
                    WaitIndicatorControl.IsWaitIndicatorVisible = false;
                    if (_trendView != null)
                        _trendView.Timer.Start();
                    else if (_trendViewType20 != null)
                        _trendViewType20.Timer.Start();

                    if (!_isPrimaryPortsAdded)
                    {
                        _isPrimaryPortsAdded = true;

                        if (_trendView != null)
                            _trendView.AddPrimaryPortsToChart();
                        else if (_trendViewType20 != null)
                            _trendViewType20.AddPrimaryPortsToChart();

                    }
                    return;
                }

                LastAxisLimitVal = 50;
                ValuesByTagNamesSensorValue.Clear();
                AxisXMins.Clear();
                DataRow trendDataTableLastRow = trendDataTable.Rows[trendDataTable.Rows.Count - 1];
                // Stard Date value of next query
                _trendStartDate = Convert.ToDateTime(trendDataTableLastRow["Time"]);

                trendDataTableLastRow = trendDataTable.Rows[0];
                _dataLogInitialDate = Convert.ToDateTime(trendDataTableLastRow["Time"]);

                var rowList = trendDataTable.Rows;
                var columnList = trendDataTable.Columns;

                List<string> columns = new List<string>();
                List<string> rows = new List<string>();

                foreach (var item in columnList)
                {
                    columns.Add(item.ToString());
                }

                foreach (var item in columns)
                {
                    if (item != "Mins" && item != "Time")
                        ValuesByTagNamesSensorValue.Add(item, new List<float>());
                }

                int columnCounter = 0;
                int rowCounter = 0;

                foreach (var item in rowList)
                {
                    DataRow dataRow = (DataRow)item;

                    rowCounter = 0;
                    columnCounter = 0;
                    foreach (var dataItem in dataRow.ItemArray)
                    {
                        string columnName = columns[columnCounter];

                        if (columnName == "Time")
                        {
                            DateTime value = (dataRow[1] != DBNull.Value) ? Convert.ToDateTime(dataRow[1]) : default;
                            DateTimeValues.Add(value);
                            rowCounter++;
                            columnCounter++;
                            continue;
                        }

                        if (columnName == "Mins")
                        {
                            //ushort value = (dataRow[0] != DBNull.Value) ? Convert.ToUInt16(dataRow[0]) : default;
                            //Mins.Add(value);
                            float value = (dataRow[0] != DBNull.Value) ? (float)Convert.ToDouble(dataRow[0]) : default;
                            Mins.Add(Math.Round(value, 2));
                        }
                        else
                        {
                            float value = (dataRow[rowCounter] != DBNull.Value) ? (float)Convert.ToDouble(dataRow[rowCounter]) : default;
                            ValuesByTagNamesSensorValue[columnName].Add(value);
                        }

                        rowCounter++;
                        columnCounter++;
                    }
                }

                // Check if first minute hasn't logged yet.
                if(Mins.Count ==1)
                    IsMarkerVisible = true;

                // Get chart AxisX values
                double startIndex = Mins.Max();
                LastMin = Convert.ToInt32(startIndex);
                startIndex++;
                var newMins = ExpandMinValues(Convert.ToInt32(startIndex));
                AxisXMins.AddRange(Mins);
                AxisXMins.AddRange(newMins);
            });
        }

        private void CreateUpdatedDataLogGridModel()
        {
            if(ValuesByTagNamesSensorValue.Count == 0)
            {
                InitializePageData();
                return;
            }

            UpdatedValuesByTagNamesSensorValue.Clear();

            DataTable updatedTrendDataTable = _trendReportService.BatchNumericReport(_batchId, _dataLogInitialDate, _trendStartDate, DateTime.Now);

            if (updatedTrendDataTable == null)
                return;

            DataRow trendDataTableLastRow = updatedTrendDataTable.Rows[updatedTrendDataTable.Rows.Count - 1];

            // Stard Date value of next query
            _trendStartDate = Convert.ToDateTime(trendDataTableLastRow["Time"]);

            var rowList = updatedTrendDataTable.Rows;
            var columnList = updatedTrendDataTable.Columns;

            List<string> columns = new List<string>();

            foreach (var item in columnList)
            {
                columns.Add(item.ToString());
            }

            foreach (var item in columns)
            {
                UpdatedValuesByTagNamesSensorValue.Add(item, new List<float>());
            }

            int columnCounter;
            int rowCounter;
            List<ushort> updatedMins = new List<ushort>();
            List<DateTime> updatedDateValues = new List<DateTime>();

            foreach (var item in rowList)
            {
                DataRow dataRow = (DataRow)item;

                rowCounter = 0;
                columnCounter = 0;
                foreach (var dataItem in dataRow.ItemArray)
                {
                    string columnName = columns[columnCounter];

                    if (columnName == "Time")
                    {
                        DateTime value = (dataRow[1] != DBNull.Value) ? Convert.ToDateTime(dataRow[1]) : default;
                        updatedDateValues.Add(value);
                        rowCounter++;
                        columnCounter++;
                        continue;
                    }

                    if (columnName == "Mins")
                    {
                        ushort value = (dataRow[0] != DBNull.Value) ? Convert.ToUInt16(dataRow[0]) : default;
                        updatedMins.Add(value);
                    }
                    else
                    {
                        float value = (dataRow[rowCounter] != DBNull.Value) ? (float)Convert.ToDouble(dataRow[rowCounter]) : default;
                        UpdatedValuesByTagNamesSensorValue[columnName].Add(value);
                    }

                    rowCounter++;
                    columnCounter++;
                }
            }

            #region Mock data logic for testing purposes
            /////////////////////////////////
            //Random randomNum = new Random();
            //foreach (var item in UpdatedValuesByTagNamesSensorValue)
            //{
            //    for (int i = 0; i < 4; i++)
            //    {
            //        float value = randomNum.Next(-600, 280);
            //        UpdatedValuesByTagNamesSensorValue[item.Key].Add(value);
            //    }
            //}

            //updatedMins.Add(18);
            //updatedMins.Add(19);
            //updatedMins.Add(20);
            //updatedMins.Add(58);
            ////////////////////////////////

            //List<ushort> updatedMins = new List<ushort>();

            //for (ushort i = 1; i <= 48; i++)
            //{
            //    updatedMins.Add(i);
            //}

            //LastMinVal++;
            //updatedMins.Add(LastMinVal);

            ////First, create mock updated data
            //Random randomNum = new Random();

            //UpdatedValuesByTagNamesSensorValue = new Dictionary<string, List<float>>();

            //var seriesNames = ValuesByTagNamesSensorValue.Keys.ToList();
            //seriesNames.Add("Mins");

            //foreach (var item in seriesNames)
            //{
            //    if (item != "Time")
            //        UpdatedValuesByTagNamesSensorValue.Add(item, new List<float>());
            //}

            //foreach (string item in seriesNames)
            //{
            //    if (item == "Mins")
            //        continue;

            //    for (int i = 0; i < 3; i++)
            //    {
            //        float value = randomNum.Next(-750, 325);
            //        UpdatedValuesByTagNamesSensorValue[item].Add(value);
            //    }
            //}
            #endregion

            if(IsMarkerVisible && updatedMins.Max() > 0)
            {
                IsMarkerVisible = false;

                if (_trendView != null)
                    _trendView.HideVisibleMarkers();
                else if (_trendViewType20 != null)
                    _trendViewType20.HideVisibleMarkers();
            }

            if (_trendView != null)
                _trendView.UpdateChartData(UpdatedValuesByTagNamesSensorValue, updatedMins, updatedDateValues);
            else if (_trendViewType20 != null)
                _trendViewType20.UpdateChartData(UpdatedValuesByTagNamesSensorValue, updatedMins, updatedDateValues);
        }
    }
}
