namespace RevoScada.AlarmService
{
    interface IPlcAlarmCycleStrategy
    {
        
        void InitializeCycle();
        void RunCycle();
        void AbortCycle();
    }
}
