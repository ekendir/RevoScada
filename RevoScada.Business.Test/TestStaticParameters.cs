using RevoScada.Configurator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Business.Test
{
    public class TestStaticParameters
    {
        static bool IsClient = false;
        public static ApplicationConfigurations ApplicationConfigurations
        {
            get
            {
                ApplicationConfigurations.Instance.InitializeConfiguration(@"C:\RevoScada.TAI.Files\Configuration\MultipleConfigurations\Application.rsconfig", IsClient);
                return ApplicationConfigurations.Instance;
            }
        }
     }  
}
