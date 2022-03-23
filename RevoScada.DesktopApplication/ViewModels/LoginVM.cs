using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Models;
using RevoScada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RevoScada.DesktopApplication.ViewModels
{
    public class LoginVM : ObservableObject
    {
        #region Services
        private readonly string _connectionString;
        private UserService _userService;
        #endregion

        #region Properties
        private Visibility _loginSectionVisibility;
        public Visibility LoginSectionVisibility
        {
            get => _loginSectionVisibility;
            set => OnPropertyChanged(ref _loginSectionVisibility, value);
        }
        private UserGridModel _activeUser;
        public UserGridModel ActiveUser
        {
            get => _activeUser;
            set => OnPropertyChanged(ref _activeUser, value);
        }
        private bool _isCreateAccountBtnEnabled;
        public bool IsCreateAccountBtnEnabled
        {
            get => _isCreateAccountBtnEnabled;
            set => OnPropertyChanged(ref _isCreateAccountBtnEnabled, value);
        }
        #endregion

        #region Collections
        public List<User> Users;
        #endregion

        public LoginVM()
        {
            _connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;
            _userService = new UserService(_connectionString);

            //Get active users
            Users = _userService.GetAll().Where(u => u.IsActive).ToList();
        }
    }
}
