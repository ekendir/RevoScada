
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RevoScada.Synchronization.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransferType
    {
        NotDefined,NonProcessChanges, AfterStart, AfterFinish, AfterHold, MissingBatchData, MissingBatchDataWithDataLogs
    }

}
