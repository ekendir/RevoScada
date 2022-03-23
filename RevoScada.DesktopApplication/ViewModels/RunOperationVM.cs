using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.WindowsUI;
using Newtonsoft.Json;
using Revo.Core;
using RevoScada.ProcessController;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Models;
using RevoScada.Entities;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.PageTagConfigurations;
using System.Windows.Media.Imaging;

namespace RevoScada.DesktopApplication.ViewModels
{
    public class RunOperationVM : UserControlBaseVM
    {
        #region Services
        private readonly string _connectionString;
        private BatchService _batchService;
        private ActiveTagService _activeTagService;
        #endregion

        #region Info Properties
        public string StartRunInfo { get; set; } = "Prosesin start edilebilmesi için aşağıdaki şartların tamamının sağlanmış olması gerekmektedir.";
        public string NextSegmentInfo { get; set; } = "Reçetede bir sonraki segmente geçmek için kullanılır.";
        public string PrevSegmentInfo { get; set; } = "Reçetede bir önceki segmente geçmek için kullanılır.";
        public string HoldProcessInfo { get; set; } = "Prosesi duraklatmak için kullanılır. Tekrar basılarak proses kaldığı yerden devam ettirilir.";
        public string EndProcessInfo { get; set; } = "Prosesi sonlandırmak için kullanılır. Proses devam ettirilemez!";
        #endregion

        #region Commands
        public ICommand StartRunCommand { get; set; }
        public ICommand GoToNextSegCommand { get; set; }
        public ICommand GoToPrevSegCommand { get; set; }
        public ICommand ActivateHoldCommand { get; set; }
        public ICommand EndRunCommand { get; set; }
        #endregion

        #region Fields
        private PlcCommandManager _plcCommandManager;
        private bool _isTagConfigsLoaded;
        private bool _isProcessEnded;
        private RunOperationTagConfigurations _runOperationTagConfigurations;
        private string crossImageSource;
        private string tickImageSource;
        public MainWindow MainWindow;
        #endregion

        #region Properties

        public bool EnterPartsValue { get; set; }
        public bool RecipeEditorValue { get; set; }
        public bool IntegrityCheckValue { get; set; }
        public bool SkipIntegrityCheckValue { get; set; }
        public float DoorStatusValue { get; set; }

        #region Operation Buttons Enable States
        private bool _startButtonEnableValue;
        public bool StartButtonEnableValue
        {
            get => _startButtonEnableValue;
            set => OnPropertyChanged(ref _startButtonEnableValue, value);
        }
        private bool _holdButtonEnableValue;
        public bool HoldButtonEnableValue
        {
            get => _holdButtonEnableValue;
            set => OnPropertyChanged(ref _holdButtonEnableValue, value);
        }
        private bool _endRunButtonEnableValue;
        public bool EndRunButtonEnableValue
        {
            get => _endRunButtonEnableValue;
            set => OnPropertyChanged(ref _endRunButtonEnableValue, value);
        }
        private bool _goToPreSegButtonEnableValue;
        public bool GoToPreSegButtonEnableValue
        {
            get => _goToPreSegButtonEnableValue;
            set => OnPropertyChanged(ref _goToPreSegButtonEnableValue, value);
        }
        private bool _goToNextSegButtonEnableValue;
        public bool GoToNextSegButtonEnableValue
        {
            get => _goToNextSegButtonEnableValue;
            set => OnPropertyChanged(ref _goToNextSegButtonEnableValue, value);
        }
        #endregion

        public bool BatchStartActionValue { get; set; }
        public bool GoToNextSegmentValue { get; set; }
        public bool HoldRunValue { get; set; }
        public bool EndRunValue { get; set; }



        private string _startButtonContent;
        public string StartButtonContent
        {
            get => _startButtonContent;
            set => OnPropertyChanged(ref _startButtonContent, value);
        }

        private string _stopButtonContent;
        public string StopButtonContent
        {
            get => _stopButtonContent;
            set => OnPropertyChanged(ref _stopButtonContent, value);
        }

        private string _holdButtonContent;
        public string HoldButtonContent
        {
            get => _holdButtonContent;
            set => OnPropertyChanged(ref _holdButtonContent, value);
        }

        private string _loadNumber;
        public string LoadNumber
        {
            get => _loadNumber;
            set => OnPropertyChanged(ref _loadNumber, value);
        }

        private ImageSource _enterPartsImageSource;
        public ImageSource EnterPartsImageSource
        {
            get => _enterPartsImageSource;
            set => OnPropertyChanged(ref _enterPartsImageSource, value);
        }
        private ImageSource _recipeEditorImageSource;
        public ImageSource RecipeEditorImageSource
        {
            get => _recipeEditorImageSource;
            set => OnPropertyChanged(ref _recipeEditorImageSource, value);
        }
        private ImageSource _integrityImageSource;
        public ImageSource IntegrityImageSource
        {
            get => _integrityImageSource;
            set => OnPropertyChanged(ref _integrityImageSource, value);
        }

        private ImageSource _doorImageSource;
        public ImageSource DoorImageSource
        {
            get => _doorImageSource;
            set => OnPropertyChanged(ref _doorImageSource, value);
        }

        private ImageSource _runOperationImageSource;
        public ImageSource RunOperationImageSource
        {
            get => _runOperationImageSource;
            set => OnPropertyChanged(ref _runOperationImageSource, value);
        }

        public bool IsProcessHoldState
        {

            get
            {
                return ProcessManager.Instance.IsProcessHoldState();
            }
        }

        private RunOperationProcessStartStepsModel _runOperationProcessStartStepsModel;

        public RunOperationProcessStartStepsModel RunOperationProcessStartStepsModel
        {
            get => _runOperationProcessStartStepsModel;
            set => OnPropertyChanged(ref _runOperationProcessStartStepsModel, value);
        }


        private string _vacuumUnitTitle;
        public string VacuumUnitTitle
        {
            get => _vacuumUnitTitle;
            set => OnPropertyChanged(ref _vacuumUnitTitle, value);
        }

        private Visibility _pressureValueVisibility;
        public Visibility PressureValueVisibility
        {
            get => _pressureValueVisibility;
            set => OnPropertyChanged(ref _pressureValueVisibility, value);
        }


        #region Vacuum Min/Max Value
        private float _vacuumMinValue;
        public float VacuumMinValue
        {
            get => _vacuumMinValue;
            set => OnPropertyChanged(ref _vacuumMinValue, value);
        }
        #endregion Vacuum Min/Max Value

        public ValueWrapper<string> CurrentBatchNumber { get; set; }
        public ValueWrapper<string> CurrentRecipeName { get; set; }

        #region Circular Gauges

        private float _airTC;
        public float AirTC
        {
            get => _airTC;
            set => OnPropertyChanged(ref _airTC, value);
        }

        private float _pressure;
        public float Pressure
        {
            get => _pressure;
            set => OnPropertyChanged(ref _pressure, value);
        }

        private float _vacuum;
        public float Vacuum
        {
            get => _vacuum;
            set => OnPropertyChanged(ref _vacuum, value);
        }


        #endregion

        public Dictionary<string, string> RunOperationLanguageSettings { get; set; }
        #endregion

        //BatchStatus
        //0	"NotStarted"
        //1	"Running"
        //2	"Finished"
        //3	"Stopped"

        public RunOperationVM(WaitIndicatorControl waitIndicatorControl, Dictionary<string, bool> permissions, UserGridModel activeUser,
                              ValueWrapper<string> currentBatchNumber, ValueWrapper<string> currentRecipeName, MainWindow mainWindow)
        {
            _connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;
            _batchService = new BatchService(_connectionString);
            _activeTagService = new ActiveTagService(_connectionString);
            _isTagConfigsLoaded = false;
            _isProcessEnded = false;
            ActiveUser = activeUser;

            CurrentBatchNumber = currentBatchNumber;
            CurrentRecipeName = currentRecipeName;
            MainWindow = mainWindow;

            switch (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId)
            {
                case 1:
                case 2:
                case 20:
                    PressureValueVisibility = Visibility.Visible;
                    break;
                case 3:
                    PressureValueVisibility = Visibility.Collapsed;
                    break;
            }

            if (ProcessManager.Instance.CurrentProcess.IsBatchLoaded)
                LoadNumber = ProcessManager.Instance.CurrentProcess.LoadNumber;

            crossImageSource = "pack://siteoforigin:,,,/Resources/Red_Cross_48px.png";
            tickImageSource = "pack://siteoforigin:,,,/Resources/Green_Tick_48px.png";

            EnterPartsImageSource = new ImageSourceConverter().ConvertFromString(crossImageSource) as ImageSource;
            RecipeEditorImageSource = new ImageSourceConverter().ConvertFromString(crossImageSource) as ImageSource;
            IntegrityImageSource = new ImageSourceConverter().ConvertFromString(crossImageSource) as ImageSource;
            DoorImageSource = new ImageSourceConverter().ConvertFromString(crossImageSource) as ImageSource;
            RunOperationImageSource = GetRunOperationImage();

            Permissions = permissions;
            // todo:h Implement language preference in a parametric way, currently I'm forcing to using English :/
            if (ApplicationLanguageSettings != null)
                RunOperationLanguageSettings = ApplicationLanguageSettings.Eng.RunOperation;

            RunOperationProcessStartStepsModel = new RunOperationProcessStartStepsModel();
            ProcessManager.Instance.IsRunOperationCommandWorking = false;
            StopButtonContent = RunOperationLanguageSettings["endRun"];
            VacuumUnitTitle = ProcessManager.Instance.ApplicationProperties["VacuumUnitTitle"].Value;
            VacuumMinValue = float.Parse(ProcessManager.Instance.ApplicationProperties["VacuumMinValue"].Value);
        }

        #region PLC commanding
        private void InitializePageTagConfigurations()
        {
            _plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
            var pageTagConfiguration = pageTagConfigurationService.GetByName("RunOperation");
            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
            _runOperationTagConfigurations = JsonConvert.DeserializeObject<RunOperationTagConfigurations>(jsonSerializedString);

            _runOperationTagConfigurations.EnterPartsOk = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.EnterPartsOk)];
            _runOperationTagConfigurations.RecipeOk = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.RecipeOk)];
            _runOperationTagConfigurations.IntegrityCheckOk = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.IntegrityCheckOk)];
            _runOperationTagConfigurations.SkipIntegrityCheck = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.SkipIntegrityCheck)];
            _runOperationTagConfigurations.DoorStatus = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.DoorStatus)];
            _runOperationTagConfigurations.StartButtonEnable = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.StartButtonEnable)];
            _runOperationTagConfigurations.BatchStartAction = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.BatchStartAction)];
            _runOperationTagConfigurations.GoToNextSegment = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.GoToNextSegment)];
            _runOperationTagConfigurations.BackToNextSegment = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.BackToNextSegment)];
            _runOperationTagConfigurations.HoldRun = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.HoldRun)];
            _runOperationTagConfigurations.EndRun = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.EndRun)];
            _runOperationTagConfigurations.ActualTemperatureCalcSetPoint = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.ActualTemperatureCalcSetPoint)];
            _runOperationTagConfigurations.VacActual = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.VacActual)];

            if (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId != 3)
            { _runOperationTagConfigurations.PressureActual = (SiemensTagConfiguration)ApplicationConfigurations.Instance.TagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.PressureActual)]; }



        }

        public void ContinuousUpdate()
        {
            if (_isTagConfigsLoaded == false)
            {
                _isTagConfigsLoaded = true;
                InitializePageTagConfigurations();
                //WaitIndicatorControl.IsWaitIndicatorVisible = false;
            }

            PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
            EnterPartsValue = plcCommandManager.Get<bool>((SiemensTagConfiguration)_runOperationTagConfigurations.EnterPartsOk, false);
            RecipeEditorValue = plcCommandManager.Get<bool>((SiemensTagConfiguration)_runOperationTagConfigurations.RecipeOk, false);
            IntegrityCheckValue = plcCommandManager.Get<bool>((SiemensTagConfiguration)_runOperationTagConfigurations.IntegrityCheckOk, false);
            SkipIntegrityCheckValue = plcCommandManager.Get<bool>((SiemensTagConfiguration)_runOperationTagConfigurations.SkipIntegrityCheck, false);
            DoorStatusValue = plcCommandManager.Get<float>((SiemensTagConfiguration)_runOperationTagConfigurations.DoorStatus, false);
            StartButtonEnableValue = plcCommandManager.Get<bool>((SiemensTagConfiguration)_runOperationTagConfigurations.StartButtonEnable, false);
            BatchStartActionValue = plcCommandManager.Get<bool>((SiemensTagConfiguration)_runOperationTagConfigurations.BatchStartAction, false);
            GoToNextSegmentValue = plcCommandManager.Get<bool>((SiemensTagConfiguration)_runOperationTagConfigurations.GoToNextSegment, false);
            HoldRunValue = plcCommandManager.Get<bool>((SiemensTagConfiguration)_runOperationTagConfigurations.HoldRun, false);
            EndRunValue = plcCommandManager.Get<bool>((SiemensTagConfiguration)_runOperationTagConfigurations.EndRun, false);



            if (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId != 3)
            {
                Pressure = plcCommandManager.Get<float>((SiemensTagConfiguration)_runOperationTagConfigurations.PressureActual, false);
            }

            Vacuum = plcCommandManager.Get<float>((SiemensTagConfiguration)_runOperationTagConfigurations.VacActual, false);
            AirTC = plcCommandManager.Get<float>((SiemensTagConfiguration)_runOperationTagConfigurations.ActualTemperatureCalcSetPoint, false);

            if (!ProcessManager.Instance.CurrentProcess.IsBatchLoaded)
                LoadNumber = null;

            SetImageIndicators();
        }

        private void SetImageIndicators()
        {

            if (EnterPartsValue)
            {
                bool isEnterPartsSkipped = ProcessManager.Instance.IsEnterPartsSkipped();


                if (isEnterPartsSkipped)
                {
                    RunOperationProcessStartStepsModel.EnterPartOkOpacity = 1f;
                    RunOperationProcessStartStepsModel.EnterPartOkSummary = $"Load Number: {ProcessManager.Instance.CurrentProcess.LoadNumber}";
                    RunOperationProcessStartStepsModel.EnterPartOkHeader = "Enter Parts Skipped";
                }
                else
                {
                    RunOperationProcessStartStepsModel.EnterPartOkOpacity = 1f;
                    RunOperationProcessStartStepsModel.EnterPartOkSummary = $"Load Number: {ProcessManager.Instance.CurrentProcess.LoadNumber}";
                    RunOperationProcessStartStepsModel.EnterPartOkHeader = "Enter Parts";
                }

            }
            else
            {
                RunOperationProcessStartStepsModel.EnterPartOkOpacity = 0.3f;
                RunOperationProcessStartStepsModel.EnterPartOkSummary = $"-";
                RunOperationProcessStartStepsModel.EnterPartOkHeader = "Enter Parts";
            }

            if (RecipeEditorValue)
            {
                RunOperationProcessStartStepsModel.RecipeOkOpacity = 1f;
                RunOperationProcessStartStepsModel.AbbreviatedRecipeOkSummary = $"{ProcessManager.Instance.CurrentProcess.ActiveRecipeName}";
                RunOperationProcessStartStepsModel.RecipeOkHeader = "Recipe";
            }
            else
            {
                RunOperationProcessStartStepsModel.RecipeOkOpacity = 0.3f;
                RunOperationProcessStartStepsModel.AbbreviatedRecipeOkSummary = $"-";
                RunOperationProcessStartStepsModel.RecipeOkHeader = "Recipe";
            }

            if (IntegrityCheckValue || SkipIntegrityCheckValue)
            {
                RunOperationProcessStartStepsModel.IntegrityCheckOkOpacity = 1f;
                RunOperationProcessStartStepsModel.IntegrityCheckOkSummary = (SkipIntegrityCheckValue == true) ? $"Skipped" : $"Passed Successfully";
                RunOperationProcessStartStepsModel.IntegrityCheckOkHeader = "Integrity Checks";
            }
            else
            {
                RunOperationProcessStartStepsModel.IntegrityCheckOkOpacity = 0.3f;
                RunOperationProcessStartStepsModel.IntegrityCheckOkSummary = $"-";
                RunOperationProcessStartStepsModel.IntegrityCheckOkHeader = "Integrity Checks";
            }

            if (DoorStatusValue == 1)
            {
                RunOperationProcessStartStepsModel.DoorStatusOpacity = 1f;
                RunOperationProcessStartStepsModel.DoorStatusSummary = $"Door Closed";
                RunOperationProcessStartStepsModel.DoorStatusHeader = "Door Status";
            }
            else
            {
                RunOperationProcessStartStepsModel.DoorStatusOpacity = 0.3f;
                RunOperationProcessStartStepsModel.DoorStatusSummary = $"-";
                RunOperationProcessStartStepsModel.DoorStatusHeader = "Door Status";
            }


            RunOperationProcessStartStepsModel.OverAllOkOpacity = (RunOperationProcessStartStepsModel.EnterPartOkOpacity == 1f && RunOperationProcessStartStepsModel.RecipeOkOpacity == 1f && RunOperationProcessStartStepsModel.IntegrityCheckOkOpacity == 1f && RunOperationProcessStartStepsModel.DoorStatusOpacity == 1f) ? 1f : 0.3f;


            if (RunOperationProcessStartStepsModel.OverAllOkOpacity == 1)
            {
                RunOperationProcessStartStepsModel.OverAllOkHeader = "Ok!";
            }

            // _overAllOkOpacity = (EnterPartOkOpacity == 1f && RecipeOkOpacity == 1f && IntegrityCheckOkOpacity == 1f && DoorStatusOpacity == 1f) ? 1f : 0.3f;

        }

        /*
        private void SetImageIndicators()
        {
            if(EnterPartsValue)
                EnterPartsImageSource = new ImageSourceConverter().ConvertFromString(tickImageSource) as ImageSource;
            else
                EnterPartsImageSource = new ImageSourceConverter().ConvertFromString(crossImageSource) as ImageSource;

            if (RecipeEditorValue)
                RecipeEditorImageSource = new ImageSourceConverter().ConvertFromString(tickImageSource) as ImageSource;
            else
                RecipeEditorImageSource = new ImageSourceConverter().ConvertFromString(crossImageSource) as ImageSource;

            if (IntegrityCheckValue || SkipIntegrityCheckValue)
                IntegrityImageSource = new ImageSourceConverter().ConvertFromString(tickImageSource) as ImageSource;
            else
                IntegrityImageSource = new ImageSourceConverter().ConvertFromString(crossImageSource) as ImageSource;

            // 0 or 3 meaning door is closed, 1 is open
            if (DoorStatusValue == 1)
                DoorImageSource = new ImageSourceConverter().ConvertFromString(tickImageSource) as ImageSource;
            else
                DoorImageSource = new ImageSourceConverter().ConvertFromString(crossImageSource) as ImageSource;
        }
         */

        //private void ResetCacheAndUpdateDb()
        //{
        //    _processManager = new ProcessManager();
        //    CurrentBatchInfo currentBatchInfo = new CurrentBatchInfo();
        //    currentBatchInfo = _processManager.CurrentBatchInfo;

        //    if(currentBatchInfo != null)
        //    {
        //        // Set current batch group id to last 5 batches
        //        Batch activeBatch = _batchService.GetById(currentBatchInfo.BatchId);

        //        if(activeBatch != null)
        //        {
        //            activeBatch.EndDate = DateTime.Now;
        //            activeBatch.BatchGroupId = 0; // Last 5 Batches group id
        //            activeBatch.Status = 2; // Finished

        //            _batchService.Update(activeBatch);

        //            // Set ActiveTags values to the false
        //            _activeTagService.ResetActiveSensors*();
        //        }

        //        // Set currentBatch object to the cache
        //        currentBatchInfo.CurrentState = BatchCurrentState.Finished;
        //        _processManager.CurrentBatchInfo = currentBatchInfo;
        //    }

        //    // Reset cache
        //    _processManager.ResetAllProcessInfo();
        //}

        #endregion

        private ImageSource GetRunOperationImage()
        {
            var runOpImageFromConfiguration = new BitmapImage();
            runOpImageFromConfiguration.BeginInit();
            runOpImageFromConfiguration.UriSource = new Uri(ApplicationConfigurations.Instance.Configuration.RunOperationImageFullPath);
            runOpImageFromConfiguration.EndInit();
            return runOpImageFromConfiguration;
        }

        public async Task<bool> StartRun()
        {
            bool result = false;
            if (ProcessManager.Instance.CurrentProcess.IsRecipeLoaded)
            {
                Dictionary<ProcessSteps, bool> processStepResults = null;


                result = await Task.Run(() =>
                {

                    processStepResults = ProcessManager.Instance.StartProcess();

                    result = processStepResults.Values.All(x => x == true);

                    if (!result)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (var item in processStepResults)
                        {
                            stringBuilder.AppendLine($"Status: {item.Value}\t{item.Key}");
                        }
                        LogManager.Instance.Log($"\n<Run Fail Steps>\n{stringBuilder}\n<Run Fail Steps\\>", LogType.Information);
                    }

                    return result;

                });

                // Close the opened windows such as Trend to refresh the data on it
                if (result)
                    MainWindow.CloseOpenedWindows();
            }
            return result;
        }

        public async Task<bool> EndRun()
        {
            bool endRunResult = false;
            try
            {
                for (int i = 0; i < 5; i++)
                {
                    await Task.Delay(500);
                    StopButtonContent = (i % 2 == 0) ? $"{RunOperationLanguageSettings["stopping"]} !" : $"{RunOperationLanguageSettings["stopping"]}  ";
                }
                endRunResult = await ProcessManager.Instance.EndProcess();

                CurrentBatchNumber.Value = null;
                CurrentRecipeName.Value = null;

                await Task.Delay(1000);
            }
            catch { }

            StopButtonContent = $"{RunOperationLanguageSettings["endRun"]}";

            return endRunResult;
        }


        public async Task<bool> GoToNextSegment()
        {
            return await ProcessManager.Instance.GoToNextSegment();
        }


        public async Task<bool> GoToPreviousSegment()
        {
            return await ProcessManager.Instance.GoToPreviousSegment();
        }


        public async Task<bool> ActivateHold()
        {

            return await ProcessManager.Instance.ActivateHold();


            //// Set currentBatch object to the cache
            //CurrentBatchInfo currentBatchInfo = new CurrentBatchInfo();
            //currentBatchInfo = processManager.CurrentBatchInfo;

            //if (!isToggleOn)
            //{
            //    currentBatchInfo.CurrentState = BatchCurrentState.Hold;
            //    // Send holdRun command to PLC
            //    _plcCommandManager.Set(_holdRun, true);

            //    // Update batch database table
            //    Batch activeBatch = _batchService.GetById(currentBatchInfo.BatchId);

            //    if (activeBatch != null)
            //    {
            //        activeBatch.EndDate = DateTime.Now;
            //        activeBatch.Status = 4; // 4 = Hold status
            //        _batchService.Update(activeBatch);
            //    }
            //}
            //else
            //{
            //    currentBatchInfo.CurrentState = BatchCurrentState.Running;
            //    // Send startRun command to PLC
            //    _plcCommandManager.Set(_holdRun, false);

            //    // Update batch database table
            //    Batch activeBatch = _batchService.GetById(currentBatchInfo.BatchId);

            //    if (activeBatch != null)
            //    {
            //        activeBatch.StartDate = DateTime.Now;
            //        activeBatch.Status = 1; // 1 = Running status
            //        _batchService.Update(activeBatch);
            //    }
            //}

            //processManager.CurrentBatchInfo = currentBatchInfo;


            // Continue here...
        }

        public void ChangeAllCommandsEnableState(bool isEnabled)
        {
            if (IsProcessHoldState)
            {
                GoToPreSegButtonEnableValue = true;
                GoToNextSegButtonEnableValue = true;
                EndRunButtonEnableValue = true;
            }
            else
            {
                GoToPreSegButtonEnableValue = false;
                GoToNextSegButtonEnableValue = false;
                EndRunButtonEnableValue = false;
            }

            if (IsBatchRunning())
            {
                HoldButtonEnableValue = isEnabled;
            }
            else
            {
                HoldButtonEnableValue = false;
            }
        }

        public bool IsBatchRunning()
        {
            return ProcessManager.Instance.IsBatchRunning();
        }

        public bool IsRunOperationCommandWorking()
        {
            return ProcessManager.Instance.IsRunOperationCommandWorking;
        }


        public int GetCurrentSegmentNo()
        {
            int currentSegmentNo = ProcessManager.Instance.CurrentSegmentNo();
            return currentSegmentNo;
        }
        public Batch GetCurrentBatchFromDB()
        {

            BatchService batchService = new BatchService(_connectionString);
            Batch batch = batchService.GetActiveCurrentBatch();
            return batch;
        }
    }
}