using RevoScada.Configurator;
using RevoScada.Entities.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DataAccess.Test
{
    public class TestStaticParameters
    {
        private static bool IsClient = true;
        private static int PlcDevice = 2;
        public static ApplicationConfiguration ApplicationConfiguration
        {
            get
            {
                ApplicationConfigurations.Instance.InitializeConfiguration(@"C:\\RevoScada.TAI.Files\\Configuration\\MultipleConfigurations\Application.rsconfig", false);
                return ApplicationConfigurations.Instance.Configuration;
            }
        }
        public static Dictionary<int,ITagConfiguration> Tags
        {
            get
            {
                return ApplicationConfigurations.Instance.TagConfigurations;
            }
        }
    }  
}
