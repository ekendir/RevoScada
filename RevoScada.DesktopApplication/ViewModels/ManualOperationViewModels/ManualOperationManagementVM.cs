using RevoScada.Configurator;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Views.ManualOperationViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RevoScada.DesktopApplication.ViewModels.ManualOperationViewModels
{
    public class ManualOperationManagementVM
    {
        public object CurrentManualOperation { get; set; }

        public ManualOperationManagementVM(WaitIndicatorControl waitIndicatorControl, Dictionary<string, bool> permissions)
        {
            switch (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId)
            {
                case 1:
                    CurrentManualOperation = new ManualOperationType1();
                    var manualOperationType1View = (ManualOperationType1)CurrentManualOperation;
                    manualOperationType1View.DataContext = new ManualOperationVM(waitIndicatorControl, permissions);
                    break;
                case 2:
                    CurrentManualOperation = new ManualOperationType2();
                    var manualOperationType2View = (ManualOperationType2)CurrentManualOperation;
                    manualOperationType2View.DataContext = new ManualOperationVM(waitIndicatorControl, permissions);
                    break;

                case 3:
                    CurrentManualOperation = new ManualOperationType3();
                    var manualOperationType3View = (ManualOperationType3)CurrentManualOperation;
                    manualOperationType3View.DataContext = new ManualOperationVM(waitIndicatorControl, permissions);
                    break;

                case 4:
                case 5:
                    CurrentManualOperation = new ManualOperationType4();
                    var manualOperationType4View = (ManualOperationType4)CurrentManualOperation;
                    manualOperationType4View.DataContext = new ManualOperationVM(waitIndicatorControl, permissions);
                    break;

                case 20:
                    CurrentManualOperation = new ManualOperationType20();
                    var manualOperationType20View = (ManualOperationType20)CurrentManualOperation;
                    manualOperationType20View.DataContext = new ManualOperationVM(waitIndicatorControl, permissions);
                    break;

            }
        }
    }
}
