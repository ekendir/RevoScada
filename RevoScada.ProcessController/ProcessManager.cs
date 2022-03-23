using Newtonsoft.Json;
using Revo.Core;
using RevoScada.Cache;
using RevoScada.Synchronization;
using RevoScada.Synchronization.Enums;
using RevoScada.Synchronization.Types;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.Entities;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Complex.Alarm;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.Configuration.Service;
using RevoScada.Entities.Enums;
using RevoScada.Entities.PageTagConfigurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RevoScada.ProcessController
{
    public sealed class ProcessManager
    {
        private static readonly Lazy<ProcessManager> lazy = new Lazy<ProcessManager>(() => new ProcessManager());

        public static ProcessManager Instance => lazy.Value;
        private CacheManager _mainCacheManager;
        private CacheManager _writeCacheManager;

        public Task SaveIntegrityTask { get; set; }
        private RunOperationTagConfigurations _runOperationTagConfigurations;
        private Dictionary<string, BagDetailDto> _processBagDetails;
        Dictionary<string, ApplicationProperty> _applicationProperties;
        private string _connectionString;

        private Dictionary<int, ITagConfiguration> _tagConfigurations;

        private PlcCommandManager _plcCommandManager;
        private ServiceManager _serviceManager;
        private ApplicationConfiguration _applicationConfiguration;
        private List<int> _onDemandDB;
        public int PlcDeviceId;

        public bool IsRunOperationCommandWorking
        {
            get
            {
                string isRunOperationCommandWorkingSerialized = _mainCacheManager.GetString($"IsRunOperationCommandWorkingPLC{PlcDeviceId}");
                return isRunOperationCommandWorkingSerialized != null && Convert.ToBoolean(Convert.ToInt32(isRunOperationCommandWorkingSerialized));
            }
            set
            {
                TimeSpan keyExpire = TimeSpan.FromSeconds(20);
                _mainCacheManager.Set($"IsRunOperationCommandWorkingPLC{PlcDeviceId}", value, keyExpire);
            }
        }
        public Dictionary<string, ApplicationProperty> ApplicationProperties
        {
            get
            {
                if (_applicationProperties is null)
                {
                    _applicationProperties = new Dictionary<string, ApplicationProperty>();

                    ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(_connectionString);

                    _applicationProperties = applicationPropertyService.GetAll().ToDictionary(x => x.Name, x => x);
                }
                return _applicationProperties;
            }
        }

        /// <summary>
        /// Retrieves bag detail info for given ptc,mon,vac name
        /// </summary>
        public Dictionary<string, BagDetailDto> ProcessBagDetails
        {
            get
            {
                _processBagDetails = new Dictionary<string, BagDetailDto>();

                Dictionary<string, BagDetailDto> bagDetails;

                ActiveTagService activeTagService = new ActiveTagService(_connectionString);
                BagService bagService = new BagService(_connectionString);

                Dictionary<int, string> activeTagsNameCollection = activeTagService.GetAll().Where(x => x.ActiveTagGroupId != ActiveTagGroups.DefaultSensor).ToList().ToDictionary(x => x.id, x => x.TagName);

                var bags = bagService.BagsByBatch(CurrentProcess.BatchId);

                bagDetails = new Dictionary<string, BagDetailDto>();

                foreach (var bagItem in bags.GroupBy(x => x.BagName))
                {
                    foreach (var bagdetail in bagItem)
                    {
                        foreach (int selectedPort in bagdetail.SelectedPorts)
                        {
                            BagDetailDto bagDetailDto = new BagDetailDto();

                            if (activeTagsNameCollection.ContainsKey(selectedPort))
                            {
                                bagDetailDto.SelectedPortName = activeTagsNameCollection[selectedPort];
                            }
                            else
                            {
                                continue;
                            }

                            bagDetailDto.BagName = bagdetail.BagName;
                            bagDetailDto.SelectedPortTagId = selectedPort;
                            bagDetailDto.BagId = bagdetail.id;
                            bagDetailDto.BatchId = bagdetail.BatchId;

                            bagDetails.Add(activeTagsNameCollection[selectedPort], bagDetailDto);
                        }
                    }
                }

                _processBagDetails = bagDetails;

                return _processBagDetails;
            }
        }
        public List<PortDetailInfo> CurrentProcessPortDetailInfos()
        {
            List<PortDetailInfo> portDetailInfos = new List<PortDetailInfo>();

            try
            {
                ActiveTagService activeTagService = new ActiveTagService(_connectionString);
                BagService bagService = new BagService(_connectionString);
                var bags = bagService.BagsByBatch(CurrentProcess.BatchId);
                var allSelectedPorts = bags.SelectMany(x => x.SelectedPorts);
                var activeTags = activeTagService.ActiveTagsByTagIdKey().Where(x => x.Value.ActiveTagGroupId != ActiveTagGroups.DefaultSensor).ToDictionary(x => x.Key, x => x.Value);

                foreach (var activeTagItem in activeTags)
                {
                    int portNumeric = Convert.ToInt32(activeTagItem.Value.TagName.Remove(0, 3));
                    PortDetailInfo portDetailInfo = new PortDetailInfo
                    {
                        ActiveTagGroup = activeTagItem.Value.ActiveTagGroupId,
                        PortNameLiteral = activeTagItem.Value.TagName,
                        PortNumeric = portNumeric,
                        TagId = activeTagItem.Key,
                        IsSelected = allSelectedPorts.Contains(activeTagItem.Key)
                    };

                    portDetailInfos.Add(portDetailInfo);
                }
            }
            catch (Exception ex)
            {
                portDetailInfos = new List<PortDetailInfo>();
            }


            return portDetailInfos;
        }
        private ProcessManager()
        {

        }
        public void Initialize(ApplicationConfiguration applicationConfiguration, Dictionary<int, ITagConfiguration> tagConfigurations)
        {
            _applicationConfiguration = applicationConfiguration;
            _tagConfigurations = tagConfigurations;

            _mainCacheManager = new CacheManager(CacheDBType.Main, _applicationConfiguration.RedisServer);
            _writeCacheManager = new CacheManager(CacheDBType.WriteService, _applicationConfiguration.RedisServer);
            _plcCommandManager = new PlcCommandManager(_applicationConfiguration.RedisServer);
            _serviceManager = new ServiceManager();
            _onDemandDB = new List<int>();

            //  IsRunOperationCommandWorking = false;
            CurrentProcess = new CurrentProcessInfo();
            _applicationProperties = null;
        }

        public void InitializeSelectedDevice(int plcDeviceId)
        {
            PlcDeviceId = plcDeviceId;
            _connectionString = _applicationConfiguration.PostgreSqlConnectionStrings[PlcDeviceId];
        }

        public void InitializeRunOperationTags()
        {
            PageTagConfigurationService pageTagConfigurationService = new PageTagConfigurationService(_connectionString);
            var pageTagConfiguration = pageTagConfigurationService.GetByName("RunOperation");
            string jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
            _runOperationTagConfigurations = JsonConvert.DeserializeObject<RunOperationTagConfigurations>(jsonSerializedString);
            _runOperationTagConfigurations.EnterPartsOk = (SiemensTagConfiguration)_tagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.EnterPartsOk)];
            _runOperationTagConfigurations.RecipeOk = (SiemensTagConfiguration)_tagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.RecipeOk)];
            _runOperationTagConfigurations.IntegrityCheckOk = (SiemensTagConfiguration)_tagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.IntegrityCheckOk)];
            _runOperationTagConfigurations.SkipIntegrityCheck = (SiemensTagConfiguration)_tagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.SkipIntegrityCheck)];
            _runOperationTagConfigurations.DoorStatus = (SiemensTagConfiguration)_tagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.DoorStatus)];
            _runOperationTagConfigurations.StartButtonEnable = (SiemensTagConfiguration)_tagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.StartButtonEnable)];
            _runOperationTagConfigurations.BatchStartAction = (SiemensTagConfiguration)_tagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.BatchStartAction)];
            _runOperationTagConfigurations.GoToNextSegment = (SiemensTagConfiguration)_tagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.GoToNextSegment)];
            _runOperationTagConfigurations.BackToNextSegment = (SiemensTagConfiguration)_tagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.BackToNextSegment)];
            _runOperationTagConfigurations.HoldRun = (SiemensTagConfiguration)_tagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.HoldRun)];
            _runOperationTagConfigurations.EndRun = (SiemensTagConfiguration)_tagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.EndRun)];
            _runOperationTagConfigurations.SegmentNo = (SiemensTagConfiguration)_tagConfigurations[Convert.ToInt32(_runOperationTagConfigurations.SegmentNo)];
        }

        #region RunOperation Methods

        /// <summary>
        /// Updates batch table, current batch cache and sends plc
        /// StartBatchTableUpdateStep,
        /// StartCacheUpdateStep,
        /// StartSetPlcStep,
        /// StartRecipeTableUpdateStep,
        /// StartIsBatchRunningStep,
        /// </summary>
        /// <returns>Returns each steps success info</returns>
        public Dictionary<ProcessSteps, bool> StartProcess()
        {
            IsRunOperationCommandWorking = true;


            Dictionary<ProcessSteps, bool> processSteps = new Dictionary<ProcessSteps, bool> {
                { ProcessSteps.StartBatchTableUpdateStep,false},
                { ProcessSteps.StartCacheUpdateStep,false},
                { ProcessSteps.StartSetPlcStep,false},
                { ProcessSteps.StartRecipeTableUpdateStep,false},
                { ProcessSteps.StartSaveRecipeToRecipeHistories,false},
                { ProcessSteps.StartIsBatchRunningStep,false},
                { ProcessSteps.ResetPlcAlarmsFromCache,false}
            };

            Batch batch = null;
            Batch batchRollBackEntity = null;
            Recipe recipe = null;
            Recipe recipeRollBackEntity = null;

            BatchService batchService = new BatchService(_connectionString);
            RecipeService recipeService = new RecipeService(_connectionString);

            batch = batchService.GetById(CurrentProcess.BatchId);

            StringBuilder stringBuilder = new StringBuilder();

            if (batch != null)
            {
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine("<Parameters Before Start Command>");
                stringBuilder.AppendLine($"BatchId     :{batch.id} ");
                stringBuilder.AppendLine($"LoadNumber  :{batch.LoadNumber}");
                stringBuilder.AppendLine($"Status      :{batch.Status}");
                stringBuilder.AppendLine($"StartDate   :{batch.StartDate}");
                stringBuilder.AppendLine($"EndDate     :{batch.EndDate}");
                stringBuilder.AppendLine($"BatchGroupId:{batch.BatchGroupId}");
                stringBuilder.AppendLine($"RecipeId    :{batch.RecipeId}");
                stringBuilder.AppendLine("<Parameters Before Start Command\\>");

                LogManager.Instance.Log($" {stringBuilder.ToString()}", LogType.Information);


                batchRollBackEntity = new Batch();
                batchRollBackEntity.StartDate = batch.StartDate;
                batchRollBackEntity.Status = batch.Status;

                batch.StartDate = DateTime.Now;
                batch.Status = BatchCurrentState.Running;// 1; // 1 = Running status
                batchService.Update(batch);

                batch = batchService.GetById(CurrentProcess.BatchId);

                processSteps[ProcessSteps.StartBatchTableUpdateStep] = batch.Status == BatchCurrentState.Running ? true : false;
            }


            if (processSteps[ProcessSteps.StartBatchTableUpdateStep])
            {
                CurrentProcess.BatchCurrentState = BatchCurrentState.Running;
                bool syncResult = SynchronizeCurrentProcessInfo(false);

                processSteps[ProcessSteps.StartCacheUpdateStep] = syncResult;
            }


            if (processSteps[ProcessSteps.StartCacheUpdateStep])
            {
                recipe = recipeService.GetById(CurrentProcess.ActiveRecipeId);

                DateTime currentDate = DateTime.Now;

                if (recipe != null)
                {
                    recipeRollBackEntity = new Recipe();

                    recipeRollBackEntity.LastRunDate = recipe.LastRunDate;

                    recipe.LastRunDate = currentDate;
                    bool recipeUpdateResult = recipeService.Update(recipe);

                    processSteps[ProcessSteps.StartRecipeTableUpdateStep] = recipeUpdateResult;
                }
            }

            if (processSteps[ProcessSteps.StartRecipeTableUpdateStep])
            {

                RecipeDetailHistoryService recipeHistoryService = new RecipeDetailHistoryService(_connectionString);

                RecipeDetailService recipeDetailService = new RecipeDetailService(_connectionString);

                var recipeDetailList = recipeDetailService.GetAllByRecipeId(CurrentProcess.ActiveRecipeId).ToList();

                RecipeDetailHistoryService recipeDetailHistoryService = new RecipeDetailHistoryService(_connectionString);

                var recipeDetailHistories = (from recipeDetail in recipeDetailList
                                             select new RecipeDetailHistory
                                             {
                                                 RecipeId = recipeDetail.RecipeId,
                                                 SegmentNo = recipeDetail.SegmentNo,
                                                 RecipeFieldId = recipeDetail.RecipeFieldId,
                                                 RecipeCellValue = recipeDetail.RecipeFieldValue,
                                                 BatchId = CurrentProcess.BatchId
                                             }).ToList();

                bool insertResult = recipeDetailHistoryService.InsertMany(recipeDetailHistories);

                processSteps[ProcessSteps.StartSaveRecipeToRecipeHistories] = insertResult;
            }


            if (processSteps[ProcessSteps.StartSaveRecipeToRecipeHistories])
            {
                Guid guid = Guid.NewGuid();

                PlcCommandManager plcCommandManager = new PlcCommandManager(_applicationConfiguration.RedisServer);

                SiemensTagConfiguration batchStartActionTag = (SiemensTagConfiguration)_runOperationTagConfigurations.BatchStartAction;

                plcCommandManager.Set(batchStartActionTag, true, guid);


                int tryAmount = 100;

                do
                {
                    Thread.Sleep(1000);

                    if (tryAmount == 0)
                    {
                        break;
                    }
                    tryAmount--;

                    processSteps[ProcessSteps.StartIsBatchRunningStep] = IsBatchRunning();

                } while (!IsBatchRunning());

                bool result = plcCommandManager.IsUpdatedResult(guid, true, 500);

                processSteps[ProcessSteps.StartSetPlcStep] = result;

            }


            bool resetPlcAlarmsResult = AlarmManager.Instance.ResetPlcAlarms();
            processSteps[ProcessSteps.ResetPlcAlarmsFromCache] = resetPlcAlarmsResult;

            bool totalResult = processSteps.Values.All(x => x == true);

            if (!totalResult)
            {
                // rollback succeeded parts all

                if (processSteps[ProcessSteps.StartBatchTableUpdateStep])
                {
                    batch.StartDate = batchRollBackEntity.StartDate;
                    batch.Status = batchRollBackEntity.Status;
                    batchService.Update(batch);
                }
                if (processSteps[ProcessSteps.StartCacheUpdateStep])
                {
                    CurrentProcess.BatchCurrentState = BatchCurrentState.NotStarted;
                }
                if (processSteps[ProcessSteps.StartRecipeTableUpdateStep])
                {
                    recipe.LastRunDate = recipeRollBackEntity.LastRunDate;
                    recipeService.Update(recipe);
                }

                if (processSteps[ProcessSteps.StartSaveRecipeToRecipeHistories])
                {
                    RecipeDetailHistoryService recipeHistoryService = new RecipeDetailHistoryService(_connectionString);
                    recipeHistoryService.DeleteAll(CurrentProcess.ActiveRecipeId, CurrentProcess.BatchId);
                }
            }
            else
            {

                var fromToDirection = _applicationConfiguration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
                SyncIssueManager syncIssueManager = new SyncIssueManager(_applicationConfiguration.RedisServer);
                bool createResult = syncIssueManager.CreateNewSyncIssue(fromToDirection, TransferType.AfterStart, SyncStatus.BatchStartDataPending, CurrentProcess.PlcDeviceId, CurrentProcess.BatchId);

            }

            stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("<Parameters After Start Command>");
            stringBuilder.AppendLine($"BatchId     :{batch.id} ");
            stringBuilder.AppendLine($"LoadNumber  :{batch.LoadNumber}");
            stringBuilder.AppendLine($"Status      :{batch.Status}");
            stringBuilder.AppendLine($"StartDate   :{batch.StartDate}");
            stringBuilder.AppendLine($"EndDate     :{batch.EndDate}");
            stringBuilder.AppendLine($"BatchGroupId:{batch.BatchGroupId}");
            stringBuilder.AppendLine($"RecipeId    :{batch.RecipeId}");
            stringBuilder.AppendLine($"RecipeId    :{batch.RecipeId}");
            stringBuilder.AppendLine("<Parameters After Start Command\\>");

            LogManager.Instance.Log($" {stringBuilder}", LogType.Information);

            IsRunOperationCommandWorking = false;

            return processSteps;
        }

        /// <summary>
        /// Updates batch table, current batch cache and sends plc
        /// FinishedBatchTableUpdateStep 
        /// FinishedResetAllProcessInfoCache 
        /// FinishedIsBatchFinished 
        /// </summary>
        /// <returns>Returns each steps success info</returns>
        public Dictionary<ProcessSteps, bool> FinishProcess()
        {


            CurrentProcessInfo currentProcessInfo = GetCurrentProcessFromCacheDB();
            CurrentProcess = currentProcessInfo;

            //todo:l refactor
            int plcDeviceId = currentProcessInfo.PlcDeviceId;
            int batchId = currentProcessInfo.BatchId;
            ////////////////////////////////////////////////////


            Dictionary<ProcessSteps, bool> processSteps = new Dictionary<ProcessSteps, bool> {
                { ProcessSteps.FinishedCacheUpdateStep,false},
                { ProcessSteps.FinishedBatchTableUpdateStep,false},
                { ProcessSteps.ResetActiveSensors,false},
                { ProcessSteps.FinishedIsBatchFinished,false},

            };

            Batch batch = null;
            Batch batchRollBackEntity = new Batch();


            //todo:l refactor rollback and obeject itself may refer same address
            CurrentProcessInfo currentProcessInfoRollBackItem = currentProcessInfo;
            BatchService batchService = new BatchService(_connectionString);
            RecipeService recipeService = new RecipeService(_connectionString);

            // Set current batch group id to last 5 batches
            batch = batchService.GetById(CurrentProcess.BatchId);

            StringBuilder stringBuilder = new StringBuilder();

            if (batch != null)
            {
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine("<Parameters Before Start Command>");
                stringBuilder.AppendLine($"BatchId     :{batch.id} ");
                stringBuilder.AppendLine($"LoadNumber  :{batch.LoadNumber}");
                stringBuilder.AppendLine($"Status      :{batch.Status}");
                stringBuilder.AppendLine($"StartDate   :{batch.StartDate}");
                stringBuilder.AppendLine($"EndDate     :{batch.EndDate}");
                stringBuilder.AppendLine($"BatchGroupId:{batch.BatchGroupId}");
                stringBuilder.AppendLine($"RecipeId    :{batch.RecipeId}");
                stringBuilder.AppendLine("<Parameters Before Start Command\\>");

                LogManager.Instance.Log($" {stringBuilder}", LogType.Information);

                batchRollBackEntity.EndDate = batch.EndDate;
                batchRollBackEntity.BatchGroupId = batch.BatchGroupId;
                batchRollBackEntity.Status = batch.Status;

                batch.EndDate = DateTime.Now;
                batch.BatchGroupId = 0; // Last 5 Batches group id
                batch.Status = BatchCurrentState.Finished; // Finished
                batchService.Update(batch);

                batch = batchService.GetById(CurrentProcess.BatchId);

                processSteps[ProcessSteps.FinishedBatchTableUpdateStep] = batch.Status == BatchCurrentState.Finished ? true : false;

            }

            if (processSteps[ProcessSteps.FinishedBatchTableUpdateStep])
            {
                CurrentProcess.LoadNumber = string.Empty;
                CurrentProcess.ActiveRecipeName = string.Empty;
                CurrentProcess.ActiveRecipeId = 0;
                CurrentProcess.IsBatchLoaded = false;
                CurrentProcess.IsRecipeLoaded = false;
                CurrentProcess.BatchId = 0;
                CurrentProcess.CurrentSegmentDescription = string.Empty;
                CurrentProcess.CurrentSegmentNo = 0;
                CurrentProcess.LastUpdateDate = DateTime.Now;
                CurrentProcess.BatchCurrentState = BatchCurrentState.Finished;
                //SynchronizeCurrentProcessInfo(false);

                bool result = SynchronizeCurrentProcessInfo(false);
                processSteps[ProcessSteps.FinishedCacheUpdateStep] = result;
            }

            //bool resetPlcAlarmsResult = AlarmManager.Instance.ResetPlcAlarms();
            //processSteps[ProcessSteps.ResetPlcAlarmsFromCache] = resetPlcAlarmsResult;


            int tryAmount = 100;

            do
            {
                Thread.Sleep(50);
                if (tryAmount == 0)
                {
                    break;
                }
                tryAmount--;
                processSteps[ProcessSteps.FinishedIsBatchFinished] = IsBatchFinished();
            } while (!IsBatchFinished());

            processSteps[ProcessSteps.ResetActiveSensors] = ResetActiveSensors();

            if (!processSteps.Values.All(x => x == true))
            {
                // rollback succeeded parts all

                if (processSteps[ProcessSteps.FinishedBatchTableUpdateStep])
                {
                    stringBuilder.AppendLine("");
                    stringBuilder.AppendLine("<Parameters Before Start Command>");
                    stringBuilder.AppendLine($"BatchId     :{batch.id} ");
                    stringBuilder.AppendLine($"LoadNumber  :{batch.LoadNumber}");
                    stringBuilder.AppendLine($"Status      :{batch.Status}");
                    stringBuilder.AppendLine($"StartDate   :{batch.StartDate}");
                    stringBuilder.AppendLine($"EndDate     :{batch.EndDate}");
                    stringBuilder.AppendLine($"BatchGroupId:{batch.BatchGroupId}");
                    stringBuilder.AppendLine($"RecipeId    :{batch.RecipeId}");
                    stringBuilder.AppendLine("<Parameters Before Start Command\\>");
                    batch.EndDate = batchRollBackEntity.EndDate;
                    batch.BatchGroupId = batch.BatchGroupId;
                    batch.Status = batch.Status;
                    batchService.Update(batch);
                }
                if (processSteps[ProcessSteps.FinishedCacheUpdateStep])
                {
                    CurrentProcess = currentProcessInfoRollBackItem;
                }
            }
            else
            {
                var fromToDirection = _applicationConfiguration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
                SyncIssueManager syncIssueManager = new SyncIssueManager(_applicationConfiguration.RedisServer);

                ProcessEventLogService processEventLogService = new ProcessEventLogService(_connectionString);

                ProcessEventLog processEventLog = new ProcessEventLog
                {
                    EventText = $"Process Finished! Batch: {currentProcessInfo.LoadNumber}",
                    CreateDate = DateTime.Now,
                    BatchId = batch.id,
                    Type = ProcessEventLogType.System.ToString()
                };

                bool processEventLogResult = processEventLogService.Insert(processEventLog);

                if (processEventLogResult)
                {
                    string serializedEntityObject = JsonConvert.SerializeObject(processEventLog);
                    SyncIssue syncIssue = new SyncIssue
                    {
                        SerializedEntityObject = serializedEntityObject,
                        EntityObjectType = typeof(ProcessEventLog),
                        SyncDBCommand = SyncDBCommand.Insert,
                        FromToDirection = fromToDirection,
                        PlcDeviceId = plcDeviceId,
                        SyncStatus = SyncStatus.NoneProcessChangesPending,
                        TransferType = TransferType.NonProcessChanges,
                        BatchId = batchId
                    };
                    syncIssueManager.CreateNewSyncIssue(syncIssue);
                }

                bool createResult = syncIssueManager.CreateNewSyncIssue(fromToDirection, TransferType.AfterFinish, SyncStatus.BatchFinishDataPending, plcDeviceId, batchId);

                Thread.Sleep(1000);

                ChangeUserAcknowledgeForFinish(true);

                AlarmSaveOrder alarmSaveOrder = new AlarmSaveOrder();
                alarmSaveOrder.CurrentProcessInfo = CurrentProcess;
                alarmSaveOrder.IsAlarmSaved = false;
                alarmSaveOrder.SaveOrderId = Guid.NewGuid().ToString("N");
                string serializedSaveAlarmOrder = JsonConvert.SerializeObject(alarmSaveOrder);
                _mainCacheManager.Set($"AlarmSaveOrderPLC{CurrentProcess.PlcDeviceId}", serializedSaveAlarmOrder, TimeSpan.FromHours(1));

            }

            return processSteps;
        }

        public async Task<bool> EndProcess()
        {
            IsRunOperationCommandWorking = true;

            // Send holdRun command to PLC
            PlcCommandManager plcCommandManager = new PlcCommandManager(_applicationConfiguration.RedisServer);

            bool result = await Task.Run(() =>
            {
                int tryAmount = 20;
                result = false;
                do
                {
                    if (tryAmount == 0)
                    {
                        break;
                    }

                    tryAmount--;
                    Guid guid = Guid.NewGuid();
                    plcCommandManager.Set((SiemensTagConfiguration)_runOperationTagConfigurations.EndRun, true, guid);
                    result = plcCommandManager.IsUpdatedResult(guid, false, 500);

                } while (!result);
                return result;
            });

            if (result)
            {
                AlarmSaveOrder alarmSaveOrder = new AlarmSaveOrder();
                alarmSaveOrder.CurrentProcessInfo = CurrentProcess;
                alarmSaveOrder.IsAlarmSaved = false;
                alarmSaveOrder.SaveOrderId = Guid.NewGuid().ToString("N");
                string serializedSaveAlarmOrder = JsonConvert.SerializeObject(alarmSaveOrder);
                _mainCacheManager.Set($"AlarmSaveOrderPLC{CurrentProcess.PlcDeviceId}", serializedSaveAlarmOrder, TimeSpan.FromHours(1));


                //bool resetPlcAlarmsResult = AlarmManager.Instance.ResetPlcAlarms();

                //if (!resetPlcAlarmsResult)
                //{
                //    LogManager.Instance.Log("ProcessManager > EndProcess > Error in reseting plc alarms!", LogType.Error);
                //}

                BatchService batchService = new BatchService(_connectionString);

                Batch batch = batchService.GetById(CurrentProcess.BatchId);
                batch.EndDate = DateTime.Now;
                batch.BatchGroupId = 0; // Last 5 Batches group id
                batch.Status = BatchCurrentState.Finished; // Finished
                batchService.Update(batch);

                ResetActiveSensors();

                var fromToDirection = _applicationConfiguration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
                SyncIssueManager syncIssueManager = new SyncIssueManager(_applicationConfiguration.RedisServer);
                bool createResult = syncIssueManager.CreateNewSyncIssue(fromToDirection, TransferType.AfterFinish, SyncStatus.BatchFinishDataPending, CurrentProcess.PlcDeviceId, CurrentProcess.BatchId);

                CurrentProcess.LoadNumber = string.Empty;
                CurrentProcess.ActiveRecipeName = string.Empty;
                CurrentProcess.ActiveRecipeId = 0;
                CurrentProcess.IsBatchLoaded = false;
                CurrentProcess.IsRecipeLoaded = false;
                CurrentProcess.BatchId = 0;
                CurrentProcess.CurrentSegmentDescription = string.Empty;
                CurrentProcess.CurrentSegmentNo = 0;
                CurrentProcess.LastUpdateDate = DateTime.Now;
                CurrentProcess.BatchCurrentState = BatchCurrentState.Finished;
                SynchronizeCurrentProcessInfo(false);

                //todo:m check process
            }

            await Task.Delay(2000);
            IsRunOperationCommandWorking = false;
            return result;
        }

        public async Task<bool> ActivateHold()
        {
            IsRunOperationCommandWorking = true;

            Dictionary<ProcessSteps, bool> processSteps = new Dictionary<ProcessSteps, bool> {
                { ProcessSteps.HoldUpdateCacheStep,false},
                { ProcessSteps.HoldUpdateDBStep,false},
                { ProcessSteps.HoldPlcSetStep,false},
            };
            BatchService batchService = new BatchService(_connectionString);

            bool result = await Task.Run(() =>
            {
                CurrentProcessInfo currentProcessInfoRollBackItem = new CurrentProcessInfo();
                Batch activeBatch = null;
                Batch activeBatchRollbackItem = null;
                currentProcessInfoRollBackItem = CurrentProcess;

                if (!IsProcessHoldState())
                {
                    CurrentProcess.BatchCurrentState = BatchCurrentState.Hold;
                    bool syncResult = SynchronizeCurrentProcessInfo(false);
                    processSteps[ProcessSteps.HoldUpdateCacheStep] = syncResult;

                    if (processSteps[ProcessSteps.HoldUpdateCacheStep])
                    {
                        activeBatchRollbackItem = new Batch();
                        activeBatch = batchService.GetActiveCurrentBatch();
                        activeBatchRollbackItem.Status = activeBatch.Status;

                        if (activeBatch != null)
                        {
                            activeBatch.Status = BatchCurrentState.Hold; // 4 = Hold status
                            bool updateResult = batchService.Update(activeBatch);
                            activeBatch = batchService.GetActiveCurrentBatch();
                            processSteps[ProcessSteps.HoldUpdateDBStep] = activeBatch.Status == BatchCurrentState.Hold;
                        }
                    }

                    if (processSteps[ProcessSteps.HoldUpdateDBStep])
                    {
                        Guid guid = Guid.NewGuid();
                        _plcCommandManager.Set((SiemensTagConfiguration)_runOperationTagConfigurations.HoldRun, true, guid);
                        result = _plcCommandManager.IsUpdatedResult(guid, false, 500);
                        processSteps[ProcessSteps.HoldPlcSetStep] = result;
                    }

                }
                else
                {
                    CurrentProcess.BatchCurrentState = BatchCurrentState.Running;
                    bool syncResult = SynchronizeCurrentProcessInfo(false);
                    processSteps[ProcessSteps.HoldUpdateCacheStep] = syncResult;
                    if (processSteps[ProcessSteps.HoldUpdateCacheStep])
                    {
                        activeBatchRollbackItem = new Batch();
                        activeBatch = batchService.GetActiveCurrentBatch();
                        activeBatchRollbackItem.Status = activeBatch.Status;

                        if (activeBatch != null)
                        {
                            activeBatch.Status = BatchCurrentState.Running;
                            bool updateResult = batchService.Update(activeBatch);
                            activeBatch = batchService.GetActiveCurrentBatch();
                            processSteps[ProcessSteps.HoldUpdateDBStep] = activeBatch.Status == BatchCurrentState.Running;
                        }
                    }

                    if (processSteps[ProcessSteps.HoldUpdateDBStep])
                    {
                        Guid guid = Guid.NewGuid();
                        _plcCommandManager.Set((SiemensTagConfiguration)_runOperationTagConfigurations.HoldRun, false, guid);
                        result = _plcCommandManager.IsUpdatedResult(guid, false, 500);
                        processSteps[ProcessSteps.HoldPlcSetStep] = result;
                    }
                }

                bool totalResult = processSteps.Values.All(x => x == true);

                if (!totalResult)
                {
                    // rollback succeeded parts all
                    if (processSteps[ProcessSteps.HoldUpdateCacheStep])
                    {
                        CurrentProcess = currentProcessInfoRollBackItem;
                    }
                    if (processSteps[ProcessSteps.HoldUpdateDBStep])
                    {
                        activeBatch.Status = activeBatchRollbackItem.Status;
                        bool updateResult = batchService.Update(activeBatch);
                    }
                }
                else
                {
                    var fromToDirection = _applicationConfiguration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
                    SyncIssueManager syncIssueManager = new SyncIssueManager(_applicationConfiguration.RedisServer);
                    bool createResult = syncIssueManager.CreateNewSyncIssue(fromToDirection, TransferType.AfterHold, SyncStatus.BatchHoldDataPending, CurrentProcess.PlcDeviceId, CurrentProcess.BatchId);
                    Thread.Sleep(1000);
                }

                return totalResult;
            });

            IsRunOperationCommandWorking = false;

            return result;
        }

        public async Task<bool> GoToNextSegment()
        {
            IsRunOperationCommandWorking = true;

            PlcCommandManager plcCommandManager = new PlcCommandManager(_applicationConfiguration.RedisServer);

            bool result = await Task.Run(() =>
            {
                int tryAmount = 20;

                result = false;

                do
                {
                    if (tryAmount == 0)
                    {
                        break;
                    }
                    tryAmount--;

                    Guid guid = Guid.NewGuid();

                    plcCommandManager.Set((SiemensTagConfiguration)_runOperationTagConfigurations.GoToNextSegment, true, guid);

                    result = plcCommandManager.IsUpdatedResult(guid, false, 500);

                } while (!result);
                return result;
            });

            IsRunOperationCommandWorking = false;

            return result;
        }

        public async Task<bool> GoToPreviousSegment()
        {
            IsRunOperationCommandWorking = true;

            PlcCommandManager plcCommandManager = new PlcCommandManager(_applicationConfiguration.RedisServer);

            bool result = await Task.Run(() =>
            {

                int tryAmount = 20;

                result = false;

                do
                {
                    if (tryAmount == 0)
                    {
                        break;
                    }
                    tryAmount--;

                    Guid guid = Guid.NewGuid();

                    plcCommandManager.Set((SiemensTagConfiguration)_runOperationTagConfigurations.BackToNextSegment, true, guid);

                    result = plcCommandManager.IsUpdatedResult(guid, false, 500);

                } while (!result);

                return result;

            });

            IsRunOperationCommandWorking = false;

            return result;
        }

        /// <summary>
        /// Continuos operation
        /// </summary>
        public void CheckRunningProcess()
        {

            if (!IsRunOperationCommandWorking)
            {
                bool isBatchRunning = IsBatchRunning();
                bool isBatchFinished = IsBatchFinished();

                // cache eski ise dbden check yap...
                CurrentProcessInfo currentProcessInfo = new CurrentProcessInfo();
                currentProcessInfo = GetCurrentProcessFromCacheDB();


                //if (isBatchRunning == true && isBatchFinished == false)// running
                //{
                //   // currentProcessInfo.BatchCurrentState = BatchCurrentState.Running;

                //    if (CurrentProcess.BatchCurrentState != BatchCurrentState.Running)
                //    {
                //        CurrentProcess = currentProcessInfo;
                //        SynchronizeCurrentProcessInfo(false);
                //    }
                //}

                if (isBatchRunning == false && isBatchFinished == true)// stop
                {
                    if (currentProcessInfo != null && currentProcessInfo.BatchCurrentState == BatchCurrentState.Running)
                    {

                        try
                        {
                            Dictionary<ProcessSteps, bool> processStepResults = FinishProcess();

                            if (!processStepResults.Values.All(x => x == true))
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine("");
                                sb.AppendLine("Error while finishing process!");
                                sb.AppendLine("<Finish Steps>");
                                foreach (var item in processStepResults)
                                {
                                    sb.AppendLine($"{item.Key} : {item.Value}");
                                }
                                sb.AppendLine("<Finish Steps\\>");

                                throw new Exception(sb.ToString());
                            }
                            else
                            {
                                // ResetActiveSensors();

                                LogManager.Instance.Log($"A fns O {JsonConvert.SerializeObject(CurrentProcess)}", LogType.Fatal);
                                LogManager.Instance.Log($"A fns C {JsonConvert.SerializeObject(GetCurrentProcessFromCacheDB())}", LogType.Fatal);

                                ChangeUserAcknowledgeForFinish(false);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogManager.Instance.Log($"Cache: BatchId:{CurrentProcess.BatchId} Batch Current State: {CurrentProcess.BatchCurrentState}  {ex.Message}", LogType.Fatal);
                        }
                    }
                }
            }
        }

        #endregion

        public bool IsBatchRunning()
        {
            bool isProcessRunning;

            try
            {

                int batchRunningInfoTagId = Convert.ToInt32(ApplicationProperties["BatchRunningInfoTagId"].Value);

                SiemensTagConfiguration batchRunningInfoTag = (SiemensTagConfiguration)_tagConfigurations[batchRunningInfoTagId];

                isProcessRunning = _plcCommandManager.Get<bool>(batchRunningInfoTag, true);

            }
            catch (Exception ex)
            {

                isProcessRunning = false;
            }

            return isProcessRunning;

        }
        public bool IsBatchFinished()
        {
            bool isProcessFinished;

            try
            {
                int batchFinishInfoTagId = Convert.ToInt32(ApplicationProperties["BatchFinishInfoTagId"].Value);
                SiemensTagConfiguration batchFinishInfoTag = (SiemensTagConfiguration)_tagConfigurations[batchFinishInfoTagId];
                isProcessFinished = _plcCommandManager.Get<bool>(batchFinishInfoTag, true);
            }
            catch (Exception ex)
            {
                isProcessFinished = false;
            }
            return isProcessFinished;
        }
        public bool IsProcessHoldState()
        {
            bool isProcessHoldState;

            try
            {
                SiemensTagConfiguration holdStateTag = (SiemensTagConfiguration)_runOperationTagConfigurations.HoldRun;

                isProcessHoldState = _plcCommandManager.Get<bool>(holdStateTag, true);

            }
            catch (Exception ex)
            {

                isProcessHoldState = false;
            }

            return isProcessHoldState;


        }
        public int CurrentSegmentNo()
        {
            PlcCommandManager plcCommandManager = new PlcCommandManager(_applicationConfiguration.RedisServer);
            try
            {
                var val = plcCommandManager.Get<string>((SiemensTagConfiguration)_runOperationTagConfigurations.SegmentNo, true);
                int currentSegmentNo = Convert.ToInt32(val);
                return currentSegmentNo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int GetDailyProcessOrder(int batchId, out Batch batch)
        {
            int order;
            try
            {
                BatchService batchService = new BatchService(_connectionString);
                order = batchService.GetDailyProcessOrder(batchId, out batch);
            }
            catch
            {
                order = 0;
                batch = null;
            }
            return order;
        }
        public Dictionary<string, bool> ServiceStates()
        {
            var serviceNames = _applicationConfiguration.ServiceNames;
            var serviceStates = _serviceManager.CheckServicesRunning(serviceNames);
            return serviceStates;
        }

        public bool IsAllServicesRunning()
        {
            return !ServiceStates().Values.Contains(false);
        }
        public Dictionary<int, LastDBStatus> LastDbReadStatuses()
        {
            Dictionary<int, LastDBStatus> lastDBStatuses = new Dictionary<int, LastDBStatus>();

            try
            {
                List<string> list = _mainCacheManager.GetKeyNames("LastDBStatus*");
                foreach (var item in list)
                {
                    string serializedLastDBStatus = _mainCacheManager.GetString(item);
                    LastDBStatus deserializedLastDBStatus = JsonConvert.DeserializeObject<LastDBStatus>(serializedLastDBStatus);
                    if (deserializedLastDBStatus.PlcId == PlcDeviceId)
                    {
                        lastDBStatuses.Add(deserializedLastDBStatus.DBNumber, deserializedLastDBStatus);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.Insert(0, "\nCouldnt get LastDbReadStatuses. Check cache or keys \n");
                throw ex;
            }

            return lastDBStatuses;
        }
        public ReadServiceState LastReadStatus()
        {
            ReadServiceState readServiceState = null;
            try
            {
                string readServiceStateSerialized = _mainCacheManager.GetString($"ReadServiceStatePLC{PlcDeviceId}");
                readServiceState = JsonConvert.DeserializeObject<ReadServiceState>(readServiceStateSerialized);
            }
            catch (Exception ex)
            {
                ex.Message.Insert(0, "\nCouldnt get ReadServiceState. Check cache or keys \n"); 
                readServiceState = new ReadServiceState
                {
                    GetAllDBCount = 0,
                    LastCycleRunTime = DateTime.Now.AddDays(-100),
                    PlcId = -1
                };
            }
            return readServiceState;
        }
        public bool IsPlcConnected()
        {
            int diffInSeconds = Convert.ToInt32((DateTime.Now - LastReadStatus().LastCycleRunTime).TotalMilliseconds / 1000);

            return (diffInSeconds < 10);

        }
        public bool SynchronizeCurrentProcessInfo(bool isFirstTimeInitialization)
        {
            List<bool> checkResultList = new List<bool>();
            try
            {
                CurrentProcessInfoService currentProcessInfoService = new CurrentProcessInfoService(_connectionString);
                CurrentProcessInfo currentProcessInfo = null;
                string serializedCurrentBatchInfo = string.Empty;

                if (isFirstTimeInitialization)
                {
                    currentProcessInfo = currentProcessInfoService.Get();
                    CurrentProcess = currentProcessInfo;
                    CurrentProcess.PlcDeviceId = PlcDeviceId;
                    serializedCurrentBatchInfo = JsonConvert.SerializeObject(CurrentProcess);
                    bool result = _mainCacheManager.Set($"CurrentProcessInfoPLC{PlcDeviceId}", serializedCurrentBatchInfo);
                    checkResultList.Add(result);
                }
                else
                {
                    bool updateResult = false;

                    Thread.Sleep(20);
                    CurrentProcess.PlcDeviceId = PlcDeviceId;
                    updateResult = currentProcessInfoService.Update(CurrentProcess);
                    LogManager.Instance.Log($" |{JsonConvert.SerializeObject(CurrentProcess)}", LogType.Information);

                    checkResultList.Add(updateResult);
                    serializedCurrentBatchInfo = JsonConvert.SerializeObject(CurrentProcess);
                    bool result = _mainCacheManager.Set($"CurrentProcessInfoPLC{PlcDeviceId}", serializedCurrentBatchInfo);

                    var fromToDirection = _applicationConfiguration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;

                    SyncIssue syncIssue = new SyncIssue
                    {
                        SerializedEntityObject = serializedCurrentBatchInfo,
                        EntityObjectType = typeof(CurrentProcessInfo),
                        SyncDBCommand = SyncDBCommand.Update,
                        FromToDirection = fromToDirection,
                        PlcDeviceId = PlcDeviceId,
                        SyncStatus = SyncStatus.NoneProcessChangesPending,
                        TransferType = TransferType.NonProcessChanges,
                        BatchId = CurrentProcess.BatchId
                    };

                    SyncIssueManager syncIssueManager = new SyncIssueManager(_applicationConfiguration.RedisServer);
                    syncIssueManager.CreateNewSyncIssue(syncIssue);

                    checkResultList.Add(result);
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log($"SynchronizeCurrentProcessInfo Error:\n{ex}\n", LogType.Fatal);
            }
            return checkResultList.TrueForAll(x => x == true);
        }

        public CurrentProcessInfo GetCurrentProcessFromCacheDB()
        {
            CurrentProcessInfo currentProcessInfo = null;
            string serializedCurrentBatchInfo = _mainCacheManager.GetString($"CurrentProcessInfoPLC{PlcDeviceId}");
            if (!string.IsNullOrEmpty(serializedCurrentBatchInfo))
            {
                currentProcessInfo = JsonConvert.DeserializeObject<CurrentProcessInfo>(serializedCurrentBatchInfo);
            }
            return currentProcessInfo;
        }

        public CurrentProcessInfo CurrentProcess;

        public bool ChangeUserAcknowledgeForFinish(bool acknowledgeState)
        {
            return _mainCacheManager.Set($"IsUserAcknowledgedFinishPLC{PlcDeviceId}", acknowledgeState);
        }
        public bool IsFinishAcknowledged()
        {
            string isUserAcknowledgedFinishRaw = _mainCacheManager.GetString($"IsUserAcknowledgedFinishPLC{PlcDeviceId}");
            return (isUserAcknowledgedFinishRaw == null) || Convert.ToBoolean(Convert.ToInt32(isUserAcknowledgedFinishRaw));
        }

        #region Write Command List Operations
        public List<SiemensWriteCommandItem> SiemensWriteCommandItems()
        {
            List<SiemensWriteCommandItem> siemensWriteCommandItems = new List<SiemensWriteCommandItem>();

            var cacheResponse = _writeCacheManager.GetAll($"SetCommandQueuePLC{PlcDeviceId}", 10);

            if (cacheResponse.ResultValue != null)
            {
                foreach (var item in cacheResponse.ResultValue as List<string>)
                {
                    SiemensWriteCommandItem writeCommandItem = JsonConvert.DeserializeObject<SiemensWriteCommandItem>(Convert.ToString(item));
                    siemensWriteCommandItems.Add(writeCommandItem);
                }
            }

            return siemensWriteCommandItems;
        }
        public bool ResetSiemensWriteCommandItems()
        {
            bool result = _writeCacheManager.DeleteKey($"SetCommandQueuePLC{PlcDeviceId}");
            return result;
        }
        #endregion

        #region Skip Operations

        public bool SkipEnterParts()
        {
            bool skipResult = false;

            //set to plc
            SiemensTagConfiguration siemensTagConfiguration = (SiemensTagConfiguration)_tagConfigurations[Convert.ToInt32(ApplicationProperties["SkipEnterParts"].Value)];

            Guid guid = Guid.NewGuid();
            bool setResult = _plcCommandManager.Set(siemensTagConfiguration, true, guid);
            bool result = _plcCommandManager.IsUpdatedResult(guid, false, 500);
            bool plcResult = setResult == true && result == true;

            if (plcResult)
            {
                Batch skippedBatch = new Batch();
                ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(_connectionString);
                ApplicationProperty lastLoadNumberAppProperty = applicationPropertyService.GetByName("LastLoadNumber");
                int lastLoadNumberIterationCount = Convert.ToInt32(lastLoadNumberAppProperty.Value);
                int nextValue = lastLoadNumberIterationCount + 1;
                lastLoadNumberAppProperty.Value = nextValue.ToString();
                applicationPropertyService.Update(lastLoadNumberAppProperty);

                string newBatchName = $"{_applicationConfiguration.Furnace.FurnaceName}-{lastLoadNumberIterationCount}";

                // Check if currentRecipeInfo is  null
                if (!CurrentProcess.IsRecipeLoaded)
                {
                    CurrentProcess.ActiveRecipeId = 0;
                }


                skippedBatch = new Batch()
                {
                    LoadNumber = newBatchName,
                    StartDate = DateTime.Now,
                    BatchGroupId = 2,
                    RecipeId = CurrentProcess.ActiveRecipeId,
                    Status = 0,
                    Revision = 0,
                    IsEnterPartsSkip = true
                };

                BatchService batchService = new BatchService(_connectionString);
                bool batchInsertResult = batchService.Insert(skippedBatch);
                CurrentProcess.BatchId = skippedBatch.id;
                CurrentProcess.BatchCurrentState = BatchCurrentState.NotStarted;
                CurrentProcess.IsBatchLoaded = true;
                CurrentProcess.LoadNumber = skippedBatch.LoadNumber;
                SynchronizeCurrentProcessInfo(false);

                // !! raporların ve diğer sayfaların açılışlarına dikkat edilecek.

                skipResult = true;
            }

            return skipResult;
        }

        public bool IsEnterPartsSkipped()
        {
            SiemensTagConfiguration siemensTagConfiguration = (SiemensTagConfiguration)_tagConfigurations[Convert.ToInt32(ApplicationProperties["SkipEnterParts"].Value)];
            bool isEnterPartsSkipped = _plcCommandManager.Get<bool>(siemensTagConfiguration, true);
            return isEnterPartsSkipped;
        }

        #endregion

        /// <summary>
        /// Bu method dışarıdan müdahaleyle durdurulabilecek fırınla ilgili işlemleri tamamen durdurur.
        /// </summary>
        /// <param name="saveCurrentProcessInfo">True durumunda var olan process bitirilmiş olarak kaydedilir. False durumunda loglar da dahil olmak üzere çalışan processle ilgili tüm datalar silinir!</param>
        /// <returns></returns>
        public bool EmergencyStopAllProcess(bool saveCurrentProcessInfo)
        {
            if (saveCurrentProcessInfo)
            {
            }
            else
            {
                // datalog
                // batch
                // baglar
                // reçete tarihi gibi şeyler silinecek..
            }
            return true;
        }

        #region ProcessResetMethods
        public bool ResetActiveSensors()
        {
            ActiveTagService activeTagService = new ActiveTagService(_connectionString);
            var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
            SyncIssueManager syncIssueManager = new SyncIssueManager(ApplicationConfigurations.Instance.Configuration.RedisServer);

            try
            {
                var activeSensors = activeTagService.GetAll().Where(x => x.ActiveTagGroupId == ActiveTagGroups.PTC || x.ActiveTagGroupId == ActiveTagGroups.MON).Where(x => x.IsLogData == true).ToList();

                foreach (var activeSensorItem in activeSensors)
                {
                    activeSensorItem.IsLogData = false;
                    bool updateResult = activeTagService.Update(activeSensorItem);

                    if (updateResult)
                    {
                        string serializedEntityObject = JsonConvert.SerializeObject(activeSensorItem);

                        SyncIssue syncIssue = new SyncIssue
                        {
                            SerializedEntityObject = serializedEntityObject,
                            EntityObjectType = typeof(ActiveTag),
                            SyncDBCommand = SyncDBCommand.Update,
                            FromToDirection = fromToDirection,
                            PlcDeviceId = PlcDeviceId,
                            SyncStatus = SyncStatus.NoneProcessChangesPending,
                            TransferType = TransferType.NonProcessChanges,
                        };

                        syncIssueManager.CreateNewSyncIssue(syncIssue);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        /// <summary>
        /// Updates demand state of db
        /// </summary>
        /// <param name="value">True for reading related db</param>
        /// <returns></returns>
        public bool ChangeDemandReadStateOnCache(int plcDeviceId, int dbNumber,bool value)
        {
            return _mainCacheManager.Set($"DemandRead:PLC{plcDeviceId}:DB{dbNumber}", value);
        }

        /// <summary>
        /// Updates demand state of db
        /// </summary>
        /// <param name="value">True for reading related db</param>
        /// <returns></returns>
        public bool ChangeDemandReadStateOnCache(string onDemandKey,bool value)
        {
            return _mainCacheManager.Set(onDemandKey, value);
        }

        public List<string> GetOnDemandKeyNames()
        {
            List<string> keyNames = null;
            try
            {
                keyNames= _mainCacheManager.GetKeyNames($"DemandRead:PLC{PlcDeviceId}:*");
            }
            catch (Exception)
            {

                keyNames = new List<string>();
            }


            return keyNames;
        }
    }
    public enum ProcessSteps
    {
        StartSaveRecipeToRecipeHistories,
        StartBatchTableUpdateStep,
        StartCacheUpdateStep,
        StartSetPlcStep,
        StartRecipeTableUpdateStep,
        StartIsBatchRunningStep,
        StartResetAlarmCache,
        ResetPlcAlarmsFromCache,
        ResetActiveSensors,
        FinishedCacheUpdateStep,
        FinishedBatchTableUpdateStep,
        FinishedPlcEnterPartsOKResetStep,
        FinishedPlcRecipeOKResetStep,
        FinishedPlcIntegrityChecksOkResetStep,
        FinishedPlcBatchFinishStep,
        FinishedResetAllProcessInfoCache,
        FinishedIsBatchFinished,
        StopCacheUpdateStep,
        StopBatchTable,
        HoldUpdateDBStep,
        HoldUpdateCacheStep,
        HoldPlcSetStep
    }

}



/*

    /// <summary>
    /// Continuos operation
    /// </summary>
    public void CheckRunningProcess()
    {

        if (!IsRunOperationCommandWorking)
        {
            bool isBatchRunning = IsBatchRunning();
            bool isBatchFinished = IsBatchFinished();

            // cache eski ise dbden check yap...
            CurrentProcessInfo currentProcessInfo = new CurrentProcessInfo();
            currentProcessInfo = CurrentProcess;


            if (isBatchRunning == true && isBatchFinished == false)// running
            {
                currentProcessInfo.BatchCurrentState = BatchCurrentState.Running;

                if (CurrentProcess.BatchCurrentState != BatchCurrentState.Running)
                {
                    CurrentProcess = currentProcessInfo;
                    SynchronizeCurrentProcessInfo(false);
                }
            }

            if (isBatchRunning == false && isBatchFinished == true)// stop
            {
                if (CurrentProcess.BatchCurrentState == BatchCurrentState.Running)
                {

                    try
                    {
                        Dictionary<ProcessSteps, bool> processStepResults = FinishProcess();

                        if (!processStepResults.Values.All(x => x == true))
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("");
                            sb.AppendLine("Error while finishing process!");
                            sb.AppendLine("<Finish Steps>");
                            foreach (var item in processStepResults)
                            {
                                sb.AppendLine($"{item.Key} : {item.Value}");
                            }
                            sb.AppendLine("<Finish Steps\\>");


                            throw new Exception(sb.ToString());
                        }
                        else
                        {
                             ResetActiveSensors();
                             CurrentProcess.BatchCurrentState = BatchCurrentState.NotStarted;
                             CurrentProcess.LoadNumber = string.Empty;
                             CurrentProcess.ActiveRecipeName = string.Empty;
                             CurrentProcess.ActiveRecipeId = 0;
                             CurrentProcess.IsBatchLoaded = false;
                             CurrentProcess.IsRecipeLoaded = false;
                             CurrentProcess.BatchId = 0;
                             CurrentProcess.CurrentSegmentDescription = string.Empty;
                             CurrentProcess.CurrentSegmentNo = 0;
                             CurrentProcess.LastUpdateDate = DateTime.Now;
                             SynchronizeCurrentProcessInfo(false);

                            ChangeUserAcknowledgeForFinish(false);
                        }



                    }
                    catch (Exception ex)
                    {
                        LogManager.Instance.Log($"Cache: BatchId:{CurrentProcess.BatchId} Batch Current State: {CurrentProcess.BatchCurrentState}  {ex.Message}", LogType.Fatal);

                    }
                }
            }
        }
    }
*/
