using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.PageTagConfigurations
{
    public class GlobalTagConfigurations
    {
        public object LeakageTestInfoCheckStatusOk { get; set; }
        public object LeakageTestInfoCheckStatusRun { get; set; }
        public object LeakageTestInfoCheckStatusStop { get; set; }
        public object LeakageTestInfoCheckStatusFault { get; set; }
        public object LeakageTestSkipCommand { get; set; }
    }
}
