using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.PageTagConfigurations
{
    public class EnterPartsTagConfigurations
    {
        public Dictionary<string, EnterPartsSelectedPortsInfo> SelectedPortsInfo { get; set; }


        public object ScadaSentBatch { get; set; }
        public object EnterPartsOk { get; set; }
        public object MoveToPre { get; set; }
        public object SkipEnterParts { get; set; }
        public object ActiveBatchName { get; set; }
        public int SelectedPortInfoDbNumber { get; set; }
        public int PortBagInfoDbNumber { get; set; }
        public List<int> DbNumbers { get; set; }


    }

 
}
