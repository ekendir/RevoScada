using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class TrendSelectedPortUIProperty
    {
        public string DisplayName { get; set; }
        public int Thickness { get; set; } = 1;
        public Color Color { get; set; } = Color.Empty;
        public bool IsLegendChecked { get; set; }
        public float YRangeMinValue { get; set; }
        public float YRangeMaxValue { get; set; }       
        public float XRangeMinValue { get; set; }
        public float XRangeMaxValue { get; set; }
        public bool IsShowCommonForAllSeriesChecked { get; set; }
    }
}
