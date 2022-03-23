using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using RevoScada.ProcessController;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.ViewModels;

namespace RevoScada.DesktopApplication.Views
{
    /// <summary>
    /// Interaction logic for Recipe.xaml
    /// </summary>
    public partial class ActiveRecipeControl : UserControl
    {
        #region Fields
        private Style _textBoxStyle;
        private Style _comboBoxStyle;
        private Style _comboBoxValueChangedStyle;
        private ActiveRecipeVM _viewModel;
        private BackgroundWorker bgWorker;
        private DispatcherTimer _timer;
        public bool InitialLoading;
        private int _lastActiveSegmentNo;
        private bool _isComboBoxTight;
        #endregion

        public ActiveRecipeControl()
        {
            InitializeComponent();

            bgWorker = new BackgroundWorker(); //Initializing the worker object
            bgWorker.DoWork += Worker_DoWork; //Binding Worker_DoWork method
            bgWorker.RunWorkerCompleted += Worker_RunWorkerCompleted; //Binding worker_RunWorkerCompleted method

            _comboBoxValueChangedStyle = (Style)FindResource("ComboBoxValueChanged");

            //if (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId == 20)
            //    _isComboBoxTight = true;
            //else
            //    _isComboBoxTight = false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Set active datablock which is related with recipes
            ProcessManager.Instance.ChangeDemandReadStateOnCache(ApplicationConfigurations.Instance.Configuration.PlcDevice.Id, 1, true);

            _viewModel = DataContext as ActiveRecipeVM;
            if (_viewModel != null)
                _viewModel.RecipeView = this;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            // Set deactive datablock which is related with recipes
            _viewModel.SetRecipeDatablock(false);
            _timer.Stop();
        }

        #region Loading Functionality Section
        public void StartLoading()
        {
            bgWorker.RunWorkerAsync(); //Executing the worker
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            LoadActiveRecipeDataToTable();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _viewModel.WaitIndicatorControl.IsWaitIndicatorVisible = false;
        }
        #endregion Loading Functionality Section Ends Here

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1,
                (Duration)TimeSpan.FromSeconds(1));
            this.BeginAnimation(UIElement.OpacityProperty, animation);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += Timer_Tick;

            if(_viewModel != null)
                _timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (!InitialLoading)
            {
                InitialLoading = true;
               _viewModel.GetDataFromPlc();
            }

            _viewModel.ContinuousUpdate();
        }

        private DataTemplate DataGridTemplateCreator(string controlBindingName)
        {
            DataTemplate myTemplate = new DataTemplate { DataType = typeof(ContentControl) };
            FrameworkElementFactory myContentControl = new FrameworkElementFactory(typeof(ContentControl));
            myContentControl.SetBinding(ContentControl.ContentProperty, new Binding(controlBindingName));
            myTemplate.VisualTree = myContentControl;

            return myTemplate;
        }

        public void MakeColumnActive(int plcSegNo)
        {
            if (_lastActiveSegmentNo == plcSegNo || recipeTable.Columns.Count == 0)
                return;

            _lastActiveSegmentNo = plcSegNo;
            int segColumnIndex = plcSegNo + 1; // Add one for row name column.

            // First, make all of the columns' header styles standard.
            foreach (var column in recipeTable.Columns)
            {
                column.HeaderStyle = (Style)FindResource("standardDatagridColumnHeader");
            }

            // Then set the desired column's header style.
            if(segColumnIndex < recipeTable.Columns.Count)
                recipeTable.Columns[segColumnIndex].HeaderStyle = (Style)FindResource("activeSegmentColumnHeader");
        }

        private void LoadActiveRecipeDataToTable()
        {
            this.Dispatcher.Invoke(() =>
            {
                recipeTable.Columns.Clear();
                recipeTable.Items.Clear();
                recipeTable.Items.Refresh();

                var SegAndTableLists = _viewModel.SegAndTableLists;
                var totalRecipeRows = _viewModel.TotalRecipeRows;

                if (SegAndTableLists.Count == 0)
                    noDataAvailableSec.Visibility = Visibility.Visible;
                else
                    noDataAvailableSec.Visibility = Visibility.Collapsed;

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

                int totalColumns = SegAndTableLists.Count - 1;
                bool isItColorA = true;
                bool isTableEnabled = false;

                DataGridTemplateColumn segCol;

                
                for (int i = 1; i <= totalColumns; i++)
                {
                    segCol = new DataGridTemplateColumn();
                    segCol.Header = $"{_viewModel.ActiveRecipeLanguageSettings["segment"]} {i}";

                    if (_isComboBoxTight)
                        segCol.Width = new DataGridLength(65);
                    else
                        segCol.Width = new DataGridLength(100);

                    string controlBindingName = "MyControl" + i;
                    segCol.CellTemplate = DataGridTemplateCreator(controlBindingName);

                    Trigger templateTrigger = new Trigger { Property = Button.IsPressedProperty, Value = true };
                    segCol.CellTemplate.Triggers.Add(templateTrigger);

                    recipeTable.Columns.Add(segCol);
                }

                // Check permission granted to edit this datagrid
                if (_viewModel.Permissions.Contains(new KeyValuePair<string, bool>("changeValue", true)))
                {
                    if (_viewModel.Permissions.Contains(new KeyValuePair<string, bool>("changeValue", true)))
                        isTableEnabled = true;
                }

                // Check if process is NOT running
                if(!_viewModel.IsProcessRunning)
                    isTableEnabled = false;

                int offset = 0;
                int offsetIncrement = 0;
                int rowOffsetVal = 0;
                for (int i = 0; i < totalRecipeRows; i++)
                {
                    short fieldIdVal = _viewModel.RecipeFieldIdNumbers[i];
                    var recipeRowItemVM = new RecipeRowItemVM();
                    recipeRowItemVM.PredefinedRecipeFields = _viewModel.PredefinedRecipeFields;
                    recipeRowItemVM.TwoOffsetFieldIdNumbers = _viewModel.TwoOffsetFieldIdNumbers;

                    for (int j = 1; j <= totalColumns; j++)
                    {
                        if (j > 1)
                        {
                            rowOffsetVal = _viewModel.RecipeTagConfigurations.IncrementAmount * (j - 1);
                            rowOffsetVal += offset;
                        } else
                            rowOffsetVal = offset;

                        if (SegAndTableLists[j].Count > j)
                        {
                            recipeRowItemVM.SegTextColl.Add(SegAndTableLists[j][i]);

                            // Get original value from database
                            recipeRowItemVM.OriginalRecipeValues.Add(_viewModel.RecipeDetailValuesFromDb[j - 1][i]);

                            // Check if value that sent to PLC is different from value which is located in the database.
                            if (SegAndTableLists[j][i].Equals(_viewModel.RecipeDetailValuesFromDb[j - 1][i]))
                                recipeRowItemVM.ValueChangedStates.Add(false);
                            else
                                recipeRowItemVM.ValueChangedStates.Add(true);
                        }
                        else
                            recipeRowItemVM.SegTextColl.Add("");

                        recipeRowItemVM.RowHeader = SegAndTableLists[0][i];
                        recipeRowItemVM.RowHeaderId = _viewModel.RecipeFieldIdNumbers[i];

                        recipeRowItemVM.SegOffsetColl.Add(rowOffsetVal);
                        rowOffsetVal = 0;
                    }

                    if (_viewModel.TwoOffsetFieldIdNumbers.Contains(fieldIdVal))
                        offsetIncrement = _viewModel.RecipeTagConfigurations.Length * 2;
                    else
                        offsetIncrement = _viewModel.RecipeTagConfigurations.Length;

                    offset += offsetIncrement;

                    // Check whether to show this row on the UI or not
                    if (_viewModel.DisabledRecipeFieldIdNumbers.Contains(fieldIdVal))
                        continue;

                    if (isItColorA)
                    {
                        _textBoxStyle = (Style)FindResource("whiteTextBoxDataGridA1");
                        if (_isComboBoxTight)
                            _comboBoxStyle = (Style)FindResource("ComboBoxA1_tight");
                        else
                            _comboBoxStyle = (Style)FindResource("ComboBoxA1");
                        recipeRowItemVM.RowColorIndex = 0;
                        isItColorA = false;
                    }
                    else
                    {
                        _textBoxStyle = (Style)FindResource("whiteTextBoxDataGridA2");
                        if (_isComboBoxTight)
                            _comboBoxStyle = (Style)FindResource("ComboBoxA2_tight");
                        else
                            _comboBoxStyle = (Style)FindResource("ComboBoxA2");
                        recipeRowItemVM.RowColorIndex = 1;
                        isItColorA = true;
                    }

                    recipeRowItemVM.TextBoxStyle = _textBoxStyle;
                    recipeRowItemVM.IsComboBoxTight = _isComboBoxTight;
                    recipeRowItemVM.ComboBoxStyle = _comboBoxStyle;
                    recipeRowItemVM.ComboBoxValueChangedStyle = _comboBoxValueChangedStyle;
                    recipeRowItemVM.IsTableEnabled = isTableEnabled;
                    recipeRowItemVM.ActiveRecipeVM = _viewModel;
                    recipeRowItemVM.PlcDeviceId = _viewModel.PlcDeviceId;
                    recipeRowItemVM.RecipeDbNumber = (short) _viewModel.RecipeTagConfigurations.Dbnumber;
                    recipeRowItemVM.RecipeOffsetLength = (short)_viewModel.RecipeTagConfigurations.Length;
                    recipeTable.Items.Add(recipeRowItemVM);
                }
            });
        }

        private void RecipeTable_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
