using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models.SettingModels
{
    public class AirTcSelection
    {
        public string DisplayName { get; set; }
        public int Value { get; set; }
        public int TagId { get; set; }
        public bool IsSelected { get; set; }
    }
}
