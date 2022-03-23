using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RevoScada.Synchronization.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SyncDBCommand
    {
        None,Delete,Insert,Update,InsertMany
    }
}
 