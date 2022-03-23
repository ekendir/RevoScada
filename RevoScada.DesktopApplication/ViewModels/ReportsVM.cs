
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.Helpers;
using System.Windows;
using System.Windows.Input;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.Entities;
using System.Text.RegularExpressions;
using RevoScada.Entities.Complex;
using RevoScada.Business.Report;
using RevoScada.DesktopApplication.Views;
using Newtonsoft.Json;
using System.Data;
using RevoScada.Entities.Complex.Report;
using RevoScada.Entities.Configuration;

namespace RevoScada.DesktopApplication.ViewModels
{
    public class ReportsVM : UserControlBaseVM
    {
        #region Services
        private readonly string _connectionString;
        private BatchService _batchService;
        private BagService _bagService;
        private ActiveTagService _activeTagService;
        private LotPropertyService _lotPropertyService;
        private RecipeService _recipeService;
        private RecipeFieldService _recipeFieldService;
        private RecipeDetailHistoryService _recipeDetailHistoryService;
        private DataLogService _dataLogService;
        private NumericReportService _numericReportService;
        private IntegratedCheckResultService _integratedCheckResultService;
        private PlcAlarmService _plcAlarmService;
        private SkippedIntegratedCheckResultService _skippedIntegratedCheckResultService;
        private ApplicationPropertyService _applicationPropertyService;
        private ProcessEventLogService _processEventLogService;
        private BatchReportService _batchReportService;
        private TrendReportService _trendReportService;
        private BatchQualityService _batchQualityService;




        #endregion

        #region Collections
        private List<Bag> BagsColl;
        private List<LotProperty> LotPropertiesColl;
        private List<ActiveTag> ActiveTagsColl;
        public List<ReportBatchGridModel> ReportBatchGridModels;
        public List<RecipeField> RecipeFieldsColl;
        public List<RecipeDetailHistory> RecipeDetailHistoriesColl;
        private IEnumerable<DataLog> DataLogsColl;
        public Dictionary<string, List<string>> NumericValuesByTagNames;
        public Dictionary<string, List<float>> TrendValuesByTagNames;
        public Dictionary<int, SiemensTagConfiguration> AlarmTagConfigurations { get; private set; }

        public List<DateTime> TrendDateTimeValues;
        public List<double> Mins;
        private IEnumerable<IntegratedCheckResult> IntegratedChecksColl;
        private IEnumerable<PlcAlarm> PlcAlarmsColl;
        

        private IEnumerable<string> NumericPrimaryPorts;
        public List<string> SelectedSeriesNames;

        private ObservableCollection<IntegratedCheckGridModel> _integratedChecksGridModels;
        public ObservableCollection<IntegratedCheckGridModel> IntegratedChecksGridModels
        {
            get => _integratedChecksGridModels;
            set => OnPropertyChanged(ref _integratedChecksGridModels, value);
        }
        private ObservableCollection<SkippedIntegratedCheckResult> _skippedIntegratedChecksColl;
        public ObservableCollection<SkippedIntegratedCheckResult> SkippedIntegratedChecksColl
        {
            get => _skippedIntegratedChecksColl;
            set => OnPropertyChanged(ref _skippedIntegratedChecksColl, value);
        }
        private ObservableCollection<PlcAlarmGridModel> _plcAlarmsGridModels;
        public ObservableCollection<PlcAlarmGridModel> PlcAlarmsGridModels
        {
            get => _plcAlarmsGridModels;
            set => OnPropertyChanged(ref _plcAlarmsGridModels, value);
        }
        private ObservableCollection<ProcessEventLog> _processEventLogsFiltered;
        public ObservableCollection<ProcessEventLog> ProcessEventLogsFiltered
        {
            get => _processEventLogsFiltered;
            set => OnPropertyChanged(ref _processEventLogsFiltered, value);
        }
        public List<ProcessEventLog> ProcessEventLogs;
        private ObservableCollection<BatchSearchDto> _batchesAndRecipesColl;
        public ObservableCollection<BatchSearchDto> BatchesAndRecipesColl
        {
            get => _batchesAndRecipesColl;
            set => OnPropertyChanged(ref _batchesAndRecipesColl, value);
        }
        private BatchInformationGrid _batchInfo;
        public BatchInformationGrid BatchInfo
        {
            get => _batchInfo;
            set => OnPropertyChanged(ref _batchInfo, value);
        }

        public Dictionary<int, string> QualityCards
        {
            get
            {
                var batchQualities = _batchQualityService.GetAll();
                var batchQualityList = new Dictionary<int, string>();

                if (batchQualities.Count() == 0)
                    return batchQualityList;

                batchQualityList.Add(0, "Select a Quality Card");
                foreach (var item in batchQualities)
                {
                    batchQualityList.Add(item.id, item.CardName);
                }

                return batchQualityList;
            }
        }

        public Dictionary<string, short> NumericTimeFilterValues { get; set; } = new Dictionary<string, short>()
        {
            {"1 Minute", 1},
            {"3 Minute", 3},
            {"5 Minute", 5}
        };

        public short SelectedNumericTimeFilterValue { get; set; }

        #region Port Lists
        private ObservableCollection<int> _ptcPortList;
        public ObservableCollection<int> PtcPortList
        {
            get => _ptcPortList;
            set => OnPropertyChanged(ref _ptcPortList, value);
        }
        private ObservableCollection<int> _monPortList;
        public ObservableCollection<int> MonPortList
        {
            get => _monPortList;
            set => OnPropertyChanged(ref _monPortList, value);
        }
        private ObservableCollection<int> _vacPortList;
        public ObservableCollection<int> VacPortList
        {
            get => _vacPortList;
            set => OnPropertyChanged(ref _vacPortList, value);
        }
        private ObservableCollection<LotProperty> _lotSelectedData;
        public ObservableCollection<LotProperty> LotSelectedData
        {
            get => _lotSelectedData;
            set => OnPropertyChanged(ref _lotSelectedData, value);
        }
        #endregion
        #endregion

        #region Properties
        private int _selectedBatchId;
        public int SelectedBatchId
        {
            get => _selectedBatchId;
            set
            {
                OnPropertyChanged(ref _selectedBatchId, value);

                if (_selectedBatchId > 0)
                    AreNavButtonsEnabled = true;
                else
                    AreNavButtonsEnabled = false;
            }
        }
        public Batch SelectedBatch { get; set; }
        public Entities.Recipe SelectedRecipe { get; set; }
        private string _batchName;
        public string BatchName
        {
            get
            {
                return _batchName;
            }
            set
            {
                OnPropertyChanged(ref _batchName, value);
            }
        }
        private string _ptcTotalText;
        public string PtcTotalText
        {
            get
            {
                return _ptcTotalText;
            }
            set
            {
                OnPropertyChanged(ref _ptcTotalText, value);
            }
        }
        private string _monTotalText;
        public string MonTotalText
        {
            get
            {
                return _monTotalText;
            }
            set
            {
                OnPropertyChanged(ref _monTotalText, value);
            }
        }
        private string _vacTotalText;
        public string VacTotalText
        {
            get
            {
                return _vacTotalText;
            }
            set
            {
                OnPropertyChanged(ref _vacTotalText, value);
            }
        }

        private bool _areNavButtonsEnabled;
        public bool AreNavButtonsEnabled
        {
            get => _areNavButtonsEnabled;
            set => OnPropertyChanged(ref _areNavButtonsEnabled, value);
        }

        private bool _isQualityCardsSpVisible;
        public bool IsQualityCardsSpVisible
        {
            get => _isQualityCardsSpVisible;
            set => OnPropertyChanged(ref _isQualityCardsSpVisible, value);
        }

        private bool _isShowReportBtnVisible;
        public bool IsShowReportBtnVisible
        {
            get => _isShowReportBtnVisible;
            set => OnPropertyChanged(ref _isShowReportBtnVisible, value);
        }
        private bool _isShowReportBtnEnabled;
        public bool IsShowReportBtnEnabled
        {
            get => _isShowReportBtnEnabled;
            set => OnPropertyChanged(ref _isShowReportBtnEnabled, value);
        }

        private Visibility _trendNoDataSecVisibility;
        public Visibility TrendNoDataSecVisibility
        {
            get => _trendNoDataSecVisibility;
            set => OnPropertyChanged(ref _trendNoDataSecVisibility, value);
        }
        
        private Visibility _printAllTrendBtnVisibility = Visibility.Collapsed;
        public Visibility PrintAllTrendBtnVisibility
        {
            get => _printAllTrendBtnVisibility;
            set => OnPropertyChanged(ref _printAllTrendBtnVisibility, value);
        }
        #endregion

        #region Fields
        public Views.Reports ReportsView;
        private DataTable _numericDataTable;
        #endregion

        #region Section Visibility Properties
        private Visibility _batchVisibility;
        public Visibility BatchVisibility
        {
            get
            {
                return _batchVisibility;
            }
            set
            {
                OnPropertyChanged(ref _batchVisibility, value);
            }
        }

        private Visibility _numericVisibility;
        public Visibility NumericVisibility
        {
            get
            {
                return _numericVisibility;
            }
            set
            {
                OnPropertyChanged(ref _numericVisibility, value);
            }
        }

        private Visibility _trendVisibility;
        public Visibility TrendVisibility
        {
            get
            {
                return _trendVisibility;
            }
            set
            {
                OnPropertyChanged(ref _trendVisibility, value);
            }
        }

        private Visibility _messagesVisibility;
        public Visibility MessagesVisibility
        {
            get
            {
                return _messagesVisibility;
            }
            set
            {
                OnPropertyChanged(ref _messagesVisibility, value);
            }
        }

        private Visibility _recipeVisibility;
        public Visibility RecipeVisibility
        {
            get
            {
                return _recipeVisibility;
            }
            set
            {
                OnPropertyChanged(ref _recipeVisibility, value);
            }
        }

        private Visibility _reportVisibility;
        public Visibility ReportVisibility
        {
            get
            {
                return _reportVisibility;
            }
            set
            {
                OnPropertyChanged(ref _reportVisibility, value);
            }
        }

        private Visibility _integrityCheckVisibility;
        public Visibility IntegrityCheckVisibility
        {
            get
            {
                return _integrityCheckVisibility;
            }
            set
            {
                OnPropertyChanged(ref _integrityCheckVisibility, value);
            }
        }
        #endregion

        #region Public Commands
        public ICommand NavigationCommand { get; set; }
        public ICommand FilterSystemEventCommand { get; set; }
        #endregion

        public ReportsVM(WaitIndicatorControl waitIndicatorControl)
        {
            WaitIndicatorControl = waitIndicatorControl;
            WaitIndicatorControl.IsWaitIndicatorVisible = true;
            WaitIndicatorControl.IsWaitIndicatorTextActive = true;
            _connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;
            _batchService = new BatchService(_connectionString);
            _bagService = new BagService(_connectionString);
            _activeTagService = new ActiveTagService(_connectionString);
            _lotPropertyService = new LotPropertyService(_connectionString);
            _recipeService = new RecipeService(_connectionString);
            _recipeFieldService = new RecipeFieldService(_connectionString);
            _recipeDetailHistoryService = new RecipeDetailHistoryService(_connectionString);
            _dataLogService = new DataLogService(_connectionString);
            _numericReportService = new NumericReportService(_connectionString);
            _integratedCheckResultService = new IntegratedCheckResultService(_connectionString);
            _plcAlarmService = new PlcAlarmService(_connectionString);
            _skippedIntegratedCheckResultService = new SkippedIntegratedCheckResultService(_connectionString);
            _applicationPropertyService = new ApplicationPropertyService(_connectionString);
            _processEventLogService = new ProcessEventLogService(_connectionString);
            _batchReportService = new BatchReportService(_connectionString);
            _trendReportService = new TrendReportService(_connectionString);
            _batchQualityService = new BatchQualityService(_connectionString);

            NavigationCommand = new RelayCommand(LoadSelectedSection);
            FilterSystemEventCommand = new RelayCommand(FilterSystemEventData);

            CollapseAllSections();
            BatchVisibility = Visibility.Visible;
            if (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId == 20)
            {
                PrintAllTrendBtnVisibility = Visibility.Visible;
            }
            LotPropertiesColl = new List<LotProperty>();
            LotSelectedData = new ObservableCollection<LotProperty>();
            ReportBatchGridModels = new List<ReportBatchGridModel>();
            NumericValuesByTagNames = new Dictionary<string, List<string>>();
            TrendValuesByTagNames = new Dictionary<string, List<float>>();
            TrendDateTimeValues = new List<DateTime>();
            PtcPortList = new ObservableCollection<int>();
            MonPortList = new ObservableCollection<int>();
            VacPortList = new ObservableCollection<int>();
            BatchesAndRecipesColl = new ObservableCollection<BatchSearchDto>();
            BatchInfo = new BatchInformationGrid();
            BagsColl = new List<Bag>();
            ProcessEventLogs = new List<ProcessEventLog>();
            ActiveTagsColl = new List<ActiveTag>();
            RecipeFieldsColl = new List<RecipeField>();
            RecipeDetailHistoriesColl = new List<RecipeDetailHistory>();
            IntegratedChecksGridModels = new ObservableCollection<IntegratedCheckGridModel>();
            PlcAlarmsGridModels = new ObservableCollection<PlcAlarmGridModel>();
            Mins = new List<double>();
            SelectedSeriesNames = new List<string>();
            BatchName = string.Empty;
            SelectedNumericTimeFilterValue = 1;
            AlarmTagConfigurations = ApplicationConfigurations.Instance.TagConfigurations.Where(x => ((SiemensTagConfiguration)x.Value).SiemensTagGroupId == 3 && ((SiemensTagConfiguration)x.Value).IsActive == true).ToDictionary(x => x.Key, x => (SiemensTagConfiguration)x.Value);


            InitializePageData();
        }

        private void InitializePageData()
        {
            ActiveTagsColl = _activeTagService.GetAll().OrderBy(a => a.id).ToList();
            try
            {
                var numericPrimaryPortsAppProperty = _applicationPropertyService.GetByName("UI_Numeric_PrimaryPorts");
                NumericPrimaryPorts = JsonConvert.DeserializeObject<IEnumerable<string>>(numericPrimaryPortsAppProperty.Value);


            }
            catch 
            {

              
            }
          
            BatchesAndRecipesColl = SearchBatchAndRecipeName(string.Empty, 150).ToObservableCollection();

            WaitIndicatorControl.IsWaitIndicatorVisible = false;
        }

        private async void LoadBatchSection()
        {
            await LoadBatchSectionAsync();
        }

        private async Task LoadBatchSectionAsync()
        {
            WaitIndicatorControl.IsWaitIndicatorVisible = true;
            await Task.Run(() =>
            {
                BagsColl = _bagService.BagsByBatch(_selectedBatchId).ToList();
                GetSelectedLotProperties();
                CreateReportBatchGridModel();
            });

            // To avoid STA error, load these two outside of the new thread.
            UpdatePortLists();
            UpdateLotData();
            ReportsView.LoadBagsToTreeView();

            WaitIndicatorControl.IsWaitIndicatorVisible = false;
        }

        private async Task LoadRecipeSectionAsync()
        {
            await Task.Run(() =>
            {
                if (RecipeFieldsColl.Count == 0)
                    RecipeFieldsColl = _recipeFieldService.GetAll().Where(r => r.IsActive == true).OrderBy(r => r.RecipeFieldOrder).ToList();

                if (SelectedRecipe != null && SelectedBatch != null)
                    RecipeDetailHistoriesColl = _recipeDetailHistoryService.GetByBatch(SelectedRecipe.id, SelectedBatch.id).ToList();
            });
        }

        private async Task LoadNumericSectionAsync()
        {
            // todo:m report servislerinden alınmalı burası.

            await Task.Run(() =>
            {
                DataLogsColl = _dataLogService.GetByBatch(_selectedBatchId).OrderBy(d => d.TagConfigurationId);
                _numericDataTable = null;
                _numericDataTable = _numericReportService.BatchNumericReport(_selectedBatchId).NumericDataTable;
                _numericDataTable = FilterDataTableByTime(_numericDataTable, SelectedNumericTimeFilterValue);
            });
        }

        /// <summary>
        /// Since our current numeric datatable contains data for every minute, this method skips it specific amount of rows according to desiredMin value.
        /// </summary>
        /// <param name="dataTable">To be filtered datatable</param>
        /// <param name="desiredMin"></param>
        /// <returns></returns>
        private DataTable FilterDataTableByTime(DataTable dataTable, short desiredMin)
        {
            if (dataTable == null || dataTable.Rows.Count == 1)
                return dataTable;

            switch (desiredMin)
            {
                case 1:
                    // 1 minute, do nothing
                    break;
                case 3:
                    // 3 minute
                    if (dataTable.Rows.Count <= 3)
                        dataTable = dataTable.AsEnumerable().Take(1).CopyToDataTable();
                    else
                        dataTable = dataTable.AsEnumerable().Every(3).CopyToDataTable();
                    break;
                case 5:
                    // 5 minute
                    if (dataTable.Rows.Count <= 5)
                        dataTable = dataTable.AsEnumerable().Take(1).CopyToDataTable();
                    else
                        dataTable = dataTable.AsEnumerable().Every(5).CopyToDataTable();
                    break;
            }

            return dataTable;
        }

        private async Task LoadIntegritySectionAsync()
        {
            await Task.Run(() =>
            {
                IntegratedChecksColl = _integratedCheckResultService.GetAllByBatchId(_selectedBatchId);
                IntegratedChecksGridModels = CreateIntegrityCheckGridModel();
            });
        }

        private async Task LoadAlarmSectionAsync()
        {
            await Task.Run(() =>
            {
                PlcAlarmsColl = _plcAlarmService.GetByBatchId(_selectedBatchId);
                PlcAlarmsGridModels = CreatePlcAlarmGridModel();
                ProcessEventLogs.Clear();
                ProcessEventLogs = _processEventLogService.GetByBatchId(_selectedBatchId).ToList();
                ProcessEventLogsFiltered = ProcessEventLogs.ToObservableCollection();
            });
        }

        private async void LoadSelectedSection(object value)
        {
            string buttonName = null;
            if (value != null)
                buttonName = value.ToString();

            WaitIndicatorControl.IsWaitIndicatorVisible = true;
            switch (buttonName)
            {
                case "Batch":
                    CollapseAllSections();
                    BatchVisibility = Visibility.Visible;
                    break;
                case "Numeric":
                    CollapseAllSections();
                    NumericVisibility = Visibility.Visible;
                    await LoadNumericSectionAsync();
                    await ReportsView.LoadNumericDataAsync(_numericDataTable);
                    break;
                case "Trend":
                    CollapseAllSections();
                    TrendVisibility = Visibility.Visible;
                    var itemCount = await GetTrendDataAsync();

                    TrendNoDataSecVisibility = Visibility.Hidden;
                    if (itemCount == 0)
                    {
                        TrendNoDataSecVisibility = Visibility.Visible;
                        break;
                    }

                    bool isChartMarkerVisible = false;

                    if (itemCount == 1)
                        isChartMarkerVisible = true;

                    Trend_Report_Window trendReportWindow = new Trend_Report_Window(this, isChartMarkerVisible);
                    trendReportWindow.ShowDialog();
                    break;
                case "Messages":
                    CollapseAllSections();
                    MessagesVisibility = Visibility.Visible;
                    await LoadAlarmSectionAsync();
                    break;
                case "Recipe":
                    CollapseAllSections();
                    RecipeVisibility = Visibility.Visible;
                    await LoadRecipeSectionAsync();
                    ReportsView.LoadRecipeData();
                    break;
                case "Report":
                    CollapseAllSections();
                    ReportVisibility = Visibility.Visible;
                    break;
                case "Integrity Check":
                    CollapseAllSections();
                    IntegrityCheckVisibility = Visibility.Visible;
                    await LoadIntegritySectionAsync();
                    break;
                default:
                    break;
            }
            WaitIndicatorControl.IsWaitIndicatorVisible = false;
        }

        public void CollapseAllSections()
        {
            BatchVisibility = Visibility.Collapsed;
            NumericVisibility = Visibility.Collapsed;
            TrendVisibility = Visibility.Collapsed;
            MessagesVisibility = Visibility.Collapsed;
            RecipeVisibility = Visibility.Collapsed;
            ReportVisibility = Visibility.Collapsed;
            IntegrityCheckVisibility = Visibility.Collapsed;
        }

        public void GetSelectedRecipeAndBatch(int batchId)
        {
            SelectedBatchId = batchId;
            SelectedBatch = _batchService.GetById(batchId);

            //if(SelectedBatch != null) böyle bir mantık yok batch skip edilerekte çalıştırılabilir  E.K
            SelectedRecipe = _recipeService.GetById(SelectedBatch.RecipeId);
            LoadBatchSection();
        }

        public ObservableCollection<BatchSearchDto> SearchBatchAndRecipeName(string text, int count)
        {
            BatchesAndRecipesColl = _batchService.SearchBatchAndRecipeName(text, count).ToObservableCollection();
            return BatchesAndRecipesColl;
        }

        public DateTime GetStartDateOfBatch(int batchId)
        {
            Batch batch = _batchService.GetById(batchId);
            DateTime dateTime = new DateTime();

            if (batch != null)
                dateTime = batch.StartDate;

            return dateTime;
        }

        public DateTime GetEndDateOfBatch(int batchId)
        {
            Batch batch = _batchService.GetById(batchId);
            DateTime dateTime = new DateTime();

            if (batch != null)
                dateTime = batch.EndDate;

            return dateTime;
        }


        private void CreateReportBatchGridModel()
        {
            ReportBatchGridModels.Clear();

            foreach (var bag in BagsColl)
            {
                ReportBatchGridModel reportBatch = new ReportBatchGridModel();

                // Add bag name and id
                reportBatch.BagName = bag.BagName;
                reportBatch.BagId = bag.id;

                // Get ports by bags
                foreach (var item in bag.SelectedPorts)
                {
                    var activeTag = ActiveTagsColl.Where(a => a.id == item).FirstOrDefault();

                    if(activeTag == null)
                    { return; }

                    string portType = null;

                    string[] portTagName = Regex.Matches(activeTag.TagName, @"[a-zA-Z]+|\d+")
                                                 .Cast<Match>()
                                                 .Select(m => m.Value)
                                                 .ToArray();
                    // Gets port type name e.g, PTC
                    portType = portTagName[0];

                    switch (portType)
                    {
                        case "PTC":
                            reportBatch.PartTc += activeTag.TagName + "\n";
                            break;
                        case "MON":
                            reportBatch.Monitor += activeTag.TagName + "\n";
                            break;
                        case "VAC":
                            reportBatch.Src += activeTag.TagName + "\n";
                            break;
                        default:
                            break;
                    }
                }

                // Get lot data by bags
                List<LotProperty> LotData = LotPropertiesColl.Where(l => l.BagId == bag.id).ToList();

                foreach (var item in LotData)
                {
                    reportBatch.SoirName += item.SoirNumber + "\n";
                    reportBatch.PartName += item.PartName + "\n";
                    reportBatch.ToolName += item.ToolName + "\n";
                }

                // Add this report batch data grid object to the collection
                ReportBatchGridModels.Add(reportBatch);
            }

            if (ReportBatchGridModels.Count > 0)
                ReportBatchGridModels = ReportBatchGridModels.OrderBy(r => r.BagName).ToList();
        }

        private void GetSelectedLotProperties()
        {
            LotPropertiesColl.Clear();
            var bagIdList = new List<int>();

            foreach (var item in BagsColl)
            {
                bagIdList.Add(item.id);
            }

            var allLotProperties = _lotPropertyService.GetByBagIdListProperties(bagIdList);

            if(allLotProperties != null)
                LotPropertiesColl.AddRange(allLotProperties);
        }

        public void UpdatePortLists(int selectedBagId = 0)
        {
            //First, clear all port lists
            PtcPortList.Clear();
            MonPortList.Clear();
            VacPortList.Clear();

            foreach (var item in ReportBatchGridModels)
            {
                // If method passed a paramater then just load this bag's port list.
                // otherwise, load all bags' port lists.
                if (selectedBagId > 0 && selectedBagId != item.BagId)
                    continue;
                
                for (int i = 0; i < 3; i++)
                {
                    string selectedPort = string.Empty;
                    switch (i)
                    {
                        case 0: // PTC
                            selectedPort = item.PartTc;
                            break;
                        case 1: // MON
                            selectedPort = item.Monitor;
                            break;
                        case 2: // VAC
                            selectedPort = item.Src;
                            break;
                        default:
                            break;
                    }

                    var portNames = selectedPort?.Split('\n');
                    // If it is null then go on with next one.
                    if (portNames == null)
                        continue;

                    foreach (var port in portNames)
                    {
                        if (string.IsNullOrWhiteSpace(port))
                            break;

                        string portType = null;
                        int portNum = 0;

                        string[] portTagName = Regex.Matches(port, @"[a-zA-Z]+|\d+")
                                                     .Cast<Match>()
                                                     .Select(m => m.Value)
                                                     .ToArray();
                        // Gets port type name e.g, PTC
                        portType = portTagName[0];
                        portNum = Convert.ToInt32(portTagName[1]);

                        switch (portType)
                        {
                            case "PTC":
                                PtcPortList.Add(portNum);
                                break;
                            case "MON":
                                MonPortList.Add(portNum);
                                break;
                            case "VAC":
                                VacPortList.Add(portNum);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            PtcPortList = PtcPortList.OrderBy(p => p).ToObservableCollection();
            MonPortList = MonPortList.OrderBy(p => p).ToObservableCollection();
            VacPortList = VacPortList.OrderBy(p => p).ToObservableCollection();
            PtcTotalText = "Total: " + PtcPortList.Count;
            MonTotalText = "Total: " + MonPortList.Count;
            VacTotalText = "Total: " + VacPortList.Count;
        }

        public void UpdateLotData(int selectedBagId = 0)
        {
            LotSelectedData.Clear();

            if (selectedBagId > 0)
                LotSelectedData = LotPropertiesColl.Where(l => l.BagId == selectedBagId).ToObservableCollection();
            else
                LotSelectedData = LotPropertiesColl.ToObservableCollection();
        }

        private async Task<int> GetTrendDataAsync()
        {
            DataTable trendDataTable = new DataTable();
            await Task.Run(() =>
            {
                TrendValuesByTagNames.Clear();
                Mins.Clear();
                trendDataTable = _trendReportService.BatchNumericReport(SelectedBatchId);

                if (trendDataTable == null)
                    return;

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
                    if (item != "Time")
                    {
                        TrendValuesByTagNames.Add(item, new List<float>());
                    }
                }

                if(SelectedSeriesNames.Count == 0)
                {
                    SelectedSeriesNames.AddRange(TrendValuesByTagNames.Keys);
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
                            TrendDateTimeValues.Add(value);
                            rowCounter++;
                            columnCounter++;
                            continue;
                        }

                        if (columnName == "Mins")
                        {
                            float value = (dataRow[0] != DBNull.Value) ? (float)Convert.ToDouble(dataRow[0]) : default;
                            Mins.Add(value);
                        }
                        else
                        {
                            float value = (dataRow[rowCounter] != DBNull.Value) ? (float)Convert.ToDouble(dataRow[rowCounter]) : default;

                            TrendValuesByTagNames[columnName].Add(value);
                        }

                        rowCounter++;
                        columnCounter++;
                    }
                }
            });

            return trendDataTable?.Rows.Count ?? 0;
        }

        private ObservableCollection<IntegratedCheckGridModel> CreateIntegrityCheckGridModel()
        {
            ObservableCollection<IntegratedCheckGridModel> list = new ObservableCollection<IntegratedCheckGridModel>();
            foreach (var item in IntegratedChecksColl)
            {
                IntegratedCheckGridModel integratedCheckGrid = new IntegratedCheckGridModel();
                integratedCheckGrid.PortName = ActiveTagsColl.Where(a => a.id == item.SensorTagId).Select(a => a.TagName).FirstOrDefault();
                integratedCheckGrid.ActualValue = item.ActualValue;
                integratedCheckGrid.StartValue = item.StartValue;
                integratedCheckGrid.RequirementValue = item.RequirementValue;
                integratedCheckGrid.FinishValue = item.FinishValue;
                integratedCheckGrid.Deviation = item.Deviation;
                integratedCheckGrid.CheckResultSaveDate = item.CheckResultSaveDate;
                integratedCheckGrid.id = item.id;

                Bag bag = BagsColl.Where(b => b.id == item.BagId).FirstOrDefault();
                if (bag != null)
                    integratedCheckGrid.BagName = bag.BagName;

                list.Add(integratedCheckGrid);
            }

            // Check if there are any integrated check results if not,
            // then get the skipped integrated check results.
            if(list.Count == 0)
            {
                DateTime startDate = new DateTime(2000, 01, 01);
                DateTime endDate = new DateTime(3000, 01, 01);
                SkippedIntegratedChecksColl = _skippedIntegratedCheckResultService.GetByBatchIdAndDate(SelectedBatchId, startDate, endDate).ToObservableCollection();
            }

            return list;
        }

        private ObservableCollection<PlcAlarmGridModel> CreatePlcAlarmGridModel()
        {
            ObservableCollection<PlcAlarmGridModel> list = new ObservableCollection<PlcAlarmGridModel>();

            foreach (var item in PlcAlarmsColl)
            {
                PlcAlarmGridModel PlcAlarmGrid = new PlcAlarmGridModel();
                PlcAlarmGrid.id = item.id;
                if (item.AcknowledgedDateTime == new DateTime(0001, 1, 1, 12, 00, 00))
                    PlcAlarmGrid.AcknowledgedDateTime = null;
                else
                    PlcAlarmGrid.AcknowledgedDateTime = item.AcknowledgedDateTime;

                if (item.InDateTime == new DateTime(0001, 1, 1, 12, 00, 00))
                    PlcAlarmGrid.InDateTime = null;
                else
                    PlcAlarmGrid.InDateTime = item.InDateTime;

                PlcAlarmGrid.OutDateTime = item.OutDateTime;
                PlcAlarmGrid.Status = item.Status;
                PlcAlarmGrid.PlcValue = item.PlcValue;

              
                SiemensTagConfiguration siemensTagConfiguration =   AlarmTagConfigurations[item.TagConfigurationId];



            //    ActiveTag activeTag = ActiveTagsColl.Where(a => a.id == item.TagConfigurationId).FirstOrDefault();
                if (siemensTagConfiguration != null)
                    PlcAlarmGrid.TagName = siemensTagConfiguration.TagName;

                list.Add(PlcAlarmGrid);
            }
            return list;
        }

        private void FilterSystemEventData(object param)
        {
            string filterValue = (string)param;

            switch (filterValue)
            {
                case "System":
                    ProcessEventLogsFiltered = ProcessEventLogs.Where(s => s.Type == "System").ToObservableCollection();
                    break;
                case "Manual":
                    ProcessEventLogsFiltered = ProcessEventLogs.Where(s => s.Type == "Manual").ToObservableCollection();
                    break;
                case "All":
                    ProcessEventLogsFiltered = ProcessEventLogs.ToObservableCollection();
                    break;
                default:
                    break;
            }
        }

    }
}
