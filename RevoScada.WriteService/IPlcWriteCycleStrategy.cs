namespace RevoScada.WriteService
{
    interface IPlcWriteCycleStrategy
    {
        bool Connect();
        void InitializeCycle();
        void RunCycle();
        void AbortCycle();
    }
}
