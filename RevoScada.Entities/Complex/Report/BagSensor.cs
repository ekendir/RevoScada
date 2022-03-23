using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.Complex.Report
{
    

    public class BagSensors
    {

        /// <summary>
        /// T/C
        /// </summary>
        public List<string> PTCs { get; set; }
        public List<string> MONs { get; set; }

        /// <summary>
        /// SRC
        /// </summary>
        public List<string> VACs { get; set; }

        private string _allPTCs;
        public string AllPTCs 
        { 
            get
            {
                foreach (var item in PTCs)
                    _allPTCs += item + "\n";

                return _allPTCs;
            }
            set
            {
                _allPTCs = value;
            }
        }

        private string _allMONs;
        public string AllMONs
        {
            get
            {
                foreach (var item in MONs)
                    _allMONs += item + "\n";

                return _allMONs;
            }
            set
            {
                _allMONs = value;
            }
        }

        private string _allVACs;
        public string AllVACs
        {
            get
            {
                foreach (var item in VACs)
                    _allVACs += item + "\n";

                return _allVACs;
            }
            set
            {
                _allVACs = value;
            }
        }
    }
}
