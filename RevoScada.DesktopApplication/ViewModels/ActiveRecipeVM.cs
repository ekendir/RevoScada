using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Xpf.WindowsUI;
using Newtonsoft.Json;
using Revo.Core;
using RevoScada.ProcessController;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.Views;
using RevoScada.Entities;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.PageTagConfigurations;
using RevoScada.Entities.PageTagConfigurations.PageTagConfigEnum;

namespace RevoScada.DesktopApplication.ViewModels
{

    public class ActiveRecipeVM : UserControlBaseVM
    {
        #region Services
        private PlcCommandManager _plcCommandManager;
        
        private RecipeFieldService _recipeFieldService;
        private RecipeDetailService _recipeDetailService;
        private PredefinedRecipeFieldService _predefinedRecipeFieldService;

        #endregion

        #region Collections
        private List<string> _recipeDetailValues;
        public List<List<string>> SegAndTableLists;
        public List<short> RecipeFieldIdNumbers;
        public short[] TwoOffsetFieldIdNumbers;
        private IEnumerable<RecipeField> _recipeFields;
        public IEnumerable<short> DisabledRecipeFieldIdNumbers;
        public List<KeyValuePair<string, string>> PredefinedRecipeFields;
        public List<List<string>> RecipeDetailValuesFromDb;
        public Dictionary<string, string> ActiveRecipeLanguageSettings { get; set; }

        #endregion

        #region Fields
        private readonly string _connectionString;
        public RecipeTagConfigurations RecipeTagConfigurations;
        private SiemensTagConfiguration _activeBatchSegmentNo;
        public ActiveRecipeControl RecipeView;
        private int _totalSegments;
        public int TotalRecipeRows;
        private int _segNo;
        #endregion

        #region Properties
        private bool _isProcessRunning;
        public bool IsProcessRunning
        {
            get => _isProcessRunning;
            set => OnPropertyChanged(ref _isProcessRunning, value);
        }
        #endregion

        //todo:h recipe ok olmadığı zaman görüntülenmeyecek...
        //todo:m runninng durumunda plcden gelen değerler görüntülenecek
        //todo:l finish durumunda son çalışan reçete arşiveden çekilecek....

        public ActiveRecipeVM(WaitIndicatorControl waitIndicatorControl, Dictionary<string, bool> permissions, UserGridModel activeUser)
        {
            WaitIndicatorControl = waitIndicatorControl;
            WaitIndicatorControl.IsWaitIndicatorVisible = true;
            _connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;

            InitializePageTagConfigurations();

            Permissions = permissions;
            ActiveUser = activeUser;
            _recipeFieldService = new RecipeFieldService(_connectionString);
            _recipeDetailService = new RecipeDetailService(_connectionString);
            _predefinedRecipeFieldService = new PredefinedRecipeFieldService(_connectionString);

            SegAndTableLists = new List<List<string>>();
            _recipeFields = _recipeFieldService.GetAll().OrderBy(x => x.RecipeFieldOrder);
            DisabledRecipeFieldIdNumbers = _recipeFields.Where(r => r.IsActive == false).Select(r => r.id);
            PredefinedRecipeFields = new List<KeyValuePair<string, string>>();
            PredefinedRecipeFields = GetPredefinedRecipeFields();
            RecipeFieldIdNumbers = new List<short>();
            RecipeDetailValuesFromDb = new List<List<string>>();

            // todo:h Implement language preference in a parametric way, currently I'm forcing to using English :/
            if (ApplicationLanguageSettings != null)
                ActiveRecipeLanguageSettings = ApplicationLanguageSettings.Eng.ActiveRecipe;
        }

        private List<KeyValuePair<string, string>> GetPredefinedRecipeFields()
        {
            List<KeyValuePair<string, string>> predefinedRecipeFields = new List<KeyValuePair<string, string>>();

            if (!_recipeFields.Any())
                return predefinedRecipeFields;

            var values = _predefinedRecipeFieldService.GetAll();
            TwoOffsetFieldIdNumbers = _recipeFields.Where(r => r.IsMultipleCell).Select(r => r.id).ToArray();

            foreach (var field in values)
            {
                var headerFieldName = _recipeFields.Where(r => r.id == field.RecipeFieldId)
                                                   .Select(r => r.RecipeFieldName).FirstOrDefault();

                if (!string.IsNullOrEmpty(headerFieldName))
                    predefinedRecipeFields.Add(new KeyValuePair<string, string>(headerFieldName, field.RecipeFieldValue));
            }

            return predefinedRecipeFields;
        }

        private void InitializePageTagConfigurations()
        {
            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
            var pageTagConfiguration = pageTagConfigurationService.GetByName("Recipe");
            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
            RecipeTagConfigurations = JsonConvert.DeserializeObject<RecipeTagConfigurations>(jsonSerializedString);

            _activeBatchSegmentNo = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[RecipeTagConfigurations.ActiveBatchSegmentNo];

            SetRecipeDatablock(true);
        }

        public bool SetRecipeDatablock(bool value)
        {
            if (RecipeTagConfigurations != null)
                return ProcessManager.Instance.ChangeDemandReadStateOnCache(PlcDeviceId, RecipeTagConfigurations.Dbnumber, value);

            return false;
        }

        public void ContinuousUpdate()
        {
            PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
            string segNoString = plcCommandManager.Get<string>(_activeBatchSegmentNo, false);
            IsProcessRunning = ProcessManager.Instance.IsBatchRunning();

            if (!string.IsNullOrEmpty(segNoString))
            {
                _segNo = Convert.ToInt32(segNoString);
                RecipeView.MakeColumnActive(_segNo);
            }
        }

        public async void GetDataFromPlc()
        {
            await GetDataFromPlcAsync();
        }
        
        private async Task GetDataFromPlcAsync()
        {
            bool anyNullValue = false;

            await Task.Run(() =>
            {
                _recipeDetailValues = new List<string>();
                _plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);

                for (int i = 0; i < RecipeTagConfigurations.BufferSizeofRecipe; i += RecipeTagConfigurations.Length)
                {
                    SiemensTagConfiguration siemensTagConfiguration = new SiemensTagConfiguration
                    {
                        DBNumber = (short)RecipeTagConfigurations.Dbnumber,
                        DataType = "string[8]",
                        PlcId = PlcDeviceId,
                        Offset = i,
                    };

                    string stringFromReadCache = _plcCommandManager.Get<string>(siemensTagConfiguration, false);

                    if (stringFromReadCache == null)
                        anyNullValue = true;

                    _recipeDetailValues.Add(stringFromReadCache);
                }

                // Get recipe id from cache
                if (!ProcessManager.Instance.CurrentProcess.IsRecipeLoaded)
                {
                    WaitIndicatorControl.IsWaitIndicatorVisible = false;
                    return;
                }

                // Get total number of segments
                int activeRecipeId = ProcessManager.Instance.CurrentProcess.ActiveRecipeId;

                _totalSegments = _recipeDetailService.GetSegmentTotal(activeRecipeId);

                if (_totalSegments == 0)
                {
                    WaitIndicatorControl.IsWaitIndicatorVisible = false;
                    return;
                }

                OrganizeActiveRecipeDataForUI();
                RecipeDetailValuesFromDb = GetLoadedRecipeValueFromDb(activeRecipeId);
            });

            if (RecipeView != null && !anyNullValue)
                RecipeView.StartLoading();

            if (anyNullValue)
                RecipeView.InitialLoading = false;
            else
                RecipeView.InitialLoading = true;
        }

        /// <summary>
        /// Get loaded recipe values from db to compare it with PLC
        /// </summary>
        /// <param name="activeRecipeId"></param>
        /// <returns></returns>
        private List<List<string>> GetLoadedRecipeValueFromDb(int activeRecipeId)
        {
            List<List<string>> recipeDetailValues = new List<List<string>>();
            var selectedDetailValues = _recipeDetailService.GetAllByRecipeId(activeRecipeId).ToList();

            for (int i = 1; i <= _totalSegments; i++)
            {
                List<string> curSegValues = new List<string>();
                foreach (short number in _recipeFields.Select(r => r.id).ToList())
                {
                    RecipeDetail detail = selectedDetailValues.Where(r => r.SegmentNo == i && r.RecipeFieldId == number).FirstOrDefault();
                    if(detail != null)
                        curSegValues.Add(detail.RecipeFieldValue);
                }
                recipeDetailValues.Add(curSegValues);
            }

            return recipeDetailValues;
        }

        /// <summary>
        /// Organizes recipe data for our using purposes on WPF UI.
        /// On different platform, a new logic implementation may be needed.
        /// </summary>
        public void OrganizeActiveRecipeDataForUI()
        {
            SegAndTableLists = new List<List<string>>(new List<string>[_totalSegments + 1]); // Add one extra for recipe fields

            for (int i = 0; i < SegAndTableLists.Count; i++)
            {
                SegAndTableLists[i] = new List<string>();
            }

            foreach (var fieldItem in _recipeFields)
            {
                SegAndTableLists[0].Add(fieldItem.RecipeFieldName);
                RecipeFieldIdNumbers.Add(fieldItem.id);
            }

            TotalRecipeRows = _recipeFields.Count();

            int skippedIndexes = 0;
            int collIndex = 1;
            string SegTime = string.Empty;
            string twoOffsetVal = string.Empty;
            int lengthModified = 100;

            for (int j = 0; j < _totalSegments * lengthModified; j += lengthModified)
            {
                for (int k = 0; k < TotalRecipeRows; k++)
                {
                    short fieldIdVal = RecipeFieldIdNumbers[k];
                    int index = k + j + skippedIndexes;

                    if(TwoOffsetFieldIdNumbers.Contains(fieldIdVal))
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            twoOffsetVal += (_recipeDetailValues[index + i] == null) ? string.Empty : _recipeDetailValues[index + i];
                            if (i == 1)
                            {
                                SegAndTableLists[collIndex].Add(twoOffsetVal);
                                skippedIndexes++;
                            }
                        }
                    }
                    else
                    {
                        if (index < (_totalSegments * lengthModified))
                        {
                            string recipeVal = (_recipeDetailValues[index] == null) ? string.Empty : _recipeDetailValues[index];
                            SegAndTableLists[collIndex].Add(recipeVal);
                        }
                    }

                    // Reset twoOffsetVal value
                    twoOffsetVal = string.Empty;
                }
                if (collIndex < 30)
                    collIndex++;

                skippedIndexes = 0;
            }
        }
    }
}
