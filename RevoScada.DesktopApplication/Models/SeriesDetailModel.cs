using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class SeriesDetailModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public float Minimum { get; set; }
        public float Maximum { get; set; }
        public string Duration { get; set; }
        public string Type { get; set; }
    }
}
