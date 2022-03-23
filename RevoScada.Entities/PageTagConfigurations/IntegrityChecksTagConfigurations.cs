using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.PageTagConfigurations
{
    public class IntegrityChecksTagConfigurations
    {
        public Dictionary<string, Dictionary<string, IntegrityChecksItem>> IntegrityChecksItems { get; set; }
        public object LeakageTestFailureCriteriaTestValue { get; set; }
        public int LeakageTestFailureSetTime { get; set; }
        public int LeakageTestInfoCheckStatusOk { get; set; }
        public int LeakageTestInfoCheckStatusRun { get; set; }
        public int LeakageTestInfoCheckStatusStop { get; set; }
        public int LeakageTestInfoCheckStatusFault { get; set; }
        public int LeakageTestInfoCheckElapsedMinute { get; set; }
        public int LeakageTestInfoCheckElapsedSecond { get; set; }
        public int SensorDataMinMonValue { get; set; }
        public int SensorDataMaxMonValue { get; set; }
        public int SensorDataMinMonTitle { get; set; }
        public int SensorDataMaxMonTitle { get; set; }
        public int OperationCommand { get; set; }
        public int SkipCommand { get; set; }
        public int IntegrityCheckOk { get; set; }
        

        //todo:h dbler silinmiş bu kodda sabit mi duruyor????
        //public int ActualDbNumber { get; set; }

        //public int ActualIncrementAmount { get; set; }

        //public int FinishDbNumber { get; set; }

        //public int FinishIncrementAmount { get; set; }

        //public int StartDbNumber { get; set; }

        //public int StartIncrementAmount { get; set; }

        //public int DeviationDbNumber { get; set; }

        //public int DeviationIncrementAmount { get; set; }



    }
}
 