using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.Views;
using RevoScada.DesktopApplication.Views.Popups;
using RevoScada.Entities;
using RevoScada.ProcessController;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RevoScada.DesktopApplication.ViewModels
{
    public class QualityVM : UserControlBaseVM
    {
        #region Services
        private readonly string _connectionString;
        private BatchQualityService _batchQualityService;
        private BatchQualityDetailService _batchQualityDetailService;
        private DialogService _dialogService;
        #endregion

        #region Collections
        private ObservableCollection<BatchQualityModel> _qualityCards;
        public ObservableCollection<BatchQualityModel> QualityCards
        {
            get => _qualityCards;
            set => OnPropertyChanged(ref _qualityCards, value);
        }
        private ObservableCollection<BatchQualityDetailModel> _phaseCards;
        public ObservableCollection<BatchQualityDetailModel> PhaseCards 
        {
            get => _phaseCards;
            set => OnPropertyChanged(ref _phaseCards, value);
        }
        public ObservableCollection<string> CriteriaList { get; set; }
        public Dictionary<string, string> PhaseChangeValues { get; set; }
        #endregion

        #region ICommands
        public RelayCommand AddQualityCardCommand { get; set; }
        public RelayCommand AddPhaseCardCommand { get; set; }
        public RelayCommand SaveAllCommand { get; set; }
        public RelayCommand DeletePhaseCardCommand { get; set; }
        public RelayCommand MoveToPhaseUpCommand { get; set; }
        public RelayCommand MoveToPhaseDownCommand { get; set; }
        public RelayCommand DeleteQualityCardCommand { get; set; }
        public RelayCommand EditQualityCardCommand { get; set; }
        public RelayCommand EditPhaseSettingsCommand { get; set; }
        #endregion

        #region Fields
        public Quality QualityView;
        #endregion

        #region Properties

        private string _vacuumUnitTitle;
        public string VacuumUnitTitle
        {
            get => _vacuumUnitTitle;
            set => OnPropertyChanged(ref _vacuumUnitTitle, value);
        }



        private BatchQualityModel _selectedQualityCard;
        public BatchQualityModel SelectedQualityCard
        {
            get => _selectedQualityCard;
            set
            {
                OnPropertyChanged(ref _selectedQualityCard, value);
                if(_selectedQualityCard != null)
                    GetPhaseCardsByQualityId(_selectedQualityCard.id);
            }
        }
        private BatchQualityDetailModel _selectedPhaseCard;
        public BatchQualityDetailModel SelectedPhaseCard
        {
            get => _selectedPhaseCard;
            set
            {
                OnPropertyChanged(ref _selectedPhaseCard, value);
                if (_selectedPhaseCard != null)
                    IsPhaseItemAvailable = true;
                else
                    IsPhaseItemAvailable = false;
            }
        }
        private bool _isAddPhaseButtonEnabled;
        public bool IsAddPhaseButtonEnabled 
        {
            get => _isAddPhaseButtonEnabled;
            set => OnPropertyChanged(ref _isAddPhaseButtonEnabled, value);
        }
        private bool _allowEditingPhaseSettings;
        public bool AllowEditingPhaseSettings
        {
            get => _allowEditingPhaseSettings;
            set 
            {
                OnPropertyChanged(ref _allowEditingPhaseSettings, value);
                if (value || AllowEditingCardSettings)
                    IsSaveAllButtonEnabled = true;
                else
                    IsSaveAllButtonEnabled = false;
            }
        }
        private bool _allowEditingCardSettings;
        public bool AllowEditingCardSettings
        {
            get => _allowEditingCardSettings;
            set
            {
                OnPropertyChanged(ref _allowEditingCardSettings, value);
                if (value || AllowEditingPhaseSettings)
                    IsSaveAllButtonEnabled = true;
                else
                    IsSaveAllButtonEnabled = false;
            }
        }
        private bool _isQualityItemSelected;
        public bool IsQualityItemSelected
        {
            get => _isQualityItemSelected;
            set => OnPropertyChanged(ref _isQualityItemSelected, value);
        }

        private bool _isPhaseItemSelected;
        public bool IsPhaseItemSelected 
        {
            get => _isPhaseItemSelected;
            set => OnPropertyChanged(ref _isPhaseItemSelected, value);
        }

        private bool _isPhaseItemAvailable;
        public bool IsPhaseItemAvailable
        {
            get => _isPhaseItemAvailable;
            set => OnPropertyChanged(ref _isPhaseItemAvailable, value);
        }

        private bool _isQualityEditButtonEnabled;
        public bool IsQualityEditButtonEnabled
        {
            get => _isQualityEditButtonEnabled;
            set => OnPropertyChanged(ref _isQualityEditButtonEnabled, value);
        }

        private bool _isPhaseEditButtonEnabled;
        public bool IsPhaseEditButtonEnabled
        {
            get => _isPhaseEditButtonEnabled;
            set => OnPropertyChanged(ref _isPhaseEditButtonEnabled, value);
        }

        private bool _isSaveAllButtonEnabled;
        public bool IsSaveAllButtonEnabled
        {
            get => _isSaveAllButtonEnabled;
            set => OnPropertyChanged(ref _isSaveAllButtonEnabled, value);
        }

        private string _wizardText;
        public string WizardText
        {
            get => _wizardText;
            set => OnPropertyChanged(ref _wizardText, value);
        }
        #endregion

        public QualityVM()
        {
            _connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;
            _batchQualityService = new BatchQualityService(_connectionString);
            _batchQualityDetailService = new BatchQualityDetailService(_connectionString);
            _dialogService = new DialogService();

            QualityCards = new ObservableCollection<BatchQualityModel>();
            LoadQualityCards();

            PhaseCards = new ObservableCollection<BatchQualityDetailModel>();
            CriteriaList = new ObservableCollection<string>() { string.Empty, "<", ">", ">=", "<=" };
            PhaseChangeValues = new Dictionary<string, string>()
            {
                {string.Empty, string.Empty },
                {"Air_Tc", "Air Temp" },
                {"Low_Tc", "Min Part Temp" },
                {"High_Tc", "Max Part Temp" },
                {"Low_Mon", "Min Probe" },
                {"High_Mon", "Max Probe" },
                {"Segment_No", "Segment" },
                {"End", "End" }
            };

            AddQualityCardCommand = new RelayCommand(AddQualityCard);
            AddPhaseCardCommand = new RelayCommand(AddPhaseCard);
            MoveToPhaseUpCommand = new RelayCommand(MoveToPhaseUp);
            MoveToPhaseDownCommand = new RelayCommand(MoveToPhaseDown);
            DeletePhaseCardCommand = new RelayCommand(DeletePhaseCard);
            SaveAllCommand = new RelayCommand(SaveAll);
            DeleteQualityCardCommand = new RelayCommand(DeleteQualityCard);
            EditQualityCardCommand = new RelayCommand(EditQualityCard);
            EditPhaseSettingsCommand = new RelayCommand(EditPhaseSettings);

            WizardText = "Lütfen kalite kartı seçiniz veya ekleyiniz.";

            VacuumUnitTitle = ProcessManager.Instance.ApplicationProperties["VacuumUnitTitle"].Value;
        }

        private void LoadQualityCards()
        {
            QualityCards = _batchQualityService.GetAll().OrderBy(q => q.SortOrder).Select(q => new BatchQualityModel
            {
                id = q.id,
                CardName = q.CardName,
                Description = q.Description,
                LastModified = q.LastModified,
                SortOrder = q.SortOrder
            }).ToObservableCollection();
        }

        private void AddQualityCard()
        {
            var result = AddQualityCardResult();

            if(!result)
                _dialogService.WinUIMessageBoxShowResult("Veritabanına yeni kalite kartı ekleme işlemi başarısız oldu. Lütfen tekrar deneyiniz.", "Başarısız",
                                                          MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private bool AddQualityCardResult()
        {
            BatchQualityModel newBatchQualityModel = new BatchQualityModel();
            newBatchQualityModel.CardName = "New Card";
            newBatchQualityModel.Description = string.Empty;
            newBatchQualityModel.LastModified = DateTime.Now;
            if (QualityCards.Count > 0)
                newBatchQualityModel.SortOrder = (short)(QualityCards.Max(q => q.SortOrder) + 1);
            else
                newBatchQualityModel.SortOrder = 1;

            BatchQuality batchQuality = new BatchQuality();
            batchQuality.CardName = newBatchQualityModel.CardName;
            batchQuality.Description = newBatchQualityModel.Description;
            batchQuality.LastModified = newBatchQualityModel.LastModified;
            batchQuality.SortOrder = newBatchQualityModel.SortOrder;

            bool isItSuccess = _batchQualityService.Insert(batchQuality);
            int getLastAddedItemId = 0;

            if (isItSuccess)
            {
                getLastAddedItemId = _batchQualityService.GetAll().Max(q => q.id);
            } else
            {
                return false;
            }

            newBatchQualityModel.id = getLastAddedItemId;
            QualityCards.Add(newBatchQualityModel);

            return true;
        }

        private void AddPhaseCard()
        {
            BatchQualityDetail newBatchQualityDetail = new BatchQualityDetail();
            newBatchQualityDetail.PhaseName = "New Phase";
            newBatchQualityDetail.LastModified = DateTime.Now;
            newBatchQualityDetail.BatchQualityId = SelectedQualityCard.id;
            if(PhaseCards.Count > 0)
                newBatchQualityDetail.SortOrder = (short)(PhaseCards.Max(q => q.SortOrder) + 1);
            else
                newBatchQualityDetail.SortOrder = 1;

            BatchQualityDetailModel newBatchQualityDetailModel = new BatchQualityDetailModel();
            newBatchQualityDetailModel.PhaseName = newBatchQualityDetail.PhaseName;
            newBatchQualityDetailModel.LastModified = newBatchQualityDetail.LastModified;
            newBatchQualityDetailModel.BatchQualityId = newBatchQualityDetail.BatchQualityId;
            newBatchQualityDetailModel.SortOrder = newBatchQualityDetail.SortOrder;

            _batchQualityDetailService.Insert(newBatchQualityDetail);

            GetPhaseCardsByQualityId(SelectedQualityCard.id);
        }

        private void DeletePhaseCard()
        {
            if (SelectedPhaseCard == null)
            {
                _dialogService.WinUIMessageBoxShowResult("Lütfen silme işlemi gerçekleştirmek için faz seçimi yapınız.", "Uyarı", 
                                                           MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var isResultConfirmed = _dialogService.WinUIMessageBoxShowResult("Seçili fazı silmek istediğinize emin misiniz?", "Soru", 
                                                                              MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (isResultConfirmed == MessageBoxResult.No)
                return;

            bool deletePhaseCardResult = DeletePhaseCardResult();

            if(!deletePhaseCardResult)
                _dialogService.WinUIMessageBoxShowResult("Seçili fazı silme işlemi başarısız oldu. Lütfen tekrar deneyiniz.", "Başarısız", 
                                                          MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private bool DeletePhaseCardResult()
        {
            bool deleteResult = _batchQualityDetailService.Delete(new BatchQualityDetail { id = SelectedPhaseCard.id });

            if (!deleteResult)
                return false;

            var allPhases = _batchQualityDetailService.GetAllByQualityBatchId(SelectedPhaseCard.BatchQualityId).OrderBy(p => p.SortOrder);

            if(allPhases.Count() == 0)
            {
                // Do not proceed...
                PhaseCards.Clear();
                return true;
            }

            short sortValue = 1;
            foreach (BatchQualityDetail phaseItem in allPhases)
            {
                phaseItem.SortOrder = sortValue;
                _batchQualityDetailService.Update(phaseItem);
                sortValue++;
            }

            // Update UI
            GetPhaseCardsByQualityId(SelectedPhaseCard.BatchQualityId);

            return true;
        }

        private void DeleteQualityCard()
        {
            if (SelectedQualityCard == null)
            {
                _dialogService.WinUIMessageBoxShowResult("Lütfen silme işlemini gerçekleştirmek için kalite kartı seçimi yapınız.", "Uyarı",
                                                           MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var isResultConfirmed = _dialogService.WinUIMessageBoxShowResult("Seçili kalite kartını silmek istediğinize emin misiniz?", "Soru",
                                                                              MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (isResultConfirmed == MessageBoxResult.No)
                return;

            bool deleteQualityCardResult = DeleteQualityCardResult();

            if (!deleteQualityCardResult)
                _dialogService.WinUIMessageBoxShowResult("Seçili kalite kartı silme işlemi başarısız oldu. Lütfen tekrar deneyiniz.", "Uyarı",
                                                          MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private bool DeleteQualityCardResult()
        {
            int selectedQualityCardId = SelectedQualityCard.id;
            var deleteResult = _batchQualityService.Delete(new BatchQuality { id = selectedQualityCardId });

            if (!deleteResult)
                return false;

            var allQualityCards = _batchQualityService.GetAll().OrderBy(q => q.SortOrder);

            if(allQualityCards.Count() == 0)
            {
                QualityCards.Clear();
                return true;
            }

            short sortValue = 1;
            foreach (BatchQuality qualityItem in allQualityCards)
            {
                qualityItem.SortOrder = sortValue;
                _batchQualityService.Update(qualityItem);
                sortValue++;
            }

            // Update UI
            QualityCards = _batchQualityService.GetAll().OrderBy(q => q.SortOrder).Select(q => new BatchQualityModel
            {
                id = q.id,
                CardName = q.CardName,
                Description = q.Description,
                LastModified = q.LastModified,
                SortOrder = q.SortOrder
            }).ToObservableCollection();

            // Delete all phases of this quality card
            var selectedPhases = _batchQualityDetailService.GetAllByQualityBatchId(selectedQualityCardId);

            foreach (BatchQualityDetail phase in selectedPhases)
            {
                _batchQualityDetailService.Delete(phase);
            }

            // Clear phases from UI
            PhaseCards.Clear();

            return true;
        }

        private void EditQualityCard(object param)
        {
            bool isChecked = (bool)param;

            if(isChecked)
            {
                if (IsQualityItemSelected)
                    AllowEditingCardSettings = true;
            }
            else
            {
                AllowEditingCardSettings = false;
            }
        }

        private void EditPhaseSettings(object param)
        {
            bool isChecked = (bool)param;

            if (isChecked)
            {
                if (SelectedPhaseCard != null)
                    AllowEditingPhaseSettings = true;
            }
            else
            {
                AllowEditingPhaseSettings = false;
            }
        }

        //todo:l Refactor this code
        private void GetPhaseCardsByQualityId(int batchQualityId)
        {
            PhaseCards = _batchQualityDetailService.GetAllByQualityBatchId(batchQualityId).Select(q => new BatchQualityDetailModel()
            {
                id = q.id,
                BatchQualityId = q.BatchQualityId,
                AirTempMin = q.AirTempMin,
                AirTempMax = q.AirTempMax,
                AirTempStyle = q.AirTempStyle,
                AirTempTitle = q.AirTempTitle,
                LastModified = q.LastModified,
                PartTempHighRange = q.PartTempHighRange,
                PartTempLowRange = q.PartTempLowRange,
                PartTempRateCalcInterval = q.PartTempRateCalcInterval,
                PartTempRateMax = q.PartTempRateMax,
                PartTempRateMin = q.PartTempRateMin,
                PartTempStyle = q.PartTempStyle,
                PartTempTitle = q.PartTempTitle,
                PhaseChange = q.PhaseChange,
                PhaseCriteria = q.PhaseCriteria,
                PhaseCriteriaValue = q.PhaseCriteriaValue,
                PhaseMaxTime = q.PhaseMaxTime,
                PhaseMinTime = q.PhaseMinTime,
                PhaseName = q.PhaseName,
                PhaseStyle = q.PhaseStyle,
                PhaseTitle = q.PhaseTitle,
                PressurePhaseEndMax = q.PressurePhaseEndMax,
                PressurePhaseEndMin = q.PressurePhaseEndMin,
                PressurePhaseStartMax = q.PressurePhaseStartMax,
                PressurePhaseStartMin = q.PressurePhaseStartMin,
                PressureRateMax = q.PressureRateMax,
                PressureRateMin = q.PressureRateMin,
                PressureStyle = q.PressureStyle,
                PressureTitle = q.PressureTitle,
                ProbePhaseEndMax = q.ProbePhaseEndMax,
                ProbePhaseEndMin = q.ProbePhaseEndMin,
                ProbePhaseStartMax = q.ProbePhaseStartMax,
                ProbePhaseStartMin = q.ProbePhaseStartMin,
                ProbeStyle = q.ProbeStyle,
                ProbeTitle = q.ProbeTitle,
                SortOrder = q.SortOrder
            }).OrderBy(q => q.SortOrder).ToObservableCollection();

            if(PhaseCards.Count == 0)
            {
                SelectedPhaseCard = null;
                AllowEditingPhaseSettings = false;
            }
        }

        private bool SaveAllResult()
        {
            bool qualityServiceResult = true;
            bool qualityDetailServiceResult = true;

            BatchQualityModel retrievedBatchQualityModel = QualityView.GetFormValues();

            BatchQualityDetail retrievedBatchQualityDetail = new BatchQualityDetail();
            if (AllowEditingPhaseSettings)
                retrievedBatchQualityDetail = QualityView.GetQualityDetailValues();

            SelectedQualityCard.CardName = retrievedBatchQualityModel?.CardName ?? string.Empty;
            SelectedQualityCard.Description = retrievedBatchQualityModel?.Description ?? string.Empty;
            SelectedQualityCard.LastModified = retrievedBatchQualityModel.LastModified;

            BatchQuality updatedBatchQuality = new BatchQuality();
            updatedBatchQuality.id = SelectedQualityCard.id;
            updatedBatchQuality.CardName = retrievedBatchQualityModel.CardName;
            updatedBatchQuality.Description = retrievedBatchQualityModel.Description;
            updatedBatchQuality.LastModified = retrievedBatchQualityModel.LastModified;
            updatedBatchQuality.SortOrder = SelectedQualityCard.SortOrder;

            qualityServiceResult = _batchQualityService.Update(updatedBatchQuality);

            if (!qualityServiceResult)
                return false;

            if (SelectedPhaseCard != null && AllowEditingPhaseSettings)
            {
                retrievedBatchQualityDetail.id = SelectedPhaseCard.id;
                retrievedBatchQualityDetail.BatchQualityId = SelectedPhaseCard.BatchQualityId;
                retrievedBatchQualityDetail.SortOrder = SelectedPhaseCard.SortOrder;
                retrievedBatchQualityDetail.LastModified = DateTime.Now;

                qualityDetailServiceResult = _batchQualityDetailService.Update(retrievedBatchQualityDetail);
                GetPhaseCardsByQualityId(SelectedPhaseCard.BatchQualityId);
            }

            if (!qualityServiceResult || !qualityDetailServiceResult)
                return false;

            // Both quality card details and phase details have been succesfully saved to db.
            return true;
        }

        private void SaveAll()
        {
            bool saveResult = SaveAllResult();

            if (saveResult)
                QualityView.DbResultPositiveFadeOutAnim.Begin();
            else
                QualityView.DbResultNegativeFadeOutAnim.Begin();

            //SelectedPhaseCard = null;
            AllowEditingCardSettings = false;
            AllowEditingPhaseSettings = false;
        }

        private void ChangePhaseSortOrder(bool isMoveUp)
        {
            if (SelectedPhaseCard == null)
                return;

            short curSortValue = SelectedPhaseCard.SortOrder;

            if (isMoveUp)
                curSortValue -= 1;
            else
                curSortValue += 1;

            if (curSortValue == 0)
                return;

            var specificBatchQualityDetail = _batchQualityDetailService.GetAllByQualityBatchId(SelectedPhaseCard.BatchQualityId)
                                             .Where(q => q.SortOrder == curSortValue).FirstOrDefault();

            if(_batchQualityDetailService.GetAllByQualityBatchId(SelectedPhaseCard.BatchQualityId).Count() == 1)
                return;

            SelectedPhaseCard.SortOrder = curSortValue;

            if (specificBatchQualityDetail != null)
            {
                if (isMoveUp)
                    specificBatchQualityDetail.SortOrder += 1;
                else
                    specificBatchQualityDetail.SortOrder -= 1;

                _batchQualityDetailService.Update(specificBatchQualityDetail);
            }

            BatchQualityDetail updatedBatchQualityDetail = QualityView.GetQualityDetailValues();
            updatedBatchQualityDetail.id = SelectedPhaseCard.id;
            updatedBatchQualityDetail.BatchQualityId = SelectedPhaseCard.BatchQualityId;
            updatedBatchQualityDetail.SortOrder = SelectedPhaseCard.SortOrder;
            _batchQualityDetailService.Update(updatedBatchQualityDetail);

            var allPhases = _batchQualityDetailService.GetAllByQualityBatchId(SelectedPhaseCard.BatchQualityId).OrderBy(p => p.SortOrder);

            short sortValue = 1;
            foreach (BatchQualityDetail phaseItem in allPhases)
            {
                phaseItem.SortOrder = sortValue;
                _batchQualityDetailService.Update(phaseItem);
                sortValue++;
            }

            GetPhaseCardsByQualityId(SelectedPhaseCard.BatchQualityId);
        }

        private void MoveToPhaseUp()
        {
            ChangePhaseSortOrder(true);
        }

        private void MoveToPhaseDown()
        {
            ChangePhaseSortOrder(false);
        }
    }
}
