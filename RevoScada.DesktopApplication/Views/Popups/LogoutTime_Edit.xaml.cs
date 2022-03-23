using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.WindowsUI;
using Revo.Core.Data;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.ViewModels;
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
using System.Windows.Shapes;

namespace RevoScada.DesktopApplication.Views.Popups
{
    /// <summary>
    /// Interaction logic for Password_Edit.xaml
    /// </summary>
    public partial class LogoutTime_Edit : Window
    {
        #region Fields
        private UserManagementVM _userManagementVM;
        private User_Management_View _userManagementView;
        private UserGridModel _userGridModel;
        private int _userId;
        #endregion

        public LogoutTime_Edit(UserManagementVM userManagementVM, User_Management_View userManagementView, int userId, UserGridModel userGridModel)
        {
            InitializeComponent();
            _userManagementVM = userManagementVM;
            _userManagementView = userManagementView;
            _userId = userId;
            _userGridModel = userGridModel;

            UpdateLogoutValueOfTextBox();
        }

        #region Validation Section
        public bool ValidateName(string name)
        {
            bool isValid = !string.IsNullOrEmpty(name);
            return isValid;
        }
        private void GeneralNameValidate(object sender, ValidationEventArgs e)
        {
            e.IsValid = ValidateName((string)e.Value);
        }
      
        #endregion

        private void UpdateLogoutValueOfTextBox()
        {
            if (_userGridModel == null)
                return;

            currLogoutTimeBox.Text = _userGridModel.LogoutTime.ToString();
            currLogoutTimeBox.Focus();
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void changeLogoutTimeBtn_Click(object sender, RoutedEventArgs e)
        {
            string logoutTimeValue = currLogoutTimeBox.Text;
            var result = _userManagementVM.UpdateUserLogoutTime(_userId, logoutTimeValue);

            if (result)
                _userManagementView.DbResultPositiveFadeOutAnim.Begin();
            else
                _userManagementView.DbResultNegativeFadeOutAnim.Begin();

            Close();
        }
    }
}
