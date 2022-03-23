using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models.SettingModels
{
   public class ReportExportSettings
    {
        public string ExcelExportPassword { get; set; }
        public string ExcelExportFilePath { get; set; }
        public string ExcelExportFileNameBase { get; set; }
    }
}
