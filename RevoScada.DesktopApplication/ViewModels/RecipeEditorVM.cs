using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Xpf.WindowsUI;
using DevExpress.XtraReports.UI;
using Newtonsoft.Json;
using Revo.Core.Data;
using RevoScada.ProcessController;
using RevoScada.Synchronization;
using RevoScada.Synchronization.Enums;
using RevoScada.Synchronization.Types;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.Reports;
using RevoScada.DesktopApplication.Views;
using RevoScada.DesktopApplication.Views.Popups;
using RevoScada.DesktopApplication.Views.ReportTemplates;
using RevoScada.Entities;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.Configuration.Service;
using RevoScada.Entities.Enums;
using RevoScada.Entities.PageTagConfigurations;

namespace RevoScada.DesktopApplication.ViewModels
{
    public class RecipeEditorVM : UserControlBaseVM
    {
        #region Services
        private RecipeGroupService _recipeGroupService;
        private RecipeService _recipeService;
        private BatchService _batchService;
        private RecipeFieldService _recipeFieldService;
        private RecipeDetailService _recipeDetailService;
        private PredefinedRecipeFieldService _predefinedRecipeFieldService;
        private ProcessEventLogService _processEventLogService;
        private UserService _userService;
        #endregion

        #region Fields
        private readonly string _connectionString;
        public RecipeEditorTagConfigurations RecipeEditorTagConfigurations;
        public bool IsInitiallyLoaded;
        private Leakage_Test_Failure_Criteria _leakageTestFailureriteriaView;  
        private RecipeActivation _recipeActivationView;
        private SiemensTagConfiguration _leakageTestFailureCriteriaTest;
        private SiemensTagConfiguration _leakageTestFailureCriteriaSetTime;
        #endregion

        #region Properties
        private RecipeInfo _recipeInfo;
        public RecipeInfo RecipeInfo
        {
            get => _recipeInfo;
            set => OnPropertyChanged(ref _recipeInfo, value);
        }
        private Entities.Recipe _selectedRecipe;
        public Entities.Recipe SelectedRecipe
        {
            get => _selectedRecipe;
            set
            {
                _selectedRecipe = value;
                UpdateRecipeInfo();
            }
        }
        private int _selectedRecipeId;
        public int SelectedRecipeId
        {
            get => _selectedRecipeId;
            set
            {
                _selectedRecipeId = value;
                RecipeDetailsColl = _recipeDetailService.GetAllByRecipeId(_selectedRecipeId).ToList();
                SelectedRecipe = _recipeService.GetById(_selectedRecipeId);
                TableAndSegList = GetTableAndSegmentData();
            }
        }
        private short _selectedGroupId;
        public short SelectedGroupId
        {
            get => _selectedGroupId;
            set
            {
                _selectedGroupId = value;
            }
        }


        private string _activeRecipeName;
        public string ActiveRecipeName
        {
            get => _activeRecipeName;
            set
            {
                OnPropertyChanged(ref _activeRecipeName, value);
                AbbreviatedActiveRecipeName = value;
            }
        }

        private string _fullActiveRecipeName;
        public string FullActiveRecipeName
        {
            get => _fullActiveRecipeName;
            set => OnPropertyChanged(ref _fullActiveRecipeName, value);
        }

        private string _abbreviatedActiveRecipeName;
        public string AbbreviatedActiveRecipeName
        {
            get => _abbreviatedActiveRecipeName;
            set
            {
                _abbreviatedActiveRecipeName = value;

                if (_abbreviatedActiveRecipeName?.Length > 42)
                {
                    var fixedVal = _abbreviatedActiveRecipeName.Substring(0, 42) + "...";
                    _abbreviatedActiveRecipeName = fixedVal;
                    FullActiveRecipeName = value;
                }
                else
                {
                    FullActiveRecipeName = string.Empty;
                }

                OnPropertyChanged(ref _abbreviatedActiveRecipeName, _abbreviatedActiveRecipeName);
            }
        }

        private string _selectedFullRecipeName;
        public string SelectedFullRecipeName
        {
            get => _selectedFullRecipeName;
            set => OnPropertyChanged(ref _selectedFullRecipeName, value);
        }

        private string _selectedAbbreviatedActiveRecipeName;
        public string SelectedAbbreviatedActiveRecipeName
        {
            get => _selectedAbbreviatedActiveRecipeName;
            set
            {
                _selectedAbbreviatedActiveRecipeName = value;

                if (_selectedAbbreviatedActiveRecipeName?.Length > 36)
                {
                    var fixedVal = _selectedAbbreviatedActiveRecipeName.Substring(0, 33) + "...";
                    _selectedAbbreviatedActiveRecipeName = fixedVal;
                    SelectedFullRecipeName = value;
                }
                else
                {
                    SelectedFullRecipeName = string.Empty;
                }

                OnPropertyChanged(ref _selectedAbbreviatedActiveRecipeName, _selectedAbbreviatedActiveRecipeName);
            }
        }

        private string _selectedFullCreatedUserName;
        public string SelectedFullCreatedUserName
        {
            get => _selectedFullCreatedUserName;
            set => OnPropertyChanged(ref _selectedFullCreatedUserName, value);
        }

        private string _selectedAbbreviatedCreatedUserName;
        public string SelectedAbbreviatedCreatedUserName
        {
            get => _selectedAbbreviatedCreatedUserName;
            set
            {
                _selectedAbbreviatedCreatedUserName = value;

                if (_selectedAbbreviatedCreatedUserName?.Length > 36)
                {
                    var fixedVal = _selectedAbbreviatedCreatedUserName.Substring(0, 33) + "...";
                    _selectedAbbreviatedCreatedUserName = fixedVal;
                    SelectedFullCreatedUserName = value;
                }
                else
                {
                    SelectedFullCreatedUserName = string.Empty;
                }

                OnPropertyChanged(ref _selectedAbbreviatedCreatedUserName, _selectedAbbreviatedCreatedUserName);
            }
        }
        public Entities.Recipe LoadedRecipe { get; set; }
        public int TotalRecipeRows { get; set; }
        public Recipe_Editor Recipe_Editor_View { get; set; }
        private bool _isDescriptionReadOnly;
        public bool IsDescriptionReadOnly
        {
            get => _isDescriptionReadOnly;
            set => OnPropertyChanged(ref _isDescriptionReadOnly, value);
        }
        private bool _isRecipePasteButtonEnabled;
        public bool IsRecipePasteButtonEnabled
        {
            get => _isRecipePasteButtonEnabled;
            set => OnPropertyChanged(ref _isRecipePasteButtonEnabled, value);
        }
        private Visibility _groupDeleteButtonVisibility;
        public Visibility GroupDeleteButtonVisibility
        {
            get => _groupDeleteButtonVisibility;
            set => OnPropertyChanged(ref _groupDeleteButtonVisibility, value);
        }
        private Visibility _groupEditButtonVisibility;
        public Visibility GroupEditButtonVisibility
        {
            get => _groupEditButtonVisibility;
            set => OnPropertyChanged(ref _groupEditButtonVisibility, value);
        }

        private bool _recipeActiveValue;
        public bool RecipeActiveValue
        {
            get => _recipeActiveValue;
            set => OnPropertyChanged(ref _recipeActiveValue, value);
        }

        private bool _recipeDeactiveValue;
        public bool RecipeDeactiveValue
        {
            get => _recipeDeactiveValue;
            set => OnPropertyChanged(ref _recipeDeactiveValue, value);
        }

        public ValueWrapper<bool> IsRecipeTableDataChanged { get; set; }
        public ValueWrapper<string> CurrentRecipeName { get; set; }
        public Dictionary<string, string> RecipeEditorLanguageSettings { get; set; }
        #endregion

        #region Commands
        public ICommand SendToPlcCommand { get; set; }
        public ICommand ReportViewCommand { get; set; }
        public ICommand GoToActiveRecipeCommand { get; set; }
        public ICommand NewGroupCommand { get; set; }
        public ICommand NewRecipeCommand { get; set; }
        public ICommand CopyRecipeCommand { get; set; }
        public ICommand PasteRecipeCommand { get; set; }
        public ICommand DeleteRecipeCommand { get; set; }
        public ICommand DeleteRecipeGroupCommand { get; set; }
        public ICommand RenameRecipeCommand { get; set; }
        public ICommand RenameRecipeGroupCommand { get; set; }
        public ICommand SaveChangesCommand { get; set; }
        public ICommand IsActiveRecipeBtnCommand { get; set; }
        public ICommand IntegrityCheckCommand { get; set; }
        #endregion

        #region Collections
        public IEnumerable<RecipeGroup> RecipeGroupsColl { get; set; }
        public IEnumerable<Entities.Recipe> RecipesColl { get; set; }
        public List<RecipeField> RecipeFieldsColl;
        public List<short> RecipeFieldIdNumbers;
        public List<RecipeDetail> RecipeDetailsColl;
        public List<List<TableSegmentDataGrid>> TableAndSegList;
        public List<KeyValuePair<string, string>> PredefinedRecipeFields;
        public short[] TwoOffsetFieldIdNumbers;
        #endregion

        public RecipeEditorVM(WaitIndicatorControl waitIndicatorControl, ValueWrapper<bool> isRecipeTableDataChanged, UserGridModel activeUser,
                              Dictionary<string, bool> permissions, ValueWrapper<string> currentRecipeName)
        {
            _connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;
            _recipeGroupService = new RecipeGroupService(_connectionString);
            _recipeService = new RecipeService(_connectionString);
            _recipeFieldService = new RecipeFieldService(_connectionString);
            _recipeDetailService = new RecipeDetailService(_connectionString);
            _batchService = new BatchService(_connectionString);
            _predefinedRecipeFieldService = new PredefinedRecipeFieldService(_connectionString);
            _processEventLogService = new ProcessEventLogService(_connectionString);
            _userService = new UserService(_connectionString);
            WaitIndicatorControl = waitIndicatorControl;
            WaitIndicatorControl.IsWaitIndicatorVisible = false;
            IsRecipeTableDataChanged = isRecipeTableDataChanged;
            ActiveUser = activeUser;
            Permissions = permissions;
            CurrentRecipeName = currentRecipeName;

            IsDescriptionReadOnly = true;
            GroupDeleteButtonVisibility = Visibility.Collapsed;
            GroupEditButtonVisibility = Visibility.Collapsed;

            // todo:h Implement language preference in a parametric way, currently I'm forcing to using English :/
            if (ApplicationLanguageSettings != null)
                RecipeEditorLanguageSettings = ApplicationLanguageSettings.Eng.RecipeEditor;

            NewGroupCommand = new RelayCommand(AddNewGroupOnCommand);
            NewRecipeCommand = new RelayCommand(AddNewRecipeOnCommand);
            CopyRecipeCommand = new RelayCommand(CopyRecipeOnCommand);
            CopyRecipeCommand = new RelayCommand(CopyRecipeOnCommand);
            PasteRecipeCommand = new RelayCommand(PasteRecipeOnCommand);
            DeleteRecipeCommand = new RelayCommand(DeleteRecipeOnCommand);
            DeleteRecipeGroupCommand = new RelayCommand(DeleteRecipeGroupOnCommand);
            RenameRecipeCommand = new RelayCommand(RenameRecipeOnCommand);
            RenameRecipeGroupCommand = new RelayCommand(RenameRecipeGroupOnCommand);
            SaveChangesCommand = new RelayCommand(SaveChangesOnCommand);
            IsActiveRecipeBtnCommand = new RelayCommand(IsActiveRecipeBtnOnCommand);
            IntegrityCheckCommand = new RelayCommand(IntegrityCheckOnCommand);

            SendToPlcCommand = new RelayCommand(SendToPlc);
            ReportViewCommand = new RelayCommand(ReportView);
            GoToActiveRecipeCommand = new RelayCommand(GoToActiveRecipe);

            TableAndSegList = new List<List<TableSegmentDataGrid>>();
            RecipeInfo = new RecipeInfo();
            RecipeDetailsColl = new List<RecipeDetail>();
            PredefinedRecipeFields = new List<KeyValuePair<string, string>>();
            RecipeFieldIdNumbers = new List<short>();

            SelectedRecipe = new Entities.Recipe();
        }

        private void InitializePageTagConfigurations()
        {
            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);

            var pageTagConfiguration = pageTagConfigurationService.GetByName("RecipeEditor");
            var jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
            RecipeEditorTagConfigurations = JsonConvert.DeserializeObject<RecipeEditorTagConfigurations>(jsonSerializedString);

            _leakageTestFailureCriteriaTest = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[RecipeEditorTagConfigurations.LeakageTestFailureCriteriaTestValue];
            _leakageTestFailureCriteriaSetTime = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[RecipeEditorTagConfigurations.LeakageTestFailureSetTime];

            // Set active datablock which is related with recipes
            SetRecipeDatablock(true);
        }

        public void InitialLoad()
        {
            IsInitiallyLoaded = true;

            InitializePageTagConfigurations();
            
            RecipeGroupsColl = _recipeGroupService.GetAll().Where(r => r.IsActive).OrderBy(r => r.GroupName); // Get activated recipe groups
            RecipesColl = _recipeService.GetAll().Where(r => r.IsActive == true).OrderBy(x => x.id).ToList(); // Get activated recipes
            RecipeFieldsColl = _recipeFieldService.GetAll().OrderBy(x => x.RecipeFieldOrder).ToList();
            TwoOffsetFieldIdNumbers = RecipeFieldsColl.Where(r => r.IsMultipleCell).Select(r => r.id).ToArray();
            PredefinedRecipeFields = GetPredefinedRecipeFieldsByHeaders();

            LoadedRecipe = new Entities.Recipe();
            LoadedRecipe.id = ProcessManager.Instance.CurrentProcess.ActiveRecipeId;
            ActiveRecipeName = ProcessManager.Instance.CurrentProcess.ActiveRecipeName;
        }

        public bool SetRecipeDatablock(bool value)
        {
            if(RecipeEditorTagConfigurations != null)
                return ProcessManager.Instance.ChangeDemandReadStateOnCache(PlcDeviceId, RecipeEditorTagConfigurations.DbNumber, value);

            return false;
        }

        private List<KeyValuePair<string, string>> GetPredefinedRecipeFieldsByHeaders()
        {
            List<KeyValuePair<string, string>> predefinedRecipeFields = new List<KeyValuePair<string, string>>();
            var values = _predefinedRecipeFieldService.GetAll();
            var recipeFields = _recipeFieldService.GetAll();

            foreach (var field in values)
            {
                var headerFieldName = recipeFields.Where(r => r.id == field.RecipeFieldId)
                                                  .Select(r => r.RecipeFieldName).FirstOrDefault();

                if (!string.IsNullOrEmpty(headerFieldName))
                    predefinedRecipeFields.Add(new KeyValuePair<string, string>(headerFieldName, field.RecipeFieldValue));
            }

            return predefinedRecipeFields;
        }

        /// <summary>
        /// Adds selected Recipe Info data from the recmas table from the database.
        /// </summary>
        public void UpdateRecipeInfo()
        {
            RecipeInfo = new RecipeInfo();
            SelectedAbbreviatedActiveRecipeName = SelectedRecipe.RecipeName;
            RecipeInfo.RecipeName = SelectedAbbreviatedActiveRecipeName;
            RecipeInfo.Description = SelectedRecipe.Description;
            RecipeInfo.Specification = SelectedRecipe.Specification;
            RecipeInfo.RevisedDate = (SelectedRecipe.CreateDate > DateTime.MinValue) ? SelectedRecipe.CreateDate : (DateTime?)null;
            RecipeInfo.ModifiedDate = (SelectedRecipe.ModifyDate > DateTime.MinValue) ? SelectedRecipe.ModifyDate : (DateTime?)null;
            RecipeInfo.LastRunDate = (SelectedRecipe.LastRunDate > DateTime.MinValue) ? SelectedRecipe.LastRunDate : (DateTime?)null;

            if (SelectedRecipe.CreatedByUserId == 0)
                return;

            string selectedUserName = _userService.GetById(SelectedRecipe.CreatedByUserId)?.UserName ?? "";
            SelectedAbbreviatedCreatedUserName = selectedUserName;
            RecipeInfo.CreatedByUser = SelectedAbbreviatedCreatedUserName;
        }

        private RecipeDetail SynchroniseRecipeDetails(short fieldId, short segNo, int recipeId)
        {
            var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;

            SyncIssueManager syncIssueManager = new SyncIssueManager(ApplicationConfigurations.Instance.Configuration.RedisServer);

            RecipeDetail newRecipeRow = new RecipeDetail();
            newRecipeRow.RecipeFieldId = fieldId;
            newRecipeRow.SegmentNo = segNo;
            newRecipeRow.RecipeFieldValue = string.Empty;
            newRecipeRow.RecipeId = recipeId;

            _recipeDetailService.Insert(newRecipeRow);

            string serializedEntityObject = JsonConvert.SerializeObject(newRecipeRow);

            SyncIssue syncIssue = new SyncIssue
            {
                SerializedEntityObject = serializedEntityObject,
                EntityObjectType = typeof(RecipeDetail),
                SyncDBCommand = SyncDBCommand.Insert,
                FromToDirection = fromToDirection,
                PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                SyncStatus = SyncStatus.NoneProcessChangesPending,
                TransferType = TransferType.NonProcessChanges,
            };

            syncIssueManager.CreateNewSyncIssue(syncIssue);

            return newRecipeRow;
        }

        private List<List<TableSegmentDataGrid>> GetTableAndSegmentData()
        {
            if (RecipeFieldsColl == null || RecipeFieldsColl.Count == 0)
                return new List<List<TableSegmentDataGrid>>();

            List<List<TableSegmentDataGrid>> TableAndSegmentList = new List<List<TableSegmentDataGrid>>();
            // Add table names first
            List<TableSegmentDataGrid> tableValList = new List<TableSegmentDataGrid>();
            // Assign total number of rows
            TotalRecipeRows = RecipeFieldsColl.Count;
            foreach (var item in RecipeFieldsColl)
            {
                tableValList.Add(new TableSegmentDataGrid()
                {
                    RecipeDetail = null,
                    RecipeFieldValue = item.RecipeFieldName,
                    RecipeFieldId = item.id,
                    RecipeFieldOrderId = item.RecipeFieldOrder,
                    RecipeFieldDisplayColor = item.DisplayColor,
                    IsMultipleCell = item.IsMultipleCell,
                    IsActive = item.IsActive
                });
            }

            TableAndSegmentList.Add(tableValList);

            // If selected recipe details collection is empty return
            if (RecipeDetailsColl == null || RecipeDetailsColl.Count == 0)
                return TableAndSegmentList;

            short totalSegNum = RecipeDetailsColl.Max(r => r.SegmentNo);

            for (short i = 1; i <= totalSegNum; i++)
            {
                List<RecipeDetail> curSegDetails = RecipeDetailsColl.Where(r => r.SegmentNo == i).OrderBy(r => r.id).ToList();
                var allFieldIds = RecipeFieldsColl.Select(r => r.id);

                foreach (var fieldNo in allFieldIds)
                {
                    if(!curSegDetails.Where(c => c.RecipeFieldId == fieldNo).Any())
                    {
                        RecipeDetail newDetail = SynchroniseRecipeDetails(fieldNo, i, RecipeDetailsColl[1].RecipeId);
                        if (newDetail != null)
                            curSegDetails.Add(newDetail);
                    }
                }

                List<TableSegmentDataGrid> curTableAndSegList = new List<TableSegmentDataGrid>();

                foreach (var item in curSegDetails)
                {
                    curTableAndSegList.Add(new TableSegmentDataGrid() { RecipeDetail = item, RecipeFieldValue = "" });
                }

                if (curSegDetails.Count() == 0)
                {
                    for (short j = 1; j <= TotalRecipeRows; j++)
                    {
                        curTableAndSegList.Add(new TableSegmentDataGrid()
                        {
                            RecipeDetail = new RecipeDetail()
                            {
                                RecipeId = _selectedRecipeId,
                                RecipeFieldValue = "",
                                SegmentNo = i,
                            },
                        });
                    }
                }

                TableAndSegmentList.Add(curTableAndSegList);
            }

            return TableAndSegmentList;
        }

        public bool UpdateRecipeDetailsTable(int recipeId, List<List<TableSegmentDataGrid>> tableAndSegList, bool isSegmentRemoving = false)
        {
            bool processResult = true;
            bool recipeGetResult = false;
            TableAndSegList = tableAndSegList;
            List<RecipeDetail> recipeDetails = new List<RecipeDetail>();
            int totalSegments = tableAndSegList.Count - 1;

            var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
            SyncIssueManager syncIssueManager = new SyncIssueManager(ApplicationConfigurations.Instance.Configuration.RedisServer);

            foreach (var item in TableAndSegList[0])
            {
                short fieldId = item.RecipeFieldId;
                short fieldIdIndex = (short)(item.RecipeFieldId - 1);

                for (short i = 1; i <= totalSegments; i++)
                {
                    foreach (var value in TableAndSegList[i])
                    {
                        if (value.RecipeDetail.RecipeFieldId == fieldId || isSegmentRemoving)
                        {
                            RecipeDetail newRecipeRow = new RecipeDetail();

                            if (isSegmentRemoving)
                                newRecipeRow.RecipeFieldId = value.RecipeDetail.RecipeFieldId;
                            else
                                newRecipeRow.RecipeFieldId = item.RecipeFieldId;

                            newRecipeRow.SegmentNo = value.RecipeDetail.SegmentNo;
                            newRecipeRow.RecipeFieldValue = value.RecipeDetail.RecipeFieldValue;
                            newRecipeRow.RecipeId = value.RecipeDetail.RecipeId;

                            switch (value.CellChangeState)
                            {
                                case CellChangeStates.Modified:
                                    newRecipeRow.id = value.RecipeDetail.id;
                                    processResult = _recipeDetailService.Update(newRecipeRow);

                                    if (processResult)
                                    {
                                        // Make sure not to update same detail again
                                        value.CellChangeState = CellChangeStates.Unchanged;

                                        string serializedEntityObject = JsonConvert.SerializeObject(newRecipeRow);
                                        SyncIssue syncIssue = new SyncIssue
                                        {
                                            SerializedEntityObject = serializedEntityObject,
                                            EntityObjectType = typeof(RecipeDetail),
                                            SyncDBCommand = SyncDBCommand.Update,
                                            FromToDirection = fromToDirection,
                                            PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                                            SyncStatus = SyncStatus.NoneProcessChangesPending,
                                            TransferType = TransferType.NonProcessChanges,
                                        };

                                        syncIssueManager.CreateNewSyncIssue(syncIssue);
                                    }

                                    break;
                                case CellChangeStates.Added:
                                    bool isCurFieldAlreadyExists = false;
                                    if (RecipeDetailsColl != null && RecipeDetailsColl.Count > 0)
                                    {
                                        var selectedDetails = RecipeDetailsColl.Where(r => r.SegmentNo == i);
                                        if (selectedDetails != null && selectedDetails.Any())
                                            isCurFieldAlreadyExists = selectedDetails.Where(r => r.RecipeFieldId == newRecipeRow.RecipeFieldId).Any();
                                    }
                                    // Check if desired field number already exists on current segment.
                                    if (!isCurFieldAlreadyExists)
                                    {
                                        processResult = _recipeDetailService.Insert(newRecipeRow);
                                        if (processResult)
                                        {
                                            string serializedEntityObject = JsonConvert.SerializeObject(newRecipeRow);

                                            SyncIssue syncIssue = new SyncIssue
                                            {
                                                SerializedEntityObject = serializedEntityObject,
                                                EntityObjectType = typeof(RecipeDetail),
                                                SyncDBCommand = SyncDBCommand.Insert,
                                                FromToDirection = fromToDirection,
                                                PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                                                SyncStatus = SyncStatus.NoneProcessChangesPending,
                                                TransferType = TransferType.NonProcessChanges,
                                            };

                                            syncIssueManager.CreateNewSyncIssue(syncIssue);
                                        }

                                    }
                                    break;
                                case CellChangeStates.Deleted:
                                    newRecipeRow.id = value.RecipeDetail.id;
                                    processResult = _recipeDetailService.Delete(newRecipeRow);

                                    if (processResult)
                                    {

                                        string serializedEntityObject = JsonConvert.SerializeObject(newRecipeRow);

                                        SyncIssue syncIssue = new SyncIssue
                                        {
                                            SerializedEntityObject = serializedEntityObject,
                                            EntityObjectType = typeof(RecipeDetail),
                                            SyncDBCommand = SyncDBCommand.Delete,
                                            FromToDirection = fromToDirection,
                                            PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                                            SyncStatus = SyncStatus.NoneProcessChangesPending,
                                            TransferType = TransferType.NonProcessChanges,
                                        };

                                        syncIssueManager.CreateNewSyncIssue(syncIssue);
                                    }

                                    break;
                                case CellChangeStates.Unchanged:
                                    // Do NOT update database
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }

            RecipeDetailsColl = _recipeDetailService.GetAllByRecipeId(recipeId).ToList();
            // Update the collection
            TableAndSegList = GetTableAndSegmentData();

            Entities.Recipe updateRecipe = _recipeService.GetById(recipeId);

            if (updateRecipe == null)
                return false;

            updateRecipe.ModifyDate = DateTime.Now;
            updateRecipe.ModifiedByUserId = ActiveUser.id;
            // Update description of recipe
            if (RecipeInfo != null)
            {
                updateRecipe.Description = RecipeInfo.Description;
                updateRecipe.Specification = RecipeInfo.Specification;
            }

            SelectedRecipe = updateRecipe;

            recipeGetResult = _recipeService.Update(updateRecipe);

            if (recipeGetResult)
            {
                string serializedEntityObject = JsonConvert.SerializeObject(updateRecipe);
                SyncIssue syncIssue = new SyncIssue
                {
                    SerializedEntityObject = serializedEntityObject,
                    EntityObjectType = typeof(Recipe),
                    SyncDBCommand = SyncDBCommand.Update,
                    FromToDirection = fromToDirection,
                    PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                    SyncStatus = SyncStatus.NoneProcessChangesPending,
                    TransferType = TransferType.NonProcessChanges,
                };

                syncIssueManager.CreateNewSyncIssue(syncIssue);
            }

            if (!processResult || !recipeGetResult)
                return false;

            return true;
        }

        public int AddNewRecipe()
        {
            Entities.Recipe newRecipe = new Entities.Recipe();
            newRecipe.IsActive = true;
            newRecipe.CreateDate = DateTime.Now;
            newRecipe.RecipeGroupId = SelectedGroupId;
            newRecipe.RecipeName = "NEW RECIPE";
            newRecipe.CreatedByUserId = ActiveUser.id;
            newRecipe.ModifiedByUserId = ActiveUser.id;
            newRecipe.IsValid = true;

            //Insert to the database
            var result = _recipeService.Insert(newRecipe);

            if (result)
            {
                var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;

                string serializedEntityObject = JsonConvert.SerializeObject(newRecipe);

                SyncIssue syncIssue = new SyncIssue
                {
                    SerializedEntityObject = serializedEntityObject,
                    EntityObjectType = typeof(Recipe),
                    SyncDBCommand = SyncDBCommand.Insert,
                    FromToDirection = fromToDirection,
                    PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                    SyncStatus = SyncStatus.NoneProcessChangesPending,
                    TransferType = TransferType.NonProcessChanges,
                };

                SyncIssueManager syncIssueManager = new SyncIssueManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
                syncIssueManager.CreateNewSyncIssue(syncIssue);
            }

            int lastAddedRecipeId = 0;

            //Update recipe collection
            if (result)
            {
                lastAddedRecipeId = _recipeService.GetAll().Max(r => r.id);
                RecipesColl = _recipeService.GetAll().Where(r => r.IsActive == true);
            }

            return lastAddedRecipeId;
        }

        public object[] CloneRecipe(int copiedRecipeId)
        {
            Entities.Recipe copiedRecipe = _recipeService.GetById(copiedRecipeId);

            if (copiedRecipe == null)
            {
                WinUIMessageBox.Show("Kopyalanan reçete kaydı bulunamadı.", "Reçete bulunamadı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return new object[0];
            }

            string copiedRecipeName = copiedRecipe.RecipeName;
            if (!copiedRecipeName.Contains("kopya"))
            {
                copiedRecipeName = copiedRecipe.RecipeName + " kopya ";
            }
            else
            {
                string[] splitVals = copiedRecipeName.Split(' ');
                copiedRecipeName = string.Empty;

                for (int i = 0; i < splitVals.Count() - 1; i++)
                {
                    copiedRecipeName += splitVals[i] + " ";
                }
            }

            // Get number of how many we have same recipes
            var alreadyCopiedRecipes = _recipeService.GetAll().Where(r => r.IsActive == true && r.RecipeName.StartsWith(copiedRecipeName));

            if (alreadyCopiedRecipes.Count() > 0)
                copiedRecipeName += alreadyCopiedRecipes.Count() + 1;
            else
                copiedRecipeName += "1";

            Entities.Recipe cloneRecipe = new Entities.Recipe();
            cloneRecipe.IsActive = true;
            cloneRecipe.CreateDate = copiedRecipe.CreateDate;
            cloneRecipe.RecipeGroupId = SelectedGroupId;
            cloneRecipe.RecipeName = copiedRecipeName;
            cloneRecipe.ModifyDate = copiedRecipe.ModifyDate;
            cloneRecipe.LastRunDate = copiedRecipe.LastRunDate;
            cloneRecipe.CreatedByUserId = ActiveUser.id;
            cloneRecipe.ModifiedByUserId = ActiveUser.id;
            cloneRecipe.IsValid = true;

            // Insert to the database
            bool checkIfInserted = _recipeService.Insert(cloneRecipe);

            var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
            SyncIssueManager syncIssueManager = new SyncIssueManager(ApplicationConfigurations.Instance.Configuration.RedisServer);


            if (checkIfInserted)
            {

                string serializedEntityObject = JsonConvert.SerializeObject(cloneRecipe);

                SyncIssue syncIssue = new SyncIssue
                {
                    SerializedEntityObject = serializedEntityObject,
                    EntityObjectType = typeof(Recipe),
                    SyncDBCommand = SyncDBCommand.Insert,
                    FromToDirection = fromToDirection,
                    PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                    SyncStatus = SyncStatus.NoneProcessChangesPending,
                    TransferType = TransferType.NonProcessChanges,
                };

                syncIssueManager.CreateNewSyncIssue(syncIssue);
            }



            // Copy details of recipe
            if (!checkIfInserted)
                return new object[0];
           int clonedRecipeId = _recipeService.GetAll().Max(r => r.id);
            foreach (RecipeDetail recipeDetail in RecipeDetailsColl)
            {
                recipeDetail.RecipeId = clonedRecipeId;
                _recipeDetailService.Insert(recipeDetail);

                //todo:m refactor insertmany olacak
                string serializedEntityObject = JsonConvert.SerializeObject(recipeDetail);

                SyncIssue syncIssue = new SyncIssue
                {
                    SerializedEntityObject = serializedEntityObject,
                    EntityObjectType = typeof(RecipeDetail),
                    SyncDBCommand = SyncDBCommand.Insert,
                    FromToDirection = fromToDirection,
                    PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                    SyncStatus = SyncStatus.NoneProcessChangesPending,
                    TransferType = TransferType.NonProcessChanges,
                };


                syncIssueManager.CreateNewSyncIssue(syncIssue);


            }

            // Update recipe collection
            RecipesColl = _recipeService.GetAll().Where(r => r.IsActive == true);

            object[] clonedRecipeParams = new object[] { clonedRecipeId, copiedRecipeName };

            return clonedRecipeParams;
        }

        public bool ChangeRecipeName(string newName, int recipeId)
        {
            Entities.Recipe recipe = _recipeService.GetById(recipeId);

            if (recipe == null)
                return false;

            recipe.RecipeName = newName;
            recipe.ModifyDate = DateTime.Now;
            recipe.ModifiedByUserId = ActiveUser.id;

            //Update database
            var result = _recipeService.Update(recipe);

            if (result)
            {
                var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;

                string serializedEntityObject = JsonConvert.SerializeObject(recipe);

                SyncIssue syncIssue = new SyncIssue
                {
                    SerializedEntityObject = serializedEntityObject,
                    EntityObjectType = typeof(Recipe),
                    SyncDBCommand = SyncDBCommand.Update,
                    FromToDirection = fromToDirection,
                    PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                    SyncStatus = SyncStatus.NoneProcessChangesPending,
                    TransferType = TransferType.NonProcessChanges,
                };

                SyncIssueManager syncIssueManager = new SyncIssueManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
                syncIssueManager.CreateNewSyncIssue(syncIssue);
            }

            if (!result)
                return false;

            //Update recipe collection
            RecipesColl = _recipeService.GetAll().Where(r => r.IsActive == true);

            return result;
        }

        public void DeleteRecipe(int recipeId)
        {
            Entities.Recipe recipe = _recipeService.GetById(recipeId);

            if (recipe == null)
                return;

            recipe.IsActive = false;
            recipe.DeletedByUserId = ActiveUser.id;

            //Set "isActive" to false from database
            _recipeService.Update(recipe);

             var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;

                string serializedEntityObject = JsonConvert.SerializeObject(recipe);

                SyncIssue syncIssue = new SyncIssue
                {
                    SerializedEntityObject = serializedEntityObject,
                    EntityObjectType = typeof(Recipe),
                    SyncDBCommand = SyncDBCommand.Update,
                    FromToDirection = fromToDirection,
                    PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                    SyncStatus = SyncStatus.NoneProcessChangesPending,
                    TransferType = TransferType.NonProcessChanges,
                };

                SyncIssueManager syncIssueManager = new SyncIssueManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
                syncIssueManager.CreateNewSyncIssue(syncIssue);
          

            //Update recipe collection
            RecipesColl = _recipeService.GetAll().Where(r => r.IsActive == true);
        }

        public void DeleteEmptyRecipeGroup()
        {
            Entities.RecipeGroup recipeGroup = _recipeGroupService.GetById(SelectedGroupId);

            if (recipeGroup == null)
                return;

            //Set "isActive" to false from database
            recipeGroup.IsActive = false;

            var result = _recipeGroupService.Update(recipeGroup);

            if (result)
            {
                var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;

                string serializedEntityObject = JsonConvert.SerializeObject(recipeGroup);

                SyncIssue syncIssue = new SyncIssue
                {
                    SerializedEntityObject = serializedEntityObject,
                    EntityObjectType = typeof(RecipeGroup),
                    SyncDBCommand = SyncDBCommand.Update,
                    FromToDirection = fromToDirection,
                    PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                    SyncStatus = SyncStatus.NoneProcessChangesPending,
                    TransferType = TransferType.NonProcessChanges,
                };

                SyncIssueManager syncIssueManager = new SyncIssueManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
                syncIssueManager.CreateNewSyncIssue(syncIssue);
            }

            // Get activated recipe groups
            RecipeGroupsColl = _recipeGroupService.GetAll().Where(r => r.IsActive).OrderBy(r => r.GroupName);
        }

        public int AddNewRecipeGroup()
        {
            RecipeGroup newRecipeGroup = new RecipeGroup();
            newRecipeGroup.GroupName = "NEW GROUP";
            newRecipeGroup.IsActive = true;

            //Insert to database
            var result = _recipeGroupService.Insert(newRecipeGroup);

            if (result)
            {
                var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;

                string serializedEntityObject = JsonConvert.SerializeObject(newRecipeGroup);

                SyncIssue syncIssue = new SyncIssue
                {
                    SerializedEntityObject = serializedEntityObject,
                    EntityObjectType = typeof(RecipeGroup),
                    SyncDBCommand = SyncDBCommand.Insert,
                    FromToDirection = fromToDirection,
                    PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                    SyncStatus = SyncStatus.NoneProcessChangesPending,
                    TransferType = TransferType.NonProcessChanges,
                };

                SyncIssueManager syncIssueManager = new SyncIssueManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
                syncIssueManager.CreateNewSyncIssue(syncIssue);
            }

            int lastAddedGroupId = 0;
            //Update recipe group collection
            if (result)
            {
                RecipeGroupsColl = _recipeGroupService.GetAll().Where(r => r.IsActive).OrderBy(r => r.GroupName); // Get activated recipe groups
                lastAddedGroupId = RecipeGroupsColl.Max(r => r.id);
            }

            return lastAddedGroupId;
        }

        public bool ChangeRecipeGroupName(string newName)
        {
            if (SelectedGroupId < 1)
                return false;

            RecipeGroup recipeGroup = _recipeGroupService.GetById(SelectedGroupId);

            if (recipeGroup == null)
                return false;

            recipeGroup.GroupName = newName;

            //Update database
            bool result = _recipeGroupService.Update(recipeGroup);

            if (result)
            {
                var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;

                string serializedEntityObject = JsonConvert.SerializeObject(recipeGroup);

                SyncIssue syncIssue = new SyncIssue
                {
                    SerializedEntityObject = serializedEntityObject,
                    EntityObjectType = typeof(RecipeGroup),
                    SyncDBCommand = SyncDBCommand.Update,
                    FromToDirection = fromToDirection,
                    PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                    SyncStatus = SyncStatus.NoneProcessChangesPending,
                    TransferType = TransferType.NonProcessChanges,
                };

                SyncIssueManager syncIssueManager = new SyncIssueManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
                syncIssueManager.CreateNewSyncIssue(syncIssue);
            }

            RecipeGroupsColl = _recipeGroupService.GetAll().Where(r => r.IsActive).OrderBy(r => r.GroupName); // Get activated recipe groups

            return result;
        }

        public async Task SaveRecipeDescription(string descriptionText)
        {
            WaitIndicatorControl.IsWaitIndicatorVisible = true;
            await Task.Run(() =>
            {
                Entities.Recipe selectedRecipe = _recipeService.GetById(SelectedRecipeId);

                if (selectedRecipe == null)
                    return;

                selectedRecipe.Description = descriptionText;
                _recipeService.Update(selectedRecipe);

                    var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;

                    string serializedEntityObject = JsonConvert.SerializeObject(selectedRecipe);

                    SyncIssue syncIssue = new SyncIssue
                    {
                        SerializedEntityObject = serializedEntityObject,
                        EntityObjectType = typeof(Recipe),
                        SyncDBCommand = SyncDBCommand.Update,
                        FromToDirection = fromToDirection,
                        PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                        SyncStatus = SyncStatus.NoneProcessChangesPending,
                        TransferType = TransferType.NonProcessChanges,
                    };

                    SyncIssueManager syncIssueManager = new SyncIssueManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
                    syncIssueManager.CreateNewSyncIssue(syncIssue);



            });
            WaitIndicatorControl.IsWaitIndicatorVisible = false;
        }

        public int GetLatestRecipeId()
        {
            int latestRecipeId = _recipeService.GetAll().Max(r => r.id);
            return latestRecipeId;
        }

        private async void SendToPlc()
        {
            // First, check if there are any unsaved values.
            Recipe_Editor_View.CheckIfAnyUnsavedTableValues();

            bool checkResult = Recipe_Editor_View.CheckIfRecipeValuesAreValid();

            if (!checkResult)
            {
                WinUIMessageBox.Show("Lütfen Segment Time ve Grace sütunlarında değer ve zaman dilimi formatında giriniz.", "Uyarı",
                                      MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            await SendToPlcAsync();
        }

        private async Task SendToPlcAsync()
        {
            // todo:l Remove this collection when program has been released.
            var values = new List<string>();

            bool isBatchRunning = ProcessManager.Instance.IsBatchRunning();

            // Check if process is running
            if (isBatchRunning)
            {
                WinUIMessageBox.Show("Reçete çalışır durumda! Yeni reçete yüklenemez.", "Process Çalışır Durumda", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            var sendToPlcResult = WinUIMessageBox.Show($"'{SelectedRecipe.RecipeName}' adlı reçeteyi yüklemek istediğinize emin misiniz?", "Activate Recipe",
                                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (sendToPlcResult == MessageBoxResult.No)
                return;

            WaitIndicatorControl.IsWaitIndicatorVisible = true;

            await Task.Run(() =>
            {
                // Load selected recipe
                LoadedRecipe = _recipeService.GetById(SelectedRecipeId);

                if (LoadedRecipe == null)
                    throw new Exception("Selected recipe not found!");

                ActiveRecipeName = LoadedRecipe.RecipeName;

                PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
                // plcCommandManager.ResetRecipe(_recipeEditorTagConfigurations.BufferSizeofRecipe, PlcDeviceId, _recipeEditorTagConfigurations.DbNumber);

                byte[] recipeBufferInitial = plcCommandManager.GetAllBuffer(PlcDeviceId, RecipeEditorTagConfigurations.DbNumber);
                short getBufferTryAmount = 50;

                do
                {
                    if (getBufferTryAmount <= 0)
                        break;

                    recipeBufferInitial = plcCommandManager.GetAllBuffer(PlcDeviceId, RecipeEditorTagConfigurations.DbNumber);
                    getBufferTryAmount--;

                } while (recipeBufferInitial != null);

                if(recipeBufferInitial == null)
                {
                    WinUIMessageBox.Show("Reçeteyi aktif etme işlemi başarısız oldu.", "Başarısız", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                byte[] recipeBuffer = plcCommandManager.ResetBuffer(recipeBufferInitial);

                int currSeg = 1;
                int currOffset = 0;
                int totalSegs = TableAndSegList.Count - 1; // Subtract 1 for the table names

                #region Recipe values
                for (short i = 1; i <= totalSegs; i++)
                {
                    foreach (var item in TableAndSegList[0])
                    {
                        short fieldId = item.RecipeFieldId;
                        short fieldIdIndex = (short)(item.RecipeFieldId - 1);
                        string recipeValueToBeSent = string.Empty;

                        foreach (var value in TableAndSegList[i])
                        {
                            if (value.RecipeDetail.RecipeFieldId == fieldId)
                            {
                                recipeValueToBeSent = value.RecipeDetail.RecipeFieldValue;
                                break;
                            }
                        }

                        if (TwoOffsetFieldIdNumbers.Contains(fieldId) && !string.IsNullOrEmpty(recipeValueToBeSent))
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                string[] segTimeVals = Regex.Matches(recipeValueToBeSent, @"[a-zA-Z]+|\d+")
                                                            .Cast<Match>()
                                                            .Select(m => m.Value)
                                                            .ToArray();

                                if (recipeValueToBeSent.Equals("X"))
                                {
                                    segTimeVals = new string[2] { "X", string.Empty };
                                }

                                SiemensTagConfiguration siemensTagConfiguration = new SiemensTagConfiguration
                                {
                                    DBNumber = RecipeEditorTagConfigurations.DbNumber,
                                    DataType = "string[8]",
                                    PlcId = PlcDeviceId,
                                    Offset = currOffset
                                };

                                plcCommandManager.SetToBuffer(siemensTagConfiguration, recipeBuffer, segTimeVals[j]);
                                values.Add(segTimeVals[j]);
                                currOffset += RecipeEditorTagConfigurations.Length;
                            }
                        }
                        else
                        {
                            if (TwoOffsetFieldIdNumbers.Contains(fieldId))
                                currOffset += RecipeEditorTagConfigurations.Length;

                            SiemensTagConfiguration siemensTagConfiguration = new SiemensTagConfiguration
                            {
                                DBNumber = RecipeEditorTagConfigurations.DbNumber,
                                DataType = "string[8]",
                                PlcId = PlcDeviceId,
                                Offset = currOffset
                            };

                            plcCommandManager.SetToBuffer(siemensTagConfiguration, recipeBuffer, recipeValueToBeSent);

                            values.Add(recipeValueToBeSent);
                            currOffset += RecipeEditorTagConfigurations.Length;
                        }
                    }

                    currSeg++;
                    currOffset = (currSeg - 1) * RecipeEditorTagConfigurations.IncrementAmount;
                }
                #endregion

                // Send recipe buffer
                plcCommandManager.SetBufferToPLC(recipeBuffer, PlcDeviceId, RecipeEditorTagConfigurations.DbNumber);

                // Set recipe name to PLC
                SiemensTagConfiguration recipeNameTagId = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[RecipeEditorTagConfigurations.RecipeName]);

                StringManipulation stringManipulation = new StringManipulation();
                string modifiedRecipeName = stringManipulation.GetSubString(ActiveRecipeName, 0, 18);
                modifiedRecipeName = stringManipulation.ReplaceTurkishAndSpecialChars(modifiedRecipeName);
                bool recipeNameSetResult = plcCommandManager.Set(recipeNameTagId, modifiedRecipeName);

                // Set requirement values to PLC
                plcCommandManager.Set(_leakageTestFailureCriteriaTest, LoadedRecipe.RequirementValue);
                plcCommandManager.Set(_leakageTestFailureCriteriaSetTime, LoadedRecipe.RequirementTime);

                CurrentRecipeName.Value = ActiveRecipeName;

                // Set recipe load ok to PLC
                SiemensTagConfiguration recipeLoadOk = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[RecipeEditorTagConfigurations.RecipeLoadOk]);
                bool recipeLoadOkSetResult = plcCommandManager.Set<bool>(recipeLoadOk, true);

                ProcessEventLog recipeEventLog = new ProcessEventLog();
                // Get currentBatch object from the cache
                if (ProcessManager.Instance.CurrentProcess != null)
                    recipeEventLog.BatchId = ProcessManager.Instance.CurrentProcess.BatchId;

                recipeEventLog.CreateDate = DateTime.Now;
                recipeEventLog.Type = ProcessEventLogType.Manual.ToString();
                recipeEventLog.ModifiedByUserId = ActiveUser.id;
                recipeEventLog.EventText = $"A user called '{ActiveUser.UserName}' sent a recipe to PLC which is called '{modifiedRecipeName}'.";
                _processEventLogService.Insert(recipeEventLog);


                var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
                ProcessEventLogAdapter processEventLogAdapter = new ProcessEventLogAdapter(ApplicationConfigurations.Instance.Configuration.RedisServer);
                processEventLogAdapter.CreateProcessEventLogSyncIssue(recipeEventLog, fromToDirection, ApplicationConfigurations.Instance.Configuration.PlcDevice.Id);
            });

            Recipe_Editor_View.LoadRecipeGroupsToTreeview(LoadedRecipe.id);
            SetCurrentBatchRecipeId();
            WaitIndicatorControl.IsWaitIndicatorVisible = false;
        }

        private async void ReportView()
        {
            await ReportViewAsync();
        }

        private async Task ReportViewAsync()
        {
            WaitIndicatorControl.IsWaitIndicatorVisible = true;

            int recipeDetailsCount = RecipeDetailsColl.Count;

            if (recipeDetailsCount == 0)
            {
                WaitIndicatorControl.IsWaitIndicatorVisible = false;
                WinUIMessageBox.Show("Reçeteye ait rapor bulunamadı.", "Rapor bulunamadı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ReportCreator reportCreator = new ReportCreator(_connectionString);
            XtraReport xtraReportItem = null;

            await Task.Run(() =>
            {
                xtraReportItem = reportCreator.RecipeReportByRecipe(SelectedRecipeId);
            });

            ReportViewer reportViewer = new ReportViewer(xtraReportItem);
            WaitIndicatorControl.IsWaitIndicatorVisible = false;
            reportViewer.ShowDialog();
        }

        private void SetCurrentBatchRecipeId()
        {
            // Get currentBatch object from the cache
            Batch activeBatch = new Batch();

            if (ProcessManager.Instance.CurrentProcess?.BatchId > 0)
            {
                activeBatch = _batchService.GetById(ProcessManager.Instance.CurrentProcess.BatchId);
            }

            Entities.Recipe recipe = new Entities.Recipe();
            recipe.RecipeName = ActiveRecipeName;
            recipe.id = SelectedRecipeId;
            recipe.Description = "---";


            ProcessManager.Instance.CurrentProcess.ActiveRecipeId = recipe.id;
            ProcessManager.Instance.CurrentProcess.ActiveRecipeName = recipe.RecipeName;
            ProcessManager.Instance.CurrentProcess.IsRecipeLoaded = true;

            ProcessManager.Instance.SynchronizeCurrentProcessInfo(false);


            // If there is an active batch, update its recipeId value
            if (activeBatch.id > 0)
            {
                activeBatch.RecipeId = LoadedRecipe.id;
                var result = _batchService.Update(activeBatch);
                if (result)
                {
                    var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;

                    string serializedEntityObject = JsonConvert.SerializeObject(activeBatch);

                    SyncIssue syncIssue = new SyncIssue
                    {
                        SerializedEntityObject = serializedEntityObject,
                        EntityObjectType = typeof(Batch),
                        SyncDBCommand = SyncDBCommand.Update,
                        FromToDirection = fromToDirection,
                        PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                        SyncStatus = SyncStatus.NoneProcessChangesPending,
                        TransferType = TransferType.NonProcessChanges,
                    };

                    SyncIssueManager syncIssueManager = new SyncIssueManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
                    syncIssueManager.CreateNewSyncIssue(syncIssue);
                }


            }
        }

        public void GoToActiveRecipe()
        {
            if (RecipeGroupsColl.Count() < 1 || RecipesColl.Count() < 1)
            {
                return;
            }

            Recipe_Editor_View.CheckIfAnyUnsavedTableValues();

            int loadedRecipeId = 0;

            if (LoadedRecipe != null)
            {
                loadedRecipeId = LoadedRecipe.id;
            }
            else
            {


                if (ProcessManager.Instance.CurrentProcess.IsRecipeLoaded)
                {
                    LoadedRecipe = new Entities.Recipe();
                    LoadedRecipe.id = ProcessManager.Instance.CurrentProcess.ActiveRecipeId;
                    LoadedRecipe.RecipeName = ProcessManager.Instance.CurrentProcess.ActiveRecipeName;
                }
            }

            if (LoadedRecipe.id == 0)
                return;

            Entities.Recipe loadedRecipe = _recipeService.GetById(loadedRecipeId);

            string recipeItemName = "recipeId_" + loadedRecipe.id;
            string recipeGroupItemName = "recipeGroupId_" + loadedRecipe.RecipeGroupId;

            Recipe_Editor_View.GetRecipeTreeViewItemByName(recipeGroupItemName, recipeItemName);
        }

        private void AddNewGroupOnCommand()
        {
            Recipe_Editor_View.AddNewGroupItem();
        }

        private void AddNewRecipeOnCommand()
        {
            Recipe_Editor_View.AddNewRecipeItem();
        }

        private void CopyRecipeOnCommand()
        {
            Recipe_Editor_View.CopyItem();
        }
        private void PasteRecipeOnCommand()
        {
            Recipe_Editor_View.PasteItem();
        }
        private void DeleteRecipeOnCommand(object param)
        {
            string tagVal = (string)param;
            if (tagVal == "disabled" || tagVal == "isValid")
                return;

            int recipeId = Convert.ToInt32(tagVal);
            Recipe_Editor_View.DeleteRecipeItem(recipeId);
        }
        private void DeleteRecipeGroupOnCommand()
        {
            Recipe_Editor_View.DeleteRecipeGroup();
        }
        private void RenameRecipeOnCommand(object param)
        {
            string tagVal = (string)param;
            if (tagVal == "disabled" || tagVal == "isValid")
                return;

            int recipeId = Convert.ToInt32(tagVal);
            Recipe_Editor_View.RenameRecipeItem(recipeId);
        }

        private void RenameRecipeGroupOnCommand()
        {
            Recipe_Editor_View.RenameGroupItem();
        }

        private async void SaveChangesOnCommand(object param)
        {
            string tagVal = (string)param;
            if (tagVal == "disabled" || tagVal == "isValid")
                return;

            int recipeId = Convert.ToInt32(tagVal);
            await Recipe_Editor_View.SaveRecipeChanges(recipeId);
        }

        private void IsActiveRecipeBtnOnCommand(object param)
        {
            Entities.Recipe selectedRecipe = _recipeService.GetById(SelectedRecipeId);
            if (selectedRecipe == null)
                selectedRecipe.IsActive = true;

            _recipeActivationView = new RecipeActivation(this, selectedRecipe.IsValid);
            _recipeActivationView.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            _recipeActivationView.ShowDialog();

        }



        private void IntegrityCheckOnCommand(object param)
        {
            string tagVal = (string)param;
            if (tagVal == "disabled" || tagVal == "isValid")
                return;

            _leakageTestFailureriteriaView = new Leakage_Test_Failure_Criteria(this, SelectedRecipe.RequirementValue, SelectedRecipe.RequirementTime);
            _leakageTestFailureriteriaView.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            _leakageTestFailureriteriaView.ShowDialog();
        }

        public void UpdateRecipeRequirementValues(short requirementValue, short requirementTime)
        {
            Entities.Recipe updateRecipe = _recipeService.GetById(SelectedRecipe.id);

            if (updateRecipe == null)
                return;

            updateRecipe.ModifyDate = DateTime.Now;
            updateRecipe.ModifiedByUserId = ActiveUser.id;
            // Update requirement values of recipe
            updateRecipe.RequirementValue = requirementValue;
            updateRecipe.RequirementTime = requirementTime;

            SelectedRecipe = updateRecipe;
            _recipeService.Update(updateRecipe);
        }

        public bool UpdateRecipeValidValues(bool validValue)
        {
            try
            {
                Entities.Recipe updateRecipe = _recipeService.GetById(SelectedRecipe.id);

                if (updateRecipe == null)
                    return false;

                updateRecipe.ModifyDate = DateTime.Now;
                updateRecipe.ModifiedByUserId = ActiveUser.id;
                updateRecipe.IsValid = validValue;

                SelectedRecipe = updateRecipe;
                _recipeService.Update(updateRecipe);
               // Recipe_Editor_View.LoadRecipeGroupsToTreeview();


            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}