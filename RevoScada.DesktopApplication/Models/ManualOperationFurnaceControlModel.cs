using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    // sürekli okunacak 2sn de bir diyelim.
    // setlenirken loader ya da bekleme background worker olabilir...
    public class ManualOperationFurnaceControlModel : ObservableObject
    {
        #region -Temperature Control Section-

        #region PtcFanControlStateIsAuto Properties
        private int _ptcFanControlStateIsAuto;
        public int PtcFanControlStateIsAuto
        {
            get => _ptcFanControlStateIsAuto;
            set
            {
                _ptcFanControlStateIsAuto = value;
                if (_ptcFanControlStateIsAuto == 0)
                {
                    PtcFanControlStateMan = true;
                    PtcFanControlStateAuto = false;
                }
                else
                {
                    PtcFanControlStateMan = false;
                    PtcFanControlStateAuto = true;
                }
            }
        }

        private bool _ptcFanControlStateMan;
        public bool PtcFanControlStateMan
        {
            get => _ptcFanControlStateMan;
            set => OnPropertyChanged(ref _ptcFanControlStateMan, value);
        }

        private bool _ptcFanControlStateAuto;
        public bool PtcFanControlStateAuto
        {
            get => _ptcFanControlStateAuto;
            set => OnPropertyChanged(ref _ptcFanControlStateAuto, value);
        }
        #endregion

        #region PtcFanControlStateIsEnable Properties
        private int _ptcFanControlStateIsEnable;
        public int PtcFanControlStateIsEnable
        {
            get => _ptcFanControlStateIsEnable;
            set
            {
                _ptcFanControlStateIsEnable = value;
                if (_ptcFanControlStateIsEnable == 1)
                {
                    PtcFanControlStateIsOn = true;
                    PtcFanControlStateIsOff = false;
                }
                else
                {
                    PtcFanControlStateIsOn = false;
                    PtcFanControlStateIsOff = true;
                }
            }
        }

        private bool _ptcFanControlStateIsOn;
        public bool PtcFanControlStateIsOn
        {
            get => _ptcFanControlStateIsOn;
            set => OnPropertyChanged(ref _ptcFanControlStateIsOn, value);
        }

        private bool _ptcFanControlStateIsOff;
        public bool PtcFanControlStateIsOff
        {
            get => _ptcFanControlStateIsOff;
            set => OnPropertyChanged(ref _ptcFanControlStateIsOff, value);
        }
        #endregion

        #region PtcFanControlStatePid
        private int _ptcFanControlStatePid;
        public int PtcFanControlStatePid
        {
            get => _ptcFanControlStatePid;
            set => OnPropertyChanged(ref _ptcFanControlStatePid, value);
        }
        #endregion

        #region PtcHeatControlStateIsAuto Properties
        private int _ptcHeatControlStateIsAuto;
        public int PtcHeatControlStateIsAuto
        {
            get => _ptcHeatControlStateIsAuto;
            set
            {
                _ptcHeatControlStateIsAuto = value;
                if (_ptcHeatControlStateIsAuto == 0)
                {
                    PtcHeatControlStateMan = true;
                    PtcHeatControlStateAuto = false;
                }
                else
                {
                    PtcHeatControlStateMan = false;
                    PtcHeatControlStateAuto = true;
                }
            }
        }

        private bool _ptcHeatControlStateMan;
        public bool PtcHeatControlStateMan
        {
            get => _ptcHeatControlStateMan;
            set => OnPropertyChanged(ref _ptcHeatControlStateMan, value);
        }

        private bool _ptcHeatControlStateAuto;
        public bool PtcHeatControlStateAuto
        {
            get => _ptcHeatControlStateAuto;
            set => OnPropertyChanged(ref _ptcHeatControlStateAuto, value);
        }
        #endregion

        #region PtcHeatControlStateIsEnable Properties
        private int _ptcHeatControlStateIsEnable;
        public int PtcHeatControlStateIsEnable
        {
            get => _ptcFanControlStateIsEnable;
            set
            {
                _ptcHeatControlStateIsEnable = value;
                if (_ptcHeatControlStateIsEnable == 1)
                {
                    PtcHeatControlStateIsOn = true;
                    PtcHeatControlStateIsOff = false;
                }
                else
                {
                    PtcHeatControlStateIsOn = false;
                    PtcHeatControlStateIsOff = true;
                }
            }
        }

        private bool _ptcHeatControlStateIsOn;
        public bool PtcHeatControlStateIsOn
        {
            get => _ptcHeatControlStateIsOn;
            set => OnPropertyChanged(ref _ptcHeatControlStateIsOn, value);
        }

        private bool _ptcHeatControlStateIsOff;
        public bool PtcHeatControlStateIsOff
        {
            get => _ptcHeatControlStateIsOff;
            set => OnPropertyChanged(ref _ptcHeatControlStateIsOff, value);
        }
        #endregion

        #region PtcHeatControlStatePid
        private int _ptcHeatControlStatePid;
        public int PtcHeatControlStatePid
        {
            get => _ptcHeatControlStatePid;
            set => OnPropertyChanged(ref _ptcHeatControlStatePid, value);
        }
        #endregion

        #region PtcCoolControlStateIsAuto Properties
        private int _ptcCoolControlStateIsAuto;
        public int PtcCoolControlStateIsAuto
        {
            get => _ptcCoolControlStateIsAuto;
            set
            {
                _ptcCoolControlStateIsAuto = value;
                if (_ptcCoolControlStateIsAuto == 0)
                {
                    PtcCoolControlStateMan = true;
                    PtcCoolControlStateAuto = false;
                }
                else
                {
                    PtcCoolControlStateMan = false;
                    PtcCoolControlStateAuto = true;
                }
            }
        }

        private bool _ptcCoolControlStateMan;
        public bool PtcCoolControlStateMan
        {
            get => _ptcCoolControlStateMan;
            set => OnPropertyChanged(ref _ptcCoolControlStateMan, value);
        }

        private bool _ptcCoolControlStateAuto;
        public bool PtcCoolControlStateAuto
        {
            get => _ptcCoolControlStateAuto;
            set => OnPropertyChanged(ref _ptcCoolControlStateAuto, value);
        }
        #endregion

        #region PtcCoolControlStateIsEnable Properties
        private int _ptcCoolControlStateIsEnable;
        public int PtcCoolControlStateIsEnable
        {
            get => _ptcCoolControlStateIsEnable;
            set
            {
                _ptcCoolControlStateIsEnable = value;
                if (_ptcCoolControlStateIsEnable == 1)
                {
                    PtcCoolControlStateIsOn = true;
                    PtcCoolControlStateIsOff = false;
                }
                else
                {
                    PtcCoolControlStateIsOn = false;
                    PtcCoolControlStateIsOff = true;
                }
            }
        }

        private bool _ptcCoolControlStateIsOn;
        public bool PtcCoolControlStateIsOn
        {
            get => _ptcCoolControlStateIsOn;
            set => OnPropertyChanged(ref _ptcCoolControlStateIsOn, value);
        }

        private bool _ptcCoolControlStateIsOff;
        public bool PtcCoolControlStateIsOff
        {
            get => _ptcCoolControlStateIsOff;
            set => OnPropertyChanged(ref _ptcCoolControlStateIsOff, value);
        }
        #endregion

        #region * Temperature Set Value Section *
        private int _ptcValueStateIsAuto;
        public int PtcValueStateIsAuto
        {
            get => _ptcValueStateIsAuto;
            set
            {
                _ptcValueStateIsAuto = value;
                if (_ptcValueStateIsAuto == 0)
                {
                    PtcValueStateMan = true;
                    PtcValueStateAuto = false;
                }
                else
                {
                    PtcValueStateMan = false;
                    PtcValueStateAuto = true;
                }
            }
        }

        private bool _ptcValueStateMan;
        public bool PtcValueStateMan
        {
            get => _ptcValueStateMan;
            set => OnPropertyChanged(ref _ptcValueStateMan, value);
        }

        private bool _ptcValueStateAuto;
        public bool PtcValueStateAuto
        {
            get => _ptcValueStateAuto;
            set => OnPropertyChanged(ref _ptcValueStateAuto, value);
        }

        private float _ptcValue;
        public float PtcValue
        {
            get => _ptcValue;
            set => OnPropertyChanged(ref _ptcValue, value);
        }
        #endregion

        #endregion

        #region * Temperature Watch Section *


        private float _airTcHigh;
        public float AirTcHigh
        {
            get => _airTcHigh;
            set => OnPropertyChanged(ref _airTcHigh, value);
        }

        private float _airTcMediumHigh;
        public float AirTcMediumHigh
        {
            get => _airTcMediumHigh;
            set => OnPropertyChanged(ref _airTcMediumHigh, value);
        }

        private float _airTcLow;
        public float AirTcLow
        {
            get => _airTcLow;
            set => OnPropertyChanged(ref _airTcLow, value);
        }

        private float _airTcMediumLow;
        public float AirTcMediumLow
        {
            get => _airTcMediumLow;
            set => OnPropertyChanged(ref _airTcMediumLow, value);
        }


        private float _airTcHighRate;
        public float AirTcHighRate
        {
            get => _airTcHighRate;
            set => OnPropertyChanged(ref _airTcHighRate, value);
        }

        private float _airTcMediumHighRate;
        public float AirTcMediumHighRate
        {
            get => _airTcMediumHighRate;
            set => OnPropertyChanged(ref _airTcMediumHighRate, value);
        }

        private float _airTcLowRate;
        public float AirTcLowRate
        {
            get => _airTcLowRate;
            set => OnPropertyChanged(ref _airTcLowRate, value);
        }

        private float _airTcMediumLowRate;
        public float AirTcMediumLowRate
        {
            get => _airTcMediumLowRate;
            set => OnPropertyChanged(ref _airTcMediumLowRate, value);
        }







        private float _ptcWatchAirTempActual;
        public float PtcWatchAirTempActual
        {
            get => _ptcWatchAirTempActual;
            set => OnPropertyChanged(ref _ptcWatchAirTempActual, value);
        }

        private float _ptcWatchAirTempRate;
        public float PtcWatchAirTempRate
        {
            get => _ptcWatchAirTempRate;
            set => OnPropertyChanged(ref _ptcWatchAirTempRate, value);
        }

        private float _lowPtcPort;
        public float LowPtcPort
        {
            get => _lowPtcPort;
            set => OnPropertyChanged(ref _lowPtcPort, value);
        }

        private float _lowPtcActual;
        public float LowPtcActual
        {
            get => _lowPtcActual;
            set => OnPropertyChanged(ref _lowPtcActual, value);
        }

        private float _lowPtcRate;
        public float LowPtcRate
        {
            get => _lowPtcRate;
            set => OnPropertyChanged(ref _lowPtcRate, value);
        }

        private float _highPtcPort;
        public float HighPtcPort
        {
            get => _highPtcPort;
            set => OnPropertyChanged(ref _highPtcPort, value);
        }

        private float _HighPtcActual;
        public float HighPtcActual
        {
            get => _HighPtcActual;
            set => OnPropertyChanged(ref _HighPtcActual, value);
        }

        private float _highPtcRate;
        public float HighPtcRate
        {
            get => _highPtcRate;
            set => OnPropertyChanged(ref _highPtcRate, value);
        }
        #endregion

        #region -Pressure Control Section-

        #region PressureControlStateIsAuto Properties
        private int _pressureValveControlStateIsAuto;
        public int PressureValveControlStateIsAuto
        {
            get => _pressureValveControlStateIsAuto;
            set
            {
                _pressureValveControlStateIsAuto = value;
                if (_pressureValveControlStateIsAuto == 0)
                {
                    PressureControlStateMan = true;
                    PressureControlStateAuto = false;
                }
                else
                {
                    PressureControlStateMan = false;
                    PressureControlStateAuto = true;
                }
            }
        }

        private bool _pressureControlStateMan;
        public bool PressureControlStateMan
        {
            get => _pressureControlStateMan;
            set => OnPropertyChanged(ref _pressureControlStateMan, value);
        }

        private bool _pressureControlStateAuto;
        public bool PressureControlStateAuto
        {
            get => _pressureControlStateAuto;
            set => OnPropertyChanged(ref _pressureControlStateAuto, value);
        }

        #region Purge Control
        private int _purgeControlStateIsAuto;
        public int PurgeControlStateIsAuto
        {
            get => _purgeControlStateIsAuto;
            set
            {
                _purgeControlStateIsAuto = value;
                if (_purgeControlStateIsAuto == 0)
                {
                    PurgeControlStateMan = true;
                    PurgeControlStateAuto = false;
                }
                else
                {
                    PurgeControlStateMan = false;
                    PurgeControlStateAuto = true;
                }
            }
        }

        private bool _purgeControlStateMan;
        public bool PurgeControlStateMan
        {
            get => _purgeControlStateMan;
            set => OnPropertyChanged(ref _purgeControlStateMan, value);
        }

        private bool _purgeControlStateAuto;
        public bool PurgeControlStateAuto
        {
            get => _purgeControlStateAuto;
            set => OnPropertyChanged(ref _purgeControlStateAuto, value);
        }

        private int _purgeControlStateOnOff;
        public int PurgeControlStateOnOff
        {
            get => _purgeControlStateOnOff;
            set
            {
                _purgeControlStateOnOff = value;

                if (_purgeControlStateOnOff == 1)
                {
                    PurgeControlStateIsOn = true;
                    PurgeControlStateIsOff = false;
                }
                else
                {
                    PurgeControlStateIsOn = false;
                    PurgeControlStateIsOff = true;
                }
            }
        }

        private bool _purgeControlStateIsOn;
        public bool PurgeControlStateIsOn
        {
            get => _purgeControlStateIsOn;
            set => OnPropertyChanged(ref _purgeControlStateIsOn, value);
        }

        private bool _purgeControlStateIsOff;
        public bool PurgeControlStateIsOff
        {
            get => _purgeControlStateIsOff;
            set => OnPropertyChanged(ref _purgeControlStateIsOff, value);
        }

        #endregion PurgeControl


        private int _pressureLineControlStateIsAir;
        public int PressureLineControlStateIsAir
        {
            get => _pressureLineControlStateIsAir;
            set
            {
                _pressureLineControlStateIsAir = value;
                if (_pressureLineControlStateIsAir == 0)
                {
                    PressureLineControlStateAir = true;
                    PressureLineControlStateAzot = false;
                }
                else
                {
                    PressureLineControlStateAir = false;
                    PressureLineControlStateAzot = true;
                }
            }
        }

        private bool _pressureLineControlStateAir;
        public bool PressureLineControlStateAir
        {
            get => _pressureLineControlStateAir;
            set => OnPropertyChanged(ref _pressureLineControlStateAir, value);
        }

        private bool _pressureLineControlStateAzot;
        public bool PressureLineControlStateAzot
        {
            get => _pressureLineControlStateAzot;
            set => OnPropertyChanged(ref _pressureLineControlStateAzot, value);
        }

        #endregion

        #region PressureControlStateIsEnable Properties

        private int _pressureValveControlStateOnOff;
        public int PressureValveControlStateOnOff
        {
            get => _pressureValveControlStateOnOff;
            set
            {
                _pressureValveControlStateOnOff = value;

                if (_pressureValveControlStateOnOff == 1)
                {
                    PressureControlStateIsOn = true;
                    PressureControlStateIsOff = false;
                }
                else
                {
                    PressureControlStateIsOn = false;
                    PressureControlStateIsOff = true;
                }
            }
        }

        private bool _pressureControlStateIsOn;
        public bool PressureControlStateIsOn
        {
            get => _pressureControlStateIsOn;
            set => OnPropertyChanged(ref _pressureControlStateIsOn, value);
        }

        private bool _pressureControlStateIsOff;
        public bool PressureControlStateIsOff
        {
            get => _pressureControlStateIsOff;
            set => OnPropertyChanged(ref _pressureControlStateIsOff, value);
        }
        #endregion

        #region PressureControlStatePid
        private int _pressureControlStatePid;
        public int PressureControlStatePid
        {
            get => _pressureControlStatePid;
            set => OnPropertyChanged(ref _pressureControlStatePid, value);
        }
        #endregion

        #region * Pressure Set Value Section *
        private int _pressureValueStateIsAuto;
        public int PressureValueStateIsAuto
        {
            get => _pressureValueStateIsAuto;
            set
            {
                _pressureValueStateIsAuto = value;
                if (_pressureValueStateIsAuto == 0)
                {
                    PressureValueStateMan = true;
                    PressureValueStateAuto = false;
                }
                else
                {
                    PressureValueStateMan = false;
                    PressureValueStateAuto = true;
                }
            }
        }

        private bool _pressureValueStateMan;
        public bool PressureValueStateMan
        {
            get => _pressureValueStateMan;
            set => OnPropertyChanged(ref _pressureValueStateMan, value);
        }

        private bool _pressureValueStateAuto;
        public bool PressureValueStateAuto
        {
            get => _pressureValueStateAuto;
            set => OnPropertyChanged(ref _pressureValueStateAuto, value);
        }

        private float _pressureValue;
        public float PressureValue
        {
            get => _pressureValue;
            set => OnPropertyChanged(ref _pressureValue, value);
        }

        #endregion
        #endregion

        #region * Pressure Watch Section *

        private float _pressureSystemActual;
        public float PressureSystemActual
        {
            get => _pressureSystemActual;
            set => OnPropertyChanged(ref _pressureSystemActual, value);
        }

        private float _pressureSystemRate;
        public float PressureSystemRate
        {
            get => _pressureSystemRate;
            set => OnPropertyChanged(ref _pressureSystemRate, value);
        }

        private float _lowPressurePort;
        public float LowPressurePort
        {
            get => _lowPressurePort;
            set => OnPropertyChanged(ref _lowPressurePort, value);
        }

        private float _lowPressureActual;
        public float LowPressureActual
        {
            get => _lowPressureActual;
            set => OnPropertyChanged(ref _lowPressureActual, value);
        }

        private float _lowPressureRate;
        public float LowPressureRate
        {
            get => _lowPressureRate;
            set => OnPropertyChanged(ref _lowPressureRate, value);
        }

        private float _highPressurePort;
        public float HighPressurePort
        {
            get => _highPressurePort;
            set => OnPropertyChanged(ref _highPressurePort, value);
        }

        private float _highPressureActual;
        public float HighPressureActual
        {
            get => _highPressureActual;
            set => OnPropertyChanged(ref _highPressureActual, value);
        }

        private float _highPressureRate;
        public float HighPressureRate
        {
            get => _highPressureRate;
            set => OnPropertyChanged(ref _highPressureRate, value);
        }
        #endregion

        #region -Vacuum Control Section-
        #region VacuumControlStateIsAuto Properties
        private int _vacPumpControlStateIsAuto;
        public int VacPumpControlStateIsAuto
        {
            get => _vacPumpControlStateIsAuto;
            set
            {
                _vacPumpControlStateIsAuto = value;
                if (_vacPumpControlStateIsAuto == 0)
                {
                    VacuumControlStateMan = true;
                    VacuumControlStateAuto = false;
                }
                else
                {
                    VacuumControlStateMan = false;
                    VacuumControlStateAuto = true;
                }
            }
        }

        private bool _vacuumControlStateMan;
        public bool VacuumControlStateMan
        {
            get => _vacuumControlStateMan;
            set => OnPropertyChanged(ref _vacuumControlStateMan, value);
        }

        private bool _vacuumControlStateAuto;
        public bool VacuumControlStateAuto
        {
            get => _vacuumControlStateAuto;
            set => OnPropertyChanged(ref _vacuumControlStateAuto, value);
        }

        #region Equipment Tank

        private int _equipmentTankControlStateIsAuto;
        public int EquipmentTankControlStateIsAuto
        {
            get => _equipmentTankControlStateIsAuto;
            set
            {
                _equipmentTankControlStateIsAuto = value;
                if (_equipmentTankControlStateIsAuto == 0)
                {
                    EquipmentTankControlStateMan = true;
                    EquipmentTankControlStateAuto = false;
                }
                else
                {
                    EquipmentTankControlStateMan = false;
                    EquipmentTankControlStateAuto = true;
                }
            }
        }

        private bool _equipmentTankControlStateMan;
        public bool EquipmentTankControlStateMan
        {
            get => _equipmentTankControlStateMan;
            set => OnPropertyChanged(ref _equipmentTankControlStateMan, value);
        }

        private bool _equipmentTankControlStateAuto;
        public bool EquipmentTankControlStateAuto
        {
            get => _equipmentTankControlStateAuto;
            set => OnPropertyChanged(ref _equipmentTankControlStateAuto, value);
        }

        private int _equipmentTankControlStateOnOff;
        public int EquipmentTankControlStateOnOff
        {
            get => _equipmentTankControlStateOnOff;
            set
            {
                _equipmentTankControlStateOnOff = value;
                if (_equipmentTankControlStateOnOff == 1)
                {
                    EquipmentTankControlStateIsOn = true;
                    EquipmentTankControlStateIsOff = false;
                }
                else
                {
                    EquipmentTankControlStateIsOn = false;
                    EquipmentTankControlStateIsOff = true;
                }
            }
        }

        private bool _equipmentTankControlStateIsOn;
        public bool EquipmentTankControlStateIsOn
        {
            get => _equipmentTankControlStateIsOn;
            set => OnPropertyChanged(ref _equipmentTankControlStateIsOn, value);
        }

        private bool _equipmentTankControlStateIsOff;
        public bool EquipmentTankControlStateIsOff
        {
            get => _equipmentTankControlStateIsOff;
            set => OnPropertyChanged(ref _equipmentTankControlStateIsOff, value);
        }

        private int _equipmentTankControlStatusPid;
        public int EquipmentTankControlStatusPid
        {
            get => _equipmentTankControlStatusPid;
            set => OnPropertyChanged(ref _equipmentTankControlStatusPid, value);
        }



        #endregion Equipment Tank

        /**/
        private int _vacPumpControlStateIsAutoRight;
        public int VacPumpControlStateIsAutoRight
        {
            get => _vacPumpControlStateIsAutoRight;
            set
            {
                _vacPumpControlStateIsAutoRight = value;
                if (_vacPumpControlStateIsAutoRight == 0)
                {
                    VacuumControlStateManRight = true;
                    VacuumControlStateAutoRight = false;
                }
                else
                {
                    VacuumControlStateManRight = false;
                    VacuumControlStateAutoRight = true;
                }
            }
        }


        private bool _vacuumControlStateManRight;
        public bool VacuumControlStateManRight
        {
            get => _vacuumControlStateManRight;
            set => OnPropertyChanged(ref _vacuumControlStateManRight, value);
        }

        private bool _vacuumControlStateAutoRight;
        public bool VacuumControlStateAutoRight
        {
            get => _vacuumControlStateAutoRight;
            set => OnPropertyChanged(ref _vacuumControlStateAutoRight, value);
        }

        #endregion

        #region VacuumControlStateIsEnable Properties
        private int _vacPumpControlStateOnOff;
        public int VacPumpControlStateOnOff
        {
            get => _vacPumpControlStateOnOff;
            set
            {
                _vacPumpControlStateOnOff = value;
                if (_vacPumpControlStateOnOff == 1)
                {
                    VacuumControlStateIsOn = true;
                    VacuumControlStateIsOff = false;
                }
                else
                {
                    VacuumControlStateIsOn = false;
                    VacuumControlStateIsOff = true;
                }
            }
        }

        private bool _vacuumControlStateIsOn;
        public bool VacuumControlStateIsOn
        {
            get => _vacuumControlStateIsOn;
            set => OnPropertyChanged(ref _vacuumControlStateIsOn, value);
        }

        private bool _vacuumControlStateIsOff;
        public bool VacuumControlStateIsOff
        {
            get => _vacuumControlStateIsOff;
            set => OnPropertyChanged(ref _vacuumControlStateIsOff, value);
        }


        private int _vacPumpControlStateOnOffRight;
        public int VacPumpControlStateOnOffRight
        {
            get => _vacPumpControlStateOnOffRight;
            set
            {
                _vacPumpControlStateOnOffRight = value;
                if (_vacPumpControlStateOnOffRight == 1)
                {
                    VacuumControlStateIsOnRight = true;
                    VacuumControlStateIsOffRight = false;
                }
                else
                {
                    VacuumControlStateIsOnRight = false;
                    VacuumControlStateIsOffRight = true;
                }
            }
        }

        private bool _vacuumControlStateIsOnRight;
        public bool VacuumControlStateIsOnRight
        {
            get => _vacuumControlStateIsOnRight;
            set => OnPropertyChanged(ref _vacuumControlStateIsOnRight, value);
        }

        private bool _vacuumControlStateIsOffRight;
        public bool VacuumControlStateIsOffRight
        {
            get => _vacuumControlStateIsOffRight;
            set => OnPropertyChanged(ref _vacuumControlStateIsOffRight, value);
        }




        #endregion

        #region VacuumControlStatePid
        private int _vacControlStatusPid;
        public int VacControlStatusPid
        {
            get => _vacControlStatusPid;
            set => OnPropertyChanged(ref _vacControlStatusPid, value);
        }


        private int _vacControlStatusPidRight;
        public int VacControlStatusPidRight
        {
            get => _vacControlStatusPidRight;
            set => OnPropertyChanged(ref _vacControlStatusPidRight, value);
        }
        #endregion

        #region * Vacuum Set Value Section *
        private int _vacControlStatusIsAuto;
        public int VacControlStatusIsAuto
        {
            get => _vacControlStatusIsAuto;
            set
            {
                _vacControlStatusIsAuto = value;
                if (_vacControlStatusIsAuto == 0)
                {
                    VacuumValueStateMan = true;
                    VacuumValueStateAuto = false;
                }
                else
                {
                    VacuumValueStateMan = false;
                    VacuumValueStateAuto = true;
                }
            }
        }

        private bool _vacuumValueStateMan;
        public bool VacuumValueStateMan
        {
            get => _vacuumValueStateMan;
            set => OnPropertyChanged(ref _vacuumValueStateMan, value);
        }

        private bool _vacuumValueStateAuto;
        public bool VacuumValueStateAuto
        {
            get => _vacuumValueStateAuto;
            set => OnPropertyChanged(ref _vacuumValueStateAuto, value);
        }

        private float _vacControlStatusSp;
        public float VacControlStatusSp
        {
            get => _vacControlStatusSp;
            set => OnPropertyChanged(ref _vacControlStatusSp, value);
        }

        private float _vacControlStatusSpRight;
        public float VacControlStatusSpRight
        {
            get => _vacControlStatusSpRight;
            set => OnPropertyChanged(ref _vacControlStatusSpRight, value);
        }

        private int _vacControlStatusIsAutoRight;
        public int VacControlStatusIsAutoRight
        {
            get => _vacControlStatusIsAutoRight;
            set
            {
                _vacControlStatusIsAutoRight = value;
                if (_vacControlStatusIsAutoRight == 0)
                {
                    VacuumValueStateManRight = true;
                    VacuumValueStateAutoRight = false;
                }
                else
                {
                    VacuumValueStateManRight = false;
                    VacuumValueStateAutoRight = true;
                }
            }
        }

        private bool _vacuumValueStateManRight;
        public bool VacuumValueStateManRight
        {
            get => _vacuumValueStateManRight;
            set => OnPropertyChanged(ref _vacuumValueStateManRight, value);
        }

        private bool _vacuumValueStateAutoRight;
        public bool VacuumValueStateAutoRight
        {
            get => _vacuumValueStateAutoRight;
            set => OnPropertyChanged(ref _vacuumValueStateAutoRight, value);
        }


        #endregion
        #endregion

        #region * Vacuum Watch Section *

        private float _vacSystemVacuumActual;
        public float VacSystemVacuumActual
        {
            get => _vacSystemVacuumActual;
            set => OnPropertyChanged(ref _vacSystemVacuumActual, value);
        }

        private float _vacSystemVacuumActualRight;
        public float VacSystemVacuumActualRight
        {
            get => _vacSystemVacuumActualRight;
            set => OnPropertyChanged(ref _vacSystemVacuumActualRight, value);
        }

        private float _vacSystemVacuumRate;
        public float VacSystemVacuumRate
        {
            get => _vacSystemVacuumRate;
            set => OnPropertyChanged(ref _vacSystemVacuumRate, value);
        }

        private float _vacSystemVacuumRateRight;
        public float VacSystemVacuumRateRight
        {
            get => _vacSystemVacuumRateRight;
            set => OnPropertyChanged(ref _vacSystemVacuumRateRight, value);
        }

        private float _lowVacuumPort;
        public float LowVacuumPort
        {
            get => _lowVacuumPort;
            set => OnPropertyChanged(ref _lowVacuumPort, value);
        }

        private float _lowVacuumActual;
        public float LowVacuumActual
        {
            get => _lowVacuumActual;
            set => OnPropertyChanged(ref _lowVacuumActual, value);
        }

        private float _lowVacuumRate;
        public float LowVacuumRate
        {
            get => _lowVacuumRate;
            set => OnPropertyChanged(ref _lowVacuumRate, value);
        }

        private float _highVacuumPort;
        public float HighVacuumPort
        {
            get => _highVacuumPort;
            set => OnPropertyChanged(ref _highVacuumPort, value);
        }

        private float _highVacuumActual;
        public float HighVacuumActual
        {
            get => _highVacuumActual;
            set => OnPropertyChanged(ref _highVacuumActual, value);
        }

        private float _highVacuumRate;
        public float HighVacuumRate
        {
            get => _highVacuumRate;
            set => OnPropertyChanged(ref _highVacuumRate, value);
        }
        #endregion


        #region Furnace Watch Section

        private float _fanTemperature;
        public float FanTemperature
        {
            get => _fanTemperature;
            set => OnPropertyChanged(ref _fanTemperature, value);
        }

        private float _fanRpm;
        public float FanRpm
        {
            get => _fanRpm;
            set => OnPropertyChanged(ref _fanRpm, value);
        }

        private float _fanSpeed;
        public float FanSpeed
        {
            get => _fanSpeed;
            set => OnPropertyChanged(ref _fanSpeed, value);
        }

        private float _fanSpeedSetValue;
        public float FanSpeedSetValue
        {
            get => _fanSpeedSetValue;
            set => OnPropertyChanged(ref _fanSpeedSetValue, value);
        }


        private float _circulationFanSpeedFeedback1;
        public float CirculationFanSpeedFeedback1
        {
            get => _circulationFanSpeedFeedback1;
            set => OnPropertyChanged(ref _circulationFanSpeedFeedback1, value);
        }

        private float _circulationFanSpeedFeedback2;
        public float CirculationFanSpeedFeedback2
        {
            get => _circulationFanSpeedFeedback2;
            set => OnPropertyChanged(ref _circulationFanSpeedFeedback2, value);
        }

        private float _fanVibration;
        public float FanVibration
        {
            get => _fanVibration;
            set => OnPropertyChanged(ref _fanVibration, value);
        }

        private float _pidHeatOut;
        public float PidHeatOut
        {
            get => _pidHeatOut;
            set => OnPropertyChanged(ref _pidHeatOut, value);
        }

        private float _pidCoolOut;
        public float PidCoolOut
        {
            get => _pidCoolOut;
            set => OnPropertyChanged(ref _pidCoolOut, value);
        }

        private float _pidCoolOutStatus;
        public float PidCoolOutStatus
        {
            get => _pidCoolOutStatus;
            set => OnPropertyChanged(ref _pidCoolOutStatus, value);
        }

        private float _vacuumLineValue;
        public float VacuumLineValue
        {
            get => _vacuumLineValue;
            set => OnPropertyChanged(ref _vacuumLineValue, value);
        }

        private float _pidVacOut;
        public float PidVacOut
        {
            get => _pidVacOut;
            set => OnPropertyChanged(ref _pidVacOut, value);
        }

        private float _pidPressureOut;
        public float PidPressureOut
        {
            get => _pidPressureOut;
            set => OnPropertyChanged(ref _pidPressureOut, value);
        }

        private float _pressureBar;
        public float PressureBar
        {
            get => _pressureBar;
            set => OnPropertyChanged(ref _pressureBar, value);
        }


        private float _towerWaterTemperature;
        public float TowerWaterTemperature
        {
            get => _towerWaterTemperature;
            set => OnPropertyChanged(ref _towerWaterTemperature, value);
        }

        private string _batchTotalWorkingTime;
        public string BatchTotalWorkingTime
        {
            get => _batchTotalWorkingTime;
            set => OnPropertyChanged(ref _batchTotalWorkingTime, value);
        }



        private float _pidAtmosphereOut;
        public float PidAtmosphereOut
        {
            get => _pidAtmosphereOut;
            set => OnPropertyChanged(ref _pidAtmosphereOut, value);
        }

        private float _doorStatus;
        public float DoorStatus
        {
            get => _doorStatus;
            set => OnPropertyChanged(ref _doorStatus, value);
        }

        # endregion
    }
}