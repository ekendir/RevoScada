using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models.SettingModels
{
    public class ApplicationLanguageDetails
    {
        public Dictionary<string, string> VacuumLines { get; set; }
        public Dictionary<string, string> EnterParts { get; set; }
        public Dictionary<string, string> RecipeEditor { get; set; }
        public Dictionary<string, string> ActiveRecipe { get; set; }
        public Dictionary<string, string> RunOperation { get; set; }
        public Dictionary<string, string> GlobalPortName { get; set; }
    }
}
