using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.WindowsUI;
using Revo.Core;
using RevoScada.ProcessController;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.ViewModels;
using RevoScada.Entities;
using RevoScada.Entities.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RevoScada.DesktopApplication.Views.Popups;
using RevoScada.Configurator;
using RevoScada.Business;
using DevExpress.Xpf.LayoutControl;
using RevoScada.DesktopApplication.Models.SettingModels;
using Newtonsoft.Json;

namespace RevoScada.DesktopApplication.Views
{
    /// <summary>
    /// Interaction logic for Enter_Parts2.xaml
    /// </summary>
    public partial class Enter_Parts : UserControl
    {

        private EnterPartsVM _viewModel;

        public Enter_Parts()
        {
            InitializeComponent();

            setOrderOfPort();
        }

        private void setOrderOfPort() 
        {
            ApplicationPropertyService _applicationPropertyService = new ApplicationPropertyService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
            var SetOrderByPort = _applicationPropertyService.GetByName("SetOrderByPort").Value;
            OrderByPortInEnterPart orderByPortInEnterPart = JsonConvert.DeserializeObject<OrderByPortInEnterPart>(SetOrderByPort);

            int ptcIndex = orderByPortInEnterPart.ptcIndex;
            int monIndex = orderByPortInEnterPart.monIndex;
            int vacIndex = orderByPortInEnterPart.vacIndex;

            portTabControl.Items.Remove(monTab);
            portTabControl.Items.Remove(ptcTab);
            portTabControl.Items.Remove(vacTab);

            if (ptcIndex == 0)
            {
                portTabControl.Items.Insert(ptcIndex, ptcTab);
                if (monIndex == 1)
                {
                    portTabControl.Items.Insert(monIndex, monTab);
                    portTabControl.Items.Insert(vacIndex, vacTab);
                }
                else
                {
                    portTabControl.Items.Insert(vacIndex, vacTab);
                    portTabControl.Items.Insert(monIndex, monTab);
                }
            }
            else if (monIndex == 0)
            {
                portTabControl.Items.Insert(monIndex, monTab);
                if (ptcIndex == 1)
                {
                    portTabControl.Items.Insert(ptcIndex, ptcTab);
                    portTabControl.Items.Insert(vacIndex, vacTab);
                }
                else
                {
                    portTabControl.Items.Insert(vacIndex, vacTab);
                    portTabControl.Items.Insert(ptcIndex, ptcTab);
                }

            }
            else if (vacIndex == 0)
            { 
                portTabControl.Items.Insert(vacIndex, vacTab);
                if (ptcIndex == 1 )
                {
                    portTabControl.Items.Insert(ptcIndex, ptcTab);
                    portTabControl.Items.Insert(monIndex, monTab);
                }
                else
                {
                    portTabControl.Items.Insert(monIndex, monTab);
                    portTabControl.Items.Insert(ptcIndex, ptcTab); 
                }
            }


        }

        private async void DockPanel_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1,
                (Duration)TimeSpan.FromSeconds(1));
            this.BeginAnimation(UIElement.OpacityProperty, animation);

            await DelayedCurrentBatchLoader();
        }

        private async Task DelayedCurrentBatchLoader()
        {
            await Task.Delay(10);

            if (_viewModel.EnterPartsUIElementStates.LayoutGroupBagsVisibility.Equals("Collapsed"))
                return;

            if (_viewModel.CurrentBatch?.id > 0)
                BtnViewCurrentPorts_Click(this, null);
            else
                _viewModel.EnterPartsUIElementStates.LayoutGroupBagsVisibility = Visibility.Collapsed.ToString();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as EnterPartsVM;
            _viewModel.EnterPartsView = this;
            await _viewModel.CheckCurrentBatch();
            ListBoxPrepareParts.Items.Clear();
            await _viewModel.FillPrepareParts();
            ListBoxCompletedBatches.Items.Clear();
            await _viewModel.FillCompletedBatches();
        }

        #region StartupMenu     
        private async void BtnShowPreparePartsList_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.EnterPartsUIElementStates.LayoutGroupInitialCommandsVisibility = Visibility.Collapsed.ToString();
            _viewModel.EnterPartsUIElementStates.LayoutGroupPreparePartsVisibility = Visibility.Visible.ToString();
            _viewModel.EnterPartsUIElementStates.LayoutGroupBagsVisibility = Visibility.Collapsed.ToString();
            _viewModel.SaveEnterPartsUIElementStatesSettings();
            await _viewModel.FillPrepareParts();
            ListBoxPrepareParts.SelectedIndex = -1;
        }
        private async void BtnSkipParts_Click(object sender, RoutedEventArgs e)
        {

            if (ProcessManager.Instance.IsBatchRunning())
            {
                WinUIMessageBox.Show("Batch çalışır durumdayken değişiklik yapılamaz!", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            MessageBoxResult dialogResult = WinUIMessageBox.Show($"Batch skip yapmak istediğinize emin misiniz?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (dialogResult == MessageBoxResult.Yes)
            {
                bool result = await _viewModel.SkipEnterParts();

                if (!result)
                {
                    WinUIMessageBox.Show(_viewModel.DisplayMessage, "Batch Skip", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    _viewModel.EnterPartsUIElementStates.SkipPartsButtonIsEnabled = false;
                    _viewModel.EnterPartsUIElementStates.LayoutGroupBagsVisibility = Visibility.Collapsed.ToString();
                    _viewModel.EnterPartsUIElementStates.LayoutGroupBagsVisibility = Visibility.Collapsed.ToString();
                    _viewModel.SaveEnterPartsUIElementStatesSettings();
                }
            }
        }
        private async void BtnShowCompletedBatchesList_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.EnterPartsUIElementStates.LayoutGroupInitialCommandsVisibility = Visibility.Collapsed.ToString();
            _viewModel.EnterPartsUIElementStates.LayoutGroupCompletedBatchesVisibility = Visibility.Visible.ToString();
            _viewModel.EnterPartsUIElementStates.LayoutGroupBagsVisibility = Visibility.Collapsed.ToString();
            _viewModel.SaveEnterPartsUIElementStatesSettings();


            await _viewModel.FillCompletedBatches();
            ListBoxCompletedBatches.SelectedIndex = -1;
        }

        #endregion

        #region Current
        private async void BtnViewCurrentPorts_Click(object sender, RoutedEventArgs e)
        {
            Batch batch = await _viewModel.CheckCurrentBatch();

            if (batch != null && batch.IsEnterPartsSkip == false)
            {
                ListBoxPrepareParts.SelectedIndex = -1;
                _viewModel.EnterPartsUIElementStates.LayoutGroupBagsVisibility = Visibility.Visible.ToString();
                _viewModel.SaveEnterPartsUIElementStatesSettings();
                _viewModel.EnterPartsSelectedBatchModelByBatch(batch);
                _viewModel.EnterPartsSelectedBatchModel.ShowPortsInDisabledMode = true;
                CreatePortView(default);

                lotPropertyGrid.IsEnabled = false;
            }
        }
        private async void BtnUnloadCurrent_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.CheckCurrentBatch();

            // Check if Integrity Check result on "Running" status
            if (_viewModel.GetIntegrityCheckStatus())
            {
                WinUIMessageBox.Show("Check yapılırken unload yapılamaz lütfen check’i durdurun!", "Check çalışır durumda", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (ProcessManager.Instance.IsBatchRunning())
            {
                WinUIMessageBox.Show("Batch çalışır durumdayken değişiklik yapılamaz!", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            else
            {
                MessageBoxResult dialogResult = MessageBoxResult.No;

                if (_viewModel.CurrentBatch != null)
                {
                    dialogResult = WinUIMessageBox.Show($"Batch'i geri çekmek istediğinize emin misiniz?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
                }

                if (dialogResult == MessageBoxResult.Yes)
                {
                    bool result = await _viewModel.UnloadCurrent();

                    ListBoxCompletedBatches.SelectedIndex = -1;
                    ListBoxPrepareParts.SelectedIndex = -1;


                    //todo:l refactor
                    try { ListBoxCompletedBatches.SelectedIndex = 0; } catch { }
                    try { ListBoxPrepareParts.SelectedIndex = 0; } catch { }



                    if (!result)
                    {
                        WinUIMessageBox.Show(_viewModel.DisplayMessage, "Batch Yükleme", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        #endregion

        #region Prepare Parts
        private async void BarBtnPreparePartsMoveToCurrent_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            barBtnPreparePartsMoveToCurrent.IsEnabled = false;

            MessageBoxResult dialogResult = MessageBoxResult.No;

            if (_viewModel.EnterPartsSelectedBatchModel != null)
            {
                dialogResult = WinUIMessageBox.Show($"{_viewModel.EnterPartsSelectedBatchModel?.SelectedBatch?.LoadNumber} batch'i yüklemek istediğinize emin misiniz?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);

            }

            if (dialogResult == MessageBoxResult.Yes)
            {
                bool result = await _viewModel.MoveToCurrent();

                if (!result)
                {
                    WinUIMessageBox.Show(_viewModel.DisplayMessage, "Batch Yükleme", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    _viewModel.EnterPartsUIElementStates.LayoutGroupBagsVisibility = Visibility.Visible.ToString();
                    CreatePortView(default);
                }
            }
            barBtnPreparePartsMoveToCurrent.IsEnabled = true;
        }
        private async void BarBtnPrepareRemove_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            MessageBoxResult dialogResult = MessageBoxResult.No;

            if (_viewModel.EnterPartsSelectedBatchModel?.SelectedBatch != null && _viewModel.EnterPartsSelectedBatchModel.SelectedBatch.id > 0)
            {
                dialogResult = WinUIMessageBox.Show($"{_viewModel.EnterPartsSelectedBatchModel.SelectedBatch.LoadNumber} işlemini silmek istediğinize emin misiniz?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
            }

            if (dialogResult == MessageBoxResult.Yes)
            {

                int selectionIndexRemoved = ListBoxPrepareParts.SelectedIndex;
                int oldPreperePartCount = _viewModel.PrepareParts.Count;

                bool result = await _viewModel.RemovePreparePartBatch();

                if (!result)
                {
                    WinUIMessageBox.Show(_viewModel.DisplayMessage, "", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    if (selectionIndexRemoved == 0)
                    {
                        ListBoxPrepareParts.SelectedIndex = -1;
                        ListBoxPrepareParts.SelectedIndex = 0;
                    }
                    else if (oldPreperePartCount > 1 && selectionIndexRemoved == oldPreperePartCount - 1)
                    {
                        ListBoxPrepareParts.SelectedIndex = -1;
                        ListBoxPrepareParts.SelectedIndex = selectionIndexRemoved - 1;
                    }
                    else
                    {
                        ListBoxPrepareParts.SelectedIndex = -1;
                        ListBoxPrepareParts.SelectedIndex = selectionIndexRemoved;
                    }

                    string eventText = $"A user called '{_viewModel.ActiveUser.UserName}' deleted a batch which is called '{_viewModel.EnterPartsSelectedBatchModel.SelectedBatch.LoadNumber}'.";
                    _viewModel.InsertProcessEventLogToDb(eventText);
                }
            }
        }
        private void ListBoxPrepareParts_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (ListBoxPrepareParts.SelectedIndex == -1)
            {
                _viewModel.UpdateLotPropertyCollection();
                return;
            }

            try
            {
                listBoxBags.SelectedIndex = -1;
                _viewModel.EnterPartsUIElementStates.LayoutGroupBagsVisibility = Visibility.Visible.ToString();
                _viewModel.EnterPartsUIElementStates.BarbtnAddBagVisibility = true;
                _viewModel.EnterPartsUIElementStates.LayoutGroupBagsVisibility = Visibility.Visible.ToString();
                _viewModel.SaveEnterPartsUIElementStatesSettings();
                Batch batch = ((Batch)((sender as ListBoxEdit).SelectedItem));
                var enterPartsSelectedBatchModel = _viewModel.EnterPartsSelectedBatchModelByBatch(batch);
                _viewModel.EnterPartsSelectedBatchModel.ShowPortsInDisabledMode = false;
                CreatePortView(default);

                _viewModel.UpdateLotPropertyCollection();

                lotPropertyGrid.Columns["SoirNumber"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                lotPropertyGrid.Columns["PartName"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                lotPropertyGrid.Columns["ToolName"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                lotPropertyGrid.IsEnabled = false;

                _viewModel.EnterPartsUIElementStates.LotPropertyDeleteButtonIsEnabled = false;

            }
            catch (Exception ex)
            {
                LogManager.Instance.Log($"Prepare parts selection changed! \n Detail:{ex.Message}\n", LogType.Error);
            }

            if (!ptcTab.IsSelected)
                ptcTab.IsSelected = true;
        }
        private void ListBoxPrepareParts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListBoxPrepareParts.SelectedIndex == -1)
            {
                return;
            }

            try
            {
                _viewModel.EnterPartsUIElementStates.LayoutGroupBagsVisibility = Visibility.Visible.ToString();
                _viewModel.EnterPartsUIElementStates.BarbtnAddBagVisibility = true;
                _viewModel.EnterPartsUIElementStates.LayoutGroupBagsVisibility = Visibility.Visible.ToString();
                Batch batch = ((Batch)((sender as ListBoxEdit).SelectedItem));
                var enterPartsSelectedBatchModel = _viewModel.EnterPartsSelectedBatchModelByBatch(batch);
                _viewModel.EnterPartsSelectedBatchModel.ShowPortsInDisabledMode = false;
                CreatePortView(default);

                lotPropertyGrid.Columns["SoirNumber"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                lotPropertyGrid.Columns["PartName"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                lotPropertyGrid.Columns["ToolName"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                lotPropertyGrid.IsEnabled = false;

                _viewModel.EnterPartsUIElementStates.LotPropertyDeleteButtonIsEnabled = false;

            }
            catch (Exception ex)
            {
                LogManager.Instance.Log($"Prepare parts selection changed! \n Detail:{ex.Message}\n", LogType.Error);
            }
        }
        private async void BarBtnPrepareAdd_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            bool result = await _viewModel.AddNewPreparePart();

            if (!result)
            {
                WinUIMessageBox.Show(_viewModel.DisplayMessage, "Batch Yükleme", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                ListBoxPrepareParts.SelectedIndex = -1;
                listBoxBags.SelectedIndex = -1;
            }
        }

        #endregion

        #region CompletedBatches
        private void ListBoxCompletedBatches_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (ListBoxCompletedBatches.SelectedIndex == -1)
            {
                return;
            }

            _viewModel.IsBagRemoveBtnEnabled = false;
            _viewModel.IsPrepareRemoveBtnEnabled = false;
            _viewModel.IsNewBagBtnEnabled = false;
            _viewModel.IsSelectAllPortsCbEnabled = false;
            ListBoxCompletedBatches.IsEnabled = false;
            _viewModel.EnterPartsUIElementStates.LayoutGroupBagsVisibility = Visibility.Visible.ToString();
            _viewModel.SaveEnterPartsUIElementStatesSettings();
            try
            {
                Batch batch = ((Batch)(ListBoxCompletedBatches.SelectedItem));
                var enterPartsSelectedBatchModel = _viewModel.EnterPartsSelectedBatchModelByBatch(batch);
                _viewModel.EnterPartsSelectedBatchModel.ShowPortsInDisabledMode = true;
                CreatePortView(default);
            }
            catch (Exception)
            {


            }

            ListBoxCompletedBatches.IsEnabled = true;

            if (!ptcTab.IsSelected)
                ptcTab.IsSelected = true;

        }
        private async void BarBtnCompletedBatchesMoveToCurrent_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            MessageBoxResult dialogResult = MessageBoxResult.No;

            if (_viewModel.EnterPartsSelectedBatchModel != null)
            {
                dialogResult = WinUIMessageBox.Show($"Are you sure to load {_viewModel.EnterPartsSelectedBatchModel?.SelectedBatch?.LoadNumber} ? (Yüklemek istediğinizden emin misiniz?)", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
            }

            if (dialogResult == MessageBoxResult.Yes)
            {
                bool result = await _viewModel.MoveToCurrent();

                if (!result)
                {
                    WinUIMessageBox.Show(_viewModel.DisplayMessage, "Batch Yükleme", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        //todo:h bu kısma özel yetkilendirme getirilecek.
        private async void BarBtnCompletedPartsMoveCompletedToPrepare_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            MessageBoxResult dialogResult = MessageBoxResult.No;

            if (_viewModel.EnterPartsSelectedBatchModel != null)
            {
                dialogResult = WinUIMessageBox.Show($"Are you sure to move {_viewModel.EnterPartsSelectedBatchModel?.SelectedBatch?.LoadNumber} to prepare part? (Batch hazırlama bölümüne taşımak istediğinizden emin misiniz?)", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
            }

            if (dialogResult == MessageBoxResult.Yes)
            {
                bool result = await _viewModel.MoveCompletedToPrepare();

                if (!result)
                {
                    WinUIMessageBox.Show(_viewModel.DisplayMessage, "Batch Yükleme", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void ListBoxCompletedBatches_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListBoxCompletedBatches.SelectedIndex == -1)
            {
                return;
            }

            try
            {
                _viewModel.EnterPartsUIElementStates.LayoutGroupBagsVisibility = Visibility.Visible.ToString();
                _viewModel.EnterPartsUIElementStates.BarbtnAddBagVisibility = true;
                _viewModel.EnterPartsUIElementStates.LayoutGroupBagsVisibility = Visibility.Visible.ToString();
                Batch batch = ((Batch)((sender as ListBoxEdit).SelectedItem));
                ListBoxCompletedBatches.IsEnabled = false;
                var enterPartsSelectedBatchModel = _viewModel.EnterPartsSelectedBatchModelByBatch(batch);
                _viewModel.EnterPartsSelectedBatchModel.ShowPortsInDisabledMode = true;
                CreatePortView(default);

                lotPropertyGrid.Columns["SoirNumber"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                lotPropertyGrid.Columns["PartName"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                lotPropertyGrid.Columns["ToolName"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;

                _viewModel.EnterPartsUIElementStates.LotPropertyDeleteButtonIsEnabled = false;

            }
            catch (Exception ex)
            {
                LogManager.Instance.Log($"Prepare parts selection changed! \n Detail:{ex.Message}\n", LogType.Error);
            }

            ListBoxCompletedBatches.IsEnabled = true;
        }

        #endregion

        #region Bag
        private async void BarBtnBagRemove_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            MessageBoxResult dialogResult = MessageBoxResult.No;
            if (_viewModel.EnterPartsSelectedBatchModel.SelectedBag != null && _viewModel.EnterPartsSelectedBatchModel.SelectedBag.BagId > 0)
            {
                dialogResult = WinUIMessageBox.Show($"Lütfen {_viewModel.EnterPartsSelectedBatchModel.SelectedBag.BagName} bag'ini silmeyi onaylayınız!", "", MessageBoxButton.YesNo, MessageBoxImage.Question);

            }
            if (dialogResult == MessageBoxResult.Yes)
            {
                bool result = await _viewModel.RemoveBagAsync();

                if (!result)
                {
                    WinUIMessageBox.Show(_viewModel.DisplayMessage, "Bag silme hatası!", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    _viewModel.IsBagRemoveBtnEnabled = false;
                    if (listBoxBags.Items.Count == 0)
                        ListBoxPrepareParts_SelectedIndexChanged(ListBoxPrepareParts, null);

                    _viewModel.EnterPartsSelectedBatchModel.SelectedBag = new EnterPartsBagDetail();
                    _viewModel.EnterPartsSelectedBatchModel.SelectedLotProperties = new ObservableCollection<LotProperty>();
                    _viewModel.EnterPartsSelectedBatchModel.SelectedPortListPTC = new ObservableCollection<EnterPartsPortDetail>();
                    _viewModel.EnterPartsSelectedBatchModel.SelectedPortListMON = new ObservableCollection<EnterPartsPortDetail>();
                    _viewModel.EnterPartsSelectedBatchModel.SelectedPortListVAC = new ObservableCollection<EnterPartsPortDetail>();

                    string eventText = $"A user called '{_viewModel.ActiveUser.UserName}' deleted a bag which is called '{_viewModel.EnterPartsSelectedBatchModel.SelectedBag.BagName}'.";
                    _viewModel.InsertProcessEventLogToDb(eventText);
                }
            }
        }

        private async void BarBtnBagAdd_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            bool result = await _viewModel.AddNewBagAsync();
            if (!result)
            {
                WinUIMessageBox.Show(_viewModel.DisplayMessage, "Bag ekleme", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            listBoxBags.SelectedIndex = _viewModel.EnterPartsSelectedBatchModel.EnterPartsBagDetails.Count() - 1;

            if (listBoxBags.SelectedIndex != -1)
                ListBoxBags_SelectedIndexChanged(listBoxBags, null);
        }

        private void ListBoxBags_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            ListBoxEdit listBoxEdit = (sender as ListBoxEdit);

            bool isMovedFromCompleted = _viewModel.EnterPartsSelectedBatchModel.SelectedBatch.Revision > 0;

            if (listBoxEdit == null)
            {
                CreatePortView(0);
                return;
            }

            _viewModel.EnterPartsSelectedBatchModel.SelectedBag = ((EnterPartsBagDetail)((sender as ListBoxEdit).SelectedItem));

            int curBatchId = _viewModel.CurrentBatch?.id ?? 0;

            if (listBoxBags.SelectedIndex > -1 && !_viewModel.EnterPartsSelectedBatchModel.SelectedBatch.id.Equals(curBatchId)
                && _viewModel.EnterPartsSelectedBatchModel.SelectedBatch.Status == BatchCurrentState.NotStarted)
                _viewModel.IsBagRemoveBtnEnabled = true;
            else
                _viewModel.IsBagRemoveBtnEnabled = false;

            int bagId = _viewModel.EnterPartsSelectedBatchModel?.SelectedBag?.BagId ?? 0;

            CreatePortView(bagId);

            if (bagId != 0)
            {
                var enterPartsBagDetail = _viewModel.EnterPartsSelectedBatchModel.EnterPartsBagDetails.First(x => x.BagId == _viewModel.EnterPartsSelectedBatchModel.SelectedBag.BagId);
                var enterPartsPortDetails = enterPartsBagDetail.EnterPartsPortDetails.OrderBy(x => x.SelectedPortNumeric);
                _viewModel.EnterPartsSelectedBatchModel.SelectedPortListPTC = enterPartsPortDetails.Where(x => x.ActiveTagGroup == ActiveTagGroups.PTC).ToObservableCollection();
                _viewModel.EnterPartsSelectedBatchModel.SelectedPortListMON = enterPartsPortDetails.Where(x => x.ActiveTagGroup == ActiveTagGroups.MON).ToObservableCollection();
                _viewModel.EnterPartsSelectedBatchModel.SelectedPortListVAC = enterPartsPortDetails.Where(x => x.ActiveTagGroup == ActiveTagGroups.VAC).ToObservableCollection();
                _viewModel.EnterPartsSelectedBatchModel.SelectedLotProperties = enterPartsBagDetail.LotPropertiesData.Where(x => x.BagId == _viewModel.EnterPartsSelectedBatchModel.SelectedBag.BagId).ToObservableCollection();
            }

            _viewModel.UpdateLotPropertyCollection();

            AddNewLotPropertiesRow();

            if (!ptcTab.IsSelected)
                ptcTab.IsSelected = true;

            var lotPropertyAllowEditing = DevExpress.Utils.DefaultBoolean.True;

            if (_viewModel.EnterPartsSelectedBatchModel?.SelectedBatch?.id == _viewModel.CurrentBatch?.id)
            {
                lotPropertyAllowEditing = DevExpress.Utils.DefaultBoolean.False;
                lotPropertyGrid.IsEnabled = false;
            }
            else
            {
                lotPropertyGrid.IsEnabled = !isMovedFromCompleted;
            }

            lotPropertyGrid.Columns["SoirNumber"].AllowEditing = lotPropertyAllowEditing;
            lotPropertyGrid.Columns["PartName"].AllowEditing = lotPropertyAllowEditing;
            lotPropertyGrid.Columns["ToolName"].AllowEditing = lotPropertyAllowEditing;

            if (lotPropertyAllowEditing == DevExpress.Utils.DefaultBoolean.True)
            {
                _viewModel.EnterPartsUIElementStates.LotPropertyDeleteButtonIsEnabled = true;
            }
            else
            {
                _viewModel.EnterPartsUIElementStates.LotPropertyDeleteButtonIsEnabled = false;
            }
        }

        #endregion

        #region LotProperties

        [Obsolete("Sadece devexpressin grid column kullanımı eski")]
        private void GridControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Enter && lotPropertyView.ActiveEditor != null && lotPropertyView.FocusedRowHandle == GridControl.NewItemRowHandle)
            if (e.Key == Key.Enter && lotPropertyView.ActiveEditor != null)
            {
                lotPropertyView.CommitEditing();

                var focusColumnIndex = lotPropertyView.FocusedColumn;

                switch (focusColumnIndex.VisibleIndex)
                {
                    case 0: lotPropertyView.FocusedColumn = lotPropertyGrid.Columns["SoirNumber"]; break;
                    case 1: lotPropertyView.FocusedColumn = lotPropertyGrid.Columns["PartName"]; break;
                    case 2: lotPropertyView.FocusedColumn = lotPropertyGrid.Columns["ToolName"]; break;

                    default:
                        InsertOrUpdateRowProperty();
                        goto case 0;
                }
            }
        }
        private async void InsertOrUpdateRowProperty()
        {
            int selectedRowId = (int)lotPropertyGrid.GetFocusedRowCellValue("id");
            LotProperty lotProperty = new LotProperty();
            object soirNumber = lotPropertyGrid.GetFocusedRowCellValue("SoirNumber");
            object partName = lotPropertyGrid.GetFocusedRowCellValue("PartName");
            object toolName = lotPropertyGrid.GetFocusedRowCellValue("ToolName");

            
            
            string maxPropertyNameLength = soirNumber != null ? (soirNumber.ToString().Length > 25 ? "Soir Name" : "") : "";
            maxPropertyNameLength = partName != null ? (partName.ToString().Length > 25 ? "Part Name" : "") : maxPropertyNameLength;
            maxPropertyNameLength = toolName != null ? (toolName.ToString().Length > 25 ? "Tool Name" : "") : maxPropertyNameLength;

            if (!String.IsNullOrEmpty(maxPropertyNameLength))
            {
                WinUIMessageBox.Show($"{maxPropertyNameLength} 25 karakterden fazla olmaz!", "Lot Property", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            lotProperty.SoirNumber = soirNumber != null ? soirNumber.ToString() : "";
            lotProperty.PartName = partName != null ? partName.ToString() : "";
            lotProperty.ToolName = toolName != null ? toolName.ToString() : "";
            lotProperty.BagId = _viewModel.EnterPartsSelectedBatchModel.SelectedBag.BagId;

            if (selectedRowId == 0)
            {
                lotProperty.CreatedByUserId = _viewModel.ActiveUser.id;
                lotProperty.ModifiedByUserId = _viewModel.ActiveUser.id;
                _viewModel.InsertLotProperty(lotProperty);
                lotPropertyGrid.SetFocusedRowCellValue("id", lotProperty.id);
                //var enterPartsBagDetail = _viewModel.EnterPartsSelectedBatchModel.SelectedLotProperties;
                //_viewModel.EnterPartsSelectedBatchModel.SelectedLotProperties = enterPartsBagDetail;
                AddNewLotPropertiesRow();

            }
            else
            {
                lotProperty.id = selectedRowId;
                lotProperty.CreatedByUserId = _viewModel.GetSelectedLotPropertyCreatedUserId(lotProperty.id);
                lotProperty.ModifiedByUserId = _viewModel.ActiveUser.id;
                lotPropertyGrid.SetFocusedRowCellValue("SoirNumber", lotProperty.SoirNumber);
                lotPropertyGrid.SetFocusedRowCellValue("PartName", lotProperty.PartName);
                lotPropertyGrid.SetFocusedRowCellValue("ToolName", lotProperty.ToolName);

                await _viewModel.UpdateLotProperty(lotProperty);
            }

        }
        private async void BtnDeleteLotPropertyRow_Click(object sender, RoutedEventArgs e)
        {
            LotProperty lotProperty = new LotProperty();
            int id = (int)lotPropertyGrid.GetFocusedRowCellValue("id");
            lotPropertyView.DeleteRow(lotPropertyView.FocusedRowHandle);

            await _viewModel.DeleteLotProperty(id);

            //if ((lotPropertyGrid.ItemsSource as ICollection).Count == 1)
            //{}  else {}
        }
        private void AddNewLotPropertiesRow()
        {
            lotPropertyView.AddNewRow();
            lotPropertyGrid.SetFocusedRowCellValue("BagId", _viewModel.EnterPartsSelectedBatchModel?.SelectedBag?.BagId ?? 0);
        }
        #endregion

        private void UpdateSelectAllCheckBoxState()
        {
            IEnumerable<ToggleButton> enabledPorts = Enumerable.Empty<ToggleButton>();
            string portName = string.Empty;

            string nameOfMonPort = _viewModel.EnterPartsLanguageSettings["mon"]; //Get value from  37.ApplicationLanguageSettings in Postgresql ApplicationProperties table
            string nameOfPtcPort = _viewModel.EnterPartsLanguageSettings["ptc"]; //Get value from  37.ApplicationLanguageSettings in Postgresql ApplicationProperties table
            string nameOfVacPort = _viewModel.EnterPartsLanguageSettings["vac"]; //Get value from  37.ApplicationLanguageSettings in Postgresql ApplicationProperties table


            //DXTabItem selectedItem =  portTabControl.SelectedItem as DXTabItem;
            //switch (selectedItem.Header.ToString())
            //{
            //    case nameof(nameOfPtcPort):
            //        enabledPorts = GetEnabledPortButtonsByName("PTC");
            //        portName = "PTC";
            //        break;
            //    case nameof(nameOfVacPort):
            //        enabledPorts = GetEnabledPortButtonsByName("VAC");
            //        portName = "VAC";
            //        break;
            //    case nameof(nameOfMonPort):
            //        enabledPorts = GetEnabledPortButtonsByName("MON");
            //        portName = "MON";// _viewModel.EnterPartsLanguageSettings["mon"];
            //        break;
            //    default:
            //        break;

                DXTabItem selectedItem = portTabControl.SelectedItem as DXTabItem;
                switch (selectedItem.Header.ToString())
                {
                    case "PTC":
                        enabledPorts = GetEnabledPortButtonsByName("PTC");
                        portName = "PTC";
                        break;
                    case "VAC":
                        enabledPorts = GetEnabledPortButtonsByName("VAC");
                        portName = "VAC";
                        break;
                    case "MON":
                        enabledPorts = GetEnabledPortButtonsByName("MON");
                        portName = "MON";// _viewModel.EnterPartsLanguageSettings["mon"];
                        break;
                    default:
                        break;
                    }

            int totalSelectedPorts = enabledPorts.Where(p => p.IsChecked ?? false).Count();

            if (enabledPorts.Count() == totalSelectedPorts)
                selectAllPortsCb.IsChecked = true;
            else
                selectAllPortsCb.IsChecked = false;
        }

        private void CreatePortView(int bagId)
        {
            var appViewModel = new AppViewModel();

            var applicationProperties = ProcessManager.Instance.ApplicationProperties;
            int ptcCount = Convert.ToInt32(applicationProperties["PTCCount"].Value);
            int monCount = Convert.ToInt32(applicationProperties["MONCount"].Value);
            int vacCount = Convert.ToInt32(applicationProperties["VACCount"].Value);

            var activeTagByName = _viewModel.AllActiveTagsByName();
            var activeTagById = _viewModel.AllActiveTagsById();
            bool showInDisabledMode = _viewModel.EnterPartsSelectedBatchModel?.ShowPortsInDisabledMode ?? false;
            Dictionary<int, EnterPartsPortDetail> enterPartsPortDetail = new Dictionary<int, EnterPartsPortDetail>();
            Dictionary<int, EnterPartsPortDetail> allEnterPartsPortDetail = _viewModel.EnterPartsSelectedBatchModel.EnterPartsBagDetails.SelectMany(x => x.EnterPartsPortDetails).ToDictionary(x => x.SelectedPortTagId, x => x);
            Dictionary<int, EnterPartsPortDetail> allEnterPartsPortDetailExceptSelectedBag = _viewModel.EnterPartsSelectedBatchModel.EnterPartsBagDetails.SelectMany(x => x.EnterPartsPortDetails).ToDictionary(x => x.SelectedPortTagId, x => x);

            if (bagId != default)
            {
                if (layoutGroupCompletedBatches.Visibility != Visibility.Visible &&
                    !(_viewModel.CurrentBatch?.id.Equals(_viewModel.EnterPartsSelectedBatchModel.SelectedBatch.id) ?? false))
                {
                    _viewModel.IsSelectAllPortsCbEnabled = true;
                }

                IEnumerable<EnterPartsBagDetail> enterPartsBagDetail = _viewModel.EnterPartsSelectedBatchModel.EnterPartsBagDetails.Where(x => x.BagId == bagId);
                enterPartsPortDetail = enterPartsBagDetail.First().EnterPartsPortDetails.ToDictionary(x => x.SelectedPortTagId, x => x);
                allEnterPartsPortDetail = _viewModel.EnterPartsSelectedBatchModel.EnterPartsBagDetails.SelectMany(x => x.EnterPartsPortDetails).ToDictionary(x => x.SelectedPortTagId, x => x);
                allEnterPartsPortDetailExceptSelectedBag = _viewModel.EnterPartsSelectedBatchModel.EnterPartsBagDetails.Except(enterPartsBagDetail).SelectMany(x => x.EnterPartsPortDetails).ToDictionary(x => x.SelectedPortTagId, x => x);
            }
            else
            {
                _viewModel.IsSelectAllPortsCbEnabled = false;
            }

            bool lastHamMenuState = _viewModel.IsHamburgerMenuExpanded.Value;


            List<ToggleButton> buttonListPtc = new List<ToggleButton>();
            for (int i = 1; i <= ptcCount; i++)
            {

                var toggleBtn = new ToggleButton();
                toggleBtn.Name = $"PTC{i}";
                toggleBtn.FontSize = 12; //12
                toggleBtn.FontWeight = FontWeights.Bold;

                if (lastHamMenuState)//Hamburger Menu is Open
                {
                    toggleBtn.MinWidth = 38; //45
                    toggleBtn.MinHeight = 33; //40
                }
                else
                {
                    toggleBtn.MinWidth = 38; //45
                    toggleBtn.MinHeight = 33; //40
                }

                toggleBtn.Margin = new Thickness(0);
                toggleBtn.Style = (Style)FindResource("GenToggleButton");
                toggleBtn.Content = i;
                toggleBtn.IsChecked = (allEnterPartsPortDetail.ContainsKey(activeTagByName[$"PTC{i}"].id));
                toggleBtn.IsEnabled = !showInDisabledMode && (bagId != default) && !(allEnterPartsPortDetailExceptSelectedBag.ContainsKey(activeTagByName[$"PTC{i}"].id));
                object[] portParam = new object[2] { activeTagByName[$"PTC{i}"], bagId };
                toggleBtn.CommandParameter = portParam;
                toggleBtn.Command = _viewModel.SetPortToggleCommand;
                buttonListPtc.Add(toggleBtn);
            }

            List<ToggleButton> buttonListMon = new List<ToggleButton>();
            for (int i = 1; i <= monCount; i++)
            {
                var toggleBtn = new ToggleButton();
                toggleBtn.FontSize = 12; //12
                toggleBtn.FontWeight = FontWeights.Bold;

                if (lastHamMenuState)//Hamburger Menu is Open
                {
                    toggleBtn.MinWidth = 38; //45
                    toggleBtn.MinHeight = 33; //40
                }
                else
                {
                    toggleBtn.MinWidth = 38; //45
                    toggleBtn.MinHeight = 33; //40
                }

                toggleBtn.Margin = new Thickness(0);
                toggleBtn.Style = (Style)FindResource("GenToggleButton");
                toggleBtn.Content = i;
                toggleBtn.IsChecked = (allEnterPartsPortDetail.ContainsKey(activeTagByName[$"MON{i}"].id));
                toggleBtn.IsEnabled = !showInDisabledMode && (bagId != default) && !(allEnterPartsPortDetailExceptSelectedBag.ContainsKey(activeTagByName[$"MON{i}"].id));
                object[] portParam = new object[2] { activeTagByName[$"MON{i}"], bagId };
                toggleBtn.CommandParameter = portParam;
                toggleBtn.Command = _viewModel.SetPortToggleCommand;
                buttonListMon.Add(toggleBtn);
            }

            List<ToggleButton> buttonListVac = new List<ToggleButton>();
            for (int i = 1; i <= vacCount; i++)
            {
                var toggleBtn = new ToggleButton();
                toggleBtn.FontSize = 12; //12
                toggleBtn.FontWeight = FontWeights.Bold;

                if (lastHamMenuState)//Hamburger Menu is Open
                {
                    toggleBtn.MinWidth = 38; //45
                    toggleBtn.MinHeight = 33; //40
                    flowLayoutPtc.Margin = new Thickness(10, 10, 150, 0);
                    flowLayoutMon.Margin = new Thickness(10, 10, 150, 0);
                    flowLayoutVac.Margin = new Thickness(10, 10, 150, 0);

                }
                else//Hamburher Menu is Closed
                {
                    portNumLayoutGroup.Width = 300;
                    toggleBtn.MinWidth = 38; //45
                    toggleBtn.MinHeight = 33; //40
                    flowLayoutPtc.Margin = new Thickness(10, 10, 300, 0);
                    flowLayoutMon.Margin = new Thickness(10, 10, 300, 0);
                    flowLayoutVac.Margin = new Thickness(10, 10, 300, 0);

                }

                toggleBtn.Margin = new Thickness(0);
                toggleBtn.Style = (Style)FindResource("GenToggleButton");
                toggleBtn.Content = i;
                toggleBtn.IsChecked = (allEnterPartsPortDetail.ContainsKey(activeTagByName[$"VAC{i}"].id));
                toggleBtn.IsEnabled = !showInDisabledMode && (bagId != default) && !(allEnterPartsPortDetailExceptSelectedBag.ContainsKey(activeTagByName[$"VAC{i}"].id));
                object[] portParam = new object[2] { activeTagByName[$"VAC{i}"], bagId };
                toggleBtn.CommandParameter = portParam;
                toggleBtn.Command = _viewModel.SetPortToggleCommand;
                buttonListVac.Add(toggleBtn);
            }

            flowLayoutPtc.ItemsSource = buttonListPtc;
            flowLayoutMon.ItemsSource = buttonListMon;
            flowLayoutVac.ItemsSource = buttonListVac;

            UpdateSelectAllCheckBoxState();
        }

        private void selectAllPortsCb_Click(object sender, RoutedEventArgs e)
        {

            CheckBox checkBox = (CheckBox)sender;
            bool isCheckedVal = checkBox.IsChecked ?? false;

            var portButtons = new List<ToggleButton>();
            string portName = string.Empty;


            var activeTagByName = _viewModel.AllActiveTagsByName();

            string nameOfMonPort = _viewModel.EnterPartsLanguageSettings["mon"];
            string nameOfPtcPort = _viewModel.EnterPartsLanguageSettings["ptc"];
            string nameOfVacPort = _viewModel.EnterPartsLanguageSettings["vac"];

            DXTabItem selectedItem = portTabControl.SelectedItem as DXTabItem; //SelectOfItem
            var a = selectedItem.Header.ToString();
            if (selectedItem.Header.ToString() == nameOfMonPort)
            {
                portButtons = flowLayoutMon.ItemsSource.Cast<ToggleButton>().ToList();
                portName = "MON";
            }
            else if (selectedItem.Header.ToString() == nameOfPtcPort)
            {
                portButtons = flowLayoutPtc.ItemsSource.Cast<ToggleButton>().ToList();
                portName = "PTC";
            }
            else if (selectedItem.Header.ToString() == nameOfVacPort)
            {
                portButtons = flowLayoutVac.ItemsSource.Cast<ToggleButton>().ToList();
                portName = "VAC";
            }

            var disabledPortNumbers = portButtons.Where(p => p.IsEnabled == false).Select(p => Convert.ToInt32(p.Content));

            foreach (var item in portButtons.Where(p => p.IsEnabled))
                item.IsChecked = isCheckedVal;

            _viewModel.SetAllPortsOrNot(portName, isCheckedVal, disabledPortNumbers);
        }

        public IEnumerable<ToggleButton> GetEnabledPortButtonsByName(string portName)
        {
            switch (portName)
            {
                case "PTC":
                    return flowLayoutPtc.ItemsSource.Cast<ToggleButton>().Where(p => p.IsEnabled).ToList();
                case "MON":
                    return flowLayoutMon.ItemsSource.Cast<ToggleButton>().Where(p => p.IsEnabled).ToList();
                case "VAC":
                    return flowLayoutVac.ItemsSource.Cast<ToggleButton>().Where(p => p.IsEnabled).ToList();
                default:
                    break;
            }
            return Enumerable.Empty<ToggleButton>();
        }

        private void portTabControl_SelectionChanged(object sender, TabControlSelectionChangedEventArgs e)
        {
            DXTabControl tabControl = (DXTabControl)sender;
            if (!tabControl.IsLoaded)
                return;

            UpdateSelectAllCheckBoxState();
        }

        public void SetSelectAllCheckBox(bool isCheckedVal)
        {
            selectAllPortsCb.IsChecked = isCheckedVal;
        }

        private void flowLayout_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_viewModel == null)
                return;

            bool lastHamMenuState = _viewModel.IsHamburgerMenuExpanded.Value;
            FlowLayoutControl flowLayoutControl = (FlowLayoutControl)sender;

            // Hamburger menu opened
            if (lastHamMenuState)
            {
                flowLayoutControl.Margin = new Thickness(10, 10, 0, 0);
                portNumLayoutGroup.Width = 300;
                ptcListText.FontSize = 12;
                monListText.FontSize = 12;
                vacListText.FontSize = 12;

                foreach (ToggleButton item in flowLayoutPtc.ItemsSource)
                {
                    item.MinWidth = 38;
                    item.MinHeight = 33;
                }

                foreach (ToggleButton item in flowLayoutMon.ItemsSource)
                {
                    item.MinWidth = 38;
                    item.MinHeight = 33;
                }

                foreach (ToggleButton item in flowLayoutVac.ItemsSource)
                {
                    item.MinWidth = 38;
                    item.MinHeight = 33;
                }

            }
            else // Hamburger menu closed
            {
                flowLayoutControl.Margin = new Thickness(10, 10, 300, 0);
                portNumLayoutGroup.Width = 300;
                ptcListText.FontSize = 12;
                monListText.FontSize = 12;
                vacListText.FontSize = 12;

                foreach (ToggleButton item in flowLayoutPtc.ItemsSource)
                {
                    item.MinWidth = 38;
                    item.MinHeight = 33;
                }

                foreach (ToggleButton item in flowLayoutMon.ItemsSource)
                {
                    item.MinWidth = 38;
                    item.MinHeight = 33;
                }

                foreach (ToggleButton item in flowLayoutVac.ItemsSource)
                {
                    item.MinWidth = 38;
                    item.MinHeight = 33;
                }

            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _viewModel.SetEnterPartsDatablock(false);
        }
    }
}