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
    public partial class Password_Edit : Window
    {
        #region Fields
        private UserManagementVM _userManagementVM;
        private User_Management_View _userManagementView;
        private UserGridModel _userGridModel;
        private int _userId;
        #endregion

        public Password_Edit(UserManagementVM userManagementVM, User_Management_View userManagementView, int userId, UserGridModel userGridModel)
        {
            InitializeComponent();
            _userManagementVM = userManagementVM;
            _userManagementView = userManagementView;
            _userId = userId;
            _userGridModel = userGridModel;
            SetChangePasswordBindings();
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
        private void SetChangePasswordBindings()
        {
            Binding binding = new Binding() { Path = new PropertyPath(ValidationService.HasValidationErrorProperty) };
            binding.Source = genSp;
            binding.Converter = new NegationConverterExtension();
            changePassBtn.SetBinding(IsEnabledProperty, binding);
        }
        #endregion

        private void changePassBtn_Click(object sender, RoutedEventArgs e)
        {
            string currPass = SecurityManager.CreateMD5Hash(currPasswordBox.Password);

            if(!_userGridModel.Password.Equals(currPass))
            {
                WinUIMessageBox.Show("Current password incorrect!", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (!passwordBox.Password.Equals(confirmPasswordBox.Password))
            {
                WinUIMessageBox.Show("Password does not match!", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            string hashedPass = SecurityManager.CreateMD5Hash(passwordBox.Password);
            var result = _userManagementVM.UpdateUserPassword(_userId, hashedPass);

            if (result)
                _userManagementView.DbResultPositiveFadeOutAnim.Begin();
            else
                _userManagementView.DbResultNegativeFadeOutAnim.Begin();

            Close();
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
