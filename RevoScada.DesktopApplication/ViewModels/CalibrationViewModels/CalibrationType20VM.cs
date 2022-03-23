using System;
using System.Linq;
using System.Threading.Tasks;
using RevoScada.DesktopApplication.Models;
using System.Collections.Generic;
using RevoScada.DesktopApplication.Models.ModelEnums;
using RevoScada.Entities.PageTagConfigurations;
using RevoScada.Configurator;
using Newtonsoft.Json;
using RevoScada.Entities.Configuration;
using System.Collections.ObjectModel;
using RevoScada.DesktopApplication.Reports;
using RevoScada.DesktopApplication.Views.ReportTemplates;
using DevExpress.DataProcessing;
using RevoScada.ProcessController;

namespace RevoScada.DesktopApplication.ViewModels.CalibrationViewModels
{
    public class CalibrationType20VM : CalibrationBase
    {
        public CalibrationType20VM() : base(ApplicationConfigurations.Instance.Configuration)
        {
            CalibrationSettings = CalibrationSettingsSetter;
            InitializePageTagConfigurations();
            SensorTypeList = new KeyValuePair<CalibrationSensorType, string>[] {
                new KeyValuePair<CalibrationSensorType, string>(CalibrationSensorType.PTC, "PTC"),
                new KeyValuePair<CalibrationSensorType, string>(CalibrationSensorType.MON, "MON"),
                new KeyValuePair<CalibrationSensorType, string>(CalibrationSensorType.AIRTCHigh, "AIRTC-1"),
                new KeyValuePair<CalibrationSensorType, string>(CalibrationSensorType.AIRTCMediumHigh, "AIRTC-2"),
                new KeyValuePair<CalibrationSensorType, string>(CalibrationSensorType.AIRTCLow, "AIRTC-3"),
                new KeyValuePair<CalibrationSensorType, string>(CalibrationSensorType.Pressure, "PRESSURE"),
                new KeyValuePair<CalibrationSensorType, string>(CalibrationSensorType.PressureSupport, "PRESSURE-SUPPORT"),
                new KeyValuePair<CalibrationSensorType, string>(CalibrationSensorType.VacuumHeaderRight, "VACUUM-HEADER"),
                new KeyValuePair<CalibrationSensorType, string>(CalibrationSensorType.VACHeaderMonitor,  "VACUUM-LINE-MONITOR"),
                };

            UpdateCalibrationGridListAction = new Action(() =>
            {
                var calibrationSensorTypes = new CalibrationSensorType[]{
                                              CalibrationSensorType.PTC,
                                              CalibrationSensorType.MON,
                                              CalibrationSensorType.VACHeaderMonitor,
                                              CalibrationSensorType.AIRTCHigh,
                                              CalibrationSensorType.AIRTCMediumHigh,
                                              CalibrationSensorType.AIRTCLow,
                                              CalibrationSensorType.Pressure,
                                              CalibrationSensorType.PressureSupport,
                                              CalibrationSensorType.VacuumHeaderRight
                                              //,
                                              //CalibrationSensorType.VacuumHeaderLeft,
                                              //CalibrationSensorType.AIRTCMediumLow
                };

                foreach (var calibrationSensorType in calibrationSensorTypes)
                {
                    UpdateCalibrationDataGridList(calibrationSensorType);
                }

            });

            CalibrationFormInput = new CalibrationFormInput
            {
                SelectedCalibrationSensorType = CalibrationSettings.SensorType ?? CalibrationSensorType.PTC,
                DefaultSequenceOfSensorMax = 40,
                DefaultSequenceOfSensorMin = 1,
                SequenceOfSensorRangeStartSelection = 1,
                SequenceOfSensorRangeEndSelection = 4
            };

            InitSequenceOfSensorPTC = true;
            InitSequenceOfSensorMON = true;
            IsCycleRunnning = false;
            CalculateCommand = new RelayCommand(Calculate);
            AcceptCommand = new RelayCommand(Accept);
            RefreshCommand = new RelayCommand(Refresh);
            ViewReportCommand = new RelayCommand(ViewReport);
            ResetCommand = new RelayCommand(Reset);
            OpenCalibrationCertificationWindowCommand = new RelayCommand(OpenCalibrationCertificationWindow);

            #region Previous calibration grid data

            var serializedCalibrationListPTC = _mainCacheManager.GetString($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{CalibrationSensorType.PTC }");
            var serializedCalibrationListMON = _mainCacheManager.GetString($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{CalibrationSensorType.MON }");
            var serializedCalibrationListVACHeaderMonitor = _mainCacheManager.GetString($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{CalibrationSensorType.VACHeaderMonitor }");
            var serializedCalibrationListAIRTCHigh = _mainCacheManager.GetString($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{CalibrationSensorType.AIRTCHigh }");
            var serializedCalibrationListAIRTCMediumHigh = _mainCacheManager.GetString($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{CalibrationSensorType.AIRTCMediumHigh }");
            var serializedCalibrationListAIRTCLow = _mainCacheManager.GetString($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{CalibrationSensorType.AIRTCLow }");
            var serializedCalibrationListPressure = _mainCacheManager.GetString($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{CalibrationSensorType.Pressure }");
            var serializedCalibrationListPressureSupport = _mainCacheManager.GetString($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{CalibrationSensorType.PressureSupport }");
            var serializedCalibrationListVACHeaderRight = _mainCacheManager.GetString($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{CalibrationSensorType.VacuumHeaderRight }");
            //  var serializedCalibrationListVACHeaderLeft = _mainCacheManager.GetString($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{CalibrationSensorType.VacuumHeaderLeft }");
            //  var serializedCalibrationListAIRTCMediumLow = _mainCacheManager.GetString($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{CalibrationSensorType.AIRTCMediumLow }");

            CalibrationListPTC = new ObservableCollection<CalibrationDataGrid>();
            CalibrationListMON = new ObservableCollection<CalibrationDataGrid>();
            CalibrationListVACHeaderMonitor = new ObservableCollection<CalibrationDataGrid>();
            CalibrationListAIRTCHigh = new ObservableCollection<CalibrationDataGrid>();
            CalibrationListAIRTCMediumHigh = new ObservableCollection<CalibrationDataGrid>();
            CalibrationListAIRTCLow = new ObservableCollection<CalibrationDataGrid>();
            CalibrationListPressure = new ObservableCollection<CalibrationDataGrid>();
            CalibrationListPressureSupport = new ObservableCollection<CalibrationDataGrid>();
            CalibrationListVACHeaderRight = new ObservableCollection<CalibrationDataGrid>();
            // CalibrationListVACHeaderLeft = new ObservableCollection<CalibrationDataGrid>();
            //CalibrationListAIRTCMediumLow = new ObservableCollection<CalibrationDataGrid>();

            CalibrationListPTC = serializedCalibrationListPTC != null ? JsonConvert.DeserializeObject<ObservableCollection<CalibrationDataGrid>>(serializedCalibrationListPTC) : CalibrationListPTC;
            CalibrationListMON = serializedCalibrationListMON != null ? JsonConvert.DeserializeObject<ObservableCollection<CalibrationDataGrid>>(serializedCalibrationListMON) : CalibrationListMON;
            CalibrationListVACHeaderMonitor = serializedCalibrationListVACHeaderMonitor != null ? JsonConvert.DeserializeObject<ObservableCollection<CalibrationDataGrid>>(serializedCalibrationListVACHeaderMonitor) : CalibrationListVACHeaderMonitor;
            CalibrationListAIRTCHigh = serializedCalibrationListAIRTCHigh != null ? JsonConvert.DeserializeObject<ObservableCollection<CalibrationDataGrid>>(serializedCalibrationListAIRTCHigh) : CalibrationListAIRTCHigh;
            CalibrationListAIRTCMediumHigh = serializedCalibrationListAIRTCMediumHigh != null ? JsonConvert.DeserializeObject<ObservableCollection<CalibrationDataGrid>>(serializedCalibrationListAIRTCMediumHigh) : CalibrationListAIRTCMediumHigh;
            CalibrationListAIRTCLow = serializedCalibrationListAIRTCLow != null ? JsonConvert.DeserializeObject<ObservableCollection<CalibrationDataGrid>>(serializedCalibrationListAIRTCLow) : CalibrationListAIRTCLow;
            CalibrationListPressure = serializedCalibrationListPressure != null ? JsonConvert.DeserializeObject<ObservableCollection<CalibrationDataGrid>>(serializedCalibrationListPressure) : CalibrationListPressure;
            CalibrationListPressureSupport = serializedCalibrationListPressureSupport != null ? JsonConvert.DeserializeObject<ObservableCollection<CalibrationDataGrid>>(serializedCalibrationListPressureSupport) : CalibrationListPressureSupport;
            CalibrationListVACHeaderRight = serializedCalibrationListVACHeaderRight != null ? JsonConvert.DeserializeObject<ObservableCollection<CalibrationDataGrid>>(serializedCalibrationListVACHeaderRight) : CalibrationListVACHeaderRight;
            //CalibrationListVACHeaderLeft = serializedCalibrationListVACHeaderLeft != null ? JsonConvert.DeserializeObject<ObservableCollection<CalibrationDataGrid>>(serializedCalibrationListVACHeaderLeft) : CalibrationListVACHeaderLeft;
            //CalibrationListAIRTCMediumLow = serializedCalibrationListAIRTCMediumLow != null ? JsonConvert.DeserializeObject<ObservableCollection<CalibrationDataGrid>>(serializedCalibrationListAIRTCMediumLow) : CalibrationListAIRTCMediumLow;
            #endregion
        }
        private void UpdateCalibrationDataGridList(CalibrationSensorType calibrationSensorType)
        {
            PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
            CalibrationItem calibrationItem;
            CalibrationDataGrid calibrationDataGrid;
            string calibrationDataGridSerialized = string.Empty;

            CalibrationDataGrid GetCalibrationDataGrid()
            {
                calibrationDataGrid = new CalibrationDataGrid();
                calibrationDataGrid.CalibrationSensorType = calibrationSensorType;
                calibrationDataGrid.CalibrationSensorValue = "";
                calibrationDataGrid.OldGain = plcCommandManager.Get<float>((SiemensTagConfiguration)calibrationItem.OldGain, false);
                calibrationDataGrid.OldCallOffset = plcCommandManager.Get<float>((SiemensTagConfiguration)calibrationItem.OldCallOffset, false);
                calibrationDataGrid.NewCallOffset = plcCommandManager.Get<float>((SiemensTagConfiguration)calibrationItem.NewCallOffset, false);
                calibrationDataGrid.NewGain = plcCommandManager.Get<float>((SiemensTagConfiguration)calibrationItem.NewGain, false);
                calibrationDataGrid.SensorRawValue = plcCommandManager.Get<float>((SiemensTagConfiguration)calibrationItem.SensorRawValue, false);
                calibrationDataGrid.SensorValue = plcCommandManager.Get<float>((SiemensTagConfiguration)calibrationItem.SensorValue, false);
                calibrationDataGrid.Sensor = calibrationDataGrid.Sensor = 0;
                return calibrationDataGrid;
            }

            switch (calibrationSensorType)
            {
                case CalibrationSensorType.PTC:
                    CalibrationListPTC = CalibrationListPTC ?? new ObservableCollection<CalibrationDataGrid>();
                    foreach (var item in CalibrationTagConfigurations.CalibrationItemsPTC.Where(x => x.Value.SensorNo <= SequenceOfSensorRangeEndSelectionPTC && x.Value.SensorNo >= SequenceOfSensorRangeStartSelectionPTC))
                    {
                        calibrationDataGrid = new CalibrationDataGrid();
                        calibrationDataGrid.CalibrationSensorType = CalibrationSensorType.PTC;
                        calibrationDataGrid.CalibrationSensorValue = "";
                        calibrationDataGrid.NewCallOffset = plcCommandManager.Get<float>((SiemensTagConfiguration)item.Value.NewCallOffset, false);
                        calibrationDataGrid.NewGain = plcCommandManager.Get<float>((SiemensTagConfiguration)item.Value.NewGain, false);
                        calibrationDataGrid.SensorRawValue = plcCommandManager.Get<float>((SiemensTagConfiguration)item.Value.SensorRawValue, false);
                        calibrationDataGrid.SensorValue = plcCommandManager.Get<float>((SiemensTagConfiguration)item.Value.SensorValue, false);
                        calibrationDataGrid.Sensor = item.Value.SensorNo;

                        Guid guid = CalibrationListPTC.FirstOrDefault(x => x.Sensor == item.Value.SensorNo)?.TableIndex ?? Guid.Empty;

                        if (guid == Guid.Empty)
                        {
                            calibrationDataGrid.TableIndex = Guid.NewGuid();
                            CalibrationListPTC.Add(calibrationDataGrid);
                        }
                        else
                        {
                            calibrationDataGrid.TableIndex = guid;
                            int index = CalibrationListPTC.IndexOf(CalibrationListPTC.FirstOrDefault(x => x.TableIndex == calibrationDataGrid.TableIndex));
                            CalibrationListPTC[index] = calibrationDataGrid;
                        }
                    }
                    calibrationDataGridSerialized = JsonConvert.SerializeObject(CalibrationListPTC);
                    _mainCacheManager.Set($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{calibrationSensorType}", calibrationDataGridSerialized);

                    InitSequenceOfSensorPTC = false;
                    break;
                case CalibrationSensorType.MON:
                    CalibrationListMON = CalibrationListMON ?? new ObservableCollection<CalibrationDataGrid>();

                    foreach (var item in CalibrationTagConfigurations.CalibrationItemsMON.Where(x => x.Value.SensorNo <= SequenceOfSensorRangeEndSelectionMON && x.Value.SensorNo >= SequenceOfSensorRangeStartSelectionMON))
                    {
                        calibrationDataGrid = new CalibrationDataGrid();
                        calibrationDataGrid.CalibrationSensorType = CalibrationSensorType.MON;
                        calibrationDataGrid.CalibrationSensorValue = "";
                        calibrationDataGrid.NewCallOffset = plcCommandManager.Get<float>((SiemensTagConfiguration)item.Value.NewCallOffset, false);
                        calibrationDataGrid.NewGain = plcCommandManager.Get<float>((SiemensTagConfiguration)item.Value.NewGain, false);
                        calibrationDataGrid.SensorRawValue = plcCommandManager.Get<float>((SiemensTagConfiguration)item.Value.SensorRawValue, false);
                        calibrationDataGrid.SensorValue = plcCommandManager.Get<float>((SiemensTagConfiguration)item.Value.SensorValue, false);
                        calibrationDataGrid.Sensor = item.Value.SensorNo;

                        Guid guid = CalibrationListMON.FirstOrDefault(x => x.Sensor == item.Value.SensorNo)?.TableIndex ?? Guid.Empty;

                        if (guid == Guid.Empty)
                        {
                            calibrationDataGrid.TableIndex = Guid.NewGuid();
                            CalibrationListMON.Add(calibrationDataGrid);
                        }
                        else
                        {
                            calibrationDataGrid.TableIndex = guid;
                            int index = CalibrationListMON.IndexOf(CalibrationListMON.FirstOrDefault(x => x.TableIndex == calibrationDataGrid.TableIndex));
                            CalibrationListMON[index] = calibrationDataGrid;
                        }
                    }

                    calibrationDataGridSerialized = JsonConvert.SerializeObject(CalibrationListMON);
                    _mainCacheManager.Set($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{calibrationSensorType}", calibrationDataGridSerialized);

                    InitSequenceOfSensorMON = false;
                    break;
                case CalibrationSensorType.VACHeaderMonitor:

                    CalibrationListVACHeaderMonitor = CalibrationListVACHeaderMonitor ?? new ObservableCollection<CalibrationDataGrid>();
                    calibrationItem = CalibrationTagConfigurations.CalibrationItems[calibrationSensorType.ToString()];
                    calibrationDataGrid = GetCalibrationDataGrid();

                    if (CalibrationListVACHeaderMonitor.Count() == 0)
                    {
                        CalibrationListVACHeaderMonitor.Add(calibrationDataGrid);
                    }
                    else
                    {
                        CalibrationListVACHeaderMonitor[0] = calibrationDataGrid;
                    }

                    calibrationDataGridSerialized = JsonConvert.SerializeObject(CalibrationListVACHeaderMonitor);
                    _mainCacheManager.Set($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{calibrationSensorType}", calibrationDataGridSerialized);

                    break;

                case CalibrationSensorType.AIRTCHigh:
                    CalibrationListAIRTCHigh = CalibrationListAIRTCHigh ?? new ObservableCollection<CalibrationDataGrid>();
                    calibrationItem = CalibrationTagConfigurations.CalibrationItems[calibrationSensorType.ToString()];
                    calibrationDataGrid = GetCalibrationDataGrid();

                    if (CalibrationListAIRTCHigh.Count() == 0)
                    {
                        CalibrationListAIRTCHigh.Add(calibrationDataGrid);
                    }
                    else
                    {
                        CalibrationListAIRTCHigh[0] = calibrationDataGrid;
                    }

                    calibrationDataGridSerialized = JsonConvert.SerializeObject(CalibrationListAIRTCHigh);
                    _mainCacheManager.Set($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{calibrationSensorType}", calibrationDataGridSerialized);

                    break;

                case CalibrationSensorType.AIRTCMediumHigh:
                    CalibrationListAIRTCMediumHigh = CalibrationListAIRTCMediumHigh ?? new ObservableCollection<CalibrationDataGrid>();
                    calibrationItem = CalibrationTagConfigurations.CalibrationItems[calibrationSensorType.ToString()];
                    calibrationDataGrid = GetCalibrationDataGrid();

                    if (CalibrationListAIRTCMediumHigh.Count() == 0)
                    {
                        CalibrationListAIRTCMediumHigh.Add(calibrationDataGrid);
                    }
                    else
                    {
                        CalibrationListAIRTCMediumHigh[0] = calibrationDataGrid;
                    }

                    calibrationDataGridSerialized = JsonConvert.SerializeObject(CalibrationListAIRTCMediumHigh);
                    _mainCacheManager.Set($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{calibrationSensorType}", calibrationDataGridSerialized);

                    break;

                case CalibrationSensorType.AIRTCLow:
                    CalibrationListAIRTCLow = CalibrationListAIRTCLow ?? new ObservableCollection<CalibrationDataGrid>();
                    calibrationItem = CalibrationTagConfigurations.CalibrationItems[calibrationSensorType.ToString()];
                    calibrationDataGrid = GetCalibrationDataGrid();

                    if (CalibrationListAIRTCLow.Count() == 0)
                    {
                        CalibrationListAIRTCLow.Add(calibrationDataGrid);
                    }
                    else
                    {
                        CalibrationListAIRTCLow[0] = calibrationDataGrid;
                    }

                    calibrationDataGridSerialized = JsonConvert.SerializeObject(CalibrationListAIRTCLow);
                    _mainCacheManager.Set($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{calibrationSensorType}", calibrationDataGridSerialized);

                    break;

                case CalibrationSensorType.Pressure:
                    CalibrationListPressure = CalibrationListPressure ?? new ObservableCollection<CalibrationDataGrid>();
                    calibrationItem = CalibrationTagConfigurations.CalibrationItems[calibrationSensorType.ToString()];
                    calibrationDataGrid = GetCalibrationDataGrid();

                    if (CalibrationListPressure.Count() == 0)
                    {
                        CalibrationListPressure.Add(calibrationDataGrid);
                    }
                    else
                    {
                        CalibrationListPressure[0] = calibrationDataGrid;
                    }
                    calibrationDataGridSerialized = JsonConvert.SerializeObject(CalibrationListPressure);
                    _mainCacheManager.Set($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{calibrationSensorType}", calibrationDataGridSerialized);

                    break;

                case CalibrationSensorType.PressureSupport:
                    CalibrationListPressureSupport = CalibrationListPressureSupport ?? new ObservableCollection<CalibrationDataGrid>();
                    calibrationItem = CalibrationTagConfigurations.CalibrationItems[calibrationSensorType.ToString()];
                    calibrationDataGrid = GetCalibrationDataGrid();

                    if (CalibrationListPressureSupport.Count() == 0)
                    {
                        CalibrationListPressureSupport.Add(calibrationDataGrid);
                    }
                    else
                    {
                        CalibrationListPressureSupport[0] = calibrationDataGrid;
                    }
                    calibrationDataGridSerialized = JsonConvert.SerializeObject(CalibrationListPressureSupport);
                    _mainCacheManager.Set($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{calibrationSensorType}", calibrationDataGridSerialized);

                    break;

                case CalibrationSensorType.VacuumHeaderRight:
                    CalibrationListVACHeaderRight = CalibrationListVACHeaderRight ?? new ObservableCollection<CalibrationDataGrid>();
                    calibrationItem = CalibrationTagConfigurations.CalibrationItems[calibrationSensorType.ToString()];
                    calibrationDataGrid = GetCalibrationDataGrid();

                    if (CalibrationListVACHeaderRight.Count() == 0)
                    {
                        CalibrationListVACHeaderRight.Add(calibrationDataGrid);
                    }
                    else
                    {
                        CalibrationListVACHeaderRight[0] = calibrationDataGrid;
                    }

                    calibrationDataGridSerialized = JsonConvert.SerializeObject(CalibrationListVACHeaderRight);
                    _mainCacheManager.Set($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{calibrationSensorType}", calibrationDataGridSerialized);

                    break;
            }
        }
        //on unload page or usercontrol
        private void ViewReport()
        {
            ReportCreator reportCreator = new ReportCreator(ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString);

            List<CalibrationDataGrid> calibrationReport = new List<CalibrationDataGrid>();

            if (CalibrationListPTC != null) calibrationReport.AddRange(CalibrationListPTC);
            if (CalibrationListMON != null) calibrationReport.AddRange(CalibrationListMON);
            if (CalibrationListVACHeaderMonitor != null) calibrationReport.AddRange(CalibrationListVACHeaderMonitor);
            if (CalibrationListAIRTCHigh != null) calibrationReport.AddRange(CalibrationListAIRTCHigh);
            if (CalibrationListAIRTCMediumHigh != null) calibrationReport.AddRange(CalibrationListAIRTCMediumHigh);
            if (CalibrationListAIRTCLow != null) calibrationReport.AddRange(CalibrationListAIRTCLow);
            if (CalibrationListPressure != null) calibrationReport.AddRange(CalibrationListPressure);
            if (CalibrationListPressureSupport != null) calibrationReport.AddRange(CalibrationListPressureSupport);
            if (CalibrationListVACHeaderRight != null) calibrationReport.AddRange(CalibrationListVACHeaderRight);
            // if (CalibrationListVACHeaderLeft != null) calibrationReport.AddRange(CalibrationListVACHeaderLeft);
            // if (CalibrationListAIRTCMediumLow != null) calibrationReport.AddRange(CalibrationListAIRTCMediumLow);


            DevExpress.XtraReports.UI.XtraReport xtraReportItem = null;
            xtraReportItem = reportCreator.CalibrationReport(calibrationReport);

            ReportViewer reportViewer = new ReportViewer(xtraReportItem);

            reportViewer.ShowDialog();
        }
    }
}


