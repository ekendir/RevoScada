using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.PageTagConfigurations
{
    public class OscillationTagConfigurations
    {
       
        public List<OscillationCriteria> OscillationCriterias { get; set; }
        public List<int> DbNumbers { get; set; }

    }
}
