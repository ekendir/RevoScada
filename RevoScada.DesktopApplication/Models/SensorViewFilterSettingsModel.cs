using DevExpress.Data.Filtering;
using RevoScada.DesktopApplication.Models.ModelEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class SensorViewFilterSettingsModel
    {
        public string PTCGridViewFilterCriteria { get; set; }
        public string MONGridViewFilterCriteria { get; set; }
    }
}
