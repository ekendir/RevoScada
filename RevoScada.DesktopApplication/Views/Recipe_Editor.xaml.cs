using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
using System.Windows.Threading;
using DevExpress.Xpf.WindowsUI;
using RevoScada.ProcessController;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.ViewModels;
using RevoScada.DesktopApplication.Views.Popups;
using RevoScada.Entities;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Enums;
using RevoScada.Configurator;

namespace RevoScada.DesktopApplication.Views
{
    /// <summary>
    /// Interaction logic for Recipe_Editor.xaml
    /// </summary>
    public partial class Recipe_Editor : UserControl
    {
        #region Fields
        private RecipeEditorVM _viewModel;
        private BackgroundWorker _bgWorker;
        
        private string _selectedRecipeParentHeader;
        private TreeViewItem _selectedRecipeParentItem;
        private TreeViewItem _selectedRecipeItem;
        private string _selectedRecipeItemHeader;
        private Recipe_Simulate _recipeSimulateView;
        private Storyboard _dbResultPositiveFadeOutAnim;
        private Storyboard _dbResultNegativeFadeOutAnim;
        private Style _textBoxStyle;
        private Style _comboBoxStyle;
        private Style _recipeParentStyle;
        private Style _recipeItemStyle;
        private int _lastSelectedColumnIndex = 2;
        private int _selectedRecipeId;
        private int _copiedRecipeId;
        private DispatcherTimer _timer;
        private bool _isTableEditingDisabled;
        private bool _isComboBoxTight;
        private Text_Edit _textEditPopupView;
        #endregion

        #region Collections
        private List<List<TableSegmentDataGrid>> TableAndSegList;
        #endregion

        public Recipe_Editor()
        {
            InitializeComponent();

            _bgWorker = new BackgroundWorker(); //Initializing the worker object
            _bgWorker.DoWork += Worker_DoWork; //Binding Worker_DoWork method

            _dbResultPositiveFadeOutAnim = Resources["dbResultPositiveFadeOutStoryBoard"] as Storyboard;
            _dbResultNegativeFadeOutAnim = Resources["dbResultNegativeFadeOutStoryBoard"] as Storyboard;
            _recipeParentStyle = (Style)FindResource("TreeViewItemHeader_ControlBtnVer");
            _recipeItemStyle = (Style)FindResource("TreeViewItem_ControlBtnVer");

            //if (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId == 20)
            //    _isComboBoxTight = true;
            //else
            //    _isComboBoxTight = false;
        }

        #region Loading Functionality Section
        private void StartLoading()
        {
            if (!_bgWorker.IsBusy && _viewModel != null)
                _bgWorker.RunWorkerAsync(); //Executing the worker
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            LoadRecipeDataToTable();
        }
        #endregion Loading Functionality Section Ends Here

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as RecipeEditorVM;
            _viewModel.Recipe_Editor_View = this;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _viewModel.SetRecipeDatablock(false);
            _timer.Stop();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1,
                            (Duration)TimeSpan.FromSeconds(1));
            this.BeginAnimation(UIElement.OpacityProperty, animation);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += Timer_Tick;
            _timer.Start();

            TableAndSegList = new List<List<TableSegmentDataGrid>>();
            SetOperationButtons(false);
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (_viewModel.IsInitiallyLoaded == false)
            {
                _viewModel.InitialLoad();

                // Check if there is a loaded recipe in cache
                if (_viewModel.LoadedRecipe?.id > 0)
                {
                    LoadRecipeGroupsToTreeview(_viewModel.LoadedRecipe.id);
                } else
                {
                    LoadRecipeGroupsToTreeview();
                }
            }

            IsProcessRunning();
        }

        private void IsProcessRunning()
        {
            
            bool isBatchRunning = ProcessManager.Instance.IsBatchRunning();

            if(isBatchRunning)
                SetOperationButtons(true);
        }

        private void LoadTextEditPopup(string recipeName, bool isItRecipe, int recipeId)
        {
            _textEditPopupView = new Text_Edit(this, recipeName, isItRecipe, recipeId);
            _textEditPopupView.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            _textEditPopupView.ShowDialog();
        }

        private void UpdateRecipeItemSource()
        {
            List<RecipeRowItemVM> CurrRecipeData = new List<RecipeRowItemVM>();

            foreach (var item in recipeTable.Items)
            {
                CurrRecipeData.Add((RecipeRowItemVM)item);
            }

            var totalSegDataOfRecipeTable = CurrRecipeData[0].SegTextColl.Count;

            foreach (var item in CurrRecipeData)
            {
                short fieldId = item.RowHeaderId;
                short fieldIdIndex = (short)(item.RowHeaderId - 1);

                for (int i = 1; i <= totalSegDataOfRecipeTable; i++)
                {
                    if (i >= TableAndSegList.Count)
                        return;

                    foreach (var value in TableAndSegList[i])
                    {
                        if (value.RecipeDetail.RecipeFieldId == fieldId)
                        {
                            if ((i - 1) > item.SegTextColl.Count - 1)
                                continue;

                            string cellValue = item.SegTextColl[i - 1];

                            if (value.RecipeDetail.RecipeFieldValue == cellValue)
                            {
                                if (value.CellChangeState != CellChangeStates.Added &&
                                    value.CellChangeState != CellChangeStates.Modified && value.CellChangeState != CellChangeStates.Deleted)
                                {
                                    value.CellChangeState = CellChangeStates.Unchanged;
                                }
                            }
                            else if (value.RecipeDetail.RecipeFieldValue != cellValue)
                            {
                                if (value.CellChangeState == CellChangeStates.Unchanged)
                                    value.CellChangeState = CellChangeStates.Modified;

                                value.RecipeDetail.RecipeFieldValue = cellValue;
                            }
                            break;
                        }
                    }
                }
            }
        }

        private bool permissionControl(string permission, bool status)
        {
            bool result = _viewModel.Permissions.Contains(new KeyValuePair<string, bool>(permission, status));

            return result;
        }

        #region Context Menu Section

        public void AddNewRecipeItem()
        {
            if (_selectedRecipeParentItem != null)
            {
                // Update database
               int lastAddedRecipeId = _viewModel.AddNewRecipe();

               if(lastAddedRecipeId > 0)
                {
                    TreeViewItem newRecipeItem = new TreeViewItem();
                    newRecipeItem.Header = "NEW RECIPE";
                    newRecipeItem.Name = "recipeId_" + lastAddedRecipeId;
                    newRecipeItem.Tag = lastAddedRecipeId.ToString();
                    newRecipeItem.Style = _recipeItemStyle;

                    _selectedRecipeParentItem.Items.Add(newRecipeItem);

                    // Expand parent TreeView item
                    TreeViewHelpers.JumpToFolderByName(recipeTreeView, _selectedRecipeParentItem.Name);
                    // Set last added item as selected item.
                    TreeViewHelpers.SetSelectedItem(recipeTreeView, newRecipeItem);
                }
                else
                    WinUIMessageBox.Show("Reçete ekleme işlemi başarısız oldu. Lütfen tekrar deneyiniz.", 
                                         "Uyarı", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                if (string.IsNullOrEmpty(_selectedRecipeParentHeader))
                {
                    WinUIMessageBox.Show("Lütfen bir reçete grubu seçiniz.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }
        }

        public void AddNewGroupItem()
        {
            int newGroupItemId = 0;

            // Update database
            newGroupItemId = _viewModel.AddNewRecipeGroup();

            if(newGroupItemId > 0)
            {
                TreeViewItem newRecipeGroup = new TreeViewItem();

                newRecipeGroup.Name = "recipeGroupId_" + newGroupItemId;
                newRecipeGroup.Header = "NEW GROUP";
                newRecipeGroup.Style = _recipeParentStyle;
                recipeTreeView.Items.Add(newRecipeGroup);
            }
            else
            {
                WinUIMessageBox.Show("Reçete grubu ekleme işlemi başarısız oldu. Lütfen tekrar deneyiniz.",
                     "Uyarı", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public void CopyItem()
        {
            if (_selectedRecipeId == 0)
                return;

            _copiedRecipeId = _selectedRecipeId;
            _viewModel.IsRecipePasteButtonEnabled = true;
        }

        public void PasteItem()
        {
            _viewModel.IsRecipePasteButtonEnabled = false;

            if (_selectedRecipeParentItem != null)
            {
                // Update database and return recipeName
                object[] clonedRecipeParams = _viewModel.CloneRecipe(_copiedRecipeId);

                if (clonedRecipeParams.Count() == 0)
                    return;

                int clonedRecipeId = (int) clonedRecipeParams[0];
                string clonedRecipeName = (string) clonedRecipeParams[1];

                if (clonedRecipeId == 0)
                    return;

                TreeViewItem copiedTreeViewItem = new TreeViewItem();
                copiedTreeViewItem.Header = clonedRecipeName;
                copiedTreeViewItem.Name = "recipeId_" + clonedRecipeId;
                copiedTreeViewItem.Tag = clonedRecipeId.ToString();
                copiedTreeViewItem.Style = _recipeItemStyle;

                _selectedRecipeParentItem.Items.Add(copiedTreeViewItem);

                // Expand parent TreeView item
                TreeViewHelpers.JumpToFolderByName(recipeTreeView, _selectedRecipeParentItem.Name);
                // Set last added item as selected item.
                TreeViewHelpers.SetSelectedItem(recipeTreeView, copiedTreeViewItem);
            }
            else
            {
                if (string.IsNullOrEmpty(_selectedRecipeParentHeader))
                {
                    WinUIMessageBox.Show("Lütfen bir reçete grubu seçiniz.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }
        }

        public void DeleteRecipeItem(int recipeId)
        {
            TreeViewItem selectedTreeViewItem = new TreeViewItem();
            TreeViewItem selectedTreeViewParent = new TreeViewItem();
            bool isItemFound = false;

            foreach (TreeViewItem parentItem in recipeTreeView.Items)
            {
                if (isItemFound)
                    break;

                foreach (TreeViewItem recipeItem in parentItem.Items)
                {
                    string tagVal = (string)recipeItem.Tag;
                    if (tagVal == "disabled" || tagVal == "isValid")
                        continue;

                    if (Convert.ToInt32(tagVal) == recipeId)
                    {
                        selectedTreeViewItem = recipeItem;
                        selectedTreeViewParent = parentItem;
                        isItemFound = true;
                        break;
                    }
                }
            }

            if (selectedTreeViewItem.Header == null)
                return;

            var result = WinUIMessageBox.Show(selectedTreeViewItem.Header.ToString() + " kaydını silmek istediğinize emin misiniz?", "Uyarı", MessageBoxButton.YesNo, 
                                              MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes)
            {
                // Update database
                _viewModel.DeleteRecipe(recipeId);
                // Remove item from recipe tree view
                selectedTreeViewParent.Items.Remove(selectedTreeViewItem);
            }
        }

        public void DeleteRecipeGroup()
        {
            // Check if recipe group contains a recipe, if it contains do not delete otherwise delete it.
            if (_selectedRecipeParentItem.Items.Count > 0)
            {
                WinUIMessageBox.Show("Dolu reçete grupları silinemez!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                // Update database
                _viewModel.DeleteEmptyRecipeGroup();
                // Remove item from recipe tree view
                recipeTreeView.Items.Remove(_selectedRecipeParentItem);
                TreeViewHelpers.UnselectTreeViewItem(recipeTreeView);
                _selectedRecipeParentItem = null;
            }
        }

        public void RenameRecipeItem(int recipeId)
        {
            TreeViewItem selectedTreeViewItem = new TreeViewItem();
            bool isItemFound = false;

            foreach (TreeViewItem parentItem in recipeTreeView.Items)
            {
                if (isItemFound)
                    break;

                foreach (TreeViewItem recipeItem in parentItem.Items)
                {
                    int intValueOfTag;
                    string tagVal = (string)recipeItem.Tag;
                    if (tagVal == "disabled" || tagVal.Contains("IsValid"))
                        continue;

                    int x = 0; //Control of Convert string to int
                    if (Int32.TryParse(tagVal, out x))
                    {
                        intValueOfTag = Convert.ToInt32(tagVal);
                    }
                    else
                    {
                        intValueOfTag = 0;
                    }

                    if (intValueOfTag == recipeId)
                    {
                        selectedTreeViewItem = recipeItem;
                        isItemFound = true;
                        break;
                    }
                }
            }

            if (selectedTreeViewItem.Header == null)
                return;

            LoadTextEditPopup(selectedTreeViewItem.Header.ToString(), true, recipeId);
        }

        public void RenameGroupItem()
        {
            if (_selectedRecipeParentItem != null)
            {
                string currRecipeGroupName = _selectedRecipeParentItem.Header.ToString();
                LoadTextEditPopup(currRecipeGroupName, false, 0);
            }
        }

        public void ApplyNameChangesToRecipeItem(string newName, bool isItRecipe, int recipeId)
        {
            _textEditPopupView.Close();
            // Update database
            if (isItRecipe)
            {
                var isItSuccess = _viewModel.ChangeRecipeName(newName, recipeId);

                if (!isItSuccess)
                    return;

                bool isItemFound = false;

                foreach (TreeViewItem parentItem in recipeTreeView.Items)
                {
                    if (isItemFound)
                        break;

                    foreach (TreeViewItem recipeItem in parentItem.Items)
                    {
                        int intValueOfTag;
                        string tagVal = (string)recipeItem.Tag;
                        if (tagVal == "disabled" || tagVal.Contains("IsValid"))
                            continue;

                        int x = 0; //Control of Convert string to int
                        if (Int32.TryParse(tagVal, out x))
                        {
                            intValueOfTag = Convert.ToInt32(tagVal);
                        }
                        else
                        {
                            intValueOfTag = 0;
                        }

                        if (intValueOfTag == recipeId)
                        {
                            recipeItem.Header = newName;
                            isItemFound = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                bool isItSuccess = _viewModel.ChangeRecipeGroupName(newName);

                if(!isItSuccess)
                    return;

                _selectedRecipeParentItem.Header = newName;
            }
        }

        public bool CheckIfRecipeValuesAreValid()
        {
            int totalColumns = TableAndSegList.Count - 1;

            foreach (var item in TableAndSegList[0])
            {
                if (!item.IsActive)
                    continue;

                short fieldId = item.RecipeFieldId;
                short fieldIdIndex = (short)(item.RecipeFieldId - 1);

                for (int j = 1; j <= totalColumns; j++)
                {
                    if (TableAndSegList[j].Count > fieldIdIndex)
                    {
                        foreach (var value in TableAndSegList[j])
                        {
                            if (!value.RecipeDetail.RecipeFieldId.Equals(fieldId))
                                continue;

                            if (item.IsMultipleCell && !value.RecipeDetail.RecipeFieldValue.Equals("X"))
                            {
                                if (!string.IsNullOrEmpty(value.RecipeDetail.RecipeFieldValue))
                                {
                                    string[] segTimeVals = Regex.Matches(value.RecipeDetail.RecipeFieldValue, @"[a-zA-Z]+|\d+")
                                                           .Cast<Match>()
                                                           .Select(m => m.Value)
                                                           .ToArray();

                                    if (segTimeVals.Count() != 2)
                                        return false;
                                }
                                break;
                            } 
                            else if(item.RecipeFieldId == 5 || item.RecipeFieldId == 8) // Temp Value and Pressure Value
                            {
                                char[] currLetters = new char[] { 'C', 'U', 'R' };
                                foreach (var letter in currLetters)
                                {
                                    if (value.RecipeDetail.RecipeFieldValue.Contains(letter) && !value.RecipeDetail.RecipeFieldValue.Equals("CURR"))
                                        return false;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        public async Task SaveRecipeChanges(int recipeId, bool doNotShowResultPopup = false)
        {
            UpdateRecipeItemSource();

            bool checkResult = CheckIfRecipeValuesAreValid();

            if (!checkResult)
            {
                WinUIMessageBox.Show("Lütfen Segment Time ve Grace sütunlarında değer ve zaman dilimi formatında giriniz. \n" +
                                      "Eğer Temp. Value veya Pressure Value sütununda, 'CURR' değeri belirtmek istemişseniz lütfen doğru yazdığınızdan emin olunuz.", "Uyarı",
                                      MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Save changes to db
            bool dbSaveResult = false;
            await Task.Run(() =>
            {
                // Update Recipe Details database
                dbSaveResult = _viewModel.UpdateRecipeDetailsTable(recipeId, TableAndSegList);
                TableAndSegList = _viewModel.TableAndSegList;
            });

            _viewModel.IsRecipeTableDataChanged.Value = false;

            if (doNotShowResultPopup)
                return;

            if (dbSaveResult)
                _dbResultPositiveFadeOutAnim.Begin();
            else
                _dbResultNegativeFadeOutAnim.Begin();
        }

        #endregion


        public void GetRecipeTreeViewItemByName(string recipeGroupItemName, string recipeItemName)
        {
            if (recipeTreeView.Items.Count < 1)
                return;

            recipeTable.Columns.Clear();
            recipeTable.Items.Clear();
            recipeTable.Items.Refresh();

            int groupIndex = 0;
            int recipeIndex = 0;

            foreach (TreeViewItem groupItem in recipeTreeView.Items)
            {
                if (groupItem.Name == recipeGroupItemName)
                    break;

                groupIndex++;
            }

            TreeViewItem recipeGroupItem = (TreeViewItem)recipeTreeView.Items[groupIndex];

            foreach (TreeViewItem item in recipeGroupItem.Items)
            {
                if (item.Name == recipeItemName)
                    break;

                recipeIndex++;
            }

            TreeViewItem recipeItem = (TreeViewItem)recipeGroupItem.Items[recipeIndex];

            _selectedRecipeItem = recipeItem;
            // Update viewmodel's selected group id
            short selectedRecipeGroupId = 0;
            if (recipeGroupItem.Name.Contains('_'))
            {
                selectedRecipeGroupId = Convert.ToInt16(recipeGroupItem.Name.Split('_')[1]);
                _viewModel.SelectedGroupId = selectedRecipeGroupId;
            }

            // Update viewmodel's selected recipe id
            if (recipeItem.Name.Contains('_'))
            {
                _selectedRecipeId = Convert.ToInt32(recipeItem.Name.Split('_')[1]);
                _viewModel.SelectedRecipeId = _selectedRecipeId;
            }

            // Expand parent TreeView item
            TreeViewHelpers.JumpToFolderByName(recipeTreeView, recipeGroupItem.Name);
            // Set last added item as selected item.
            TreeViewHelpers.SetSelectedItem(recipeTreeView, recipeItem);

            _isTableEditingDisabled = true;

            _viewModel.IsDescriptionReadOnly = true;
            SetOperationButtons(false);

            StartLoading();
        }

        public void LoadRecipeGroupsToTreeview(int disabledRecipeId = 0)
        {
            recipeTreeView.ItemsSource = null;
            recipeTreeView.Items.Clear();
            recipeTreeView.Items.Refresh();

            if (_viewModel.RecipeGroupsColl?.Count() == 0)
            {
                return;
            }

            foreach (var groupItem in _viewModel.RecipeGroupsColl)
            {
                TreeViewItem recipeGroupItem = new TreeViewItem();
                recipeGroupItem.Name = "recipeGroupId_" + groupItem.id;
                recipeGroupItem.Style = _recipeParentStyle;
                recipeGroupItem.Header = groupItem.GroupName;

                foreach (var item in _viewModel.RecipesColl)
                {
                    if (item.RecipeGroupId == groupItem.id)
                    {
                        TreeViewItem recipeItem = new TreeViewItem();

                        recipeItem.Header = item.RecipeName;
                        recipeItem.Name = "recipeId_" + item.id;
                        recipeItem.Style = _recipeItemStyle;



                        //if (!item.IsValid)
                        //{
                        //    //recipeItem.Tag = "False";
                        //    recipeItem.Tag = "disabled";
                        //}

                        if (disabledRecipeId > 0 && disabledRecipeId == item.id)
                        {
                            recipeItem.Tag = "disabled";
                        }
                        else if (!item.IsValid)
                        {
                            recipeItem.Tag = "isValid";
                        }
                        else
                        {
                            recipeItem.Tag = item.id.ToString();
                        }

                        recipeGroupItem.Items.Add(recipeItem);
                    }
                }
                recipeTreeView.Items.Add(recipeGroupItem);
            }
            ActiveRecipeLoader();
        }

        private async Task DelayedGoToActiveRecipe()
        {
            await Task.Delay(10);
            _viewModel.GoToActiveRecipe();
        }

        /// <summary>
        /// Check if there is any active recipe on cache.
        /// </summary>
        private async void ActiveRecipeLoader()
        {
            if(!string.IsNullOrEmpty(_viewModel.ActiveRecipeName))
            {
                recipeTable.Columns.Clear();
                recipeTable.Items.Clear();
                recipeTable.Items.Refresh();
                await DelayedGoToActiveRecipe();
            }
        }

        private DataTemplate DataGridTemplateCreator(string controlBindingName)
        {
            DataTemplate myTemplate = new DataTemplate { DataType = typeof(ContentControl) };
            FrameworkElementFactory myContentControl = new FrameworkElementFactory(typeof(ContentControl));
            myContentControl.SetBinding(ContentControl.ContentProperty, new Binding(controlBindingName));
            myTemplate.VisualTree = myContentControl;

            return myTemplate;
        }

        private void CreateEmptyTable()
        {
            _viewModel = DataContext as RecipeEditorVM;
            // Removes data from the Recipe Info section.
            _viewModel.RecipeInfo = null;

            // Add an empty list to the tableAndSegList
            var TableSegmentDataGridList = new List<TableSegmentDataGrid>();
            var recipeFieldOrderedNumbers = _viewModel.RecipeFieldsColl.Select(r => r.id);
            foreach (var fieldId in recipeFieldOrderedNumbers)
            {
                TableSegmentDataGridList.Add(new TableSegmentDataGrid()
                {
                    RecipeDetail = new RecipeDetail()
                    {
                        RecipeId = _selectedRecipeId,
                        RecipeFieldValue = "",
                        RecipeFieldId = fieldId,
                        SegmentNo = 1
                    },
                    CellChangeState = CellChangeStates.Added
                });
            }

            TableAndSegList.Add(TableSegmentDataGridList);

            // Create only one column for table names.
            var tableCol = new DataGridTemplateColumn();
            string tableCtrlBindingName = "MyControlTable";
            tableCol.CellTemplate = DataGridTemplateCreator(tableCtrlBindingName);
            tableCol.CellStyle = (Style)FindResource("recipeHeaderDataGridCell");

            recipeTable.Columns.Add(tableCol); /// add table column to the data grid.

            // Create another column for unit section.
            var unitCol = new DataGridTemplateColumn();
            unitCol.Header = "Unit";
            string unitCtrlBindingName = "MyControlUnit";
            unitCol.CellTemplate = DataGridTemplateCreator(unitCtrlBindingName);
            unitCol.CellStyle = (Style)FindResource("recipeHeaderDataGridCell");

            recipeTable.Columns.Add(unitCol); /// add unit column to the data grid.

            // Add only one empty column
            DataGridTemplateColumn segCol;
            segCol = new DataGridTemplateColumn();
            segCol.Header = $"{_viewModel.RecipeEditorLanguageSettings["segment"]} 1";

            if (_isComboBoxTight)
            {
                segCol.Width = new DataGridLength(65);
            }
            else
            {
                segCol.Width = new DataGridLength(100);
            }

            string controlBindingName = "MyControl1";
            segCol.CellTemplate = DataGridTemplateCreator(controlBindingName);
            recipeTable.Columns.Add(segCol);
            bool isItColorA = true;

            if (TableAndSegList == null)
                return;

            for (int i = 0; i < TableAndSegList[1].Count; i++)
            {
                if (TableAndSegList[0][i].RecipeFieldId == 0 || !TableAndSegList[0][i].IsActive)
                    continue;

                var recipeRowItem = new RecipeRowItemVM();
                recipeRowItem.PredefinedRecipeFields = _viewModel.PredefinedRecipeFields;
                recipeRowItem.TwoOffsetFieldIdNumbers = _viewModel.TwoOffsetFieldIdNumbers;

                for (int j = 1; j < 2; j++)
                {
                    recipeRowItem.SegTextColl.Add("");
                    recipeRowItem.RowHeader = TableAndSegList[0][i].RecipeFieldValue;
                    recipeRowItem.RowHeaderId = TableAndSegList[0][i].RecipeFieldId;
                    recipeRowItem.RowHeaderOrderId = TableAndSegList[0][i].RecipeFieldOrderId;
                }

                if (isItColorA)
                {
                    _textBoxStyle = (Style)FindResource("whiteTextBoxDataGridA1");
                    if (_isComboBoxTight)
                        _comboBoxStyle = (Style)FindResource("ComboBoxA1_tight");
                    else
                        _comboBoxStyle = (Style)FindResource("ComboBoxA1");
                    recipeRowItem.RowColorIndex = 0;
                    isItColorA = false;
                }
                else
                {
                    _textBoxStyle = (Style)FindResource("whiteTextBoxDataGridA2");
                    if (_isComboBoxTight)
                        _comboBoxStyle = (Style)FindResource("ComboBoxA2_tight");
                    else
                        _comboBoxStyle = (Style)FindResource("ComboBoxA2");
                    recipeRowItem.RowColorIndex = 1;
                    isItColorA = true;
                }

                recipeRowItem.TextBoxStyle = _textBoxStyle;
                recipeRowItem.IsComboBoxTight = _isComboBoxTight;
                recipeRowItem.ComboBoxStyle = _comboBoxStyle;

                if (!_isTableEditingDisabled)
                    recipeRowItem.IsTableEnabled = true;

                recipeRowItem.RecipeEditorVM = _viewModel;
                recipeRowItem.PlcDeviceId = _viewModel.PlcDeviceId;
                recipeRowItem.RecipeDbNumber = _viewModel.RecipeEditorTagConfigurations.DbNumber;

                recipeTable.Items.Add(recipeRowItem);
            }
        }

        private async void AddSegment(bool isRightDirActive = true)
        {
            // Check if there are more than 30 segments
            int totalSegments = recipeTable.Columns.Count - 2;
            if (totalSegments == 30)
            {
                WinUIMessageBox.Show("30'dan fazla segment eklenemez!", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            UpdateRecipeItemSource();

            if (isRightDirActive)
                _lastSelectedColumnIndex++;

            // Subtract 2 for the table name and unit columns.
            int totalNumOfSegs = TableAndSegList.Count - 1;
            short segNo = (short)(totalNumOfSegs + 1);

            var TableSegmentDataGridList = new List<TableSegmentDataGrid>();

            var recipeFieldOrderedNumbers = _viewModel.RecipeFieldsColl.Select(r => r.id);
            foreach (var fieldId in recipeFieldOrderedNumbers)
            {
                TableSegmentDataGridList.Add(new TableSegmentDataGrid()
                {
                    RecipeDetail = new RecipeDetail()
                    {
                        RecipeId = _selectedRecipeId,
                        RecipeFieldValue = "",
                        RecipeFieldId = fieldId,
                        SegmentNo = segNo
                    },
                    CellChangeState = CellChangeStates.Added
                });
            }

            if (_lastSelectedColumnIndex < recipeTable.Columns.Count)
            { 
                TableAndSegList.Insert(_lastSelectedColumnIndex - 1, TableSegmentDataGridList);

                // You're inserting a new segment, reorder all segment numbers of the collection.
                totalNumOfSegs++;
                for (short i = 1; i <= totalNumOfSegs; i++)
                {
                    for (int j = 0; j < _viewModel.TotalRecipeRows; j++)
                    {
                        TableAndSegList[i][j].RecipeDetail.SegmentNo = i;

                        // Do not change last added column as modified because it is added.
                        if (i != _lastSelectedColumnIndex - 1 && TableAndSegList[i][j].CellChangeState != CellChangeStates.Added)
                            TableAndSegList[i][j].CellChangeState = CellChangeStates.Modified;
                    }
                }
            }
            else // You're adding a new segment to the end of the columns, do not make changes on segment numbers.
            {
                TableAndSegList.Add(TableSegmentDataGridList);
            }

            recipeTable.Columns.Clear();
            recipeTable.Items.Clear();
            recipeTable.Items.Refresh();

            LoadRecipeDataToTable();

            // Save changes to db
            await Task.Run(() =>
            {
                // Update Recipe Details database
                _viewModel.UpdateRecipeDetailsTable(_selectedRecipeId, TableAndSegList);

                TableAndSegList = _viewModel.TableAndSegList;
                _viewModel.IsRecipeTableDataChanged.Value = false;
            });
        }

        private async void RemoveSegment()
        {
            if (_lastSelectedColumnIndex >= recipeTable.Columns.Count || TableAndSegList.Count <= 1)
                return;

            if(TableAndSegList.Count == 2)
            {
                WinUIMessageBox.Show("Segment silebilmeniz için birden fazla segment'e sahip olmanız gerekiyor.", "Uyarı",
                                          MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            short segNo = (short) (_lastSelectedColumnIndex - 1);

            MessageBoxResult result = WinUIMessageBox.Show(string.Format("{0} nolu segment'i kalıcı olarak silmek istediğinize emin misiniz?", segNo), 
                                                            "Soru", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
                return;

            UpdateRecipeItemSource();

            int removeItemsIndex = _lastSelectedColumnIndex - 1;
            // Subtract 2 for the table name and unit columns.
            int totalNumOfSegs = recipeTable.Columns.Count - 2;

            //Remove selected column items from collection
            for (short i = 1; i <= totalNumOfSegs; i++)
            {
                if (removeItemsIndex < TableAndSegList.Count && TableAndSegList.Count != 2)
                {
                    for (int j = 0; j < _viewModel.TotalRecipeRows; j++)
                    {
                        if (TableAndSegList[i].Count == 0)
                            continue;

                        if (i == removeItemsIndex)
                            TableAndSegList[i][j].CellChangeState = CellChangeStates.Deleted;

                        if (i > removeItemsIndex)
                        {
                            TableAndSegList[i][j].RecipeDetail.SegmentNo = (short)(TableAndSegList[i][j].RecipeDetail.SegmentNo - 1);
                            TableAndSegList[i][j].CellChangeState = CellChangeStates.Modified;
                        }
                    }
                }
                else
                    continue;
            }

            // Save changes to db
            await Task.Run(() =>
            {
                // Update Recipe Details database
                _viewModel.UpdateRecipeDetailsTable(_selectedRecipeId, TableAndSegList, true);

                TableAndSegList = _viewModel.TableAndSegList;
                _viewModel.IsRecipeTableDataChanged.Value = false;
            });

            recipeTable.Columns.Clear();
            recipeTable.Items.Clear();
            recipeTable.Items.Refresh();

            LoadRecipeDataToTable();
        }

        private void LoadRecipeDataToTable()
        {
            this.Dispatcher.Invoke(() =>
            {
                TableAndSegList = _viewModel.TableAndSegList;

                if (TableAndSegList.Count == 1)
                {
                    CreateEmptyTable();
                    return;
                }

                /// Create only one column for table names.
                var tableCol = new DataGridTemplateColumn();
                string tableCtrlBindingName = "MyControlTable";
                tableCol.CellTemplate = DataGridTemplateCreator(tableCtrlBindingName);
                tableCol.CellStyle = (Style)FindResource("recipeHeaderDataGridCell");

                recipeTable.Columns.Add(tableCol); /// add table column to the data grid.

                /// Create another column for unit section.
                var unitCol = new DataGridTemplateColumn();
                unitCol.Header = "Unit";
                string unitCtrlBindingName = "MyControlUnit";
                unitCol.CellTemplate = DataGridTemplateCreator(unitCtrlBindingName);
                unitCol.CellStyle = (Style)FindResource("recipeHeaderDataGridCell");

                recipeTable.Columns.Add(unitCol); /// add unit column to the data grid.

                int totalColumns = TableAndSegList.Count - 1;

                DataGridTemplateColumn segCol;
                bool isItColorA = true;

                for (int i = 1; i <= totalColumns; i++)
                {
                    segCol = new DataGridTemplateColumn();
                    segCol.Header = $"{_viewModel.RecipeEditorLanguageSettings["segment"]} {i}";

                    if (_isComboBoxTight)
                    {
                        segCol.Width = new DataGridLength(65);
                    } else
                    {
                        segCol.Width = new DataGridLength(100);
                    }

                    string controlBindingName = "MyControl" + i;
                    segCol.CellTemplate = DataGridTemplateCreator(controlBindingName);

                    Trigger templateTrigger = new Trigger { Property = Button.IsPressedProperty, Value = true };
                    segCol.CellTemplate.Triggers.Add(templateTrigger);

                    recipeTable.Columns.Add(segCol);
                }

                /// Adds data to the Recipe Info section.
                foreach (var item in TableAndSegList[0])
                {
                    if (!item.IsActive)
                        continue;

                    var recipeRowItem = new RecipeRowItemVM();
                    recipeRowItem.PredefinedRecipeFields = _viewModel.PredefinedRecipeFields;
                    recipeRowItem.TwoOffsetFieldIdNumbers = _viewModel.TwoOffsetFieldIdNumbers;

                    short fieldId = item.RecipeFieldId;
                    short fieldIdIndex = (short) (item.RecipeFieldId - 1);

                    for (int j = 1; j <= totalColumns; j++)
                    {
                        foreach (var value in TableAndSegList[j])
                        {
                            if (value.RecipeDetail.RecipeFieldId == fieldId)
                            {
                                recipeRowItem.RecipeDetail = value.RecipeDetail;
                                recipeRowItem.SegTextColl.Add(value.RecipeDetail.RecipeFieldValue);
                                break;
                            }
                        }

                        recipeRowItem.RowHeader = item.RecipeFieldValue;
                        recipeRowItem.RowHeaderId = item.RecipeFieldId;
                        recipeRowItem.RowHeaderOrderId = item.RecipeFieldOrderId;
                        recipeRowItem.HeaderBrushString = item.RecipeFieldDisplayColor;
                    }

                    recipeTable.Items.Add(recipeRowItem);

                    if (isItColorA)
                    {
                        _textBoxStyle = (Style)FindResource("whiteTextBoxDataGridA1");
                        if(_isComboBoxTight)
                            _comboBoxStyle = (Style)FindResource("ComboBoxA1_tight");
                        else
                            _comboBoxStyle = (Style)FindResource("ComboBoxA1");
                        recipeRowItem.RowColorIndex = 0;
                        isItColorA = false;
                    }
                    else
                    {
                        _textBoxStyle = (Style)FindResource("whiteTextBoxDataGridA2");
                        if (_isComboBoxTight)
                            _comboBoxStyle = (Style)FindResource("ComboBoxA2_tight");
                        else
                            _comboBoxStyle = (Style)FindResource("ComboBoxA2");
                        recipeRowItem.RowColorIndex = 1;
                        isItColorA = true;
                    }

                    recipeRowItem.TextBoxStyle = _textBoxStyle;
                    recipeRowItem.IsComboBoxTight = _isComboBoxTight;
                    recipeRowItem.ComboBoxStyle = _comboBoxStyle;

                    if (!_isTableEditingDisabled)
                        recipeRowItem.IsTableEnabled = true;

                    recipeRowItem.RecipeEditorVM = _viewModel;
                    recipeRowItem.PlcDeviceId = _viewModel.PlcDeviceId;
                    recipeRowItem.RecipeDbNumber = _viewModel.RecipeEditorTagConfigurations.DbNumber;
                }
            });
        }

        private void DataGridColumnHeader_OnClick(object sender, RoutedEventArgs e)
        {
            var columnHeader = sender as DataGridColumnHeader;
            if (columnHeader != null && !_isTableEditingDisabled)
            {
                recipeTable.SelectedCells.Clear();
                foreach (var item in recipeTable.Items)
                {
                    recipeTable.SelectedCells.Add(new DataGridCellInfo(item, columnHeader.Column));
                }
                _lastSelectedColumnIndex = recipeTable.Columns.IndexOf(columnHeader.Column);

                if (_lastSelectedColumnIndex < 2)
                    _lastSelectedColumnIndex = 2;

                if (!columnHeader.Column.Header?.ToString().StartsWith("Segment") ?? true)
                {
                    // Make all of the columns' header styles standard.
                    foreach (var column in recipeTable.Columns)
                    {
                        column.HeaderStyle = (Style)FindResource("standardDatagridColumnHeaderWithEvent");
                    }
                    return;
                }

                // First, make all of the columns' header styles standard.
                foreach (var column in recipeTable.Columns)
                {
                    column.HeaderStyle = (Style)FindResource("standardDatagridColumnHeaderWithEvent");
                }

                // Then set the desired column's header style.
                recipeTable.Columns[_lastSelectedColumnIndex].HeaderStyle = (Style)FindResource("activeSegmentColumnHeader");
            }
        }

        /// <summary>
        /// Gets the selected TreeView's parent if there are any.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private ItemsControl GetSelectedTreeViewItemParent(TreeViewItem item)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(item);
            if (parent == null)
                return null;

            while (!(parent is TreeViewItem || parent is TreeView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as ItemsControl;
        }


       


        private void SetOperationButtons(bool isProcessRunning)
        {
            if (isProcessRunning)
            {
                sendToPlcBtn.IsEnabled = false;
                return;
            }

            if (_isTableEditingDisabled || _selectedRecipeItem == null)
            {
                addToLeftBtn.IsEnabled = false;
                addToRightBtn.IsEnabled = false;
                removeBtn.IsEnabled = false;
                sendToPlcBtn.IsEnabled = false;
            }
            else
            {
                addToLeftBtn.IsEnabled = true;
                addToRightBtn.IsEnabled = true;
                removeBtn.IsEnabled = true;
                //if(_viewModel.Permissions.Contains(new KeyValuePair<string, bool>("activateRecipe", true)))
                if (permissionControl("activateRecipe", true))                
                    sendToPlcBtn.IsEnabled = true;

                recipeTreeView.ContextMenu = (ContextMenu)Resources["contextMenu"];
            }
            // Set report and simulate buttons here
            if (_selectedRecipeItem != null || _isTableEditingDisabled)
            {
                reportBtn.IsEnabled = true;
                simulateBtn.IsEnabled = true;
            } else
            {
                reportBtn.IsEnabled = false;
                simulateBtn.IsEnabled = false;
            }
        }

        public async void CheckIfAnyUnsavedTableValues()
        {
            if (_viewModel.IsRecipeTableDataChanged.Value)
            {
                _viewModel.IsRecipeTableDataChanged.Value = false;

                var result = WinUIMessageBox.Show("Reçete tablosunda yapılmış değişiklikler var, bunları kaydetmek ister misiniz?", "Uyarı",
                                        MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                   await SaveRecipeChanges(_selectedRecipeId, true);
            }
        }

        //private void RecipeTreeView_SelectedItemChanged(object sender, RoutedEventArgs e)
        //{
           
        //}

        private void recipeTable_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }

        #region Segment Operation Event Triggers
        private void addToRightBtn_Click(object sender, RoutedEventArgs e)
        {
            AddSegment();
        }
        private void addToLeftBtn_Click(object sender, RoutedEventArgs e)
        {
            AddSegment(false);
        }
        private void removeBtn_Click(object sender, RoutedEventArgs e)
        {
            RemoveSegment();
        }
        #endregion

        private void simulateBtn_Click(object sender, RoutedEventArgs e)
        {
            _recipeSimulateView = new Recipe_Simulate(recipeTable);
            _recipeSimulateView.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            _recipeSimulateView.ShowDialog();
        }

        private void recipeInfoTable_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            RecipeInfo selectedRecipeInfo = e.Row.Item as RecipeInfo;
        }

        private void recipeInfoTable_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            RecipeInfo selectedRecipeInfo = e.Row.Item as RecipeInfo;
        }

        private void descTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            _viewModel.IsRecipeTableDataChanged.Value = true;

            if (e.Key == Key.Enter || e.Key == Key.Escape)
            {
                FocusableBorder.Focus();
                return;
            }
        }

        private void specificationTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            _viewModel.IsRecipeTableDataChanged.Value = true;

            if (e.Key == Key.Enter || e.Key == Key.Escape)
            {
                FocusableBorder.Focus();
                return;
            }
        }

        private void RecipeTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            bool allowLoadingData = true;

            TreeViewItem selectedNode = (TreeViewItem)recipeTreeView.SelectedItem;
            if (selectedNode == null)
                return;

            _selectedRecipeItem = selectedNode;


            if (_selectedRecipeItem != null && ((string)_selectedRecipeItem.Tag == "disabled" || (string)_selectedRecipeItem.Tag == "isValid"))
            {  
                allowLoadingData = false;
                //return;
            }
               

            CheckIfAnyUnsavedTableValues();

            recipeTable.Columns.Clear();
            recipeTable.Items.Clear();
            recipeTable.Items.Refresh();

            //TreeViewItem selectedNode = (TreeViewItem)recipeTreeView.SelectedItem;
            //if (selectedNode == null)
            //    return;

           // if ((string)selectedNode.Tag == "disabled" || !_viewModel.Permissions.Contains(new KeyValuePair<string, bool>("updateRecipe", true)))
            if ((string)selectedNode.Tag == "disabled" || ! permissionControl("updateRecipe",true) || (string)selectedNode.Tag == "isValid")
            {
                _isTableEditingDisabled = true;
                _viewModel.IsDescriptionReadOnly = true;

                //if (!_viewModel.Permissions.Contains(new KeyValuePair<string, bool>("updateRecipe", true)) && (string)selectedNode.Tag != "disabled")
                if (!permissionControl("updateRecipe", true) && (string)selectedNode.Tag != "disabled")
                    allowLoadingData = true;
            }
            else
            {
                _isTableEditingDisabled = false;
                _viewModel.IsDescriptionReadOnly = false;
                allowLoadingData = true;

                _isTableEditingDisabled = false;
                _viewModel.IsDescriptionReadOnly = false;
            }

            //_selectedRecipeItem = selectedNode;
            string itemName = selectedNode.Header.ToString();
            _viewModel = DataContext as RecipeEditorVM;

            ItemsControl parent = null;
            parent = GetSelectedTreeViewItemParent(selectedNode);

            TreeViewItem parentItem = parent as TreeViewItem;

            if (parentItem != null)
            {
                _selectedRecipeParentHeader = parentItem.Header.ToString(); //Gets you the immediate parent
                _selectedRecipeParentItem = parentItem;
                // Update viewmodel's selected group id
                short selectedRecipeGroupId = 0;
                if (parentItem.Name.Contains('_'))
                {
                    selectedRecipeGroupId = Convert.ToInt16(parentItem.Name.Split('_')[1]);
                    _viewModel.SelectedGroupId = selectedRecipeGroupId;
                }

                // Update viewmodel's selected recipe id
                if (selectedNode.Name.Contains('_'))
                {
                    _selectedRecipeId = Convert.ToInt32(selectedNode.Name.Split('_')[1]);
                    _viewModel.SelectedRecipeId = _selectedRecipeId;
                }

                // Collapse group item's control buttons
                _viewModel.GroupDeleteButtonVisibility = Visibility.Collapsed;
                _viewModel.GroupEditButtonVisibility = Visibility.Collapsed;
            }
            else
            {
                _selectedRecipeItemHeader = itemName;
                _selectedRecipeItem = null;
                _viewModel.SelectedRecipe = new Entities.Recipe();
                _selectedRecipeParentItem = selectedNode;
                _selectedRecipeParentHeader = string.Empty;
                _viewModel.GroupEditButtonVisibility = Visibility.Visible;

                // Check if group item has any items
                if (selectedNode.HasItems)
                    _viewModel.GroupDeleteButtonVisibility = Visibility.Collapsed;
                else
                    _viewModel.GroupDeleteButtonVisibility = Visibility.Visible;

                // Update viewmodel's selected group id
                short selectedRecipeGroupId = 0;
                if (selectedNode.Name.Contains('_'))
                {
                    selectedRecipeGroupId = Convert.ToInt16(selectedNode.Name.Split('_')[1]);
                    _viewModel.SelectedGroupId = selectedRecipeGroupId;
                }
            }

            SetOperationButtons(false);

            if (_selectedRecipeItem != null && allowLoadingData)
                LoadRecipeDataToTable();
        }
    }
}