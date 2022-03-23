using DevExpress.Xpf.WindowsUI;
using Newtonsoft.Json;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.Entities;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.Configuration.Service;
using RevoScada.Synchronization;
using RevoScada.Synchronization.Enums;
using RevoScada.Synchronization.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RevoScada.DesktopApplication.Views.Popups
{
    /// <summary>
    /// Interaction logic for LoadNumberEditorPopup.xaml
    /// </summary>
    public partial class LoadNumberEditorPopup : UserControl
    {
        private readonly ApplicationConfiguration _configuration;
        private readonly int _plcDeviceId;

        public LoadNumberEditorPopup(ApplicationConfiguration configuration, int plcDeviceId)
        {
            _configuration = configuration;
            _plcDeviceId = plcDeviceId;
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(_configuration.PostgreSqlConnectionString);
            int lastLoadNumber = Convert.ToInt32(applicationPropertyService.GetByName("LastLoadNumber").Value);
            spinEditLoadNumber.MinValue = lastLoadNumber;
        }


        private void BtnRenameLoadNumber_Click(object sender, RoutedEventArgs e)
        {
            int spinnerValue = Convert.ToInt32(spinEditLoadNumber.Value);
            //var service = new BatchService(_connectionString);
            //bool renameResult = service.RenameLoadNumber(_furnaceName, spinnerValue, _batch);

            ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(_configuration.PostgreSqlConnectionString);
            var lastLoadNumberProperty = applicationPropertyService.GetByName("LastLoadNumber");
            lastLoadNumberProperty.Value = spinnerValue.ToString();
            bool updateResult = applicationPropertyService.Update(lastLoadNumberProperty);

            if (updateResult)
            {
                var fromToDirection = _configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
                string serializedEntityObject = JsonConvert.SerializeObject(lastLoadNumberProperty);
                SyncIssue syncIssue = new SyncIssue
                {
                    SerializedEntityObject = serializedEntityObject,
                    EntityObjectType = typeof(ApplicationProperty),
                    SyncDBCommand = SyncDBCommand.Update,
                    FromToDirection = fromToDirection,
                    PlcDeviceId = _plcDeviceId,
                    SyncStatus = SyncStatus.NoneProcessChangesPending,
                    TransferType = TransferType.NonProcessChanges,
                };

                SyncIssueManager syncIssueManager = new SyncIssueManager(_configuration.RedisServer);
                syncIssueManager.CreateNewSyncIssue(syncIssue);

                textBlockOutput.Text = "Completed!";
            }
        }
    }
}
