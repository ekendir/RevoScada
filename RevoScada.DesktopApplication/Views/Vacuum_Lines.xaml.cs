using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.WindowsUI;
using RevoScada.ProcessController;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Converters;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.Models.ModelEnums;
using RevoScada.DesktopApplication.ViewModels;
using RevoScada.Entities;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.Enums;
using RevoScada.DesktopApplication.Models.SettingModels;
using Newtonsoft.Json;

namespace RevoScada.DesktopApplication.Views
{
    /// <summary>
    /// Interaction logic for Vacuum_Lines.xaml
    /// </summary>
    public partial class Vacuum_Lines : UserControl
    {
        #region Fields
        public DispatcherTimer Timer;
        private VacuumLinesVM _viewModel;
        private BackgroundWorker _bgWorker;
        public bool AllowUpdatingSpValue;
        private readonly ApplicationColors _applicationColors;
        #endregion

        #region Properties
        public bool ShowAllItems { get; set; } = true;
        #endregion

        #region Collections
        public Dictionary<string, int> VacuumPortIsAutoManValues { get; set; }
        public Dictionary<string, int> VacuumPortVacVentOffValues { get; set; }
        private List<KeyValuePair<bool, Viewbox>> _vacuumPortItems;
        private List<Viewbox> _flowLayoutItems;
        #endregion

        public Vacuum_Lines()
        {
            InitializeComponent();

            ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
            
            _applicationColors = JsonConvert.DeserializeObject<ApplicationColors>(applicationPropertyService.GetByName("ApplicationColors").Value);
            dxFlowLayoutControl.Visibility = Visibility.Collapsed;

            AllowUpdatingSpValue = true;
            _vacuumPortItems = new List<KeyValuePair<bool, Viewbox>>();
            _flowLayoutItems = new List<Viewbox>();

            _bgWorker = new BackgroundWorker(); //Initializing the worker object
            _bgWorker.DoWork += Worker_DoWork; //Binding Worker_DoWork method
            _bgWorker.RunWorkerCompleted += Worker_RunWorkerCompleted; //Binding worker_RunWorkerCompleted method
        }

        #region Loading Functionality Section
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_viewModel.VacuumLinesSettings.GeneralFilter == GeneralFilterState.CurrentItems)
            {
                CreateVacuumPortItems(true);
            }
            else
            {
                CreateVacuumPortItems();
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _viewModel.WaitIndicatorControl.IsWaitIndicatorVisible = false;
            _viewModel.ContainerVisibility = Visibility.Visible;
            dxFlowLayoutControl.Visibility = Visibility.Visible;
            _viewModel.LowOpacityOnFilter = false;

            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromMilliseconds(1000);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }
        #endregion Loading Functionality Section Ends Here

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as VacuumLinesVM;
            _viewModel.Vacuum_Lines = this;
        }

        public void StopTimer()
        {
            Timer.Stop();
        }

        public void StartTimer()
        {
            Timer.Start();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (Timer != null)
            {
                StopTimer();
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1,
                            (Duration)TimeSpan.FromSeconds(1));
            this.BeginAnimation(UIElement.OpacityProperty, animation);

            _viewModel.Vacuum_Lines_View = this;

            if (_viewModel != null)
            {
                _viewModel.VacNotFoundVisibility = Visibility.Collapsed;
            }

            _bgWorker.RunWorkerAsync(); //Executing the worker
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            _viewModel.ContinuousUpdate(AllowUpdatingSpValue);
        }

        private List<KeyValuePair<string, int>> GetSelectedPortsByBagNames(IEnumerable<Bag> bags)
        {
            List<KeyValuePair<string, int>> selectedPortsByBags = new List<KeyValuePair<string, int>>();
            foreach (var bag in bags)
            {
                // Take bag's name in a desired format such as: Bag-1
                string fixedBagName =$"Bag-{Regex.Match(bag.BagName.Split('-')[2], @"\d+").Value}";

                // Now, it should be in a format: Bag-1

                foreach (var selectedPort in bag.SelectedPorts)
                {
                    KeyValuePair<string, int> keyValuePair = new KeyValuePair<string, int>(fixedBagName, selectedPort);
                    selectedPortsByBags.Add(keyValuePair);
                }
            }
            return selectedPortsByBags;
        }

        public void CreateVacuumPortItems(bool applyFiltering = false)
        {
            BagService bagService = new BagService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
            List<Bag> bags = bagService.BagsByBatch(ProcessManager.Instance.CurrentProcess.BatchId).ToList();
            
            List<int> selectedPortsAllBags = bags.SelectMany(x=>x.SelectedPorts).ToList();
            List<KeyValuePair<string, int>> selectedPortsByBags = GetSelectedPortsByBagNames(bags);

            int totalSelectedVacs = 0;

            this.Dispatcher.Invoke(() =>
            {
                for (int i = 1; i <= _viewModel.TotalVacs; i++)
                {
                    bool isItSelected = false;
                    string bagNameOfSelectedPort = " N/A";
                    int index = i - 1;

                    // todo:l Refactor hardcoded width and height sizes.
                    Viewbox viewbox = new Viewbox();
                    viewbox.Margin = new Thickness(3, 0, 0, 0);

                    viewbox.MaxHeight = 118;
                    viewbox.MaxWidth = 118;

                    string portName = "VAC" + i;
                    string valveBindName = "Valve[" + index + "]";
                    ActiveTag activeTag = _viewModel.ActiveTagVacs.Where(a => a.TagName == portName).FirstOrDefault();

                    GroupBox groupbox = new GroupBox();

                    groupbox.Name = "grpbox" + i;

                    StackPanel mainStackpanel = new StackPanel();

                    if (activeTag != null && selectedPortsAllBags.Contains(activeTag.id))
                    //if (activeTag != null && selectedPortsByBags.v)
                    {
                        bagNameOfSelectedPort = selectedPortsByBags.Where(s => s.Value == activeTag.id).Select(s => s.Key).FirstOrDefault();
                        isItSelected = true;
                        totalSelectedVacs++;
                        mainStackpanel.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(_applicationColors.VacuumLineItemsActiveBgColor);
                    }
                    else
                    {
                        mainStackpanel.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(_applicationColors.VacuumLineItemsInActiveBbColor);
                    }

                    // Top section
                    StackPanel topStackpanel = new StackPanel();
                    topStackpanel.Orientation = Orientation.Horizontal;
                    topStackpanel.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(_applicationColors.VacuumLineItemsTopSectionBgColor);

                    Label topLabel = new Label()
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 14,
                        Content = + i,
                        Margin = new Thickness(5, 0, 0, 3),
                        Padding = new Thickness(3, 1, 0, 1),
                        FontFamily = new FontFamily("Verdana"),
                        FontWeight = FontWeights.DemiBold,
                        Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString(_applicationColors.VacuumLineItemsTopSectionTitleColor)
                    };

                    Label unitLabel = new Label()
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 12,
                       // Content = $"({ProcessManager.Instance.ApplicationProperties["VacuumUnitTitle"].Value}) {bagNameOfSelectedPort}",
                        Content = $" ({bagNameOfSelectedPort})",

                        Margin = new Thickness(3, 0, 0, 0),
                        FontFamily = new FontFamily("Verdana"),
                        FontWeight = FontWeights.Normal,
                        Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString(_applicationColors.VacuumLineItemsTopSectionUnitTitleColor)
                    };

                    topStackpanel.Children.Add(topLabel);
                    topStackpanel.Children.Add(unitLabel);

                    // Header section
                    //StackPanel headerStackpanel = new StackPanel();

                    //Label headerLabel = new Label()
                    //{
                    //    Name = "label_" + portName,
                    //    HorizontalAlignment = HorizontalAlignment.Center,
                    //    VerticalAlignment = VerticalAlignment.Center,
                    //    FontSize = 21,
                    //    Margin = new Thickness(0,3,0,3),
                    //    ContentStringFormat = "{0:F2}",
                    //    FontFamily = new FontFamily("Lucida Console"),
                    //    FontWeight = FontWeights.DemiBold,
                    //    Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString(_applicationColors.VacuumLineItemsHeaderForeground)
                    //};

                    //Binding headerBinding = new Binding();
                    //headerBinding.Path = new PropertyPath("VacuumPortItems[" + index + "].PortValue");

                    //if (_viewModel != null)
                    //    headerBinding.Source = _viewModel;

                    //BindingOperations.SetBinding(headerLabel, Label.ContentProperty, headerBinding);

                    //headerStackpanel.Children.Add(headerLabel);

                    // VAC-VENT-OFF Section
                    StackPanel middleStackpanel = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };

                    string rbVacName = "rbVac_" + _viewModel.VacuumPortItems[index].PortName;
                    string rbVentName = "rbVent_" + _viewModel.VacuumPortItems[index].PortName;
                    string rbOffName = "rbOff_" + _viewModel.VacuumPortItems[index].PortName;
                    RadioButton rbVac = new RadioButton()
                    {
                        Name = rbVacName,
                        Content = _viewModel.VacuumLinesLanguageSettings["vac"],
                        Style = (Style)FindResource("vacBtn"),
                    };

                    RadioButton rbVent = new RadioButton()
                    {
                        Name = rbVentName,
                        Content = _viewModel.VacuumLinesLanguageSettings["vent"],
                        Style = (Style)FindResource("venBtn"),
                    };
                    RadioButton rbOff = new RadioButton()
                    {
                        Name = rbOffName,
                        Content = _viewModel.VacuumLinesLanguageSettings["off"],
                        Style = (Style)FindResource("offBtn"),
                    };

                    // VAC-VENT-OFF button binding
                    Binding vacVentOffBinding = new Binding();
                    vacVentOffBinding.NotifyOnTargetUpdated = true;

                    if (_viewModel != null)
                        vacVentOffBinding.Source = _viewModel;

                    vacVentOffBinding.Path = new PropertyPath("VacuumPortItems[" + index + "].VacVentOffState");

                    BindingOperations.SetBinding(rbVac, RadioButton.TagProperty, vacVentOffBinding);
                    BindingOperations.SetBinding(rbVent, RadioButton.TagProperty, vacVentOffBinding);
                    BindingOperations.SetBinding(rbOff, RadioButton.TagProperty, vacVentOffBinding);

                    rbVac.Click += RbVac_Clicked;
                    rbVac.TargetUpdated += RbVac_TargetUpdated;
                    rbVent.Click += RbVent_Clicked;
                    rbVent.TargetUpdated += RbVent_TargetUpdated;
                    rbOff.Click += RbOff_Clicked;
                    rbOff.TargetUpdated += RbOff_TargetUpdated;

                    middleStackpanel.Children.Add(rbVac);
                    middleStackpanel.Children.Add(rbVent);
                    middleStackpanel.Children.Add(rbOff);

                    // Man-Auto Section
                    StackPanel bottomStackpanel = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 5, 0, 5)
                    };

                    string rbManName = "rbMan_" + _viewModel.VacuumPortItems[index].PortName;
                    string rbAutoName = "rbAuto_" + _viewModel.VacuumPortItems[index].PortName;

                    RadioButton rbMan = new RadioButton()
                    {
                        Name = rbManName,
                        Content = "Man",
                        Style = (Style)FindResource("vacLinesRadioButton"),
                        Margin = new Thickness(5, 0, 5, 0),
                        VerticalAlignment = VerticalAlignment.Top,
                        FontSize = 14
                    };
                    rbMan.Checked += RbMan_Clicked;
                    rbMan.TargetUpdated += RbMan_TargetUpdated;

                    RadioButton rbAuto = new RadioButton()
                    {
                        Name = rbAutoName,
                        Content = "Auto",
                        Style = (Style)FindResource("vacLinesRadioButton"),
                        Margin = new Thickness(10, 0, 0, 0),
                        VerticalAlignment = VerticalAlignment.Top,
                        FontSize = 14
                    };
                    rbAuto.Checked += RbAuto_Clicked;
                    rbAuto.TargetUpdated += RbAuto_TargetUpdated;

                    // AUTO-MAN button binding
                    Binding autoManBinding = new Binding();
                    autoManBinding.NotifyOnTargetUpdated = true;
                    autoManBinding.Path = new PropertyPath("VacuumPortItems[" + index + "].ManuelAutoState");

                    if (_viewModel != null)
                        autoManBinding.Source = _viewModel;

                    BindingOperations.SetBinding(rbMan, RadioButton.TagProperty, autoManBinding);
                    BindingOperations.SetBinding(rbAuto, RadioButton.TagProperty, autoManBinding);

                    bottomStackpanel.Children.Add(rbMan);
                    bottomStackpanel.Children.Add(rbAuto);

                    ///Add all stackpanels to the main stackpanels as children
                    mainStackpanel.Children.Add(topStackpanel);
                   // mainStackpanel.Children.Add(headerStackpanel);
                    mainStackpanel.Children.Add(middleStackpanel);
                    mainStackpanel.Children.Add(bottomStackpanel);

                    Border mainBorder = new Border();
                    mainBorder.BorderThickness = new Thickness(0);
                    mainBorder.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#303030");

                    mainBorder.Child = mainStackpanel;

                    //groupbox.Content = mainBorder;
                    viewbox.Child = mainBorder;

                    _vacuumPortItems.Add(new KeyValuePair<bool, Viewbox>(isItSelected, viewbox));
                }

                if(applyFiltering)
                {
                    FilterVacuumPorts(true);
                } else
                {
                    _flowLayoutItems = _vacuumPortItems.Select(v => v.Value).ToList();
                }

                dxFlowLayoutControl.ItemsSource = _flowLayoutItems;
            });

            Thread.Sleep(700); /// Use this to provide smooth animation for the Content Title. It'll delay the showing vacuum groups 700 milliseconds (0.7 seconds).

            if (_viewModel != null && !ShowAllItems && totalSelectedVacs == 0)
                _viewModel.VacNotFoundVisibility = Visibility.Visible;
        }

        public void FilterVacuumPorts(bool value)
        {
            dxFlowLayoutControl.Visibility = Visibility.Collapsed;
            _viewModel.VacNotFoundVisibility = Visibility.Collapsed;

            if (value)
            {
                _flowLayoutItems = _vacuumPortItems.Where(v => v.Key == true).Select(v => v.Value).ToList();
                if (_flowLayoutItems.Count == 0)
                    _viewModel.VacNotFoundVisibility = Visibility.Visible;
            }
            else
            {
                _flowLayoutItems = _vacuumPortItems.Select(v => v.Value).ToList();
            }

            dxFlowLayoutControl.ItemsSource = _flowLayoutItems;
            dxFlowLayoutControl.Visibility = Visibility.Visible;
            _viewModel.WaitIndicatorControl.IsWaitIndicatorVisible = false;
        }

        #region Target Update event handlers
        private void RbVac_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            RadioButton rbVac = (RadioButton)sender;
            VacVentOffState rbVacTag = (VacVentOffState)rbVac.Tag;

            if (rbVacTag == VacVentOffState.Vac)
                rbVac.IsChecked = true;
            else
                rbVac.IsChecked = false;
        }

        private void RbVent_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            RadioButton rbVent = (RadioButton)sender;
            VacVentOffState rbVentTag = (VacVentOffState)rbVent.Tag;

            if (rbVentTag == VacVentOffState.Vent)
                rbVent.IsChecked = true;
            else
                rbVent.IsChecked = false;
        }

        private void RbOff_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            RadioButton rbOff = (RadioButton)sender;
            VacVentOffState rbOffTag = (VacVentOffState)rbOff.Tag;

            if (rbOffTag == VacVentOffState.Off)
                rbOff.IsChecked = true;
            else
                rbOff.IsChecked = false;
        }

        private void RbAuto_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            RadioButton rbAuto = (RadioButton)sender;
            ManuelAutoState rbAutoTag = (ManuelAutoState)rbAuto.Tag;

            if (rbAutoTag == ManuelAutoState.Auto)
                rbAuto.IsChecked = true;
            else
                rbAuto.IsChecked = false;
        }

        private void RbMan_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            RadioButton rbMan = (RadioButton)sender;
            ManuelAutoState rbManTag = (ManuelAutoState)rbMan.Tag;

            if (rbManTag == ManuelAutoState.Manuel)
                rbMan.IsChecked = true;
            else
                rbMan.IsChecked = false;
        }
        #endregion

        #region Click event handlers
        private async void RbVac_Clicked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            // Prevent setting value to PLC on load
            if (!radioButton.IsLoaded)
                return;

            string portName = radioButton.Name.Split('_')[1];

            int portNum = 0;

            if(portName.Contains("MON"))
            { portNum = Convert.ToInt32(portName.Replace("MON", "")) - 1; }
            else
            {
                portNum = Convert.ToInt32(portName.Replace("PTC", "")) - 1;
            }



            var port = _viewModel.SiemensTagConfigurationsVacVentOff[portNum];

            bool plcResult = false;

            int setValue = 1;
            string customPortName = "VAC";
            plcResult = await _viewModel.SetToPlc(setValue, port, customPortName);

            if (plcResult == false)
                WinUIMessageBox.Show(string.Format("MON {0} port için Vac değeri PLC'ye setlenemedi. Lütfen tekrar deneyiniz.", (portNum + 1)), "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void RbVent_Clicked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            // Prevent setting value to PLC on load
            if (!radioButton.IsLoaded)
                return;

            string portName = radioButton.Name.Split('_')[1];

            //int portNum = Convert.ToInt32(portName.Replace("MON", "")) - 1;

            int portNum = 0;

            if (portName.Contains("MON"))
            { portNum = Convert.ToInt32(portName.Replace("MON", "")) - 1; }
            else
            {
                portNum = Convert.ToInt32(portName.Replace("PTC", "")) - 1;
            }



            var port = _viewModel.SiemensTagConfigurationsVacVentOff[portNum];

            bool plcResult = false;

            int setValue = 2;
            string customPortName = "VENT";

            plcResult = await _viewModel.SetToPlc(setValue, port, customPortName);

            if (plcResult == false)
                WinUIMessageBox.Show(string.Format("MON {0} port için Vent değeri PLC'ye setlenemedi. Lütfen tekrar deneyiniz.", (portNum + 1)), "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void RbOff_Clicked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            // Prevent setting value to PLC on load
            if (!radioButton.IsLoaded)
                return;

            string portName = radioButton.Name.Split('_')[1];

            //int portNum = Convert.ToInt32(portName.Replace("MON", "")) - 1;
            int portNum = 0;

            if (portName.Contains("MON"))
            { portNum = Convert.ToInt32(portName.Replace("MON", "")) - 1; }
            else
            {
                portNum = Convert.ToInt32(portName.Replace("PTC", "")) - 1;
            }



            var port = _viewModel.SiemensTagConfigurationsVacVentOff[portNum];

            bool plcResult = false;

            int setValue = 3;
            string customPortName = "OFF";

            plcResult = await _viewModel.SetToPlc(setValue, port, customPortName);

            if (plcResult == false)
                WinUIMessageBox.Show(string.Format("MON {0} port için Off değeri PLC'ye setlenemedi. Lütfen tekrar deneyiniz.", (portNum + 1)), "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void RbAuto_Clicked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            // Prevent setting value to PLC on load
            if (!radioButton.IsLoaded)
                return;

            string portName = radioButton.Name.Split('_')[1];

            //int portNum = Convert.ToInt32(portName.Replace("MON", "")) - 1;


            int portNum = 0;

            if (portName.Contains("MON"))
            { portNum = Convert.ToInt32(portName.Replace("MON", "")) - 1; }
            else
            {
                portNum = Convert.ToInt32(portName.Replace("PTC", "")) - 1;
            }


            var port = _viewModel.SiemensTagConfigurationsVacuumPortIsAuto[portNum];

            bool plcResult = false;

            int setValue = 1;
            string customPortName = "AUTO";

            plcResult = await _viewModel.SetToPlc(setValue, port, customPortName);

            if (plcResult == false)
                WinUIMessageBox.Show(string.Format("MON {0} port için Auto değeri PLC'ye setlenemedi. Lütfen tekrar deneyiniz.", (portNum + 1)), "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void RbMan_Clicked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            // Prevent setting value to PLC on load
            if (!radioButton.IsLoaded)
                return;

            string portName = radioButton.Name.Split('_')[1];

            //int portNum = Convert.ToInt32(portName.Replace("MON", "")) - 1;
            int portNum = 0;

            if (portName.Contains("MON"))
            { portNum = Convert.ToInt32(portName.Replace("MON", "")) - 1; }
            else
            {
                portNum = Convert.ToInt32(portName.Replace("PTC", "")) - 1;
            }


            var port = _viewModel.SiemensTagConfigurationsVacuumPortIsAuto[portNum];

            bool plcResult = false;

            int setValue = 0;
            string customPortName = "MAN";

            plcResult = await _viewModel.SetToPlc(setValue, port, customPortName);

            if (plcResult == false)
                WinUIMessageBox.Show(string.Format("MON {0} port için Manual değeri PLC'ye setlenemedi. Lütfen tekrar deneyiniz.", (portNum + 1)), "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        #endregion

        private void dockLayoutManager_Loaded(object sender, RoutedEventArgs e)
        {
            dockLayoutManager.DockController.Activate(genVacuumDataSec, true);
        }
    }
}