using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Editors;
using DevExpress.XtraReports.UI;
using Newtonsoft.Json;
using Revo.Core;

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
using RevoScada.DesktopApplication.Views.ReportTemplates;
using RevoScada.Entities;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.Configuration.Service;
using RevoScada.Entities.Enums;
using RevoScada.Entities.PageTagConfigurations;

namespace RevoScada.DesktopApplication.ViewModels
{
    public class EnterPartsVM : UserControlBaseVM
    {
        #region Commands        
        public ICommand SetPortToggleCommand { get; set; }
        public ICommand BarBtnPreparePartsBackCommand { get; set; }
        public ICommand BarBtnCompletedBatchesBackCommand { get; set; }
        public ICommand BarBtnCompletedBatchesMoveToCurrentCommand { get; set; }
        public ICommand ViewReportCommand { get; set; }
        #endregion

        #region Services
        private ProcessEventLogService _processEventLogService;
        #endregion

        #region Members
        private EnterPartsTagConfigurations _enterPartsTagConfigurations;
        private string _connectionString;
        private ObservableCollection<Batch> _prepareParts;
        private EnterPartsSelectedBatchModel _enterPartsSelectedBatchModel;
        private ObservableCollection<Batch> _completedBatches;
        private EnterPartsUIElementStates _enterPartsUIElementStates;
        private Batch _currentBatch;
        private bool _isSelectAllPortsCbEnabled;
        public Enter_Parts EnterPartsView;
        private AppViewModel _appViewModel;
        #endregion

        #region Properties
        public Batch CurrentBatch
        {
            get
            {
                return _currentBatch;
            }
            set
            {
                OnPropertyChanged(ref _currentBatch, value);
            }
        }
        public ObservableCollection<Batch> PrepareParts
        {
            get
            {
                return _prepareParts;
            }
            set
            {
                OnPropertyChanged(ref _prepareParts, value);
            }
        }
        public EnterPartsSelectedBatchModel EnterPartsSelectedBatchModel
        {
            get
            {
                return _enterPartsSelectedBatchModel;
            }
            set
            {
                OnPropertyChanged(ref _enterPartsSelectedBatchModel, value);
                _enterPartsSelectedBatchModel.PtcLanguageValue = EnterPartsLanguageSettings["ptc"];
                _enterPartsSelectedBatchModel.VacLanguageValue = EnterPartsLanguageSettings["vac"];
                _enterPartsSelectedBatchModel.MonLanguageValue = EnterPartsLanguageSettings["mon"];

                if (value != null)
                {
                    int batchId = value.SelectedBatch?.id ?? 0;

                    if (batchId > 0)
                        IsViewReportBtnEnabled = true;
                    else
                        IsViewReportBtnEnabled = false;

                    if (batchId > 0 && !(CurrentBatch?.id ?? 0).Equals(batchId))
                    {
                        IsPrepareRemoveBtnEnabled = true;
                        IsMoveToCurBtnEnabled = true;
                        if (value.SelectedBatch.Status == BatchCurrentState.NotStarted)
                            IsNewBagBtnEnabled = true;
                        else
                            IsNewBagBtnEnabled = false;
                    }
                    else
                    {
                        IsPrepareRemoveBtnEnabled = false;
                        IsMoveToCurBtnEnabled = false;
                        IsNewBagBtnEnabled = false;
                        IsBagRemoveBtnEnabled = false;
                    }
                }
            }
        }
        public ObservableCollection<Batch> CompletedBatches
        {
            get
            {
                return _completedBatches;
            }
            set
            {
                OnPropertyChanged(ref _completedBatches, value);
            }
        }
        public EnterPartsUIElementStates EnterPartsUIElementStates
        {
            get
            {
                return _enterPartsUIElementStates;
            }
            set
            {
                OnPropertyChanged(ref _enterPartsUIElementStates, value);
            }
        }
        public bool IsSelectAllPortsCbEnabled
        {
            get => _isSelectAllPortsCbEnabled;
            set
            {
                OnPropertyChanged(ref _isSelectAllPortsCbEnabled, value);

                if (EnterPartsView != null)
                    EnterPartsView.SetSelectAllCheckBox(false);
            }
        }
        private bool _isPrepareRemoveBtnEnabled;
        public bool IsPrepareRemoveBtnEnabled
        {
            get => _isPrepareRemoveBtnEnabled;
            set => OnPropertyChanged(ref _isPrepareRemoveBtnEnabled, value);
        }
        private bool _isNewBagBtnEnabled;
        public bool IsNewBagBtnEnabled
        {
            get => _isNewBagBtnEnabled;
            set => OnPropertyChanged(ref _isNewBagBtnEnabled, value);
        }
        private bool _isBagRemoveBtnEnabled;
        public bool IsBagRemoveBtnEnabled
        {
            get => _isBagRemoveBtnEnabled;
            set => OnPropertyChanged(ref _isBagRemoveBtnEnabled, value);
        }
        private bool _isMoveToCurBtnEnabled;
        public bool IsMoveToCurBtnEnabled
        {
            get => _isMoveToCurBtnEnabled;
            set => OnPropertyChanged(ref _isMoveToCurBtnEnabled, value);
        }
        private bool _isViewReportBtnEnabled;
        public bool IsViewReportBtnEnabled
        {
            get => _isViewReportBtnEnabled;
            set => OnPropertyChanged(ref _isViewReportBtnEnabled, value);
        }
        public string DisplayMessage { get; set; }
        public ValueWrapper<string> CurrentBatchNumber { get; set; }
        public Dictionary<string, string> EnterPartsLanguageSettings { get; set; }

        private ValueWrapper<bool> _isHamburgerMenuExpanded;

        public ValueWrapper<bool> IsHamburgerMenuExpanded
        {
            get { return _isHamburgerMenuExpanded; }
            set 
            { 
                _isHamburgerMenuExpanded = value; 
            }
        }

        #endregion

        public EnterPartsVM(WaitIndicatorControl waitIndicatorControl, Dictionary<string, bool> permissions, UserGridModel _activeUser, ValueWrapper<string> currentBatchNumber, AppViewModel appViewModel, ValueWrapper<bool> isHamburgerMenuExpanded)
        {
            _connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;
            _processEventLogService = new ProcessEventLogService(_connectionString);
            waitIndicatorControl.IsWaitIndicatorVisible = false;
            InitializePageTagConfigurations();
            EnterPartsUIElementStates = EnterPartsUIElementStatesObject;
            CurrentBatch = new Batch();
            Permissions = permissions;
            ActiveUser = _activeUser;
            CurrentBatchNumber = currentBatchNumber;
            _appViewModel = appViewModel;
            IsHamburgerMenuExpanded = isHamburgerMenuExpanded;

            // todo:h Implement language preference in a parametric way, currently I'm forcing to using English :/
            if (ApplicationLanguageSettings != null)
                EnterPartsLanguageSettings = ApplicationLanguageSettings.Eng.EnterParts;

            #region Commands
            SetPortToggleCommand = new RelayCommand(SetPortToggleAsync);
            BarBtnPreparePartsBackCommand = new RelayCommand(PreparePartsBack);
            BarBtnCompletedBatchesBackCommand = new RelayCommand(CompletedBatchesBack);
            BarBtnCompletedBatchesMoveToCurrentCommand = new RelayCommand(CompletedBatchesBack);
            ViewReportCommand = new RelayCommand(ViewReport);
            #endregion
        }

        #region PageInitializations
        public void SaveEnterPartsUIElementStatesSettings()
        {
            try
            {

                EnterPartsUIElementStatesObject = _enterPartsUIElementStates;
                // Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log($"SaveEnterPartsUIElementStatesSettings Detail: {ex.Message}\n\n", LogType.Error);
            }

        }

        public EnterPartsUIElementStates EnterPartsUIElementStatesObject
        {
            get
            {
                ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(_connectionString);
                ApplicationProperty applicationProperty = applicationPropertyService.GetByName("EnterPartsUIElementStates");

                if (!string.IsNullOrEmpty(applicationProperty?.Value))
                {
                    _enterPartsUIElementStates = JsonConvert.DeserializeObject<EnterPartsUIElementStates>(applicationProperty.Value);
                }
                else
                {

                    _enterPartsUIElementStates = new EnterPartsUIElementStates
                    {
                        CurrentBatchDescription = "",
                        UnloadCurrentIsEnabled = false,
                        LayoutGroupCompletedBatchesVisibility = Visibility.Collapsed.ToString(),
                        LayoutGroupPreparePartsVisibility = Visibility.Collapsed.ToString(),
                        LayoutGroupInitialCommandsVisibility = Visibility.Visible.ToString(),
                        LayoutGroupBagsVisibility = Visibility.Collapsed.ToString(),
                        BarbtnAddBagVisibility = false,
                        //  BarbtnRemoveBagVisibility=false,
                        SkipPartsButtonIsEnabled = true

                    };
                }
                return _enterPartsUIElementStates;
            }
            set
            {
                string settingsSerialized = JsonConvert.SerializeObject(value);
                ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(_connectionString);
                applicationPropertyService.UpdateByName("EnterPartsUIElementStates", settingsSerialized);

                //Properties.Settings.Default.VacuumLinesSettings = vacuumLinesSettingsSerialized;
                //Properties.Settings.Default.Save();
            }
        }

        private void InitializePageTagConfigurations()
        {
            try
            {
                PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
                var pageTagConfiguration = pageTagConfigurationService.GetByName("EnterParts");
                string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
                _enterPartsTagConfigurations = JsonConvert.DeserializeObject<EnterPartsTagConfigurations>(jsonSerializedString);
                SetEnterPartsDatablock(true);

                _enterPartsTagConfigurations.MoveToPre = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(_enterPartsTagConfigurations.MoveToPre)]);
                _enterPartsTagConfigurations.ScadaSentBatch = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(_enterPartsTagConfigurations.ScadaSentBatch)]);
                _enterPartsTagConfigurations.SkipEnterParts = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(_enterPartsTagConfigurations.SkipEnterParts)]);
                _enterPartsTagConfigurations.ActiveBatchName = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(_enterPartsTagConfigurations.ActiveBatchName)]);
               
            }
            catch (Exception ex)
            {

                LogManager.Instance.Log($"InitializePageTagConfigurations Detail: {ex.Message}\n\n", LogType.Error);
            }

        }

        #endregion

        public bool SetEnterPartsDatablock(bool value)
        {
            bool result = false;
            if (_enterPartsTagConfigurations.DbNumbers != null)
            {
                for (int i = 0; i < _enterPartsTagConfigurations.DbNumbers.Count; i++)
                {
                    result = ProcessManager.Instance.ChangeDemandReadStateOnCache(PlcDeviceId, _enterPartsTagConfigurations.DbNumbers[i], value);
                }
            }
            return result = false;
        }

        #region Current Operations

        public async Task<Batch> CheckCurrentBatch()
        {
            return await Task.Run(() =>
            {
                Batch batch = null;

                try
                {
                    BatchService batchService = new BatchService(_connectionString);

                    batch = batchService.GetActiveCurrentBatch();

                    if (batch != null)
                    {
                        CurrentBatch = batch;
                        EnterPartsUIElementStates.CurrentBatchDescription = batch.LoadNumberFormatted;
                        EnterPartsUIElementStates.UnloadCurrentIsEnabled = true;
                        SaveEnterPartsUIElementStatesSettings();
                    }
                    else
                    {
                        CurrentBatch = null;
                        EnterPartsUIElementStates.CurrentBatchDescription = "Not Loaded...";
                        EnterPartsUIElementStates.SkipPartsButtonIsEnabled = true;
                        EnterPartsUIElementStates.UnloadCurrentIsEnabled = false;
                        SaveEnterPartsUIElementStatesSettings();
                    }

                }
                catch (Exception ex)
                {
                    LogManager.Instance.Log($"InitializePageTagConfigurations Detail: {ex.Message}\n\n", LogType.Error);
                }
                return batch;
            });

        }
        public async Task<bool> UnloadCurrent()
        {
            bool result = false;

            if (ProcessManager.Instance.IsPlcConnected())
            {
                try
                {
                    result = await Task.Run(() =>
                    {
                        BatchService batchService = new BatchService(_connectionString);
                        Batch batch = batchService.GetActiveCurrentBatch();

                        var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;

                        SyncIssueManager syncIssueManager = new SyncIssueManager(ApplicationConfigurations.Instance.Configuration.RedisServer);



                        void MoveCurrentToPrepare()
                        {
                            //update
                            batch.BatchGroupId = 1;
                            batch.Status = BatchCurrentState.NotStarted;
                            batch.ModifiedByUserId = ActiveUser.id;
                            var batchUpdateResult = batchService.Update(batch);
                            if (batchUpdateResult)
                            {
                                string serializedEntityObject = JsonConvert.SerializeObject(batch);
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

                                syncIssueManager.CreateNewSyncIssue(syncIssue);
                            }
                        }


                        if (batch != null)
                        {
                            if ((CompletedBatches.Select(x => x.LoadNumber).Contains(ProcessManager.Instance.CurrentProcess.LoadNumber) || batch.IsEnterPartsSkip == true))
                            {

                                if (batch.Revision==0)
                                {
                                    BagService bagService = new BagService(_connectionString);
                                    var bags = bagService.BagsByBatch(batch.id);
                                    List<bool> resultList = new List<bool>();
                                    foreach (var bag in bags)
                                    {
                                        bool deleteResultBag = bagService.Delete(new Bag { id = bag.id });

                                        resultList.Add(deleteResultBag);

                                        if (deleteResultBag)
                                        {
                                            string serializedEntityObject = JsonConvert.SerializeObject(new Bag { id = bag.id });

                                            SyncIssue syncIssue = new SyncIssue
                                            {
                                                SerializedEntityObject = serializedEntityObject,
                                                EntityObjectType = typeof(Bag),
                                                SyncDBCommand = SyncDBCommand.Delete,
                                                FromToDirection = fromToDirection,
                                                PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                                                SyncStatus = SyncStatus.NoneProcessChangesPending,
                                                TransferType = TransferType.NonProcessChanges,
                                            };

                                            syncIssueManager.CreateNewSyncIssue(syncIssue);
                                        }
                                    }
                                    //delete
                                    var deleteResult = batchService.Delete(batch);
                                    if (deleteResult)
                                    {

                                        string serializedEntityObject = JsonConvert.SerializeObject(batch);

                                        SyncIssue syncIssue = new SyncIssue
                                        {
                                            SerializedEntityObject = serializedEntityObject,
                                            EntityObjectType = typeof(Batch),
                                            SyncDBCommand = SyncDBCommand.Delete,
                                            FromToDirection = fromToDirection,
                                            PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                                            SyncStatus = SyncStatus.NoneProcessChangesPending,
                                            TransferType = TransferType.NonProcessChanges,
                                        };

                                        syncIssueManager.CreateNewSyncIssue(syncIssue);
                                    }
                                }
                                else
                                {
                                    MoveCurrentToPrepare();
                                }
                            }
                            else
                            {
                                MoveCurrentToPrepare();
                            }

                            // Set currentBatchId 0 to the cache
                            ProcessManager.Instance.CurrentProcess.BatchCurrentState = BatchCurrentState.NotStarted;
                            ProcessManager.Instance.CurrentProcess.IsAlarmSaved = false;
                            ProcessManager.Instance.CurrentProcess.LoadNumber = string.Empty;
                            ProcessManager.Instance.CurrentProcess.IsBatchLoaded = false;
                            ProcessManager.Instance.CurrentProcess.BatchId = 0;
                            ProcessManager.Instance.CurrentProcess.CurrentSegmentDescription = string.Empty;
                            ProcessManager.Instance.CurrentProcess.CurrentSegmentNo = 0;
                            ProcessManager.Instance.CurrentProcess.LastUpdateDate = DateTime.Now;
                            ProcessManager.Instance.SynchronizeCurrentProcessInfo(false);
                            ProcessManager.Instance.ResetActiveSensors();
                            CurrentBatchNumber.Value = null;

                            PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);

                            // Set to PLC
                            plcCommandManager.Set((SiemensTagConfiguration)_enterPartsTagConfigurations.MoveToPre, true);
                            EnterPartsSelectedBatchModel = new EnterPartsSelectedBatchModel();
                        }

                        LogManager.Instance.Log($"Current unload for {batch.LoadNumber} ProcessId: {batch.id}", LogType.Information);

                        return true;
                    });

                    await FillPrepareParts();
                }
                catch (Exception ex)
                {
                    DisplayMessage = "Güncelleme işlemi tamamlanamadı! Lütfen bağlantılarınızı kontrol edin!";
                    LogManager.Instance.Log($"MoveCurrentToPreparePart Detail: {ex.Message}\n\n", LogType.Error);
                    result = false;
                }
            }
            else
            {
                DisplayMessage = "Güncelleme işlemi tamamlanamadı! Lütfen bağlantılarınızı kontrol edin!";
                LogManager.Instance.Log("MoveCurrentToPreparePart not completed!", LogType.Error);
                result = false;
            }
            await CheckCurrentBatch();
            return result;
        }
        public async Task<bool> MoveToCurrent()
        {
            bool result = false;
            var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
            SyncIssueManager syncIssueManager = new SyncIssueManager(ApplicationConfigurations.Instance.Configuration.RedisServer);

            if (ProcessManager.Instance.IsPlcConnected())
            {
                try
                {
                    if (ProcessManager.Instance.IsBatchRunning())
                    {
                        DisplayMessage = "Çalışan batch olduğundan dolayı Current batch güncellenemez!";
                        result = false;
                    }
                    else
                    {
                        if (ProcessManager.Instance.CurrentProcess.IsBatchLoaded)
                        {
                            DisplayMessage = "'Current Batch' grubunda zaten bir batch var.";
                            result = false;
                        }
                        else
                        {
                            result = true;
                        }

                        if (result)
                        {
                            BatchService batchService = new BatchService(_connectionString);
                            // Move or clone batch according to its batch group id value
                            if (EnterPartsSelectedBatchModel.SelectedBatch.BatchGroupId == 1) // Prepare to Curr
                            {
                                PrepareParts.Remove(EnterPartsSelectedBatchModel.SelectedBatch);
                                EnterPartsSelectedBatchModel.SelectedBatch.BatchGroupId = 2;
                                EnterPartsSelectedBatchModel.SelectedBatch.Status = BatchCurrentState.NotStarted;
                                EnterPartsSelectedBatchModel.SelectedBatch.ModifiedByUserId = ActiveUser.id;

                                // Check if currentRecipeInfo is not null
                                if (ProcessManager.Instance.CurrentProcess.IsRecipeLoaded)
                                {
                                    EnterPartsSelectedBatchModel.SelectedBatch.RecipeId = ProcessManager.Instance.CurrentProcess.ActiveRecipeId;
                                }
                                await Task.Run(() =>
                                {
                                    var batchUpdateResult = batchService.Update(EnterPartsSelectedBatchModel.SelectedBatch);

                                    if (batchUpdateResult)
                                    {
                                      
                                        string serializedEntityObject = JsonConvert.SerializeObject(EnterPartsSelectedBatchModel.SelectedBatch);

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

                                        syncIssueManager.CreateNewSyncIssue(syncIssue);
                                    }

                                });

                                CurrentBatch = new Batch
                                {
                                    id = EnterPartsSelectedBatchModel.SelectedBatch.id,
                                    BatchGroupId = 2,
                                    LoadNumber = EnterPartsSelectedBatchModel.SelectedBatch.LoadNumber,
                                    RecipeId = EnterPartsSelectedBatchModel.SelectedBatch.RecipeId,
                                    Revision = EnterPartsSelectedBatchModel.SelectedBatch.Revision,
                                    Status = BatchCurrentState.NotStarted,
                                    ModifiedByUserId = ActiveUser.id
                                };
                            }
                            else if (EnterPartsSelectedBatchModel.SelectedBatch.BatchGroupId == 0) // Then it is located on the "Last 5 Batches" group, clone it.
                            {
                                EnterPartsSelectedBatchModel.SelectedBatch.BatchGroupId = 2;
                                EnterPartsSelectedBatchModel.SelectedBatch.Status = BatchCurrentState.NotStarted; // Not started
                                                                                                                  // Check if currentRecipeInfo is not null
                                EnterPartsSelectedBatchModel.SelectedBatch.CreatedByUserId = ActiveUser.id;
                                EnterPartsSelectedBatchModel.SelectedBatch.ModifiedByUserId = ActiveUser.id;

                                if (ProcessManager.Instance.CurrentProcess.IsRecipeLoaded)
                                {
                                    EnterPartsSelectedBatchModel.SelectedBatch.RecipeId = ProcessManager.Instance.CurrentProcess.ActiveRecipeId;
                                }

                                BagService bagService = new BagService(_connectionString);
                                LotPropertyService lotPropertyService = new LotPropertyService(_connectionString);
                                IEnumerable<Bag> bags = bagService.BagsByBatch(EnterPartsSelectedBatchModel.SelectedBatch.id);
                                int maxRevisionNumber =   batchService.GetMaxRevisionNumber(EnterPartsSelectedBatchModel.SelectedBatch.LoadNumber);

                                Batch batch = new Batch
                                {
                                    BatchGroupId = 2,
                                    LoadNumber = EnterPartsSelectedBatchModel.SelectedBatch.LoadNumber,
                                    RecipeId = EnterPartsSelectedBatchModel.SelectedBatch.RecipeId,
                                    Revision = (short)(maxRevisionNumber + 1),
                                    Status = BatchCurrentState.NotStarted,
                                    CreatedByUserId = ActiveUser.id,
                                    ModifiedByUserId = ActiveUser.id,
                                };
                                                                 
                                await Task.Run(() =>
                                {
                                    var batchResult = batchService.Insert(batch);

                                    if (batchResult)
                                    {
                                        string serializedEntityObject = JsonConvert.SerializeObject(batch);

                                        SyncIssue syncIssue = new SyncIssue
                                        {
                                            SerializedEntityObject = serializedEntityObject,
                                            EntityObjectType = typeof(Batch),
                                            SyncDBCommand = SyncDBCommand.Insert,
                                            FromToDirection = fromToDirection,
                                            PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                                            SyncStatus = SyncStatus.NoneProcessChangesPending,
                                            TransferType = TransferType.NonProcessChanges,
                                        };

                                        syncIssueManager.CreateNewSyncIssue(syncIssue);
                                    }
                                });

                                CurrentBatch = new Batch
                                {
                                    id = batch.id,
                                    BatchGroupId = 2,
                                    LoadNumber = batch.LoadNumber,
                                    RecipeId = batch.RecipeId,
                                    Revision = batch.Revision,
                                    Status = BatchCurrentState.NotStarted,
                                    CreatedByUserId = ActiveUser.id,
                                    ModifiedByUserId = ActiveUser.id,
                                };
                                //todo:h completed tan atılırken bagler gibi soirler de kopyalanmıyor mu?
                                foreach (var bag in bags)
                                {
                                    var lotProperties = lotPropertyService.GetByBagId(bag.id);

                                    bag.id = default;
                                    bag.BatchId = batch.id;
                                    bagService.Insert(bag);
                                    
                                    string serializedEntityObject = JsonConvert.SerializeObject(bag);

                                    SyncIssue syncIssue = new SyncIssue
                                    {
                                        SerializedEntityObject = serializedEntityObject,
                                        EntityObjectType = typeof(Bag),
                                        SyncDBCommand = SyncDBCommand.Insert,
                                        FromToDirection = fromToDirection,
                                        PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                                        SyncStatus = SyncStatus.NoneProcessChangesPending,
                                        TransferType = TransferType.NonProcessChanges,
                                    };

                                    syncIssueManager.CreateNewSyncIssue(syncIssue);


                                    foreach (var lotProperty in lotProperties)
                                    {
                                        lotProperty.id = default;
                                        lotProperty.BagId = bag.id;
                                        InsertLotProperty(lotProperty);
                                    }

                                }
                            }

                            ProcessManager.Instance.CurrentProcess.BatchId = CurrentBatch.id;
                            ProcessManager.Instance.CurrentProcess.BatchCurrentState = CurrentBatch.Status;
                            ProcessManager.Instance.CurrentProcess.IsBatchLoaded = true;
                            ProcessManager.Instance.CurrentProcess.LoadNumber = CurrentBatch.LoadNumber;
                            ProcessManager.Instance.SynchronizeCurrentProcessInfo(false);
                            CurrentBatchNumber.Value = CurrentBatch.LoadNumber;
                        }
                    }

                    if (result)
                    {
                        ActiveTagService activeTagService = new ActiveTagService(_connectionString);
                        // Set selected ports to the true from ActiveTags table
                        bool SetSelectedPortValuesForDataLogsResult = await SetSelectedPortValuesForDataLogs(EnterPartsSelectedBatchModel.SelectedBatch.id);

                        if (SetSelectedPortValuesForDataLogsResult)
                        {
                            SetActivePortsAndBagToPlc();
                            bool setBatchToPlcResult = await SetBatchToPlc();
                            await FillPrepareParts();
                            EnterPartsSelectedBatchModelByBatch(CurrentBatch);
                            EnterPartsSelectedBatchModel.ShowPortsInDisabledMode = true;
                            LogManager.Instance.Log($"Move to current {CurrentBatch.LoadNumber} ProcessId: {CurrentBatch.id}", LogType.Information);
                        }

                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    DisplayMessage = "Güncelleme işlemi tamamlanamadı! Lütfen bağlantılarınızı kontrol edin!";
                    LogManager.Instance.Log($"MoveToCurrentAsync Detail: {ex.Message}\n\n", LogType.Error);
                    result = false;
                }
            }
            else
            {
                DisplayMessage = "Güncelleme işlemi tamamlanamadı! Lütfen bağlantılarınızı kontrol edin!";
                LogManager.Instance.Log("MoveToCurrentAsync", LogType.Error);
                result = false;
            }

            await CheckCurrentBatch();
            return result;
        }
        public async Task<bool> SetSelectedPortValuesForDataLogs(int batchId)
        {
            bool result = await Task.Run(() =>
            {
                List<bool> checkList = new List<bool>();
                BagService bagService = new BagService(_connectionString);
                ActiveTagService activeTagService = new ActiveTagService(_connectionString);
                var activeTagsById = activeTagService.ActiveTagsByTagIdKey();
                var selectedBags = bagService.BagsByBatch(batchId);

                var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
                SyncIssueManager syncIssueManager = new SyncIssueManager(ApplicationConfigurations.Instance.Configuration.RedisServer);


                foreach (var bag in selectedBags)
                {
                    foreach (var port in bag.SelectedPorts)
                    {
                        ActiveTag activeTag = activeTagsById[port];

                        if (activeTag != null && activeTag.ActiveTagGroupId != ActiveTagGroups.VAC)
                        {
                            activeTag.IsLogData = true;
                            bool updateResult = activeTagService.Update(activeTag);
                            checkList.Add(updateResult);

                            //todo:m refactor update many yapılacak.

                            if (updateResult)
                            {

                                string serializedEntityObject = JsonConvert.SerializeObject(activeTag);

                                SyncIssue syncIssue = new SyncIssue
                                {
                                    SerializedEntityObject = serializedEntityObject,
                                    EntityObjectType = typeof(ActiveTag),
                                    SyncDBCommand = SyncDBCommand.Update,
                                    FromToDirection = fromToDirection,
                                    PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                                    SyncStatus = SyncStatus.NoneProcessChangesPending,
                                    TransferType = TransferType.NonProcessChanges,
                                };

                                syncIssueManager.CreateNewSyncIssue(syncIssue);
                            }
                        }
                    }
                }
                return checkList.TrueForAll(x => x == true);
            });
            return result;
        }
        public async Task<bool> SkipEnterParts()
        {
            bool result = false;


            await CheckCurrentBatch();

            if (CurrentBatch != null)
            {
                DisplayMessage = "'Current Batch' grubunda zaten bir batch var.";

                result = false;
            }
            else
            {
                result = ProcessManager.Instance.SkipEnterParts();
                LogManager.Instance.Log($"Enter parts skipped", LogType.Information);

            }

            await CheckCurrentBatch();

            return result;


        }

        /// <summary>
        /// set active batch name and scada send info to plc
        /// </summary>
        private async Task<bool> SetBatchToPlc()
        {
            bool result = await Task.Run(() =>
            {
                if (ProcessManager.Instance.CurrentProcess.IsBatchLoaded)
                {
                    Guid guidSendBatch = Guid.NewGuid();
                    Guid guidSendLoadNumber = Guid.NewGuid();
                    PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
                    plcCommandManager.Set((SiemensTagConfiguration)_enterPartsTagConfigurations.ScadaSentBatch, true, guidSendBatch);
                    plcCommandManager.Set((SiemensTagConfiguration)_enterPartsTagConfigurations.ActiveBatchName, ProcessManager.Instance.CurrentProcess.LoadNumber, guidSendLoadNumber);

                    bool sentBatchResult = plcCommandManager.IsUpdatedResult(guidSendBatch, false, 500);
                    bool batchNameSetResult = plcCommandManager.IsUpdatedResult(guidSendLoadNumber, false, 500);

                    return (sentBatchResult == true && batchNameSetResult == true);
                }
                else
                {
                    return false;
                }
            });
            return result;
        }
        private void SetActivePortsAndBagToPlc()
        {
            PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);

            byte[] selectedPortsBuffer = plcCommandManager.GetAllBuffer(ApplicationConfigurations.Instance.Configuration.PlcDevice.Id, (int)_enterPartsTagConfigurations.SelectedPortInfoDbNumber);
            selectedPortsBuffer = plcCommandManager.ResetBuffer(selectedPortsBuffer);

            byte[] selectedPortBagInfoBuffer = plcCommandManager.GetAllBuffer(ApplicationConfigurations.Instance.Configuration.PlcDevice.Id, _enterPartsTagConfigurations.PortBagInfoDbNumber);
            selectedPortBagInfoBuffer = plcCommandManager.ResetBuffer(selectedPortBagInfoBuffer);

            foreach (EnterPartsBagDetail enterPartsBagDetail in EnterPartsSelectedBatchModel.EnterPartsBagDetails)
            {
                foreach (EnterPartsPortDetail enterPartsPortDetails in enterPartsBagDetail.EnterPartsPortDetails)
                {
                    EnterPartsSelectedPortsInfo enterPartsSelectedPortsInfo = _enterPartsTagConfigurations.SelectedPortsInfo[enterPartsPortDetails.SelectedPortLiteral];
                    SiemensTagConfiguration portBagInfoTag = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(enterPartsSelectedPortsInfo.PortBagInfo)]);
                    SiemensTagConfiguration selectedPortInfoTag = ((SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(enterPartsSelectedPortsInfo.SelectedPortInfo)]);
                    plcCommandManager.SetToBuffer(selectedPortInfoTag, selectedPortsBuffer, true);
                    plcCommandManager.SetToBuffer(portBagInfoTag, selectedPortBagInfoBuffer, enterPartsBagDetail.BagNumeric);
                }
            }

            plcCommandManager.SetBufferToPLC(selectedPortsBuffer, ApplicationConfigurations.Instance.Configuration.PlcDevice.Id, _enterPartsTagConfigurations.SelectedPortInfoDbNumber);
            plcCommandManager.SetBufferToPLC(selectedPortBagInfoBuffer, ApplicationConfigurations.Instance.Configuration.PlcDevice.Id, _enterPartsTagConfigurations.PortBagInfoDbNumber);
        }

        #endregion

        #region Prepare Parts

        public async Task<bool> AddNewPreparePart()
        {
            await Task.Delay(300);
            bool result = await Task.Run(() =>
            {
                Batch newBatch = new Batch();
                // Prepare group id = 1
                ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(_connectionString);
                ApplicationProperty lastLoadNumberAppProperty = applicationPropertyService.GetByName("LastLoadNumber");
                int lastLoadNumberIterationCount = Convert.ToInt32(lastLoadNumberAppProperty.Value);

                // Increase then update the DB
                int nextValue = lastLoadNumberIterationCount + 1;
                lastLoadNumberAppProperty.Value = nextValue.ToString();
                applicationPropertyService.Update(lastLoadNumberAppProperty);


              
                    var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;

                    string serializedEntityObject = JsonConvert.SerializeObject(lastLoadNumberAppProperty);

                    SyncIssue syncIssue = new SyncIssue
                    {
                        SerializedEntityObject = serializedEntityObject,
                        EntityObjectType = typeof(ApplicationProperty),
                        SyncDBCommand = SyncDBCommand.Update,
                        FromToDirection = fromToDirection,
                        PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                        SyncStatus = SyncStatus.NoneProcessChangesPending,
                        TransferType = TransferType.NonProcessChanges,
                    };

                    SyncIssueManager syncIssueManager = new SyncIssueManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
                    syncIssueManager.CreateNewSyncIssue(syncIssue);
                

                string newBatchName = $"{ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceName}-{lastLoadNumberIterationCount}";

                newBatch = new Batch()
                {
                    LoadNumber = newBatchName,
                    StartDate = DateTime.Now,
                    BatchGroupId = 1,
                    RecipeId = 0,
                    Status = 0,
                    Revision = 0,
                    CreatedByUserId = ActiveUser.id,
                    ModifiedByUserId = ActiveUser.id
                };

                BatchService batchService = new BatchService(_connectionString);
                bool batchInsertResult = batchService.Insert(newBatch);

                if (batchInsertResult)
                {

                    string serializedEntityObjectnewBatch = JsonConvert.SerializeObject(newBatch);

                    SyncIssue syncIssueNewBatch = new SyncIssue
                    {
                        SerializedEntityObject = serializedEntityObjectnewBatch,
                        EntityObjectType = typeof(Batch),
                        SyncDBCommand = SyncDBCommand.Insert,
                        FromToDirection = fromToDirection,
                        PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                        SyncStatus = SyncStatus.NoneProcessChangesPending,
                        TransferType = TransferType.NonProcessChanges,
                    };

                    syncIssueManager.CreateNewSyncIssue(syncIssueNewBatch);
                }

                EnterPartsSelectedBatchModel = EnterPartsSelectedBatchModelByBatch(newBatch);

                return batchInsertResult;
            });

            if (result)
            {
                await FillPrepareParts();
            }
            else
            {
                LogManager.Instance.Log($"Add New Prepare Part error!", LogType.Error);
            }

            return result;
        }
        public async Task<bool> RemovePreparePartBatch()
        {
            bool result = false;
            if (ProcessManager.Instance.IsPlcConnected())
            {
                try
                {
                    result = await Task.Run(() =>
                    {
                        List<bool> resultList = new List<bool>();
                        BagService bagService = new BagService(_connectionString);
                        BatchService batchService = new BatchService(_connectionString);
                        LotPropertyService lotPropertyService = new LotPropertyService(_connectionString);
                        var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
                        SyncIssueManager syncIssueManager = new SyncIssueManager(ApplicationConfigurations.Instance.Configuration.RedisServer);

                        foreach (var bagDetail in EnterPartsSelectedBatchModel.EnterPartsBagDetails)
                        {
                            bool deleteResult = bagService.Delete(new Bag { id = bagDetail.BagId });
                            resultList.Add(deleteResult);

                            if (deleteResult)
                            {
                                 string serializedEntityObject = JsonConvert.SerializeObject(new Bag { id = EnterPartsSelectedBatchModel.SelectedBag.BagId });

                                SyncIssue syncIssue = new SyncIssue
                                {
                                    SerializedEntityObject = serializedEntityObject,
                                    EntityObjectType = typeof(Bag),
                                    SyncDBCommand = SyncDBCommand.Delete,
                                    FromToDirection = fromToDirection,
                                    PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                                    SyncStatus = SyncStatus.NoneProcessChangesPending,
                                    TransferType = TransferType.NonProcessChanges,
                                };

                                syncIssueManager.CreateNewSyncIssue(syncIssue);
                            }

                            var lotPropertyEntity = lotPropertyService.GetByBagId(bagDetail.BagId);

                            foreach (var lotPropertyDetail in lotPropertyEntity)
                            {
                                bool deleteResultLotProperty = lotPropertyService.Delete(new LotProperty { id = lotPropertyDetail.id });

                                if (deleteResultLotProperty)
                                {
                                    string serializedEntityObject = JsonConvert.SerializeObject(new LotProperty { id = lotPropertyDetail.id });

                                    SyncIssue syncIssue = new SyncIssue
                                    {
                                        SerializedEntityObject = serializedEntityObject,
                                        EntityObjectType = typeof(LotProperty),
                                        SyncDBCommand = SyncDBCommand.Delete,
                                        FromToDirection = fromToDirection,
                                        PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                                        SyncStatus = SyncStatus.NoneProcessChangesPending,
                                        TransferType = TransferType.NonProcessChanges,
                                    };

                                    syncIssueManager.CreateNewSyncIssue(syncIssue);
                                }
                            }
                        }

                        bool batchDeleteResult = batchService.Delete(EnterPartsSelectedBatchModel.SelectedBatch);
                        resultList.Add(batchDeleteResult);

                        if (batchDeleteResult)
                        {
                            string serializedEntityObject = JsonConvert.SerializeObject(EnterPartsSelectedBatchModel.SelectedBatch);

                            SyncIssue syncIssue = new SyncIssue
                            {
                                SerializedEntityObject = serializedEntityObject,
                                EntityObjectType = typeof(Batch),
                                SyncDBCommand = SyncDBCommand.Delete,
                                FromToDirection = fromToDirection,
                                PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                                SyncStatus = SyncStatus.NoneProcessChangesPending,
                                TransferType = TransferType.NonProcessChanges,
                            };

                            syncIssueManager.CreateNewSyncIssue(syncIssue);
                        }

                        int maxPreparePartsId = PrepareParts.Max(x => x.id);

                        if (maxPreparePartsId == EnterPartsSelectedBatchModel.SelectedBatch.id)
                        {
                            ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(_connectionString);
                            ApplicationProperty lastLoadNumberAppProperty = applicationPropertyService.GetByName("LastLoadNumber");
                            lastLoadNumberAppProperty.Value = (Convert.ToInt32(lastLoadNumberAppProperty.Value) - 1).ToString();
                            applicationPropertyService.Update(lastLoadNumberAppProperty);

                            string serializedEntityObject = JsonConvert.SerializeObject(lastLoadNumberAppProperty);

                            SyncIssue syncIssue = new SyncIssue
                            {
                                SerializedEntityObject = serializedEntityObject,
                                EntityObjectType = typeof(ApplicationProperty),
                                SyncDBCommand = SyncDBCommand.Update,
                                FromToDirection = fromToDirection,
                                PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                                SyncStatus = SyncStatus.NoneProcessChangesPending,
                                TransferType = TransferType.NonProcessChanges,
                            };

                            syncIssueManager.CreateNewSyncIssue(syncIssue);
                        }

                        result = resultList.TrueForAll(x => x == true);

                        return result;
                    });

                    if (result)
                    {
                        EnterPartsSelectedBatchModel.EnterPartsBagDetails = new ObservableCollection<EnterPartsBagDetail>();
                        PrepareParts.Remove(EnterPartsSelectedBatchModel.SelectedBatch);
                    }
                    else
                    {
                        LogManager.Instance.Log($"Remove Prepare error! Id:{EnterPartsSelectedBatchModel.SelectedBatch.id} Load Number:{EnterPartsSelectedBatchModel.SelectedBatch.LoadNumber}", LogType.Error);
                    }
                }
                catch (Exception ex)
                {
                    DisplayMessage = "Güncelleme işlemi tamamlanamadı! Lütfen bağlantılarınızı kontrol edin!";
                    LogManager.Instance.Log($"RemovePreparePartBatch Detail: {ex.Message}\n\n", LogType.Error);
                    result = false;
                }
            }
            else
            {
                DisplayMessage = "Güncelleme işlemi tamamlanamadı! Lütfen bağlantılarınızı kontrol edin!";
                LogManager.Instance.Log("RemovePreparePartBatch not completed!", LogType.Error);
                result = false;
            }
            return result;

        }
        public async Task FillPrepareParts()
        {
            await CheckCurrentBatch();

            PrepareParts = new ObservableCollection<Batch>();
            BatchService batchService = new BatchService(_connectionString);

            var batches = batchService.GetPrepareBatches();

            foreach (var batch in batches)
            {
                PrepareParts.Add(batch);
            }

            await CheckCurrentBatch();
        }
        private void PreparePartsBack()
        {
            EnterPartsUIElementStates.LayoutGroupInitialCommandsVisibility = Visibility.Visible.ToString();
            EnterPartsUIElementStates.LayoutGroupPreparePartsVisibility = Visibility.Collapsed.ToString();
            EnterPartsUIElementStates.LayoutGroupBagsVisibility = Visibility.Collapsed.ToString();
            SaveEnterPartsUIElementStatesSettings();

            if (EnterPartsSelectedBatchModel != null)
                EnterPartsSelectedBatchModel.SelectedBatch = null;

            IsPrepareRemoveBtnEnabled = false;
            IsMoveToCurBtnEnabled = false;
        }

        #endregion

        #region Completed Batches

        public async Task<bool> MoveCompletedToPrepare()
        {
            bool result = false;
            var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
            SyncIssueManager syncIssueManager = new SyncIssueManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
            BatchService batchService = new BatchService(_connectionString);
            if (ProcessManager.Instance.IsPlcConnected())
            {
                EnterPartsSelectedBatchModel.SelectedBatch.BatchGroupId = 1;
                EnterPartsSelectedBatchModel.SelectedBatch.Status = BatchCurrentState.NotStarted;
                EnterPartsSelectedBatchModel.SelectedBatch.CreatedByUserId = ActiveUser.id;
                EnterPartsSelectedBatchModel.SelectedBatch.ModifiedByUserId = ActiveUser.id;

                BagService bagService = new BagService(_connectionString);
                LotPropertyService lotPropertyService = new LotPropertyService(_connectionString);
                IEnumerable<Bag> bags = bagService.BagsByBatch(EnterPartsSelectedBatchModel.SelectedBatch.id);
                int maxRevisionNumber = batchService.GetMaxRevisionNumber(EnterPartsSelectedBatchModel.SelectedBatch.LoadNumber);

                Batch batch = new Batch
                {
                    BatchGroupId = 1,
                    LoadNumber = EnterPartsSelectedBatchModel.SelectedBatch.LoadNumber,
                    Revision = (short)(maxRevisionNumber + 1),
                    Status = BatchCurrentState.NotStarted,
                    CreatedByUserId = ActiveUser.id,
                    ModifiedByUserId = ActiveUser.id,
                };

                var batchResult = batchService.Insert(batch);

                if (batchResult)
                {
                    #region sync batch
                    string serializedEntityObject = JsonConvert.SerializeObject(batch);

                    SyncIssue syncIssue = new SyncIssue
                    {
                        SerializedEntityObject = serializedEntityObject,
                        EntityObjectType = typeof(Batch),
                        SyncDBCommand = SyncDBCommand.Insert,
                        FromToDirection = fromToDirection,
                        PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                        SyncStatus = SyncStatus.NoneProcessChangesPending,
                        TransferType = TransferType.NonProcessChanges,
                    };

                    syncIssueManager.CreateNewSyncIssue(syncIssue);
                    #endregion

                    foreach (var bag in bags)
                    {
                        var lotProperties =lotPropertyService.GetByBagId(bag.id);
                        bag.id = default;
                        bag.BatchId = batch.id;
                        bagService.Insert(bag);

                        #region sync bags
                          serializedEntityObject = JsonConvert.SerializeObject(bag);

                          syncIssue = new SyncIssue
                        {
                            SerializedEntityObject = serializedEntityObject,
                            EntityObjectType = typeof(Bag),
                            SyncDBCommand = SyncDBCommand.Insert,
                            FromToDirection = fromToDirection,
                            PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                            SyncStatus = SyncStatus.NoneProcessChangesPending,
                            TransferType = TransferType.NonProcessChanges,
                        };

                        syncIssueManager.CreateNewSyncIssue(syncIssue);
                        #endregion

                        foreach (var lotProperty in lotProperties)
                        {
                            lotProperty.id = default;
                            lotProperty.BagId = bag.id;
                            InsertLotProperty(lotProperty);
                        }
                    }

                    result = true;
                }
                else
                {
                    DisplayMessage = "Error occured while copying batch information!";
                }

                await FillPrepareParts();
            }

            await CheckCurrentBatch();
            return result;
        }

        public async Task FillCompletedBatches()
        {
            await CheckCurrentBatch();

            CompletedBatches = new ObservableCollection<Batch>();
            BatchService batchService = new BatchService(_connectionString);

            var batches = batchService.GetCompletedBatches(5).OrderByDescending(x => x.EndDate);

            foreach (var batch in batches)
            {

                //if (batch.Revision > 0)
                //{
                //    batch.LoadNumber = $"{batch.LoadNumber}-R{batch.Revision}";
                //}

                CompletedBatches.Add(batch);
            }
        }

        private void CompletedBatchesBack()
        {
            EnterPartsUIElementStates.LayoutGroupInitialCommandsVisibility = Visibility.Visible.ToString();
            EnterPartsUIElementStates.LayoutGroupCompletedBatchesVisibility = Visibility.Collapsed.ToString();
            EnterPartsUIElementStates.LayoutGroupBagsVisibility = Visibility.Collapsed.ToString();
            SaveEnterPartsUIElementStatesSettings();

            if (EnterPartsSelectedBatchModel != null)
                EnterPartsSelectedBatchModel.SelectedBatch = null;

            IsPrepareRemoveBtnEnabled = false;
            IsMoveToCurBtnEnabled = false;
        }
        #endregion

        #region Bag
        public async Task<bool> AddNewBagAsync()
        {

            EnterPartsBagDetail enterPartsBagDetail = new EnterPartsBagDetail();

            bool result = false;

            await Task.Delay(300);

            result = await Task.Run(() =>
            {

                if (EnterPartsSelectedBatchModel.SelectedBatch == null)
                {
                    DisplayMessage = "Bag eklenecek batch bulunamadı!";
                    result = false;
                }

                if (EnterPartsSelectedBatchModel.SelectedBatch.BatchGroupId != 1)
                {
                    DisplayMessage = "Bu batch Prepare Parts grubunda olmadığı için yeni bir bag ekleyemezsiniz!";
                    result = false;
                }
                else
                {
                    result = true;
                }

                if (result == true)
                {
                    BagService bagService = new BagService(_connectionString);
                    BatchService batchService = new BatchService(_connectionString);

                    Bag bag = new Bag()
                    {
                        BatchId = EnterPartsSelectedBatchModel.SelectedBatch.id,
                        SelectedPorts = new int[0]
                    };

                    if (EnterPartsSelectedBatchModel.EnterPartsBagDetails.Count > 0)
                    {
                        int maxBagNumeric = EnterPartsSelectedBatchModel.EnterPartsBagDetails.Max(x => x.BagNumeric);
                        bag.BagName = EnterPartsSelectedBatchModel.SelectedBatch.LoadNumber + "-Bag" + (maxBagNumeric + 1);
                    }
                    else
                    {
                        bag.BagName = EnterPartsSelectedBatchModel.SelectedBatch.LoadNumber + "-Bag" + 1;
                    }

                    bagService.Insert(bag);

                    var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;

                    string serializedEntityObject = JsonConvert.SerializeObject(bag);

                    SyncIssue syncIssue = new SyncIssue
                    {
                        SerializedEntityObject = serializedEntityObject,
                        EntityObjectType = typeof(Bag),
                        SyncDBCommand = SyncDBCommand.Insert,
                        FromToDirection = fromToDirection,
                        PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                        SyncStatus = SyncStatus.NoneProcessChangesPending,
                        TransferType = TransferType.NonProcessChanges,
                    };

                    SyncIssueManager syncIssueManager = new SyncIssueManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
                    syncIssueManager.CreateNewSyncIssue(syncIssue);


                    enterPartsBagDetail = new EnterPartsBagDetail
                    {

                        BagId = bag.id,
                        BagName = bag.BagName,
                        BagNumeric = Convert.ToInt32(bag.BagName.Split('-')[2].Remove(0, 3)),
                        BatchId = bag.BatchId,
                        EnterPartsPortDetails = new ObservableCollection<EnterPartsPortDetail>(),
                        LotPropertiesData = new ObservableCollection<LotProperty>(),
                        LoadNumber = EnterPartsSelectedBatchModel.SelectedBatch.LoadNumber
                    };

                    var selectedBatch = batchService.GetById(EnterPartsSelectedBatchModel.SelectedBatch.id);
                    selectedBatch.ModifiedByUserId = ActiveUser.id;
                    var batchUpdateResult = batchService.Update(selectedBatch);
                    if (batchUpdateResult)
                    {
                      
                        string serializedEntityObjectBatch = JsonConvert.SerializeObject(selectedBatch);

                        SyncIssue syncIssueBatch = new SyncIssue
                        {
                            SerializedEntityObject = serializedEntityObjectBatch,
                            EntityObjectType = typeof(Batch),
                            SyncDBCommand = SyncDBCommand.Update,
                            FromToDirection = fromToDirection,
                            PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                            SyncStatus = SyncStatus.NoneProcessChangesPending,
                            TransferType = TransferType.NonProcessChanges,
                        };

                        syncIssueManager.CreateNewSyncIssue(syncIssueBatch);
                    }

                }
                return result;
            });

            if (result)
            {
                EnterPartsSelectedBatchModel.EnterPartsBagDetails.Add(enterPartsBagDetail);
            }
            return result;
        }
        public async Task<bool> RemoveBagAsync()
        {
            bool result = false;

            if (EnterPartsSelectedBatchModel.SelectedBatch == null)
            {
                DisplayMessage = "Batch bulunamadı.";
                result = false;
            }
            if (EnterPartsSelectedBatchModel.SelectedBatch.BatchGroupId != 1)
            {
                DisplayMessage = "Bu batch Prepare grubunda olmadığı için bu bag'i silemezsiniz!";
                result = false;
            }

            if (EnterPartsSelectedBatchModel.SelectedBag == null)
            {
                DisplayMessage = "Bag bulunamadı.";
                result = false;
            }

            await Task.Run(() =>
            {
                BagService bagService = new BagService(_connectionString);
                BatchService batchService = new BatchService(_connectionString);
                var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
                SyncIssueManager syncIssueManager = new SyncIssueManager(ApplicationConfigurations.Instance.Configuration.RedisServer);


                var bagDeleteResult = bagService.Delete(new Bag { id = EnterPartsSelectedBatchModel.SelectedBag.BagId });
                if (bagDeleteResult)
                {

                    string serializedEntityObjectBag = JsonConvert.SerializeObject(new Bag { id = EnterPartsSelectedBatchModel.SelectedBag.BagId });

                    SyncIssue syncIssueBag = new SyncIssue
                    {
                        SerializedEntityObject = serializedEntityObjectBag,
                        EntityObjectType = typeof(Bag),
                        SyncDBCommand = SyncDBCommand.Delete,
                        FromToDirection = fromToDirection,
                        PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                        SyncStatus = SyncStatus.NoneProcessChangesPending,
                        TransferType = TransferType.NonProcessChanges,
                    };

                    syncIssueManager.CreateNewSyncIssue(syncIssueBag);
                }

                LotPropertyService lotPropertyService = new LotPropertyService(_connectionString);
                var lotPropertyEntity = lotPropertyService.GetByBagId(EnterPartsSelectedBatchModel.SelectedBag.BagId);

                foreach (var lotPropertyDetail in lotPropertyEntity)
                {
                    bool deleteResultLotProperty = lotPropertyService.Delete(new LotProperty { id = lotPropertyDetail.id });
                    if (deleteResultLotProperty)
                    {
                        
                        string serializedEntityObjectLotProperty = JsonConvert.SerializeObject(new LotProperty { id = lotPropertyDetail.id });

                        SyncIssue syncIssueLotProperty = new SyncIssue
                        {
                            SerializedEntityObject = serializedEntityObjectLotProperty,
                            EntityObjectType = typeof(LotProperty),
                            SyncDBCommand = SyncDBCommand.Delete,
                            FromToDirection = fromToDirection,
                            PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                            SyncStatus = SyncStatus.NoneProcessChangesPending,
                            TransferType = TransferType.NonProcessChanges,
                        };

                        syncIssueManager.CreateNewSyncIssue(syncIssueLotProperty);
                    }
                }

                var selectedBatch = batchService.GetById(EnterPartsSelectedBatchModel.SelectedBatch.id);
                selectedBatch.ModifiedByUserId = ActiveUser.id;
                batchService.Update(selectedBatch);

               
                string serializedEntityObject = JsonConvert.SerializeObject(selectedBatch);

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

                 syncIssueManager.CreateNewSyncIssue(syncIssue);


                result = true;
            });

            if (result)
            {
                EnterPartsSelectedBatchModel.EnterPartsBagDetails.Remove(EnterPartsSelectedBatchModel.SelectedBag);
            }

            return result;
        }
        #endregion

        #region LotProperties
        public void InsertLotProperty(LotProperty lotProperty)
        {
            LotPropertyService lotPropertyService = new LotPropertyService(_connectionString);

            bool result = lotPropertyService.Insert(lotProperty);

            if (result)
            {
                var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;

                string serializedEntityObject = JsonConvert.SerializeObject(lotProperty);

                SyncIssue syncIssue = new SyncIssue
                {
                    SerializedEntityObject = serializedEntityObject,
                    EntityObjectType = typeof(LotProperty),
                    SyncDBCommand = SyncDBCommand.Insert,
                    FromToDirection = fromToDirection,
                    PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                    SyncStatus = SyncStatus.NoneProcessChangesPending,
                    TransferType = TransferType.NonProcessChanges,
                };

                SyncIssueManager syncIssueManager = new SyncIssueManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
                syncIssueManager.CreateNewSyncIssue(syncIssue);
            }
        }
        public async Task UpdateLotProperty(LotProperty lotProperty)
        {
            bool result = await Task.Run(() =>
            {
                LotPropertyService lotPropertyService = new LotPropertyService(_connectionString);
                result = lotPropertyService.Update(lotProperty);

                if (result)
                {
                    var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;

                    string serializedEntityObject = JsonConvert.SerializeObject(lotProperty);

                    SyncIssue syncIssue = new SyncIssue
                    {
                        SerializedEntityObject = serializedEntityObject,
                        EntityObjectType = typeof(LotProperty),
                        SyncDBCommand = SyncDBCommand.Update,
                        FromToDirection = fromToDirection,
                        PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                        SyncStatus = SyncStatus.NoneProcessChangesPending,
                        TransferType = TransferType.NonProcessChanges,
                    };

                    SyncIssueManager syncIssueManager = new SyncIssueManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
                    syncIssueManager.CreateNewSyncIssue(syncIssue);
                }
                return result;
            });

            if (result)
            {
                EnterPartsSelectedBatchModel.SelectedLotProperties = EnterPartsSelectedBatchModel.SelectedLotProperties.ToObservableCollection();
            }
        }

        /// <summary>
        /// To keep lot property collection updated
        /// </summary>
        public void UpdateLotPropertyCollection()
        {
            if (EnterPartsSelectedBatchModel.SelectedBag == null)
                return;

            LotPropertyService lotPropertyService = new LotPropertyService(_connectionString);
            var updatedLotProperties = lotPropertyService.GetByBagId(EnterPartsSelectedBatchModel.SelectedBag.BagId);

            EnterPartsSelectedBatchModel.SelectedLotProperties.Clear();
            foreach (var updatedLotProperty in updatedLotProperties)
            {
                EnterPartsSelectedBatchModel.SelectedLotProperties.Add(updatedLotProperty);
            }
        }

        public int GetSelectedLotPropertyCreatedUserId(int lotPropertyId)
        {
            LotPropertyService lotPropertyService = new LotPropertyService(_connectionString);
            var lotProperty = lotPropertyService.GetById(lotPropertyId);

            if (lotProperty == null)
                return 0;

            return lotProperty.CreatedByUserId;
        }

        public async Task DeleteLotProperty(int lotPropertyId)
        {
            bool result = await Task.Run(() =>
            {
                LotPropertyService lotPropertyService = new LotPropertyService(_connectionString);
                LotProperty lotProperty = new LotProperty();
                lotProperty.id = lotPropertyId;
                result = lotPropertyService.Delete(lotProperty);

                if (result)
                {
                    var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;

                    string serializedEntityObject = JsonConvert.SerializeObject(lotProperty);

                    SyncIssue syncIssue = new SyncIssue
                    {
                        SerializedEntityObject = serializedEntityObject,
                        EntityObjectType = typeof(LotProperty),
                        SyncDBCommand = SyncDBCommand.Delete,
                        FromToDirection = fromToDirection,
                        PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                        SyncStatus = SyncStatus.NoneProcessChangesPending,
                        TransferType = TransferType.NonProcessChanges,
                    };

                    SyncIssueManager syncIssueManager = new SyncIssueManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
                    syncIssueManager.CreateNewSyncIssue(syncIssue);
                }

                return result;
            });

            if (result)
            {
                var lotProperties = EnterPartsSelectedBatchModel.SelectedLotProperties.Where(x => x.id == lotPropertyId);
                EnterPartsSelectedBatchModel.SelectedLotProperties = EnterPartsSelectedBatchModel.SelectedLotProperties.Except(lotProperties).ToObservableCollection();
                EnterPartsSelectedBatchModel.SelectedBag.LotPropertiesData = EnterPartsSelectedBatchModel.SelectedLotProperties;
            }

        }
        #endregion

        public EnterPartsSelectedBatchModel EnterPartsSelectedBatchModelByBatch(Batch batch)
        {
            BatchService batchService = new BatchService(_connectionString);

            Batch rawBatch = batchService.GetById(batch.id);
            batch.LoadNumber = rawBatch.LoadNumber;
            batch.BatchGroupId = rawBatch.BatchGroupId;

            EnterPartsSelectedBatchModel = new EnterPartsSelectedBatchModel
            {
                SelectedBatch = batch,
                PortListIsEnabled = true,
                PortSelectionIsEnabled = true
            };

            BagService bagService = new BagService(_connectionString);
            ActiveTagService activeTagService = new ActiveTagService(_connectionString);
            var activeTagsByTagIdKey = activeTagService.ActiveTagsByTagIdKey();

            var bags = bagService.BagsByBatch(batch.id);

            ObservableCollection<EnterPartsBagDetail> enterPartsBagDetails = new ObservableCollection<EnterPartsBagDetail>();

            EnterPartsSelectedBatchModel.SelectedPortListPTC = new ObservableCollection<EnterPartsPortDetail>();
            EnterPartsSelectedBatchModel.SelectedPortListMON = new ObservableCollection<EnterPartsPortDetail>();
            EnterPartsSelectedBatchModel.SelectedPortListVAC = new ObservableCollection<EnterPartsPortDetail>();
            EnterPartsSelectedBatchModel.SelectedLotProperties = new ObservableCollection<LotProperty>();

            foreach (var bag in bags)
            {
                LotPropertyService lotPropertyService = new LotPropertyService(_connectionString);
                var lotProperties = lotPropertyService.GetByBagId(bag.id);

                foreach (var lotProperty in lotProperties)
                {
                    EnterPartsSelectedBatchModel.SelectedLotProperties.Add(lotProperty);
                }

                EnterPartsBagDetail enterPartsBagDetail = new EnterPartsBagDetail
                {
                    BagId = bag.id,
                    BagName = bag.BagName,
                    BagNumeric = Convert.ToInt32(bag.BagName.Split('-')[2].Remove(0, 3)),
                    BatchId = bag.BatchId,
                    LotPropertiesData = lotProperties.ToObservableCollection()
                };

                enterPartsBagDetail.EnterPartsPortDetails = new ObservableCollection<EnterPartsPortDetail>();

                foreach (var port in bag.SelectedPorts)
                {
                    EnterPartsPortDetail enterPartsPortDetail = new EnterPartsPortDetail
                    {
                        SelectedPortLiteral = activeTagsByTagIdKey[port].TagName,
                        SelectedPortNumeric = Convert.ToInt32(activeTagsByTagIdKey[port].TagName.Remove(0, 3)),
                        SelectedPortTagId = port,
                        IsSelected = true
                    };

                    enterPartsBagDetail.EnterPartsPortDetails.Add(enterPartsPortDetail);

                    switch (activeTagsByTagIdKey[port].ActiveTagGroupId)
                    {
                        case ActiveTagGroups.PTC:
                            EnterPartsSelectedBatchModel.SelectedPortListPTC.Add(enterPartsPortDetail);
                            enterPartsPortDetail.ActiveTagGroup = ActiveTagGroups.PTC;
                            break;
                        case ActiveTagGroups.MON:
                            EnterPartsSelectedBatchModel.SelectedPortListMON.Add(enterPartsPortDetail);
                            enterPartsPortDetail.ActiveTagGroup = ActiveTagGroups.MON;
                            break;
                        case ActiveTagGroups.VAC:
                            EnterPartsSelectedBatchModel.SelectedPortListVAC.Add(enterPartsPortDetail);
                            enterPartsPortDetail.ActiveTagGroup = ActiveTagGroups.VAC;
                            break;
                    }
                }


                enterPartsBagDetails.Add(enterPartsBagDetail);
            }
            enterPartsBagDetails = enterPartsBagDetails.OrderBy(x => x.BagNumeric).ToObservableCollection();
            EnterPartsSelectedBatchModel.EnterPartsBagDetails = enterPartsBagDetails;
            EnterPartsSelectedBatchModel.SelectedPortListPTC = EnterPartsSelectedBatchModel.SelectedPortListPTC.OrderBy(x => x.SelectedPortNumeric).ToObservableCollection();
            EnterPartsSelectedBatchModel.SelectedPortListMON = EnterPartsSelectedBatchModel.SelectedPortListMON.OrderBy(x => x.SelectedPortNumeric).ToObservableCollection();
            EnterPartsSelectedBatchModel.SelectedPortListVAC = EnterPartsSelectedBatchModel.SelectedPortListVAC.OrderBy(x => x.SelectedPortNumeric).ToObservableCollection();


            return EnterPartsSelectedBatchModel;
        }
        public Dictionary<string, ActiveTag> AllActiveTagsByName()
        {
            ActiveTagService activeTagService = new ActiveTagService(_connectionString);
            return activeTagService.ActiveTagsByTagNameKey();
        }
        public Dictionary<int, ActiveTag> AllActiveTagsById()
        {
            ActiveTagService activeTagService = new ActiveTagService(_connectionString);
            return activeTagService.ActiveTagsByTagIdKey();
        }

        /// <summary>
        /// It'll set ports selected or not on the UI.
        /// </summary>
        /// <param name="portObject"></param>
        public async void SetPortToggleAsync(object portObject)
        {
            object[] parameters = (object[])portObject;
            ActiveTag activeTag = (ActiveTag)parameters[0];
            BagService bagService = new BagService(_connectionString);
            Bag bag = bagService.GetById(Convert.ToInt32(parameters[1]));
            var bagDetail = EnterPartsSelectedBatchModel.EnterPartsBagDetails.First(x => x.BagId == bag.id);
            int index = EnterPartsSelectedBatchModel.EnterPartsBagDetails.IndexOf(EnterPartsSelectedBatchModel.EnterPartsBagDetails.First(x => x.BagId == bag.id));
            var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
            SyncIssueManager syncIssueManager = new SyncIssueManager(ApplicationConfigurations.Instance.Configuration.RedisServer);



            if (bag.SelectedPorts.Contains(activeTag.id))
            {
                var enterPartsPortDetail = bagDetail.EnterPartsPortDetails.Where(x => x.SelectedPortTagId == activeTag.id);
                bagDetail.EnterPartsPortDetails = bagDetail.EnterPartsPortDetails.Except(enterPartsPortDetail).ToObservableCollection();
                EnterPartsSelectedBatchModel.EnterPartsBagDetails[index].EnterPartsPortDetails = bagDetail.EnterPartsPortDetails;
                bag.SelectedPorts = bag.SelectedPorts.Except(new int[] { activeTag.id }).ToArray();
                EnterPartsView.SetSelectAllCheckBox(false);

                await Task.Run(() =>
                {
                    bool result = bagService.Update(bag);

                    if (result)
                    {
                      
                        string serializedEntityObject = JsonConvert.SerializeObject(bag);

                        SyncIssue syncIssue = new SyncIssue
                        {
                            SerializedEntityObject = serializedEntityObject,
                            EntityObjectType = typeof(Bag),
                            SyncDBCommand = SyncDBCommand.Update,
                            FromToDirection = fromToDirection,
                            PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                            SyncStatus = SyncStatus.NoneProcessChangesPending,
                            TransferType = TransferType.NonProcessChanges,
                        };

                       syncIssueManager.CreateNewSyncIssue(syncIssue);
                    }
                });
            }
            else
            {
                EnterPartsPortDetail enterPartsPortDetail = new EnterPartsPortDetail
                {
                    IsEnabled = true,
                    SelectedPortTagId = activeTag.id,
                    IsSelected = true,
                    SelectedPortLiteral = activeTag.TagName,
                    SelectedPortNumeric = Convert.ToInt32(activeTag.TagName.Remove(0, 3)),
                    ActiveTagGroup = activeTag.ActiveTagGroupId
                };

                bagDetail.EnterPartsPortDetails.Add(enterPartsPortDetail);
                EnterPartsSelectedBatchModel.EnterPartsBagDetails[index].EnterPartsPortDetails = bagDetail.EnterPartsPortDetails;
                bag.SelectedPorts = bag.SelectedPorts.Concat(new int[] { activeTag.id }).ToArray();

                string portName = activeTag.TagName.Substring(0, 3);
                int totalEnabledPorts = 0;



                //if (portName == "PTC")
                //{
                //    totalEnabledPorts = EnterPartsView.GetEnabledPortButtonsByName("PTC").Count();

                //}
                //else if (portName == "VAC")
                //{
                //    totalEnabledPorts = EnterPartsView.GetEnabledPortButtonsByName("VAC").Count();

                //}
                //else if (portName == EnterPartsLanguageSettings["mon"])
                //{
                //    totalEnabledPorts = EnterPartsView.GetEnabledPortButtonsByName("MON").Count();

                //}


                switch (portName)
                {
                    case "PTC":
                        totalEnabledPorts = EnterPartsView.GetEnabledPortButtonsByName("PTC").Count();
                        break;
                    case "MON":
                        totalEnabledPorts = EnterPartsView.GetEnabledPortButtonsByName("MON").Count();
                        break;
                    case "VAC":
                        totalEnabledPorts = EnterPartsView.GetEnabledPortButtonsByName("VAC").Count();
                        break;
                    default:
                        break;
                }

                var selectedPortCount = EnterPartsSelectedBatchModel.EnterPartsBagDetails[index].EnterPartsPortDetails
                        .Where(b => b.SelectedPortLiteral.StartsWith(portName)).Count();

                if (selectedPortCount == totalEnabledPorts)
                    EnterPartsView.SetSelectAllCheckBox(true);

                await Task.Run(() =>
                {
                    bool result = bagService.Update(bag);
                    if (result)
                    {

                        string serializedEntityObject = JsonConvert.SerializeObject(bag);

                        SyncIssue syncIssue = new SyncIssue
                        {
                            SerializedEntityObject = serializedEntityObject,
                            EntityObjectType = typeof(Bag),
                            SyncDBCommand = SyncDBCommand.Update,
                            FromToDirection = fromToDirection,
                            PlcDeviceId = ApplicationConfigurations.Instance.Configuration.PlcDevice.Id,
                            SyncStatus = SyncStatus.NoneProcessChangesPending,
                            TransferType = TransferType.NonProcessChanges,
                        };

                        syncIssueManager.CreateNewSyncIssue(syncIssue);
                    }

                });

            }

            EnterPartsSelectedBatchModel.SelectedPortListPTC = bagDetail.EnterPartsPortDetails.Where(x => x.ActiveTagGroup == ActiveTagGroups.PTC).OrderBy(x => x.SelectedPortNumeric).ToObservableCollection();
            EnterPartsSelectedBatchModel.SelectedPortListMON = bagDetail.EnterPartsPortDetails.Where(x => x.ActiveTagGroup == ActiveTagGroups.MON).OrderBy(x => x.SelectedPortNumeric).ToObservableCollection();
            EnterPartsSelectedBatchModel.SelectedPortListVAC = bagDetail.EnterPartsPortDetails.Where(x => x.ActiveTagGroup == ActiveTagGroups.VAC).OrderBy(x => x.SelectedPortNumeric).ToObservableCollection();
        }

        public void SetAllPortsOrNot(string portName, bool isSetTrue, IEnumerable<int> disabledPortNumbers)
        {
            BagService bagService = new BagService(_connectionString);
            Bag bag = bagService.GetById(EnterPartsSelectedBatchModel.SelectedBag.BagId);
            var bagDetail = EnterPartsSelectedBatchModel.EnterPartsBagDetails.First(x => x.BagId == bag.id);
            int index = EnterPartsSelectedBatchModel.EnterPartsBagDetails.IndexOf(EnterPartsSelectedBatchModel.EnterPartsBagDetails.First(x => x.BagId == bag.id));
            



            var ports = AllActiveTagsByName().Where(a => a.Key.StartsWith(portName)).Select(a => a.Value);

            if (isSetTrue)
            {
                foreach (var item in ports)
                {
                    int portNo = Convert.ToInt32(item.TagName.Remove(0, 3));
                    if (bag.SelectedPorts.Contains(item.id) || disabledPortNumbers.Contains(portNo))
                        continue;

                    EnterPartsPortDetail enterPartsPortDetail = new EnterPartsPortDetail
                    {
                        IsEnabled = true,
                        SelectedPortTagId = item.id,
                        IsSelected = true,
                        SelectedPortLiteral = item.TagName,
                        SelectedPortNumeric = portNo,
                        ActiveTagGroup = item.ActiveTagGroupId
                    };

                    bagDetail.EnterPartsPortDetails.Add(enterPartsPortDetail);
                    EnterPartsSelectedBatchModel.EnterPartsBagDetails[index].EnterPartsPortDetails = bagDetail.EnterPartsPortDetails;
                    bag.SelectedPorts = bag.SelectedPorts.Concat(new int[] { item.id }).ToArray();
                }
            }
            else
            {
                ActiveTagGroups selectedGroup;


                //if (portName == "PTC")
                //{
                //    selectedGroup = ActiveTagGroups.PTC;
                //} else if (portName == "VAC")
                //{
                //    selectedGroup = ActiveTagGroups.VAC;
                //}
                //else if(portName == EnterPartsLanguageSettings["mon"])
                //{
                //    selectedGroup = ActiveTagGroups.MON;
                //}else
                //{
                //    selectedGroup = ActiveTagGroups.DefaultSensor;
                //}


                switch (portName)
                {
                    case "PTC":
                        selectedGroup = ActiveTagGroups.PTC;
                        break;
                    case "MON":
                        selectedGroup = ActiveTagGroups.MON;
                        break;
                    case "VAC":
                        selectedGroup = ActiveTagGroups.VAC;
                        break;
                    default:
                        selectedGroup = ActiveTagGroups.DefaultSensor;
                        break;
                }

                var toBeExcludedPortDetail = bagDetail.EnterPartsPortDetails.Where(x => x.ActiveTagGroup == selectedGroup);
                bagDetail.EnterPartsPortDetails = bagDetail.EnterPartsPortDetails.Except(toBeExcludedPortDetail).ToObservableCollection();
                EnterPartsSelectedBatchModel.EnterPartsBagDetails[index].EnterPartsPortDetails = bagDetail.EnterPartsPortDetails;

                List<int> toBeExcludedPorts = new List<int>();

                foreach (var item in toBeExcludedPortDetail)
                {
                    toBeExcludedPorts.Add(item.SelectedPortTagId);
                }

                // Exclude all selected ports from bag
                bag.SelectedPorts = bag.SelectedPorts.Except(toBeExcludedPorts).ToArray();
            }



            //if (portName == "PTC")
            //{
            //    EnterPartsSelectedBatchModel.SelectedPortListPTC = bagDetail.EnterPartsPortDetails.Where(x => x.ActiveTagGroup == ActiveTagGroups.PTC).OrderBy(x => x.SelectedPortNumeric).ToObservableCollection();

            //}
            //else if (portName == "VAC")
            //{
            //    EnterPartsSelectedBatchModel.SelectedPortListVAC = bagDetail.EnterPartsPortDetails.Where(x => x.ActiveTagGroup == ActiveTagGroups.VAC).OrderBy(x => x.SelectedPortNumeric).ToObservableCollection();

            //}
            //else if (portName == EnterPartsLanguageSettings["mon"])
            //{
            //    EnterPartsSelectedBatchModel.SelectedPortListMON = bagDetail.EnterPartsPortDetails.Where(x => x.ActiveTagGroup == ActiveTagGroups.MON).OrderBy(x => x.SelectedPortNumeric).ToObservableCollection();

            //}


            switch (portName)
            {
                case "PTC":
                    EnterPartsSelectedBatchModel.SelectedPortListPTC = bagDetail.EnterPartsPortDetails.Where(x => x.ActiveTagGroup == ActiveTagGroups.PTC).OrderBy(x => x.SelectedPortNumeric).ToObservableCollection();
                    break;
                case "MON":
                    EnterPartsSelectedBatchModel.SelectedPortListMON = bagDetail.EnterPartsPortDetails.Where(x => x.ActiveTagGroup == ActiveTagGroups.MON).OrderBy(x => x.SelectedPortNumeric).ToObservableCollection();
                    break;
                case "VAC":
                    EnterPartsSelectedBatchModel.SelectedPortListVAC = bagDetail.EnterPartsPortDetails.Where(x => x.ActiveTagGroup == ActiveTagGroups.VAC).OrderBy(x => x.SelectedPortNumeric).ToObservableCollection();
                    break;
                default:
                    break;
            }

            // Update db
            var result = bagService.Update(bag);
            if (result)
            {
                var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;

                string serializedEntityObject = JsonConvert.SerializeObject(bag);

                SyncIssue syncIssue = new SyncIssue
                {
                    SerializedEntityObject = serializedEntityObject,
                    EntityObjectType = typeof(Bag),
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

        public void InsertProcessEventLogToDb(string eventText)
        {
            ProcessEventLog enterPartsEventLog = new ProcessEventLog();
            // Get currentBatch object from the cache
            if (ProcessManager.Instance.CurrentProcess != null)
                enterPartsEventLog.BatchId = ProcessManager.Instance.CurrentProcess.BatchId;

            enterPartsEventLog.CreateDate = DateTime.Now;
            enterPartsEventLog.Type = ProcessEventLogType.Manual.ToString();
            enterPartsEventLog.ModifiedByUserId = ActiveUser.id;
            enterPartsEventLog.EventText = eventText;
            _processEventLogService.Insert(enterPartsEventLog);
            
            var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
            ProcessEventLogAdapter processEventLogAdapter = new ProcessEventLogAdapter(ApplicationConfigurations.Instance.Configuration.RedisServer);
            processEventLogAdapter.CreateProcessEventLogSyncIssue(enterPartsEventLog, fromToDirection, ApplicationConfigurations.Instance.Configuration.PlcDevice.Id);



        }

        private void ViewReport()
        {
            ReportCreator reportCreator = new ReportCreator(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);

            XtraReport xtraReportItem = null;
            int batchId = EnterPartsSelectedBatchModel?.SelectedBatch?.id ?? 0;
            if (batchId == 0)
                return;

            xtraReportItem = reportCreator.BatchReportByEnterParts(batchId);
            ReportViewer reportViewer = new ReportViewer(xtraReportItem);
            reportViewer.ShowDialog();
        }

        public bool GetIntegrityCheckStatus()
        {
            return _appViewModel.IntegrityCheckRunVal;
        }
    }
}
