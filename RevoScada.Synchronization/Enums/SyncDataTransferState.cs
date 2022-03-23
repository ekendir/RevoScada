using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RevoScada.Synchronization.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    //mainly used for data transfer status
    public enum SyncDataTransferState
    {
        NotStarted, PreparingData, ReadyToFetch, Fetching, FetchCompleted
    }


}
