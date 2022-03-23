namespace RevoScada.ReadService
{
    interface IPlcReadCycleStrategy
    {
        bool Connect();
        void InitializeCycle();
        void RunCycle();
        void AbortCycle();
    }
}
