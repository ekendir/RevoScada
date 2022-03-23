using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.WindowsUI;
using DevExpress.XtraReports.UI;
using Newtonsoft.Json;
using RevoScada.ProcessController;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.Models.SettingModels;
using RevoScada.DesktopApplication.Reports;
using RevoScada.DesktopApplication.ViewModels;
using RevoScada.DesktopApplication.Views.ReportTemplates;
using RevoScada.Entities;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Configuration;
using System.Threading;
using System.ComponentModel;
using DevExpress.Xpf.Printing;
using RevoScada.DesktopApplication.Views.TrendViews;

namespace RevoScada.DesktopApplication.Views
{
    /// <summary>
    /// Interaction logic for Reports.xaml
    /// </summary>
    public partial class Reports : UserControl
    {
        #region Fields
        private ReportsVM _viewModel;
        private int selectedBagId;
        private string reportName;
        private Style batchParentStyle;
        private Style batchItemStyle;
        #endregion

        private readonly string _connectionString;

        private object _alarmTagConfigurations;

        public Reports()
        {
            InitializeComponent();
            _connectionString= ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;
            _alarmTagConfigurations =( (ApplicationConfigurations.Instance.TagConfigurations).Where(x => ((SiemensTagConfiguration)x.Value).SiemensTagGroupId == 3)).ToDictionary(x=>x.Key,x=>x.Value);
            batchParentStyle = (Style)FindResource("TreeViewItemHeader_MainBlue");
            batchItemStyle = (Style)FindResource("TreeViewItem");
        }

        public ReportExportSettings ReportExportSettings
        {
            get
            {
                ReportExportSettings reportExportSettings;
                ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(_connectionString);
                ApplicationProperty applicationProperty = applicationPropertyService.GetByName("ReportExportSettings");
                reportExportSettings = JsonConvert.DeserializeObject<ReportExportSettings>(applicationProperty.Value);
                if (reportExportSettings == null)
                {
                    reportExportSettings = new ReportExportSettings
                    {
                        ExcelExportFileNameBase = "DefaultFileName",
                        ExcelExportFilePath = @"c:\\DefaultPath",
                        ExcelExportPassword = "."
                    };
                }
                return reportExportSettings;
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1, (Duration)TimeSpan.FromSeconds(1));

            this.BeginAnimation(UIElement.OpacityProperty, animation);

            _viewModel = DataContext as ReportsVM;
            _viewModel.ReportsView = this;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _viewModel.WaitIndicatorControl.IsWaitIndicatorTextActive = false;
        }

        public void SelectBatchSection()
        {
            fileNavBtn.IsChecked = true;
            fileNavBtn.Command.Execute(fileNavBtn.CommandParameter);
        }

        public void LoadBagsToTreeView()
        {
            bagsTreeView.ItemsSource = null;
            bagsTreeView.Items.Clear();
            bagsTreeView.Items.Refresh();

            bool areThereAnyBags = _viewModel.ReportBatchGridModels.Count > 0;

            // Create a batch
            TreeViewItem batchItem = new TreeViewItem();
            batchItem.Style = batchParentStyle;
            if (areThereAnyBags)
                batchItem.Header = _viewModel.SelectedBatch?.LoadNumber ?? string.Empty;
            else
                batchItem.Header = "Bag yok";

            batchItem.IsExpanded = true;

            // Create bags here
            foreach (var bag in _viewModel.ReportBatchGridModels)
            {
                TreeViewItem bagItem = new TreeViewItem();
                bagItem.Style = batchItemStyle;
                bagItem.Header = bag.BagName;
                bagItem.Name = "bagName_" + bag.BagId;

                batchItem.Items.Add(bagItem);
            }
            bagsTreeView.Items.Add(batchItem);

            bagsTreeView.UpdateLayout();
            // Expand all of the Treeview items.
            bagsTreeView.Items.OfType<TreeViewItem>().ToList().ForEach(TreeViewHelpers.ExpandAllNodes);
        }

        public void LoadRecipeData()
        {
            if (_viewModel.RecipeDetailHistoriesColl.Count == 0)
            {
                _viewModel.RecipeVisibility = Visibility.Collapsed;
                return;
            } else
            {
                _viewModel.RecipeVisibility = Visibility.Visible;
            }

            int totalColumns = _viewModel.RecipeDetailHistoriesColl.Max(r => r.SegmentNo) + 1; // Add one for the recipe fields.
            int totalRecipeFields = _viewModel.RecipeFieldsColl.Count();

            object[] obj;
            DataTable table = new DataTable("RecipeTable");

            for (int i = 0; i < totalColumns; i++)
            {
                if (i == 0)
                    table.Columns.Add(string.Empty, typeof(string));
                else
                    table.Columns.Add("Segment " + i, typeof(string));
            }

            obj = new object[totalColumns];
            var fieldOrderNumbers = _viewModel.RecipeFieldsColl.OrderBy(r => r.RecipeFieldOrder).Select(r => r.id);

            foreach (var curItem in fieldOrderNumbers)
            {
                // Get current field's recipe detail values
                var CurrFieldDetails = _viewModel.RecipeDetailHistoriesColl.Where(f => f.RecipeFieldId == curItem).OrderBy(f => f.RecipeFieldId)
                                                                   .ThenBy(n => n.SegmentNo);
                int counter = 1;

                // Get field names for first column
                obj[0] = _viewModel.RecipeFieldsColl.Where(r => r.id == curItem).SingleOrDefault().RecipeFieldName; // Field values
                foreach (var item in CurrFieldDetails)
                {
                    obj[counter] = item.RecipeCellValue;
                    counter++;
                }
                table.Rows.Add(obj);
            }

            recipeTable.ItemsSource = table.DefaultView;
            recipeTable.Columns[0].Header = string.Empty;
        }

        public async Task LoadNumericDataAsync(DataTable numericDataTable)
        {
            await Task.Delay(1);

            this.Dispatcher.Invoke(() =>
            {
                if (numericDataTable == null)
                {
                    numericTable.ItemsSource = null;
                    numericTableNoDataSec.Visibility = Visibility.Visible;
                    return;
                }
                else
                {
                    numericTableNoDataSec.Visibility = Visibility.Collapsed;
                }

                foreach (DataColumn column in numericDataTable.Columns)
                {
                    // Replace underscores with space
                    if (column.ColumnName.Contains('_'))
                    {
                        column.ColumnName = column.ColumnName.Replace('_', ' ').ToString();
                    }
                }
                numericTable.ItemsSource = numericDataTable;

                // Custom column width for date
                if (numericTable.Columns.Count > 0)
                {
                    numericTable.Columns[1].Width = 150;
                }

                // Apply dateTime values to days,months,years format
                if (numericTable.Columns.GetColumnByFieldName("Time") != null)
                    numericTable.Columns["Time"].CellTemplate = (DataTemplate)FindResource("dateTimeTemplate");
            });


          
        }

        private void bagsTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem selectedNode = (TreeViewItem)bagsTreeView.SelectedItem;

            if (selectedNode == null)
                return;

            ItemsControl parentControl = TreeViewHelpers.GetSelectedTreeViewItemParent(selectedNode);
            TreeViewItem parentItem = parentControl as TreeViewItem;

            if (parentItem != null)
            {
                selectedBagId = Convert.ToInt32(selectedNode.Name.Split('_')[1]);
                _viewModel.UpdatePortLists(selectedBagId);
                _viewModel.UpdateLotData(selectedBagId);
            }
            else
            {
                _viewModel.UpdatePortLists();
                _viewModel.UpdateLotData();
            }
        }

        private void batchAndRecipeSearchBox_KeyUp(object sender, KeyEventArgs e)
        {
            var allItems = _viewModel.BatchesAndRecipesColl;
            string searchValueLowercase = batchAndRecipeSearchBox.Text.ToLower();

            ObservableCollection<BatchSearchDto> filteredItems = allItems.Where(f => f.LoadNumber.ToLower().Contains(searchValueLowercase)
                                                                || f.RecipeName.ToLower().Contains(searchValueLowercase) 
                                                                || f.StartDate.ToString("dd/MM/yyyy HH:mm:ss").Replace('.', '/').Contains(searchValueLowercase)
                                                                || f.EndDate.ToString("dd/MM/yyyy HH:mm:ss").Replace('.', '/').Contains(searchValueLowercase)
                                                                
                                                                )
                                                                 .ToObservableCollection();
            loadNumberTable.ItemsSource = filteredItems;
        }

        private void loadNumberTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BatchSearchDto selectedBatchSearchData = (BatchSearchDto)loadNumberTable.SelectedItem;
            if(qualityCardReportComboBox != null)
            {
                qualityCardReportComboBox.SelectedIndex = 0;
            }

            if (reportComboBox != null)
                reportComboBox.SelectedIndex = 0;

            if (selectedBatchSearchData == null)
                return;

            BatchInformationGrid batchInformation = new BatchInformationGrid();
            batchInformation.BatchName = selectedBatchSearchData.LoadNumber;
            batchInformation.Equipment = ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceName;
            batchInformation.RecipeName = selectedBatchSearchData.RecipeName;
            batchInformation.StartDate = _viewModel.GetStartDateOfBatch(selectedBatchSearchData.id);
            batchInformation.EndDate = _viewModel.GetEndDateOfBatch(selectedBatchSearchData.id);

            _viewModel.BatchInfo = batchInformation;
            _viewModel.GetSelectedRecipeAndBatch(selectedBatchSearchData.id);
        }

        private async void reportNavBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (reportComboBox.Items.Count == 0)
            {
                await Task.Run(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        ComboBoxItem comboBoxItem;

                        comboBoxItem = new ComboBoxItem();
                        comboBoxItem.IsSelected = true;
                        comboBoxItem.Content = "Select a Report";
                        reportComboBox.Items.Add(comboBoxItem);

                        comboBoxItem = new ComboBoxItem();
                        comboBoxItem.IsSelected = false;
                        comboBoxItem.Content = "Batch Report";
                        reportComboBox.Items.Add(comboBoxItem);

                        comboBoxItem = new ComboBoxItem();
                        comboBoxItem.IsSelected = false;
                        comboBoxItem.Content = "Recipe Report";
                        reportComboBox.Items.Add(comboBoxItem);

                        comboBoxItem = new ComboBoxItem();
                        comboBoxItem.IsSelected = false;
                        comboBoxItem.Content = "Numeric Report";
                        reportComboBox.Items.Add(comboBoxItem);

                        comboBoxItem = new ComboBoxItem();
                        comboBoxItem.IsSelected = false;
                        comboBoxItem.Content = "Bag Numeric Report";
                        reportComboBox.Items.Add(comboBoxItem);

                        comboBoxItem = new ComboBoxItem();
                        comboBoxItem.IsSelected = false;
                        comboBoxItem.Content = "Integrity Check Report";
                        reportComboBox.Items.Add(comboBoxItem);

                        comboBoxItem = new ComboBoxItem();
                        comboBoxItem.IsSelected = false;
                        comboBoxItem.Content = "Trend Report";
                        reportComboBox.Items.Add(comboBoxItem);

                        comboBoxItem = new ComboBoxItem();
                        comboBoxItem.IsSelected = false;
                        comboBoxItem.Content = "Quality Batch Report";
                        reportComboBox.Items.Add(comboBoxItem);

                        comboBoxItem = new ComboBoxItem();
                        comboBoxItem.IsSelected = false;
                        comboBoxItem.Content = "Quality Bag Report";
                        reportComboBox.Items.Add(comboBoxItem);
                        

                       
                    });
                });
            }
        }


        private async void btnGetReport_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.WaitIndicatorControl.IsWaitIndicatorVisible = true;

            ComboBoxItem typeItem = (ComboBoxItem)reportComboBox.SelectedItem;

            int qualityCardIndex = (int)qualityCardReportComboBox.SelectedIndex;
         
            reportName = typeItem.Content.ToString();

            int selectedBatchId = _viewModel.SelectedBatchId;

            ReportCreator reportCreator = new ReportCreator(selectedBatchId, _connectionString);

            XtraReport xtraReportItem = null;


                //todo:l  refactor

            switch (reportName)
            { 
                case "Batch Report":

                    await Task.Run(() =>
                    {
                        xtraReportItem = reportCreator.BatchReport();
                    });

                    break;
                case "Recipe Report":

                    await Task.Run(() =>
                    {
                        xtraReportItem = reportCreator.RecipeReport();
                    });

                    break;
                case "Numeric Report":

                    await Task.Run(() =>
                    {
                        xtraReportItem = reportCreator.NumericReport();
                    });


                    break;
                case "Bag Numeric Report":

                    await Task.Run(() =>
                    {
                        xtraReportItem = reportCreator.NumericBagReport();
                    });

                    break;
                case "Integrity Check Report":

                    await Task.Run(() =>
                    {
                        xtraReportItem = reportCreator.IntegrityCheckReport();
                    });
                    break;

                case "Trend Report":

                    await Task.Run(() =>
                    {
                        xtraReportItem = reportCreator.TrendReport();
                    });
                    break;
                case "Quality Batch Report":

                    if (qualityCardIndex < 0)
                    {
                        WinUIMessageBox.Show("Lütfen kalite kartı seçiniz!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Question);
                    }
                    else
                    {
                        int qualityCardId = (int)qualityCardReportComboBox.SelectedValue;
                        await Task.Run(() =>
                        {
                            xtraReportItem = reportCreator.QualityBatchReport(qualityCardId); 
                        });
                    }
                    break;

                case "Quality Bag Report":

                    if (qualityCardIndex < 0)
                    {
                        WinUIMessageBox.Show("Lütfen kalite kartı seçiniz!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Question);
                    }
                    else
                    {
                        int qualityCardId = (int)qualityCardReportComboBox.SelectedValue;
                        await Task.Run(() =>
                        {
                            xtraReportItem = reportCreator.QualityBagReport(qualityCardId);
                        });
                    }
                    break;

            }

            _viewModel.WaitIndicatorControl.IsWaitIndicatorVisible = false;

            if (xtraReportItem == null)
            {
                WinUIMessageBox.Show("There is no data to show! (Gösterilecek veri bulunamadı!)", "No Data Found",
                                      MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            if (xtraReportItem != null)
            {
                if (reportComboBox.SelectedIndex > 0)
                {
                    ReportViewer reportViewer = new ReportViewer(xtraReportItem);

                    reportViewer.ShowDialog();
                }
            }
        }

        private async void btnDownloadAllReportsToExcel_Click(object sender, RoutedEventArgs e)
        {
            btnDownloadAllReportsToExcel.IsEnabled = false;
            btnDownloadAllReportsToExcel.Content = "Preparing...";

            Batch batch;
            var order = ProcessManager.Instance.GetDailyProcessOrder(_viewModel.SelectedBatchId,out batch);

            string formattedName=string.Empty;

            if (batch != null)
            {
                formattedName = $"{ReportExportSettings.ExcelExportFileNameBase }" +
                              //  $"_{ DateTime.Now:yyyy-MM-dd}" +
                              //  $"_Process{order}" +
                                $"_{batch.LoadNumber}";
            }

            ExcelReportManager excelReportManager = new ExcelReportManager(_connectionString, _viewModel.SelectedBatchId);
            excelReportManager.ExcelFilePassword= ReportExportSettings.ExcelExportPassword;
            excelReportManager.ExcelFileFullPath = System.IO.Path.Combine(ReportExportSettings.ExcelExportFilePath, formattedName) + ".xlsx";

            excelReportManager.AlarmTagConfigurations = _alarmTagConfigurations;
            bool result = await excelReportManager.CreateAllReport();

            btnDownloadAllReportsToExcel.Content = "Download All to Excel";
            btnDownloadAllReportsToExcel.IsEnabled = true;

            if (result)
            {
                MessageBoxResult messageBoxResult= WinUIMessageBox.Show("Excel raporu oluşturuldu. Açmak için tamam tuşuna basınız!", "",   MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (messageBoxResult==  MessageBoxResult.Yes )
                {
                    System.Diagnostics.Process.Start(excelReportManager.ExcelFileFullPath);
                }
            
            }
        

            //todo:m dosya var napayım uyarısı popup. Konfigrasyondasn alınan şifre ve path kullanılacak.
        }

        private void btnPrintAllTrends_Click(object sender, RoutedEventArgs e)
        {
            Trend_Window_Print trendWindowPrint = new Trend_Window_Print(_viewModel, ReportExportSettings, _viewModel.SelectedSeriesNames);
            trendWindowPrint.ShowDialog();
        }

        private void view_ShowingEditor(object sender, ShowingEditorEventArgs e)
        {
            e.Cancel = true;
        }

        private void reportComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem)reportComboBox.SelectedItem;

            if (typeItem == null)
                return;

            if(typeItem.Content.ToString().Equals("Select a Report"))
                _viewModel.IsShowReportBtnVisible = false;
            else
                _viewModel.IsShowReportBtnVisible = true;

            if (typeItem.Content.ToString().Equals("Quality Batch Report") || typeItem.Content.ToString().Equals("Quality Bag Report"))
            {
                qualityCardReportComboBox.SelectedIndex = 0;
                _viewModel.IsQualityCardsSpVisible = true;
                _viewModel.IsShowReportBtnEnabled = false;
            }
            else
            {
                _viewModel.IsQualityCardsSpVisible = false;
                _viewModel.IsShowReportBtnEnabled = true;
            }
        }

        private void qualityCardreportComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int comboBoxIndex = (int)qualityCardReportComboBox.SelectedIndex;

            if(comboBoxIndex > 0)
                _viewModel.IsShowReportBtnEnabled = true;
            else
                _viewModel.IsShowReportBtnEnabled = false;
        }

        private void filterNumericByMin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
