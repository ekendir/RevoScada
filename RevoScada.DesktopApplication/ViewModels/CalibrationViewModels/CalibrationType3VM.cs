using System;
using System.Linq;
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
    public class CalibrationType3VM : CalibrationBase
    {
        public CalibrationType3VM() : base(ApplicationConfigurations.Instance.Configuration)
        {
            CalibrationSettings = CalibrationSettingsSetter;
            InitializePageTagConfigurations();
            SensorTypeList = new KeyValuePair<CalibrationSensorType, string>[] {
                new KeyValuePair<CalibrationSensorType, string>(CalibrationSensorType.PTC, "PTC"),
                new KeyValuePair<CalibrationSensorType, string>(CalibrationSensorType.MON, "MON"),
               // new KeyValuePair<CalibrationSensorType, string>(CalibrationSensorType.MONCalibration, "MON CALIBRATION"),
                new KeyValuePair<CalibrationSensorType, string>(CalibrationSensorType.VacuumHeaderRight, "VAC HEADER"),
                new KeyValuePair<CalibrationSensorType, string>(CalibrationSensorType.AIRTCHigh, "AIRTC-HIGH"),
                new KeyValuePair<CalibrationSensorType, string>(CalibrationSensorType.AIRTCMediumHigh, "AIRTC-MEDIUM"),
                new KeyValuePair<CalibrationSensorType, string>(CalibrationSensorType.AIRTCLow, "AIRTC-LOW"),
                };

            UpdateCalibrationGridListAction = new Action(() =>
            {
                var calibrationSensorTypes = new CalibrationSensorType[] {
                                              CalibrationSensorType.PTC,
                                              CalibrationSensorType.MON,
                                             // CalibrationSensorType.MONCalibration,
                                              CalibrationSensorType.AIRTCHigh,
                                              CalibrationSensorType.AIRTCMediumHigh,
                                              CalibrationSensorType.AIRTCLow,
                                              CalibrationSensorType.VacuumHeaderRight
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
            var serializedPTC = _mainCacheManager.GetString($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{CalibrationSensorType.PTC}");
            var serializedMON = _mainCacheManager.GetString($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{CalibrationSensorType.MON}");
            //var serializedMONCalibration = _mainCacheManager.GetString($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{CalibrationSensorType.MONCalibration}");
            var serializedCalibrationListAIRTCHigh = _mainCacheManager.GetString($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{CalibrationSensorType.AIRTCHigh }");
            var serializedCalibrationListAIRTCMediumHigh = _mainCacheManager.GetString($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{CalibrationSensorType.AIRTCMediumHigh }");
            var serializedCalibrationListAIRTCLow = _mainCacheManager.GetString($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{CalibrationSensorType.AIRTCLow }");
            var serializedVACHeaderRight = _mainCacheManager.GetString($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{CalibrationSensorType.VacuumHeaderRight}");

            CalibrationListPTC = new ObservableCollection<CalibrationDataGrid>();
            CalibrationListMON = new ObservableCollection<CalibrationDataGrid>();
            //CalibrationListMONCalibration = new ObservableCollection<CalibrationDataGrid>();
            CalibrationListAIRTCLow = new ObservableCollection<CalibrationDataGrid>();
            CalibrationListVACHeaderRight = new ObservableCollection<CalibrationDataGrid>();

            CalibrationListPTC = serializedPTC != null ? JsonConvert.DeserializeObject<ObservableCollection<CalibrationDataGrid>>(serializedPTC) : CalibrationListPTC;
            CalibrationListMON = serializedMON != null ? JsonConvert.DeserializeObject<ObservableCollection<CalibrationDataGrid>>(serializedMON) : CalibrationListMON;
           // CalibrationListMONCalibration = serializedMONCalibration != null ? JsonConvert.DeserializeObject<ObservableCollection<CalibrationDataGrid>>(serializedMONCalibration) : CalibrationListMONCalibration;
            CalibrationListAIRTCHigh = new ObservableCollection<CalibrationDataGrid>();
            CalibrationListAIRTCMediumHigh = new ObservableCollection<CalibrationDataGrid>();
            CalibrationListAIRTCLow = new ObservableCollection<CalibrationDataGrid>();
            CalibrationListVACHeaderRight = serializedVACHeaderRight != null ? JsonConvert.DeserializeObject<ObservableCollection<CalibrationDataGrid>>(serializedVACHeaderRight) : CalibrationListVACHeaderRight;
            #endregion
        }

        //todo:l refactor move to base and parameterize
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
                //case CalibrationSensorType.MONCalibration:
                //    CalibrationListMONCalibration = CalibrationListMONCalibration ?? new ObservableCollection<CalibrationDataGrid>();
                //    calibrationItem = CalibrationTagConfigurations.CalibrationItems[calibrationSensorType.ToString()];
                //    calibrationDataGrid = GetCalibrationDataGrid();

                //    if (CalibrationListMONCalibration.Count() == 0)
                //    {
                //        CalibrationListMONCalibration.Add(calibrationDataGrid);
                //    }
                //    else
                //    {
                //        CalibrationListMONCalibration[0] = calibrationDataGrid;
                //    }

                //    calibrationDataGridSerialized = JsonConvert.SerializeObject(CalibrationListMONCalibration);
                //    _mainCacheManager.Set($"PLC{ProcessManager.Instance.PlcDeviceId}CalibrationDataGrid{calibrationSensorType}", calibrationDataGridSerialized);
                //    break;
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

                default:
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
            //if (CalibrationListMONCalibration != null) calibrationReport.AddRange(CalibrationListMONCalibration);
            if (CalibrationListVACHeaderRight != null) calibrationReport.AddRange(CalibrationListVACHeaderRight);
            if (CalibrationListAIRTCHigh != null) calibrationReport.AddRange(CalibrationListAIRTCHigh);
            if (CalibrationListAIRTCMediumHigh != null) calibrationReport.AddRange(CalibrationListAIRTCMediumHigh);
            if (CalibrationListAIRTCLow != null) calibrationReport.AddRange(CalibrationListAIRTCLow);

            DevExpress.XtraReports.UI.XtraReport xtraReportItem = null;
            xtraReportItem = reportCreator.CalibrationReport(calibrationReport);

            ReportViewer reportViewer = new ReportViewer(xtraReportItem);

            reportViewer.ShowDialog();
        }
    }
}

