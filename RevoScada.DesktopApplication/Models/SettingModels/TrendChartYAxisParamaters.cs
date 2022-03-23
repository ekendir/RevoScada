using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models.SettingModels
{
    public class TrendChartYAxisParamaters
    {
        public float TemperatureParamaterMin { get; set; }
        public float TemperatureParamaterMax { get; set; }
        public float VacuumParamaterMin { get; set; }
        public float VacuumParamaterMax { get; set; }
        public float PressureParamaterMin { get; set; }
        public float PressureParamaterMax { get; set; }
    }
}