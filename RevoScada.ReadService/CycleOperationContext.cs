using System.Collections.Generic;
namespace RevoScada.ReadService
{
    enum PlcType
    {
        siemens, allenbradley
    }

    class CycleOperationContext
    {
        private static readonly Dictionary<PlcType, IPlcReadCycleStrategy> _cycleStrategies = new Dictionary<PlcType, IPlcReadCycleStrategy>();
        private  PlcType _plcType;
        public CycleOperationContext()
        {
            _cycleStrategies.Add(PlcType.siemens, new SiemensReadCycleStrategy());
        }

        public bool ConnectDevices(PlcType plcType)
        {
          _plcType = plcType; 
          return _cycleStrategies[_plcType].Connect();
        }

        public void InitializeCycle()
        {
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
