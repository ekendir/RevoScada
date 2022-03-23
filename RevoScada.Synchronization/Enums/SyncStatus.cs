using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RevoScada.Synchronization.Enums
{
    //mainly used for sync item and sync issue status

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SyncStatus
    {
        Stable, 
        Pending,
        NoneProcessChangesPending,
        NoneProcessChangesReadyToFetch,


        //BeforeProcessChangesCurrentInfoPending,
        //BeforeProcessChangesCurrentInfoReadytoFetch,
        //BeforeProcessChangesBatchPending,
        //BeforeProcessChangesBatchReadytoFetch,
        //BeforeProcessChangesRecipePending,
        //BeforeProcessChangesRecipeReadytoFetch,
        //BeforeProcessChangesRecipeGroupPending,
        //BeforeProcessChangesRecipeGroupReadytoFetch,
        BatchStartDataPending,
        BatchFinishDataPending,
        BatchStartDataReadytoFetch,
        BatchFinishDataReadytoFetch,
        BatchHoldDataPending,
        BatchHoldDataReadytoFetch,
        Completed
    }


}
