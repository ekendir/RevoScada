using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using DevExpress.Xpf.WindowsUI;
using Newtonsoft.Json;
using Revo.Core;
using RevoScada.ProcessController;
using RevoScada.Synchronization.Enums;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.ViewModels;
using RevoScada.Entities;
using RevoScada.Entities.Configuration.Service;
using RevoScada.Entities.Enums;

namespace RevoScada.DesktopApplication.Views
{
    /// <summary>
    /// Interaction logic for Run_Operation.xaml
    /// </summary>
    public partial class Run_Operation : UserControl
    {
        #region Fields
        private DispatcherTimer _timer;
        private RunOperationVM _viewModel;
        private string _connectionString;
        #endregion

        public Run_Operation()
        {
            InitializeComponent();

            ProcessManager.Instance.InitializeRunOperationTags();

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as RunOperationVM;
            _connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;
            
            //btnGoToNextSegment.IsEnabled = false;
            //btnGoToPreviousSegment.IsEnabled = false;
            //btnEndRun.IsEnabled = false;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1,(Duration)TimeSpan.FromSeconds(1));
            this.BeginAnimation(UIElement.OpacityProperty, animation);
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += timer_Tick;
            _timer.Start();

            await LoadProcessCommandsPanel();
        }

        async Task LoadProcessCommandsPanel()
        {
            await Task.Delay(1000);
            Dispatcher.Invoke(() => {
 
            grpBoxProcessCommands.IsEnabled = true;

            });
          
        }


        void timer_Tick(object sender, EventArgs e)
        {
            _viewModel.ContinuousUpdate();

            if (!ProcessManager.Instance.IsRunOperationCommandWorking)
            {

                if (_viewModel.IsBatchRunning())
                {
                    _viewModel.StartButtonContent = $"{_viewModel.RunOperationLanguageSettings["processing"]}...";
                }
                else
                {
                    _viewModel.StartButtonContent = $"{_viewModel.RunOperationLanguageSettings["startRun"]}";

                    _viewModel.ChangeAllCommandsEnableState(false);
                }

                if (_viewModel.IsProcessHoldState)
                {
                    _viewModel.HoldButtonContent = $"{_viewModel.RunOperationLanguageSettings["continueProcess"]}";
                    _viewModel.StartButtonContent = $"{_viewModel.RunOperationLanguageSettings["processHolded"]}";
                    _viewModel.ChangeAllCommandsEnableState(true);
                }
                else
                {
                    _viewModel.HoldButtonContent = $"{_viewModel.RunOperationLanguageSettings["activateHold"]}";
                    _viewModel.ChangeAllCommandsEnableState(true);
                }
            }
        }

        private async void btnGoToNextSegment_Click(object sender, RoutedEventArgs e)
        {
            if (!_viewModel.IsRunOperationCommandWorking())
            {
                var goToNextResult = WinUIMessageBox.Show("Bir sonraki segmente geçilecektir, emin misiniz?", "",
      MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (goToNextResult == MessageBoxResult.No)
                    return;

                //ChangeAllCommandsEnableState(false);

                try
                {


                    int currentSegmentNo = _viewModel.GetCurrentSegmentNo();

                    Batch batch = _viewModel.GetCurrentBatchFromDB();

                    bool result = await _viewModel.GoToNextSegment();


                    if (!result)
                    {
                        WinUIMessageBox.Show("Segment değişikliğinde hata oluştu lütfen tekrar deneyin!", "", MessageBoxButton.OK, MessageBoxImage.Error);

                        LogManager.Instance.Log($"GoToNextSegment: cannot step segment { currentSegmentNo } to { currentSegmentNo + 1}", LogType.Error);

                    }
                    else
                    {
                        ProcessEventLogService processEventLogService = new ProcessEventLogService(_connectionString);


                        ProcessEventLog processEventLog = new ProcessEventLog
                        {
                            EventText = $"Segment changed from { currentSegmentNo } to { currentSegmentNo + 1} by a user: {_viewModel.ActiveUser.UserName}.",
                            CreateDate = DateTime.Now,
                            BatchId = batch.id,
                            Type = ProcessEventLogType.Manual.ToString(),
                            ModifiedByUserId = _viewModel.ActiveUser.id
                        };

                        processEventLogService.Insert(processEventLog);

                        var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
                        ProcessEventLogAdapter processEventLogAdapter = new ProcessEventLogAdapter(ApplicationConfigurations.Instance.Configuration.RedisServer);
                        processEventLogAdapter.CreateProcessEventLogSyncIssue(processEventLog, fromToDirection, ApplicationConfigurations.Instance.Configuration.PlcDevice.Id);


                    }
                }
                catch (Exception ex)
                {
                    WinUIMessageBox.Show("Segment değişikliğinde hata oluştu lütfen tekrar deneyin!", "", MessageBoxButton.OK, MessageBoxImage.Error);

                    LogManager.Instance.Log($"\n<GoToNextSegment>\n {ex}\n<GoToNextSegment\\>", LogType.Error);

                }
                finally
                {

                }


                //ChangeAllCommandsEnableState(true);


            }


        }

        private async void btnGoToPreviousSegment_Click(object sender, RoutedEventArgs e)
        {
            if (!_viewModel.IsRunOperationCommandWorking())
            {
                var goToPrevResult = WinUIMessageBox.Show("Bir önceki segmente geçilecektir, emin misiniz?", "",
                   MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (goToPrevResult == MessageBoxResult.No)
                    return;

                //ChangeAllCommandsEnableState(false);


                try
                {


                    int currentSegmentNo = _viewModel.GetCurrentSegmentNo();

                    Batch batch = _viewModel.GetCurrentBatchFromDB();

                    bool result = await _viewModel.GoToPreviousSegment();

                    if (!result)
                    {
                        WinUIMessageBox.Show("Segment değişikliğinde hata oluştu lütfen tekrar deneyin!", "", MessageBoxButton.OK, MessageBoxImage.Error);

                        LogManager.Instance.Log($"GoToPreviousSegment: cannot step segment { currentSegmentNo } to { currentSegmentNo - 1}", LogType.Error);

                    }
                    else
                    {
                        ProcessEventLogService processEventLogService = new ProcessEventLogService(_connectionString);
                        ProcessEventLog processEventLog = new ProcessEventLog
                        {
                            EventText = $"Segment changed from { currentSegmentNo } to { currentSegmentNo - 1} by a user: {_viewModel.ActiveUser.UserName}.",
                            CreateDate = DateTime.Now,
                            BatchId = batch.id,
                            Type = ProcessEventLogType.Manual.ToString(),
                            ModifiedByUserId = _viewModel.ActiveUser.id
                        };

                        processEventLogService.Insert(processEventLog);

                        var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
                        ProcessEventLogAdapter processEventLogAdapter = new ProcessEventLogAdapter(ApplicationConfigurations.Instance.Configuration.RedisServer);
                        processEventLogAdapter.CreateProcessEventLogSyncIssue(processEventLog, fromToDirection, ApplicationConfigurations.Instance.Configuration.PlcDevice.Id);
                    }
                }
                catch (Exception ex)
                {
                    WinUIMessageBox.Show("Segment değişikliğinde hata oluştu lütfen tekrar deneyin!", "", MessageBoxButton.OK, MessageBoxImage.Error);

                    LogManager.Instance.Log($"\n<GoToPreviousSegment>\n {ex}\n<GoToPreviousSegment\\>", LogType.Error);
                }
                finally
                {

                }

                //ChangeAllCommandsEnableState(true);
            }

        }

        private async void btnStartRun_Click(object sender, RoutedEventArgs e)
        {
            if (!_viewModel.IsRunOperationCommandWorking())
            {


                var startRunResult = WinUIMessageBox.Show("Proses başlatılacaktır. Lütfen devam etmek için evet tuşuna basın.", "", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (startRunResult == MessageBoxResult.No)
                    return;

                //ChangeAllCommandsEnableState(false);

                _viewModel.StartButtonContent = "Wait for Run!";
                // btnStartRun.IsEnabled = false;

                try
                {


                    bool result = await _viewModel.StartRun();

                    if (!result)
                    {
                        WinUIMessageBox.Show("Proses çalıştırılamadı!", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        _viewModel.StartButtonContent = $"{_viewModel.RunOperationLanguageSettings["processing"]}...";

                        BatchService batchService = new BatchService(_connectionString);
                        Batch batch = batchService.GetActiveCurrentBatch();

                        ApplicationPropertyService applicationProperty = new ApplicationPropertyService(_connectionString);
                        applicationProperty.UpdateByName("TrendSelectedPortUIProperties", null);

                        ProcessEventLogService processEventLogService = new ProcessEventLogService(_connectionString);

                        ProcessEventLog processEventLog = new ProcessEventLog
                        {
                            EventText = $"Start Run command invoked for {batch.LoadNumber} by a user: {_viewModel.ActiveUser.UserName}.",
                            CreateDate = DateTime.Now,
                            BatchId = batch.id,
                            Type = ProcessEventLogType.Manual.ToString(),
                            ModifiedByUserId = _viewModel.ActiveUser.id
                        };

                        processEventLogService.Insert(processEventLog);

                        var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
                        ProcessEventLogAdapter processEventLogAdapter = new ProcessEventLogAdapter(ApplicationConfigurations.Instance.Configuration.RedisServer);
                        processEventLogAdapter.CreateProcessEventLogSyncIssue(processEventLog, fromToDirection, ApplicationConfigurations.Instance.Configuration.PlcDevice.Id);

                        string processEventLogSerialized = JsonConvert.SerializeObject(processEventLog, Formatting.Indented);

                        LogManager.Instance.Log($"\n<StartRun>{processEventLogSerialized}\n<StartRun>", LogType.Information);
                    }
                }
                catch (Exception ex)
                {
                    LogManager.Instance.Log($"StartRun Command Error:\n {ex} \n", LogType.Error);
                }
                finally
                {

                }



            }


            //ChangeAllCommandsEnableState(true);

        }

        private async void btnEndRun_Click(object sender, RoutedEventArgs e)
        {
            if (!_viewModel.IsRunOperationCommandWorking())
            {
                var endRunResult = WinUIMessageBox.Show("Proses sonlandırılacaktır, emin misiniz?", "",
                  MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (endRunResult == MessageBoxResult.No)
                    return;

                _viewModel.ChangeAllCommandsEnableState(false);

                try
                {
                    Batch batch = _viewModel.GetCurrentBatchFromDB() ?? new Batch();
                    bool result = await _viewModel.EndRun();

                    if (!result)
                    {
                        WinUIMessageBox.Show("End run error! (Process sonlandırılırken hata oluştu!)", "", MessageBoxButton.OK, MessageBoxImage.Error);
                        LogManager.Instance.Log($"Run operation end run command is false", LogType.Error);
                    }
                    else
                    {
                        ProcessEventLogService processEventLogService = new ProcessEventLogService(_connectionString);
                        ProcessEventLog processEventLog = new ProcessEventLog
                        {
                            EventText = $"End Run command invoked for {batch.LoadNumber} by a user: {_viewModel.ActiveUser.UserName}.",
                            CreateDate = DateTime.Now,
                            BatchId = batch.id,
                            Type = ProcessEventLogType.Manual.ToString()
                        };
                        processEventLogService.Insert(processEventLog);

                        var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
                        ProcessEventLogAdapter processEventLogAdapter = new ProcessEventLogAdapter(ApplicationConfigurations.Instance.Configuration.RedisServer);
                        processEventLogAdapter.CreateProcessEventLogSyncIssue(processEventLog, fromToDirection, ApplicationConfigurations.Instance.Configuration.PlcDevice.Id);

                        WinUIMessageBox.Show("Process ended successfully! (Process başarıyla sonlandırıldı!)", "", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Close the opened windows such as Trend to refresh the data on it
                        _viewModel.MainWindow.CloseOpenedWindows();
                    }
                }
                catch (Exception ex)
                {
                    WinUIMessageBox.Show("End run error! (Process sonlandırılırken hata oluştu!)", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    LogManager.Instance.Log($"\n<EndRun>\n {ex}\n<EndRun\\>", LogType.Error);
                }
                finally
                {
                    // btnEndRun.Content= "End Run";
                }
                _viewModel.ChangeAllCommandsEnableState(true);
            }
        }

        private async void btnHold_Click(object sender, RoutedEventArgs e)
        {
            if ( !_viewModel.IsRunOperationCommandWorking())
            {
                MessageBoxResult activateHoldResult;
                string stateVal = string.Empty;

                if (ProcessManager.Instance.IsProcessHoldState())
                {
                    activateHoldResult = WinUIMessageBox.Show("Proses devam ettirilecektir, emin misiniz?", "",
                                        MessageBoxButton.YesNo, MessageBoxImage.Question);
                    stateVal = "Continue Process command";
                }
                else
                {
                    activateHoldResult = WinUIMessageBox.Show("Proses duraklatılacaktır, emin misiniz?", "",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);
                    stateVal = "Hold Run command";
                }

                if (activateHoldResult == MessageBoxResult.No)
                    return;

                _viewModel.ChangeAllCommandsEnableState(false);
                bool result = await _viewModel.ActivateHold();

                try
                {
                    if (!result)
                    {
                        LogManager.Instance.Log($"Hold operation result false", LogType.Error);
                    }
                    else
                    {
                        Batch batch = _viewModel.GetCurrentBatchFromDB() ?? new Batch();
                        ProcessEventLogService processEventLogService = new ProcessEventLogService(_connectionString);

                        ProcessEventLog processEventLog = new ProcessEventLog
                        {
                            EventText = $"{stateVal} invoked for {batch.LoadNumber} by a user: {_viewModel.ActiveUser.UserName}.",
                            CreateDate = DateTime.Now,
                            BatchId = batch.id,
                            Type = ProcessEventLogType.Manual.ToString()
                        };
                        processEventLogService.Insert(processEventLog);

                        var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
                        ProcessEventLogAdapter processEventLogAdapter = new ProcessEventLogAdapter(ApplicationConfigurations.Instance.Configuration.RedisServer);
                        processEventLogAdapter.CreateProcessEventLogSyncIssue(processEventLog, fromToDirection, ApplicationConfigurations.Instance.Configuration.PlcDevice.Id);
                    }
                }
                catch (Exception)
                {
                    LogManager.Instance.Log($"Hold operation event log insert failure!", LogType.Error);
                }
                finally
                {
                }
                _viewModel.ChangeAllCommandsEnableState(true);
            } 
        }

        private void CircularGaugeControl_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = !needle.IsInteractive;
        }
    }
}