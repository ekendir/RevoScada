using Newtonsoft.Json;
using Revo.Core;
using RevoScada.Synchronization.Enums;
using RevoScada.Synchronization.Types;
using RevoScada.Entities.Complex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Synchronization
{
    public class LogFormatter
    {

        public void LogSingleSyncItem(SyncItem syncItem, bool isServer)
        {
            LogManager.Instance.Log(
                 $"Set SyncItem for {string.Format((isServer) ? "server" : "pc")}: \n" +
                 $"{JsonConvert.SerializeObject(syncItem, new JsonSerializerSettings { Formatting = Formatting.Indented })}", LogType.Information);

        }

        public void LogChangingPriority(SyncItem syncItem, UsagePriority previousUsagePriority,bool indented=false)
        {
                 LogManager.Instance.Log(
                    $"\n======================================================\n" +
                    $"Usage priority changed {previousUsagePriority} to {syncItem.UsagePriority}\n" +
                    $"{JsonConvert.SerializeObject(syncItem, new JsonSerializerSettings { Formatting = Formatting.Indented })}\n"+
                    $"--------------------------------------------------------\n"
                    , LogType.Information);
        }

        public void LogObject<T>(T objectToSerilalize,string title)
        {
            LogManager.Instance.Log(
                    $"\n<{title}>\n" +
                    $"{JsonConvert.SerializeObject(objectToSerilalize, new JsonSerializerSettings { Formatting = Formatting.Indented })}"+
                    $"</{title}>\n"
                    , LogType.Information);
        }
    }
}
