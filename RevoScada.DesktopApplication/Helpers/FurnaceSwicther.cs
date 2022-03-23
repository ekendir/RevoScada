
using Revo.Core;
using RevoScada.ProcessController;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Helpers
{
    class FurnaceSwicther
    {
        public bool DefineFurnaceSelection()
        {
            bool isDefineFailed = false;

            if (ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == Entities.Configuration.Service.WorkingEnvironment.server)
            {
                FurnaceSelector furnaceSelector = new FurnaceSelector();
                furnaceSelector.ShowDialog();
                int selectedPlcDeviceId = furnaceSelector.PlcDeviceId;
                furnaceSelector.Close();
                isDefineFailed = selectedPlcDeviceId > 0 ? false : true;
                ProcessManager.Instance.InitializeSelectedDevice(selectedPlcDeviceId);
                ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionStrings[selectedPlcDeviceId];
                AlarmManager.Instance.InitializeSelectedDevice(selectedPlcDeviceId);
                LogManager.Instance.LogPrefixText = $"PLC{selectedPlcDeviceId} ";
            }
            else
            {
                ProcessManager.Instance.InitializeSelectedDevice(ApplicationConfigurations.Instance.Configuration.PlcDevice.Id);
                AlarmManager.Instance.InitializeSelectedDevice(ApplicationConfigurations.Instance.Configuration.PlcDevice.Id);
                LogManager.Instance.LogPrefixText = $"PLC{ApplicationConfigurations.Instance.Configuration.PlcDevice.Id} ";
            }

            return isDefineFailed;
        }
    }
}
