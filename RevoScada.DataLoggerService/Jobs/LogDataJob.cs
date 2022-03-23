using System.Threading.Tasks;
using Quartz;
using System;
using System.Linq;
using System.Collections.Generic;
using Revo.Core;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.Complex;
using Newtonsoft.Json;
using Revo.SiemensDrivers.Sharp7;
using RevoScada.Entities;
using RevoScada.Configurator;
using RevoScada.Business;
using RevoScada.Entities.Complex.Alarm;
using System.Text;
using RevoScada.Synchronization;
using RevoScada.Entities.Configuration.Service;
using RevoScada.Entities.PageTagConfigurations;

namespace RevoScada.DataLoggerService.Jobs
{
    internal class LogDataJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            SyncStateManager syncStateManager = new SyncStateManager(DataLoggerServiceConfigurations.Instance.DataLoggerServiceConfiguration.RedisServer);
            var dataLoggerConfig=  DataLoggerServiceConfigurations.Instance.DataLoggerServiceConfiguration;

            await Task.Delay(20);

            int plcDeviceId = ((context.JobDetail.JobDataMap as JobDataMap).GetInt("PlcDeviceId"));
            var pTCPortTagConfigurationSerialized = ((context.JobDetail.JobDataMap as JobDataMap).GetString("PTCPortTagConfigurationSerialized"));
            var mONPortTagConfigurationSerialized = ((context.JobDetail.JobDataMap as JobDataMap).GetString("MONPortTagConfigurationSerialized"));
            Dictionary<int, SensorViewPorts> pTCPortTagConfiguration = JsonConvert.DeserializeObject<Dictionary<int, SensorViewPorts>>(pTCPortTagConfigurationSerialized);
            Dictionary<int, SensorViewPorts> mONPortTagConfiguration = JsonConvert.DeserializeObject<Dictionary<int, SensorViewPorts>>(mONPortTagConfigurationSerialized);


            foreach (var item in pTCPortTagConfiguration)
            {
                item.Value.EnableStatus = JsonConvert.DeserializeObject<SiemensTagConfiguration>(item.Value.EnableStatus.ToString());
            }

            foreach (var item in mONPortTagConfiguration)
            {
                item.Value.EnableStatus = JsonConvert.DeserializeObject<SiemensTagConfiguration>(item.Value.EnableStatus.ToString());
            }


            try
            {
                if (dataLoggerConfig.WorkingEnvironment==WorkingEnvironment.server ||  syncStateManager.IsValidMaster(plcDeviceId, dataLoggerConfig.WorkingEnvironment))
                {
                    SiemensTagConfiguration batchRunningInfoTag = (SiemensTagConfiguration)DataLoggerServiceConfigurations.Instance.BatchRunningInfoTags[plcDeviceId];
                    SiemensTagConfiguration batchFinishInfoTag = (SiemensTagConfiguration)DataLoggerServiceConfigurations.Instance.BatchFinishInfoTags[plcDeviceId];

                    //Cache key came from batchRunningInfoTag or batchFinishInfoTag are the same.
                    var readResultRunningStateJson = DataloggerService.ReadCacheManager.GetString($"PLC{batchRunningInfoTag.PlcId}DB{batchRunningInfoTag.DBNumber}");

                    ReadResult readResultRunningState = JsonConvert.DeserializeObject<ReadResult>(readResultRunningStateJson);

                    int offsetIntegral = (Convert.ToInt32(Math.Floor(batchRunningInfoTag.Offset)));
                    int offsetDecimal = Convert.ToInt32((batchRunningInfoTag.Offset - Math.Floor(batchRunningInfoTag.Offset)) * 10);
                    bool isProcessRunning = S7.GetBitAt(readResultRunningState.Result, offsetIntegral, offsetDecimal);

                    offsetIntegral = (Convert.ToInt32(Math.Floor(batchFinishInfoTag.Offset)));
                    offsetDecimal = Convert.ToInt32((batchFinishInfoTag.Offset - Math.Floor(batchFinishInfoTag.Offset)) * 10);
                    bool isProcessFinished = S7.GetBitAt(readResultRunningState.Result, offsetIntegral, offsetDecimal);

                    if (isProcessRunning && !isProcessFinished)
                    {
                        CurrentProcessInfo currentProcessInfo = null;
                        string serializedCurrentBatchInfo = DataloggerService.MainCacheManager.GetString($"CurrentProcessInfoPLC{plcDeviceId}");

                        if (!string.IsNullOrEmpty(serializedCurrentBatchInfo))
                        {
                            currentProcessInfo = JsonConvert.DeserializeObject<CurrentProcessInfo>(serializedCurrentBatchInfo);
                        }
                        else
                        {
                            LogManager.Instance.Log($"Couldn't get cache values. Current ProcessInfo for PLC{plcDeviceId} not well formed!", LogType.Error);
                            return;
                        }

                        int currentBatchId = Convert.ToInt32(currentProcessInfo?.BatchId);

                        ActiveTagService activeTagService = new ActiveTagService(DataLoggerServiceConfigurations.Instance.DataLoggerServiceConfiguration.PostgreSqlConnectionStrings[plcDeviceId]);
                        Dictionary<string, ActiveTag> activeTags = activeTagService.ActiveTagsByTagNameKey().Where(x => x.Value.IsLogData == true && x.Value.ActiveTagGroupId != Entities.Enums.ActiveTagGroups.VAC).OrderBy(x=>x.Value.id).ToDictionary(x => x.Key, x => x.Value);

                        int ptc = activeTags.Where(x => x.Value.ActiveTagGroupId == Entities.Enums.ActiveTagGroups.PTC).Count();
                        int mon = activeTags.Where(x => x.Value.ActiveTagGroupId == Entities.Enums.ActiveTagGroups.MON).Count();

                        var dbGroups = DataLoggerServiceConfigurations.Instance.DataLogTags.Values.Cast<SiemensTagConfiguration>().Where(x=>x.PlcId==plcDeviceId && (x.SiemensTagGroupId==2 || x.SiemensTagGroupId==4) ).GroupBy(x => x.DBNumber).Select(x => x.Key).ToList();
                        var activeTagDetails = DataLoggerServiceConfigurations.Instance.DataLogTags;


                        short enableStatusDb = ((SiemensTagConfiguration)pTCPortTagConfiguration.Values.First().EnableStatus).DBNumber;

                        //For Enable disable datablock one time read in defined period 
                        dbGroups.Add(enableStatusDb);

                        Dictionary<int, ReadResult> readResults = new Dictionary<int, ReadResult>();
                        Dictionary<int, LastDBStatus> lastDBStatuses = new Dictionary<int, LastDBStatus>();
                        foreach (int dbNumber in dbGroups)
                        {
                            try
                            {
                                var readResult = DataloggerService.ReadCacheManager.GetString($"PLC{plcDeviceId}DB{dbNumber}");
                                var readResultDeserialized = JsonConvert.DeserializeObject<ReadResult>(readResult);
                                readResults.Add(dbNumber, readResultDeserialized);
                                LastDBStatus lastDBStatus = JsonConvert.DeserializeObject<LastDBStatus>(DataloggerService.MainCacheManager.GetString($"LastDBStatus_PLC{plcDeviceId}DB{dbNumber}"));
                                lastDBStatuses.Add(dbNumber, lastDBStatus);
                            }
                            catch (Exception ex)
                            {
                                LogManager.Instance.Log($"Couldn't get cache values. {ex.Message}", LogType.Error);
                            }
                        }
                        DataLogService dataLogService = new DataLogService(DataLoggerServiceConfigurations.Instance.DataLoggerServiceConfiguration.PostgreSqlConnectionStrings[plcDeviceId]);
                        DisabledPortService disabledPortService = new DisabledPortService(DataLoggerServiceConfigurations.Instance.DataLoggerServiceConfiguration.PostgreSqlConnectionStrings[plcDeviceId]);
                       
                        ReadServiceState readServiceState = null;
                        string readServiceStateSerialized = DataloggerService.MainCacheManager.GetString($"ReadServiceStatePLC{plcDeviceId}");
                        readServiceState = JsonConvert.DeserializeObject<ReadServiceState>(readServiceStateSerialized);
                        int diffInSeconds = Convert.ToInt32((DateTime.Now - readServiceState.LastCycleRunTime).TotalMilliseconds / 1000);

                        if (diffInSeconds < 20)
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            List<DataLog> toBeInsertedLogs = new List<DataLog>();
                            int loggedPortCountCheck = 0;
                            List<int> disabledPortIdList = new List<int>();
                            DisabledPort disabledPort = new DisabledPort();


                            DateTime receivedDate = DateTime.Now;
                            receivedDate = new DateTime(receivedDate.Year, receivedDate.Month, receivedDate.Day, receivedDate.Hour, receivedDate.Minute, receivedDate.Second, 0);


                            foreach (ActiveTag activeTag in activeTags.Values)
                            {
                                SiemensTagConfiguration siemensTagConfigurationForPortState =null;

                                if (activeTag.ActiveTagGroupId == Entities.Enums.ActiveTagGroups.PTC)
                                {
                                    int portNumber = Convert.ToInt32(activeTag.TagName.TrimStart('P', 'T', 'C'));
                                    siemensTagConfigurationForPortState = (SiemensTagConfiguration)pTCPortTagConfiguration[portNumber].EnableStatus;
                                 

                                }
                                else if ( activeTag.ActiveTagGroupId == Entities.Enums.ActiveTagGroups.MON)
                                {
                                    int portNumber = Convert.ToInt32(activeTag.TagName.TrimStart( 'M', 'O', 'N'));
                                    siemensTagConfigurationForPortState = (SiemensTagConfiguration)mONPortTagConfiguration[portNumber].EnableStatus;
                                }

                                if (siemensTagConfigurationForPortState !=null)
                                {
                                   int  enableStateOffsetIntegral = (Convert.ToInt32(Math.Floor(siemensTagConfigurationForPortState.Offset)));
                                   int enableStateValue = S7.GetIntAt(readResults[siemensTagConfigurationForPortState.DBNumber].Result, enableStateOffsetIntegral);

                                    // sensor view enable disable state parameters. 1 and 4 are enabled state and rest of them disabled.
                                    switch (enableStateValue)
                                    {
                                        case 0:
                                        case 2:
                                        case 3:
                                            disabledPortIdList.Add(activeTag.id);
                                            break;
                                    }
                                }


                                if (!activeTagDetails.ContainsKey(activeTag.id))
                                {
                                    LogManager.Instance.Log($"Plc{plcDeviceId} The tagId {activeTag.id} ({activeTag.TagName}) not found. Check Initial Config DB. ", LogType.Fatal);
                                    continue;
                                }

                                SiemensTagConfiguration siemensTagConfiguration = (SiemensTagConfiguration)activeTagDetails[activeTag.id];
                                offsetIntegral = (Convert.ToInt32(Math.Floor(siemensTagConfiguration.Offset)));
                                float tagValue = S7.GetRealAt(readResults[siemensTagConfiguration.DBNumber].Result, offsetIntegral);
                                DataLog dataLog = new DataLog
                                {
                                    BatchId = currentBatchId,
                                    ReceivedDate = receivedDate,
                                    TagConfigurationId = activeTag.id,
                                    TagValue = tagValue,
                                };

                                toBeInsertedLogs.Add(dataLog);
                            }

                            bool insertResult = false;
                            insertResult = dataLogService.InsertMany(toBeInsertedLogs);


                            if (disabledPortIdList.Count>0)
                            {
                                disabledPort.TagConfigurationList = disabledPortIdList.ToArray<int>();
                                disabledPort.ReceivedDate = receivedDate;
                                disabledPort.BatchId = currentBatchId;
                                disabledPortService.Insert(disabledPort);
                            }

                            if (insertResult)
                            {
                                foreach (var item in toBeInsertedLogs)
                                {
                                    string activeTagName = activeTags.Values.Where(a => a.id == item.TagConfigurationId).Select(a => a.TagName).FirstOrDefault();

                                    if (string.IsNullOrEmpty(activeTagName))
                                        continue;

                                    stringBuilder.Append($"{activeTagName} ");
                                    loggedPortCountCheck++;
                                }
                            }

                            LogManager.Instance.Log($"{context.JobDetail.Key.Name} Plc: {plcDeviceId}  Batch: {currentBatchId} Load Number: {currentProcessInfo?.LoadNumber} Selected Port Counts: ptc({ptc}) mon({mon}) Total Items: {activeTags.Count()} Logged Port Count Check:{loggedPortCountCheck == activeTags.Count()} Logged Ports:{stringBuilder}", LogType.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Log($"\n{ex}", LogType.Error);
            }
        }
    }
}

 