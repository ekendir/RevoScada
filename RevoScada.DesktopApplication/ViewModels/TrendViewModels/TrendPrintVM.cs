using Newtonsoft.Json;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.Models.SettingModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.ViewModels.TrendViewModels
{
    public class TrendPrintVM : UserControlBaseVM
    {
        #region Properties
        private string _batchName;
        public string BatchName
        {
            get => _batchName;
            set => OnPropertyChanged(ref _batchName, value);
        }

        private string _bagName;
        public string BagName
        {
            get => _bagName;
            set => OnPropertyChanged(ref _bagName, value);
        }

        private string _soirName;
        public string SoirName
        {
            get => _soirName;
            set => OnPropertyChanged(ref _soirName, value);
        }

        private string _partName;
        public string PartName
        {
            get => _partName;
            set => OnPropertyChanged(ref _partName, value);
        }

        private string _toolName;
        public string ToolName
        {
            get => _toolName;
            set => OnPropertyChanged(ref _toolName, value);
        }

        private string _startDate;
        public string StartDate
        {
            get => _startDate;
            set => OnPropertyChanged(ref _startDate, value);
        }

        private string _endDate;
        public string EndDate
        {
            get => _endDate;
            set => OnPropertyChanged(ref _endDate, value);
        }

        private string _recipeName;
        public string RecipeName
        {
            get => _recipeName;
            set => OnPropertyChanged(ref _recipeName, value);
        }
        #endregion

        #region Collections
        public List<string> VacPortNames;
        public List<string> TempPortNames;
        public List<string> PressPortNames;
        private ObservableCollection<SeriesDetailModel> _seriesDetails;
        public ObservableCollection<SeriesDetailModel> SeriesDetails
        {
            get => _seriesDetails;
            set => OnPropertyChanged(ref _seriesDetails, value);
        }
        #endregion

        #region Fields
        private readonly string _connectionString;
        public TrendChartYAxisParamaters TrendChartYAxisParamaters;
        #endregion

        #region Services
        private ApplicationPropertyService _applicationPropertyService;
        #endregion

        public TrendPrintVM()
        {
            _connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;
            _applicationPropertyService = new ApplicationPropertyService(_connectionString);

            SeriesDetails = new ObservableCollection<SeriesDetailModel>();
            GetChartPortNamesByGroups();
            GetChartAxisYParamaters();
        }

        private void GetChartAxisYParamaters()
        {
            var trendChartYAxisParamVal = _applicationPropertyService.GetByName("TrendChartYAxisParamaters")?.Value ?? string.Empty;

            if (!string.IsNullOrEmpty(trendChartYAxisParamVal))
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
    }
}