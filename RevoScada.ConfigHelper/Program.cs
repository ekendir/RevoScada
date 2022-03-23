using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Revo.SiemensDrivers.Sharp7;
using RevoScada.Business;
using RevoScada.Business.Configurations;
 
using RevoScada.Configurator;
using RevoScada.DataAccess.Concrete.Postgresql;
using RevoScada.DataAccess.Concrete.SqLite;
using RevoScada.Entities;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.PageTagConfigurations;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace RevoScada.ConfigHelper
{
    class Program
    {
        static void Main(string[] args)
        {
          

            Console.Write("Enter FullPath of application configuration root: default:" + @"C:\RevoScada.TAI.Files\Configuration\MultipleConfigurations   ");
            string fileRootPathForApplicationConfig = Console.ReadLine().ToString();
            fileRootPathForApplicationConfig = (string.IsNullOrEmpty(fileRootPathForApplicationConfig)) ? @"C:\RevoScada.TAI.Files\Configuration\MultipleConfigurations" : fileRootPathForApplicationConfig;
            string fileFullPathForApplicationConfig = Path.Combine(fileRootPathForApplicationConfig, "Application.rsconfig");
            string fileFullPathForSyncConfig = Path.Combine(fileRootPathForApplicationConfig, "SyncService.rsconfig");

            ApplicationConfigurations.Instance.InitializeConfiguration(fileFullPathForApplicationConfig, false);
            SyncServiceConfigurations.Instance.InitializeConfiguration(fileFullPathForSyncConfig);
            string postgreConnStr = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;
            string sqliteConnStr = ApplicationConfigurations.Instance.Configuration.SqliteConnectionString;
            string furnaceInfos = string.Join(Environment.NewLine, ApplicationConfigurations.Instance.Configuration.Furnaces.Values.ToList().Select(x => $"id:{x.Id}\t{x.FurnaceName}\tsoftwareId:{x.FurnaceSoftwareId}"));


            Console.WriteLine("=======Active Furnaces===============");
            Console.WriteLine($"{furnaceInfos}");
            Console.WriteLine("=====================================");

            string remoteRedisConnectionResult = "failed";

            try
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.Arguments = $"-h {SyncServiceConfigurations.Instance.SyncConfiguration.RemoteRedisServer} -p 6379";
                p.StartInfo.FileName = @"C:\Program Files\Redis\redis-cli.exe";
                p.Start();

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                string output = p.StandardOutput.ReadToEnd();
                stopwatch.Stop();
                if (stopwatch.Elapsed.Seconds<10)
                {
                    remoteRedisConnectionResult = "success";
                    stopwatch.Reset();
                }
                p.WaitForExit();
            }
            catch (Exception ex)
            {
               
            }

            Console.WriteLine($"Remote redis connection result for ({SyncServiceConfigurations.Instance.SyncConfiguration.RemoteRedisServer}) {remoteRedisConnectionResult}");

            Console.Write($"Enter Furnace Name: ");
            string furnaceName = Console.ReadLine();

            Console.Write($"Are you sure to change tag config id for furnace {furnaceName}  Y/N? ");
            string checkAnswer = Console.ReadLine();

            if (checkAnswer.ToLower() == "y")
            {
                Console.Write($"Enter Tag Increment Amount: ");
                int tagIncrement;
                bool parseResult = int.TryParse(Console.ReadLine(), out tagIncrement);

                if (!parseResult)
                {
                    return;
                }


                if (ApprovalPromt("Sqlite Siemens Tag Configuration Id updating"))
                {


                    Console.Write($"Enter Source PlcId: ");
                    int sourcePlcId;
                    bool parseResultSource = int.TryParse(Console.ReadLine(), out sourcePlcId);
                    if (!parseResultSource)
                    {
                        return;
                    }

                    Console.Write($"Enter Destination PlcId: ");
                    int destinationPlcId;
                    bool parseResultDestination = int.TryParse(Console.ReadLine(), out destinationPlcId);
                    if (!parseResultDestination)
                    {
                        return;
                    }

                    bool result;
                    var siemensTagConfigurationRepository = new SiemensTagConfigurationRepository(sqliteConnStr);
                    var tagconfigurations = siemensTagConfigurationRepository.GetAll().Where(x => x.PlcId == sourcePlcId).ToList();
                    Console.WriteLine($"Source Plc Id: {sourcePlcId}");

                    tagIncrement += 100000;
                    foreach (var item in tagconfigurations)
                    {
                        int serialRemovedId = item.Id % 100000;
                        Console.Write($"{item.Id} --> {serialRemovedId}");
                        item.Offset = Math.Round(item.Offset, 1);
                        item.Id = tagIncrement + serialRemovedId;
                        item.PlcId = destinationPlcId;
                        result = siemensTagConfigurationRepository.Insert(item);
                        Console.WriteLine(" --> " + item.Id);
                    }
                    tagIncrement -= 100000;
                    var afterList = siemensTagConfigurationRepository.GetAll().Where(x => x.PlcId == destinationPlcId).ToList();
                    Console.WriteLine($"Before: {tagconfigurations.Count} After: {afterList.Count}");
                }
                if (ApprovalPromt("PostgreSql Activetag updating"))
                {
                    var activeTagRepository = new ActiveTagRepository(postgreConnStr);

                    foreach (var item in activeTagRepository.GetAll())
                    {
                        Console.Write(item.id);
                        int newId = tagIncrement + item.id;
                        activeTagRepository.Delete(item);
                        item.id = newId;
                        activeTagRepository.Insert(item, false);
                        Console.WriteLine("\t --> " + item.id);
                    }
                }
                if (ApprovalPromt("PostgreSql ApplicationProperties updating"))
                {
                    var applicationPropertyRepository = new ApplicationPropertyRepository(postgreConnStr);
                    var applicationProperties = applicationPropertyRepository.GetAll();

                    string[] appPropertiesNames = new string[] { "ApplicationProperties", "BatchRunningInfoTagId", "BatchFinishInfoTagId", "SkipEnterParts", "EnterPartsOk", "ScadaSyncPCStatusTagId", "ScadaSyncServerStatusTagId" };

                    foreach (var item in applicationProperties)
                    {
                        if (appPropertiesNames.Contains(item.Name))
                        {
                            Console.Write(item.Value);
                            int value = Convert.ToInt32(item.Value);
                            item.Value = (value + tagIncrement).ToString();
                            applicationPropertyRepository.Update(item);
                            Console.WriteLine($"\t --> {item.Value}  Name:{item.Name}");
                        }
                    }
                }
                if (ApprovalPromt("PostgreSql Bags updating"))
                {
                    var bagRepository = new BagRepository(postgreConnStr);

                    foreach (var item in bagRepository.GetAll())
                    {
                        Console.Write(string.Join(",", item.SelectedPorts));
                        int count = item.SelectedPorts.Count();
                        int[] newPorts = new int[count];

                        for (int i = 0; i < count; i++)
                        {
                            newPorts[i] = tagIncrement + item.SelectedPorts[i];
                        }

                        item.SelectedPorts = newPorts;
                        bagRepository.Update(item);
                        Console.WriteLine(" --> " + string.Join(",", item.SelectedPorts) + "\n");
                    }
                }
                if (ApprovalPromt("PostgreSql Datalogs updating"))
                {
                    var dataLogRepository = new DataLogRepository(postgreConnStr);
                    var datalogs = dataLogRepository.GetAll();

                    foreach (var item in datalogs)
                    {
                        item.TagConfigurationId = tagIncrement + item.TagConfigurationId;
                        dataLogRepository.Update(item);
                    }
                }
                if (ApprovalPromt("PostgreSql IntegratedCheckResult updating"))
                {
                    var integratedCheckResultRepository = new IntegratedCheckResultRepository(postgreConnStr);
                    foreach (var item in integratedCheckResultRepository.GetAll())
                    {
                        Console.Write(item.SensorTagId);
                        item.SensorTagId = tagIncrement + item.SensorTagId;
                        integratedCheckResultRepository.Update(item);
                        Console.WriteLine($"\t--> {item.SensorTagId}");
                    }
                }
                if (ApprovalPromt("PostgreSql Alarm updating"))
                {
                    var plcAlarmRepository = new PlcAlarmRepository(postgreConnStr);
                    foreach (var item in plcAlarmRepository.GetAll())
                    {
                        Console.Write(item.TagConfigurationId);
                        item.TagConfigurationId = tagIncrement + item.TagConfigurationId;
                        plcAlarmRepository.Update(item);
                        Console.WriteLine($"\t--> {item.TagConfigurationId}");
                    }
                }
                if (ApprovalPromt("PostgreSql PageTagConfiguration JSON id updating"))
                {

                    Console.Write($"Enter Export Directory Default: " + @"C:\\PageTagConfigurations\\");
                    string pageTagConfigurationsExportDirectory = Console.ReadLine();
                    pageTagConfigurationsExportDirectory = (string.IsNullOrEmpty(pageTagConfigurationsExportDirectory)) ? @"C:\\PageTagConfigurations\\" : pageTagConfigurationsExportDirectory;

                    DirectoryInfo directoryInfo = new DirectoryInfo(pageTagConfigurationsExportDirectory);
                    if (!directoryInfo.Exists)
                    {
                        directoryInfo.Create();
                    }

                    var pageTagConfigurationService = new PageTagConfigurationService(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);
                    var pageTagConfiguration = new PageTagConfiguration();
                    string jsonSerializedString = string.Empty;

                    DefaultContractResolver contractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    };

                    JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = contractResolver, Formatting = Formatting.Indented };

                    //"VacuumLines"
                    pageTagConfiguration = pageTagConfigurationService.GetByName("VacuumLines");
                    jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
                    var vacuumLinesPageTagConfigurations = JsonConvert.DeserializeObject<VacuumLinesTagConfigurations>(jsonSerializedString);
                    ChangePropertyValues(tagIncrement, vacuumLinesPageTagConfigurations);

                    foreach (var item in vacuumLinesPageTagConfigurations.VacuumPorts)
                    {
                        item.Value.VacVentOff = item.Value.VacVentOff + tagIncrement;
                        item.Value.Value = item.Value.Value + tagIncrement;
                        item.Value.IsAuto = item.Value.IsAuto + tagIncrement;
                    }

                    jsonSerializedString = JsonConvert.SerializeObject(vacuumLinesPageTagConfigurations, jsonSerializerSettings);
                    File.WriteAllText(Path.Combine(pageTagConfigurationsExportDirectory, "VacuumLines.txt"), jsonSerializedString);

                    //"EnterParts"
                    pageTagConfiguration = pageTagConfigurationService.GetByName("EnterParts");
                    jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
                    var enterPartsPageTagConfigurations = JsonConvert.DeserializeObject<EnterPartsTagConfigurations>(jsonSerializedString);
                    ChangePropertyValues(tagIncrement, enterPartsPageTagConfigurations, new string[] { "SelectedPortInfoDbNumber", "PortBagInfoDbNumber" });

                    foreach (var item in enterPartsPageTagConfigurations.SelectedPortsInfo)
                    {
                        item.Value.PortBagInfo = Convert.ToInt32(item.Value.PortBagInfo) + tagIncrement;
                        item.Value.SelectedPortInfo = Convert.ToInt32(item.Value.SelectedPortInfo) + tagIncrement;
                    }

                    jsonSerializedString = JsonConvert.SerializeObject(enterPartsPageTagConfigurations, jsonSerializerSettings);
                    Console.WriteLine("EnterParts");
                    Console.WriteLine(jsonSerializedString);
                    File.WriteAllText(Path.Combine(pageTagConfigurationsExportDirectory, "EnterParts.txt"), jsonSerializedString);

                    //"RecipeEditor"
                    pageTagConfiguration = pageTagConfigurationService.GetByName("RecipeEditor");
                    jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
                    var recipeEditorPageTagConfigurations = JsonConvert.DeserializeObject<RecipeEditorTagConfigurations>(jsonSerializedString);
                    recipeEditorPageTagConfigurations.RecipeLoadOk = Convert.ToInt32(recipeEditorPageTagConfigurations.RecipeLoadOk) + tagIncrement;
                    recipeEditorPageTagConfigurations.RecipeName = Convert.ToInt32(recipeEditorPageTagConfigurations.RecipeName) + tagIncrement;

                    jsonSerializedString = JsonConvert.SerializeObject(recipeEditorPageTagConfigurations, jsonSerializerSettings);
                    Console.WriteLine("RecipeEditor");
                    Console.WriteLine(jsonSerializedString);
                    File.WriteAllText(Path.Combine(pageTagConfigurationsExportDirectory, "RecipeEditor.txt"), jsonSerializedString);

                    //"IntegrityChecks"
                    pageTagConfiguration = pageTagConfigurationService.GetByName("IntegrityChecks");
                    jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
                    var integrityCheckPageTagConfigurations = JsonConvert.DeserializeObject<IntegrityChecksTagConfigurations>(jsonSerializedString);
                    ChangePropertyValues(tagIncrement, integrityCheckPageTagConfigurations);

                    foreach (var item in integrityCheckPageTagConfigurations.IntegrityChecksItems["MON"])
                    {
                        item.Value.Actual += tagIncrement;
                        item.Value.Start += tagIncrement;
                        item.Value.Finish += tagIncrement;
                        item.Value.Deviation += tagIncrement;
                    }

                    foreach (var item in integrityCheckPageTagConfigurations.IntegrityChecksItems["PTC"])
                    {
                        item.Value.Actual += tagIncrement;
                        item.Value.Start += tagIncrement;
                        item.Value.Finish += tagIncrement;
                    }

                    jsonSerializedString = JsonConvert.SerializeObject(integrityCheckPageTagConfigurations, jsonSerializerSettings);
                    Console.WriteLine("IntegrityChecks");
                    Console.WriteLine(jsonSerializedString);
                    File.WriteAllText(Path.Combine(pageTagConfigurationsExportDirectory, "IntegrityChecks.txt"), jsonSerializedString);


                    //"SensorView"
                    pageTagConfiguration = pageTagConfigurationService.GetByName("SensorView");
                    jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
                    var sensorViewPageTagConfigurations = JsonConvert.DeserializeObject<SensorViewTagConfigurations>(jsonSerializedString);
                    ChangePropertyValues(tagIncrement, sensorViewPageTagConfigurations);

                    foreach (var item in sensorViewPageTagConfigurations.PartVacuumDatas)
                    {
                        item.Value.Value =                       Convert.ToInt32(item.Value.Value) +tagIncrement;
                        item.Value.EnableStatus =              Convert.ToInt32(item.Value.EnableStatus) +tagIncrement;
                        item.Value.Rate = Convert.ToInt32(item.Value.Rate) + tagIncrement;
                    }

                    foreach (var item in sensorViewPageTagConfigurations.PartTemperaturePorts)
                    {
                        item.Value.Value = Convert.ToInt32(item.Value.Value) + tagIncrement;
                        item.Value.EnableStatus = Convert.ToInt32(item.Value.EnableStatus) + tagIncrement;
                        item.Value.Rate = Convert.ToInt32(item.Value.Rate) + tagIncrement;
                    }

                    jsonSerializedString = JsonConvert.SerializeObject(sensorViewPageTagConfigurations, jsonSerializerSettings);
                    Console.WriteLine("SensorView");
                    Console.WriteLine(jsonSerializedString);
                    File.WriteAllText(Path.Combine(pageTagConfigurationsExportDirectory, "SensorView.txt"), jsonSerializedString);


                    //"RunOperation"
                    pageTagConfiguration = pageTagConfigurationService.GetByName("RunOperation");
                    jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
                    var runOperationPageTagConfigurations = JsonConvert.DeserializeObject<RunOperationTagConfigurations>(jsonSerializedString);
                    ChangePropertyValues(tagIncrement, runOperationPageTagConfigurations);

                    jsonSerializedString = JsonConvert.SerializeObject(runOperationPageTagConfigurations, jsonSerializerSettings);
                    Console.WriteLine("RunOperation");
                    Console.WriteLine(jsonSerializedString);
                    File.WriteAllText(Path.Combine(pageTagConfigurationsExportDirectory, "RunOperation.txt"), jsonSerializedString);

                    //"ManualOperation"
                    pageTagConfiguration = pageTagConfigurationService.GetByName("ManualOperation");
                    jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
                    var manualOperationPageTagConfigurations = JsonConvert.DeserializeObject<ManualOperationTagConfigurations>(jsonSerializedString);
                    ChangePropertyValues(tagIncrement, manualOperationPageTagConfigurations);

                    jsonSerializedString = JsonConvert.SerializeObject(manualOperationPageTagConfigurations, jsonSerializerSettings);
                    Console.WriteLine("ManualOperation");
                    Console.WriteLine(jsonSerializedString);
                    File.WriteAllText(Path.Combine(pageTagConfigurationsExportDirectory, "ManualOperation.txt"), jsonSerializedString);

                    //"Alarm"
                    pageTagConfiguration = pageTagConfigurationService.GetByName("Alarm");
                    jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
                    var alarmPageTagConfigurations = JsonConvert.DeserializeObject<AlarmTagConfigurations>(jsonSerializedString);
                    ChangePropertyValues(tagIncrement, alarmPageTagConfigurations);

                    jsonSerializedString = JsonConvert.SerializeObject(alarmPageTagConfigurations, jsonSerializerSettings);
                    Console.WriteLine("Alarm");
                    Console.WriteLine(jsonSerializedString);
                    File.WriteAllText(Path.Combine(pageTagConfigurationsExportDirectory, "Alarm.txt"), jsonSerializedString);

                    //"Recipe"
                    pageTagConfiguration = pageTagConfigurationService.GetByName("Recipe");
                    jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
                    var recipePageTagConfigurations = JsonConvert.DeserializeObject<RecipeTagConfigurations>(jsonSerializedString);
                    recipePageTagConfigurations.ActiveBatchSegmentNo += tagIncrement;

                    jsonSerializedString = JsonConvert.SerializeObject(recipePageTagConfigurations, jsonSerializerSettings);
                    Console.WriteLine("Recipe");
                    Console.WriteLine(jsonSerializedString);
                    File.WriteAllText(Path.Combine(pageTagConfigurationsExportDirectory, "Recipe.txt"), jsonSerializedString);

                    //"Calibration"
                    pageTagConfiguration = pageTagConfigurationService.GetByName("Calibration");
                    jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
                    var calibrationPageTagConfigurations = JsonConvert.DeserializeObject<CalibrationTagConfigurations>(jsonSerializedString);
                    ChangePropertyValues(tagIncrement, calibrationPageTagConfigurations, new string[] { "DefaultSequenceOfSensorMinPTC", "DefaultSequenceOfSensorMaxPTC", "DefaultSequenceOfSensorMinMON", "DefaultSequenceOfSensorMaxMON" });

                    foreach (var item in calibrationPageTagConfigurations.CalibrationItems)
                    {
                        item.Value.SensorValue = Convert.ToInt32(item.Value.SensorValue) + tagIncrement;
                        item.Value.NewGain = Convert.ToInt32(item.Value.NewGain) + tagIncrement;
                        item.Value.OldGain = Convert.ToInt32(item.Value.OldGain) + tagIncrement;
                        item.Value.NewCallOffset = Convert.ToInt32(item.Value.NewCallOffset) + tagIncrement;
                        item.Value.OldCallOffset = Convert.ToInt32(item.Value.OldCallOffset) + tagIncrement;
                        item.Value.SensorRawValue = Convert.ToInt32(item.Value.SensorRawValue) + tagIncrement;
                    }

                    foreach (var item in calibrationPageTagConfigurations.CalibrationItemsMON)
                    {
                        item.Value.SensorValue = Convert.ToInt32(item.Value.SensorValue) + tagIncrement;
                        item.Value.NewGain = Convert.ToInt32(item.Value.NewGain) + tagIncrement;
                        item.Value.OldGain = Convert.ToInt32(item.Value.OldGain) + tagIncrement;
                        item.Value.NewCallOffset = Convert.ToInt32(item.Value.NewCallOffset) + tagIncrement;
                        item.Value.OldCallOffset = Convert.ToInt32(item.Value.OldCallOffset) + tagIncrement;
                        item.Value.SensorRawValue = Convert.ToInt32(item.Value.SensorRawValue) + tagIncrement;
                    }

                    foreach (var item in calibrationPageTagConfigurations.CalibrationItemsPTC)
                    {
                        item.Value.SensorValue = Convert.ToInt32(item.Value.SensorValue) + tagIncrement;
                        item.Value.NewGain = Convert.ToInt32(item.Value.NewGain) + tagIncrement;
                        item.Value.OldGain = Convert.ToInt32(item.Value.OldGain) + tagIncrement;
                        item.Value.NewCallOffset = Convert.ToInt32(item.Value.NewCallOffset) + tagIncrement;
                        item.Value.OldCallOffset = Convert.ToInt32(item.Value.OldCallOffset) + tagIncrement;
                        item.Value.SensorRawValue = Convert.ToInt32(item.Value.SensorRawValue) + tagIncrement;
                    }


                    jsonSerializedString = JsonConvert.SerializeObject(calibrationPageTagConfigurations, jsonSerializerSettings);
                    Console.WriteLine("Calibration");
                    Console.WriteLine(jsonSerializedString);
                    File.WriteAllText(Path.Combine(pageTagConfigurationsExportDirectory, "Calibration.txt"), jsonSerializedString);


                    //"Oscillation"
                    pageTagConfiguration = pageTagConfigurationService.GetByName("Oscillation");
                    jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
                    var oscilllationPageTagConfigurations = JsonConvert.DeserializeObject<OscillationTagConfigurations>(jsonSerializedString);
                    foreach (var item in oscilllationPageTagConfigurations.OscillationCriterias)
                    {
                        item.Action = Convert.ToInt32(item.Action) + tagIncrement;
                        item.CheckDurationInMs = Convert.ToInt32(item.CheckDurationInMs) + tagIncrement;
                        item.SensorFaultCount = Convert.ToInt32(item.SensorFaultCount) + tagIncrement;
                        item.ToleranceValue = Convert.ToInt32(item.ToleranceValue) + tagIncrement;
                    }

                    jsonSerializedString = JsonConvert.SerializeObject(oscilllationPageTagConfigurations, jsonSerializerSettings);
                    Console.WriteLine("Oscillation");
                    Console.WriteLine(jsonSerializedString);
                    File.WriteAllText(Path.Combine(pageTagConfigurationsExportDirectory, "Oscillation.txt"), jsonSerializedString);

                    //"HamburgerMenuLeft"
                    pageTagConfiguration = pageTagConfigurationService.GetByName("HamburgerMenuLeft");
                    jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
                    var hamburgerMenuLeftPageTagConfigurations = JsonConvert.DeserializeObject<HamburgerMenuTagConfigurations>(jsonSerializedString);
                    ChangePropertyValues(tagIncrement, hamburgerMenuLeftPageTagConfigurations);

                    jsonSerializedString = JsonConvert.SerializeObject(hamburgerMenuLeftPageTagConfigurations, jsonSerializerSettings);
                    Console.WriteLine("HamburgerMenuLeft");
                    Console.WriteLine(jsonSerializedString);
                    File.WriteAllText(Path.Combine(pageTagConfigurationsExportDirectory, "HamburgerMenuLeft.txt"), jsonSerializedString);

                    //"Global"
                    pageTagConfiguration = pageTagConfigurationService.GetByName("Global");
                    jsonSerializedString = (string)pageTagConfiguration.PageTagConfigurations;
                    var globalPageTagConfigurations = JsonConvert.DeserializeObject<GlobalTagConfigurations>(jsonSerializedString);
                    ChangePropertyValues(tagIncrement, globalPageTagConfigurations);

                    jsonSerializedString = JsonConvert.SerializeObject(globalPageTagConfigurations, jsonSerializerSettings);
                    Console.WriteLine("Global");
                    Console.WriteLine(jsonSerializedString);
                    File.WriteAllText(Path.Combine(pageTagConfigurationsExportDirectory, "Global.txt"), jsonSerializedString);
                }
            }

            if (ApprovalPromt("Check Sync Items from plc? y/n "))
            {
                Console.Write($"EnterPlcId: ");
                int plcId;
                bool parseResultSource = int.TryParse(Console.ReadLine(), out plcId);
                if (!parseResultSource)
                {
                    return;
                }

                SiemensPlcConfigService siemensPlcConfigService = new SiemensPlcConfigService(sqliteConnStr);
                var plcConfigs = siemensPlcConfigService.GetActiveFurnaceConfigurations().ToDictionary(x => x.PlcDeviceId, x => x);
                var plcConfig = plcConfigs[plcId];

                var tagConfiguration = (SiemensTagConfiguration)SyncServiceConfigurations.Instance.ScadaSyncPCStatusTags[plcId] ;
                var tagConfigurationServer = (SiemensTagConfiguration)SyncServiceConfigurations.Instance.ScadaSyncServerStatusTags[plcId] ;
                ConsoleKeyInfo scadaSyncStatusLoopAnswer = new ConsoleKeyInfo();
                byte[] buffer = new byte[100];
                var client = new S7Client();

                Console.WriteLine("esc: to exit loop! ");

                do
                {
                    var connectResult = client.ConnectTo(plcConfig.Ip, 0, 0);
                    {
                        buffer = new byte[100];
                        client.DBRead(tagConfiguration.DBNumber, Convert.ToInt32(tagConfiguration.Offset), 100, buffer);
                        string checkResultAsString = S7.GetStringAt(buffer, 0);
                        Console.WriteLine($"\nScadaSyncPCStatus:              {checkResultAsString}");
                       // Console.WriteLine($"ScadaSyncPCStatus++:     {string.Join(" : ", checkResultAsString.Split(':')) }");
                        buffer = new byte[100];
                        client.DBRead(tagConfigurationServer.DBNumber, Convert.ToInt32(tagConfigurationServer.Offset), 100, buffer);
                        checkResultAsString = S7.GetStringAt(buffer, 0);
                        Console.WriteLine($"ScadaSyncServerStatus:          {checkResultAsString}   ");
                        //Console.WriteLine($"ScadaSyncServerStatus++: {string.Join(" : ", checkResultAsString.Split(':')) }");
                    }
                    scadaSyncStatusLoopAnswer = Console.ReadKey();
                } while (scadaSyncStatusLoopAnswer.Key!=ConsoleKey.Escape);


                if (ApprovalPromt("Reset Scada_Sync_PC_Status y/n "))
                {

                    S7.SetStringAt(buffer, 0, 100, "");
                    int result = client.DBWrite(tagConfiguration.DBNumber, Convert.ToInt32(tagConfiguration.Offset), 100, buffer);
                    buffer = new byte[100];
                    client.DBRead(tagConfiguration.DBNumber, Convert.ToInt32(tagConfiguration.Offset), 100, buffer);
                    string checkResultAsString = S7.GetStringAt(buffer, 0);
                    Console.WriteLine($"Scada_Sync_PC_Status: {checkResultAsString}");
                }

                if (ApprovalPromt("Reset Scada_Sync_Server_Status y/n "))
                {

                    S7.SetStringAt(buffer, 0, 100, "");
                    int result = client.DBWrite(tagConfiguration.DBNumber, Convert.ToInt32(tagConfiguration.Offset), 100, buffer);
                    buffer = new byte[100];
                    client.DBRead(tagConfiguration.DBNumber, Convert.ToInt32(tagConfiguration.Offset), 100, buffer);
                    string checkResultAsString = S7.GetStringAt(buffer, 0);
                    Console.WriteLine($"Scada_Sync_PC_Status: {checkResultAsString}");
                }

            }

        }
        private static void ChangePropertyValues<T>(int incrementAmount, T objectToUpdate, string[] exceptProperties = null)
        {
            PropertyInfo[] pi = objectToUpdate.GetType().GetProperties();
            foreach (var item in pi)
            {
                if (exceptProperties != null && exceptProperties.Contains(item.Name))
                {
                    continue;
                }

                try
                {
                    PropertyInfo propertyInfo = objectToUpdate.GetType().GetProperty(item.Name);
                    object rawValue = propertyInfo.GetValue(objectToUpdate);
                    if (rawValue != null && Convert.ToInt32(rawValue) != 0)
                    {
                        rawValue = Convert.ToInt32(rawValue) + incrementAmount;
                        propertyInfo.SetValue(objectToUpdate, Convert.ChangeType(rawValue, propertyInfo.PropertyType), null);
                    }
                }
                catch (Exception)
                {
                }
            }
        }
        private static bool ApprovalPromt(string title)
        {
            Console.WriteLine("\n============================================");
            Console.WriteLine(title);
            Console.Write($"Are you sure to proceed! Y/N? (Default: N) ");
            string checkAnswer = Console.ReadLine();
            checkAnswer = string.IsNullOrEmpty(checkAnswer) ? "N" : checkAnswer;
            return checkAnswer.ToLower() == "y";
        }
    }
}
