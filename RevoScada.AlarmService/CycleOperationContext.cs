using System.Collections.Generic;
namespace RevoScada.AlarmService
{
    enum PlcType
    {
        siemens, allenbradley
    }

    class CycleOperationContext
    {
        private static readonly Dictionary<PlcType, IPlcAlarmCycleStrategy> _cycleStrategies = new Dictionary<PlcType, IPlcAlarmCycleStrategy>();
        private  PlcType _plcType;


        public CycleOperationContext()
        {
            _cycleStrategies.Add(PlcType.siemens, new SiemensAlarmCycleStrategy());
        }

        

        public void InitializeCycle(PlcType plcType)
        {
            _plcType = plcType;
            _cycleStrategies[_plcType].InitializeCycle();
        }

        public void RunCycle()
        {
            _cycleStrategies[_plcType].RunCycle();
        }

        public void AbortCycle()
        {
            _cycleStrategies[_plcType].AbortCycle();
        }
    }
}
