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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.WindowsUI;
using Revo.Core.Data;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.ViewModels;
using RevoScada.Entities;

namespace RevoScada.DesktopApplication.Views
{
    /// <summary>
    /// Interaction logic for Login_Window.xaml
    /// </summary>
    public partial class Login_Window : Window
    {
        #region Fields
        private LoginVM _viewModel;
        private AppViewModel _appViewModel;
        private User _attemptedUser;
        private bool _isUserSigned;
        private bool _doNotShowWarning;
        #endregion

        public Login_Window(AppViewModel appViewModel, bool isUserSigned)
        {
            InitializeComponent();
            _appViewModel = appViewModel;
            _isUserSigned = isUserSigned;

            _appViewModel.AccountWarningPanelVisibility = Visibility.Collapsed;
            SetLoginBindings();
            loginUserName.Focus();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as LoginVM;

            if (_isUserSigned)
            {
                _viewModel.LoginSectionVisibility = Visibility.Collapsed;
                _viewModel.ActiveUser = _appViewModel.ActiveUser;
            }
        //else
        //{
        //    _viewModel.IsCreateAccountBtnEnabled = true;
        //}

        //todo: h Another condition will be added here to decide whether an authorized user signed or not to allow creating a new account.
            if (_appViewModel.IsAllServicesRunning)
                _viewModel.IsCreateAccountBtnEnabled = true;
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserLogin();
        }
        private void ProcessUserLogin()
        {
            bool allowOrNot = UserLoginControl();

            if (!allowOrNot)
                return;

            _appViewModel.ActiveUser = new UserGridModel()
            {
                id = _attemptedUser.id,
                UserName = _attemptedUser.UserName,
                FirstName = _attemptedUser.FirstName,
                LastName = _attemptedUser.LastName,
                CreateDate = _attemptedUser.CreateDate,
                GroupId = _attemptedUser.GroupId,
                IsActive = _attemptedUser.IsActive,
                LogoutTime = _attemptedUser.LogoutTime
            };

            _appViewModel.UnloadPage(false);
            Close();
        }

        private bool UserLoginControl()
        {
            var userNameInput = loginUserName.Text.ToLower();
            var passInput = loginPassword.Password.ToString().ToLower();
            string encryptedPass = SecurityManager.CreateMD5Hash(passInput);

            _attemptedUser = _viewModel.Users.Where(u => u.UserName.ToLower() == userNameInput).FirstOrDefault();

            if (_attemptedUser == null)
            {
                WinUIMessageBox.Show("Invalid username or password! Please try it again. (Geçersiz kullanıcı adı veya şifre! Lütfen tekrar deneyiniz.)", 
                                     "Invalid user", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if(_attemptedUser.GroupId == 0)
            {
                WinUIMessageBox.Show("User has no assigned group on it! (Kullanıcıya atanmış bir group yok!)", "Invalid user", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (_attemptedUser.Password != encryptedPass)
            {
                WinUIMessageBox.Show("Wrong password! Please try it again. (Şifre hatalı. Lütfen tekrar deneyiniz.)", "Wrong password", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            // Allow user to login to the system.
            return true;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            if(_appViewModel.ActiveUser == null)
                _appViewModel.UnloadPage(true);

            Close();
        }

        private void logoutBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = WinUIMessageBox.Show("Are you sure you want to log out? (Hesabınızdan çıkış yapmak istediğinize emin misiniz?)", 
                                              "Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
                return;

            if(_appViewModel.ByPassUser != null)
                _appViewModel.ByPassUser = null;

            _appViewModel.ActiveUser = null;
            _isUserSigned = false;
            _appViewModel.AccountWarningPanelVisibility = Visibility.Visible;
            _appViewModel.UnloadPage(true);
            _viewModel.ActiveUser = null;
            Close();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_appViewModel.ActiveUser == null && !_doNotShowWarning)
                _appViewModel.AccountWarningPanelVisibility = Visibility.Visible;
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
        private void SetLoginBindings()
        {
            Binding binding = new Binding() { Path = new PropertyPath(ValidationService.HasValidationErrorProperty) };
            binding.Source = loginSp;
            binding.Converter = new NegationConverterExtension();
            loginBtn.SetBinding(IsEnabledProperty, binding);
        }
        #endregion

        private void createAccountBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = _appViewModel.MakeAvailableOnlyUserManagement();
            if (result)
                _doNotShowWarning = true;

            Close();
        }

        private void GenInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessUserLogin();
            }
        }
    }
}
