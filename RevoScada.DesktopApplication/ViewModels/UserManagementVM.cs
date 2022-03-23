using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.Models.ModelEnums;
using RevoScada.DesktopApplication.Views;
using RevoScada.DesktopApplication.Views.Popups;
using RevoScada.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RevoScada.DesktopApplication.ViewModels
{
    public class UserManagementVM : UserControlBaseVM
    {
        #region Services
        private readonly string _connectionString;
        private PermissionService _permissionService;
        private UserService _userService;
        private UserGroupService _userGroupService;
        #endregion

        #region Fields
        private string[] _pageNames;
        private Password_Edit _passwordEditPopup;
        private LogoutTime_Edit _logoutTimePopup;
        public User_Management_View UserManagementView;
        #endregion

        #region Properties
        private bool _isGroupSelected;
        public bool IsGroupSelected 
        {
            get => _isGroupSelected;
            set => OnPropertyChanged(ref _isGroupSelected, value);
        }
        private UserGroupGridModel _selectedUserGroup;
        public UserGroupGridModel SelectedUserGroup
        {
            get => _selectedUserGroup;
            set
            {
                OnPropertyChanged(ref _selectedUserGroup, value);
                if (value == null)
                    return;

                AssignedUsers = _userService.GetByGroupId(value.id).ToObservableCollection();
                CreatePermissionGridData(value);
            }
        }
        private Visibility _assignedUsersVisibility;
        public Visibility AssignedUsersVisibility
        {
            get => _assignedUsersVisibility;
            set => OnPropertyChanged(ref _assignedUsersVisibility, value);
        }
        private Visibility _passwordBoxVisibility;
        public Visibility PasswordBoxVisibility
        {
            get => _passwordBoxVisibility;
            set => OnPropertyChanged(ref _passwordBoxVisibility, value);
        }
        private Visibility _userControlGridVisibility;
        public Visibility UserControlGridVisibility
        {
            get => _userControlGridVisibility;
            set => OnPropertyChanged(ref _userControlGridVisibility, value);
        }
        private bool _isAssignToUserBtnEnabled;
        public bool IsAssignToUserBtnEnabled 
        {
            get => _isAssignToUserBtnEnabled;
            set => OnPropertyChanged(ref _isAssignToUserBtnEnabled, value);
        }

        private bool _isSelectAllPermissions;
        public bool IsSelectAllPermissions
        {
            get => _isSelectAllPermissions;
            set => OnPropertyChanged(ref _isSelectAllPermissions, value);
        }

        private bool _isSelectAllPermissionsCbEnabled;
        public bool IsSelectAllPermissionsCbEnabled
        {
            get => _isSelectAllPermissionsCbEnabled;
            set => OnPropertyChanged(ref _isSelectAllPermissionsCbEnabled, value);
        }

        private bool _hasGenControlPermissionGranted;
        public bool HasGenControlPermissionGranted
        {
            get => _hasGenControlPermissionGranted;
            set => OnPropertyChanged(ref _hasGenControlPermissionGranted, value);
        }
        #endregion

        #region Commands
        public ICommand LoadPasswordChangePopupCommand { get; set; }
        public ICommand LoadLogoutTimeChangePopupCommand { get; set; }
        public ICommand SelectAllPermissionsCommand { get; set; }
        public ICommand SetUserActiveOrNotCommand { get; set; }
        public ICommand SetGroupActiveOrNotCommand { get; set; }
        #endregion

        #region Collections
        private List<KeyValuePair<string, Permission>> _permissionsByPageName;
        private Dictionary<int, string> _permissionNamesById;
        public ObservableCollection<PermissionGridModel> PermissionGridData { get; set; }
        private ObservableCollection<UserGridModel> _users;
        public ObservableCollection<UserGridModel> Users
        {
            get => _users;
            set
            {
                OnPropertyChanged(ref _users, value);
                ActiveUsers = value.Where(u => u.IsActive).ToObservableCollection();
            }
        }
        private ObservableCollection<UserGridModel> _activeUsers;
        public ObservableCollection<UserGridModel> ActiveUsers
        {
            get => _activeUsers;
            set => OnPropertyChanged(ref _activeUsers, value);
        }
        private ObservableCollection<UserGroupGridModel> _userGroups;
        public ObservableCollection<UserGroupGridModel> UserGroups 
        {
            get => _userGroups;
            set
            {
                OnPropertyChanged(ref _userGroups, value);
                ActiveUserGroups = _userGroups.Where(u => u.IsActive).ToObservableCollection();
            }
        }
        private ObservableCollection<UserGroupGridModel> _activeUserGroups;
        public ObservableCollection<UserGroupGridModel> ActiveUserGroups
        {
            get => _activeUserGroups;
            set => OnPropertyChanged(ref _activeUserGroups, value);
        }
        private ObservableCollection<User> _assignedUsers;
        public ObservableCollection<User> AssignedUsers 
        {
            get => _assignedUsers;
            set => OnPropertyChanged(ref _assignedUsers, value);
        }
        #endregion

        public UserManagementVM(UserGridModel activeUser)
        {
            _connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;
            _permissionService = new PermissionService(_connectionString);
            _userService = new UserService(_connectionString);
            _userGroupService = new UserGroupService(_connectionString);

            LoadPasswordChangePopupCommand = new RelayCommand(LoadPasswordChangePopup);
            LoadLogoutTimeChangePopupCommand = new RelayCommand(LoadLogoutTimeChangePopup);
            SelectAllPermissionsCommand = new RelayCommand(SelectAllPermissions);
            SetUserActiveOrNotCommand = new RelayCommand(SetUserActiveOrNot);
            SetGroupActiveOrNotCommand = new RelayCommand(SetGroupActiveOrNot);

            ActiveUser = activeUser;
            UserControlGridVisibility = Visibility.Visible;
            AssignedUsersVisibility = Visibility.Collapsed;
            _permissionsByPageName = _permissionService.GetAll().OrderBy(p => p.id).Select(p => new KeyValuePair<string, Permission>(p.PageName, p)).ToList();
            _permissionNamesById = _permissionService.GetAll().ToDictionary(p => p.id, p => p.PermissionTag);
            _pageNames = _permissionsByPageName.Select(p => p.Key).Distinct().ToArray();

            PermissionGridData = new ObservableCollection<PermissionGridModel>();
            Users = _userService.GetAll().Select(u => new UserGridModel()
            {
                id = u.id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,
                CreateDate = u?.CreateDate ?? DateTime.MinValue,
                GroupId = u.GroupId,
                GroupName = u.GroupId > 0 ? _userGroupService.GetById(u.GroupId)?.GroupName : string.Empty,
                IsActive = u.IsActive,
                Password = u.Password,
                LogoutTime = u.LogoutTime
            }).OrderBy(u => u.id).ToObservableCollection();
            UserGroups = GetUserGroupsWithPermissionNames();

            UpdateGenPermissionStatus();
        }

        private void UpdateGenPermissionStatus()
        {
            HasGenControlPermissionGranted = true;

            if (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId == 20)
            {
                if (ActiveUser.GroupId != 1) //
                    HasGenControlPermissionGranted = false;
            }
        }

        public ObservableCollection<UserGroupGridModel> GetUserGroupsWithPermissionNames()
        {
            var userGroups = _userGroupService.GetAll().Select(u => new UserGroupGridModel()
            {
                id = u.id,
                GroupName = u.GroupName,
                PermissionIds = u.PermissionIds,
                IsActive = u.IsActive
            }).OrderBy(u => u.id).ToObservableCollection();

            foreach (var group in userGroups)
            {
                string permissionNames = string.Empty;
                short groupCounter = 0;
                for (int i = 0; i < group.PermissionIds.Length; i++)
                {
                    permissionNames += _permissionNamesById[group.PermissionIds[i]];

                    if (i != group.PermissionIds.Length -1)
                        permissionNames += " - ";

                    groupCounter++;

                    if(groupCounter == 5)
                    {
                        permissionNames += "\n";
                        groupCounter = 0;
                    }
                }
                group.PermissionNames = permissionNames;
            }
            return userGroups;
        }

        private void CreatePermissionGridData(UserGroupGridModel selectedUserGroup)
        {
            PermissionGridData.Clear();
            bool areSubActionsEnabled;
            foreach (var page in _pageNames)
            {
                areSubActionsEnabled = false;

                foreach (var item in _permissionsByPageName.Where(p => p.Key == page).Select(p => p.Value))
                {
                    object[] tagVals;
                    PermissionGridModel permissionGridModel = new PermissionGridModel()
                    {
                        id = item.id,
                        PageName = item.PageName,
                        PermissionGroup = item.PermissionGroup,
                        PermissionTag = item.PermissionTag
                    };

                    if (item.PermissionGroup == 1)
                    {
                        areSubActionsEnabled = selectedUserGroup.PermissionIds.Contains(item.id) ? true : false;

                        if (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId == 20)
                        {
                            if (selectedUserGroup.id == 1 && item.id == 42) // User Management Page
                                permissionGridModel.IsEnabled = false;
                            else
                                permissionGridModel.IsEnabled = true;
                        }
                        else
                        {
                            permissionGridModel.IsEnabled = true;
                        }
                    }
                    else
                    {
                        permissionGridModel.IsEnabled = areSubActionsEnabled;
                    }

                    permissionGridModel.IsSelected = selectedUserGroup.PermissionIds.Contains(item.id) ? true : false;
                    tagVals = new object[] { item.id, item.PermissionGroup, item.PageName };
                    permissionGridModel.CheckboxTag = tagVals;
                    PermissionGridData.Add(permissionGridModel);
                }
            }
        }

        public void UpdateSelectAllCbState()
        {
            if (PermissionGridData.Count == 0)
                return;

            if (PermissionGridData.Where(f => !f.IsSelected).Any())
                IsSelectAllPermissions = false;

            if (PermissionGridData.Where(f => f.IsSelected).Count() == PermissionGridData.Count)
                IsSelectAllPermissions = true;
        }

        public void ChangeSubPermissionCheckboxStates(string pageName, bool stateVal)
        {
            foreach (var item in PermissionGridData.Where(p => p.PageName == pageName))
            {
                if (item.PermissionGroup == 1)
                    continue;

                if(!stateVal)
                    item.IsSelected = stateVal;

                item.IsEnabled = stateVal;
            }
        }

        public bool UpdateSelectedGroupPermissions(int id, bool stateVal, bool allPermissions = false)
        {
            bool result = false;

            if(!allPermissions)
            {
                if (stateVal)
                {
                    if (!SelectedUserGroup.PermissionIds.Contains(id))
                        SelectedUserGroup.PermissionIds = SelectedUserGroup.PermissionIds.Concat(new int[] { id }).ToArray();
                }
                else
                {
                    if (SelectedUserGroup.PermissionIds.Contains(id))
                        SelectedUserGroup.PermissionIds = SelectedUserGroup.PermissionIds.Except(new int[] { id }).ToArray();
                }
            } else
            {
                if (stateVal)
                {
                    SelectedUserGroup.PermissionIds = _permissionNamesById.Keys.ToArray();
                }
                else
                {
                    SelectedUserGroup.PermissionIds = new int[0];
                }
            }

            UserGroup userGroup = new UserGroup()
            {
                id = SelectedUserGroup.id,
                GroupName = SelectedUserGroup.GroupName,
                PermissionIds = SelectedUserGroup.PermissionIds,
                IsActive = SelectedUserGroup.IsActive
            };

            result = _userGroupService.Update(userGroup);

            if (allPermissions && result)
                CreatePermissionGridData(SelectedUserGroup);

            return result;
        }

        public bool AddUserToDb(User user)
        {
            var result = _userService.Insert(user);

            // If result is success then refresh user collection.
            if (result)
            {
                Users = _userService.GetAll().Select(u => new UserGridModel()
                {
                    id = u.id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    UserName = u.UserName,
                    CreateDate = u?.CreateDate ?? DateTime.MinValue,
                    GroupId = u.GroupId,
                    GroupName = u.GroupId > 0 ? _userGroupService.GetById(u.GroupId)?.GroupName : string.Empty,
                    IsActive = u.IsActive,
                    Password = u.Password,
                    LogoutTime = u.LogoutTime
                }).OrderBy(u => u.id).ToObservableCollection();
            }

            return result;
        }

        public bool AddUserGroupToDb(UserGroup userGroup)
        {
            var result = _userGroupService.Insert(userGroup);

            // If result is success then refresh user group collection.
            if (result)
            {
                UserGroups = GetUserGroupsWithPermissionNames();
            }

            return result;
        }

        public bool AssignGroupToUser(UserGridModel userGridModel, UserGroupGridModel userGroupGridModel)
        {
            User user = new User()
            {
                id = userGridModel.id,
                FirstName = userGridModel.FirstName,
                LastName = userGridModel.LastName,
                CreateDate = userGridModel.CreateDate,
                GroupId = userGroupGridModel.id,
                IsActive = userGridModel.IsActive,
                Password = userGridModel.Password,
                UserName = userGridModel.UserName,
                LogoutTime = userGridModel.LogoutTime
            };

            var result = _userService.Update(user);

            if (!result)
                return false;

            // If result is success then refresh user collection.
            Users = _userService.GetAll().Select(u => new UserGridModel()
            {
                id = u.id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,
                CreateDate = u?.CreateDate ?? DateTime.MinValue,
                GroupId = u.GroupId,
                GroupName = u.GroupId > 0 ? _userGroupService.GetById(u.GroupId)?.GroupName : string.Empty,
                IsActive = u.IsActive,
                Password = u.Password,
                LogoutTime = u.LogoutTime
            }).OrderBy(u => u.id).ToObservableCollection();

            if (SelectedUserGroup != null)
                AssignedUsers = _userService.GetByGroupId(SelectedUserGroup.id).ToObservableCollection();

            return result;
        }

        private void SetUserActiveOrNot(object param)
        {
            int id = Convert.ToInt32(param);
            var result = false;

            var selectedUserGridModel = Users.Where(u => u.id == id).FirstOrDefault();

            if (selectedUserGridModel != null)
            {
                selectedUserGridModel.IsActive = selectedUserGridModel.IsActive ? false : true;
                result = UpdateUser(selectedUserGridModel);
            }

            if (result)
                UserManagementView.DbResultPositiveFadeOutAnim.Begin();
            else
                UserManagementView.DbResultNegativeFadeOutAnim.Begin();
        }

        private void SetGroupActiveOrNot(object param)
        {
            int id = Convert.ToInt32(param);
            var result = false;

            var selectedGroupGridModel = UserGroups.Where(u => u.id == id).FirstOrDefault();

            if (selectedGroupGridModel != null)
            {
                selectedGroupGridModel.IsActive = selectedGroupGridModel.IsActive ? false : true;
                result = UpdateGroup(selectedGroupGridModel);
            }

            if (result)
                UserManagementView.DbResultPositiveFadeOutAnim.Begin();
            else
                UserManagementView.DbResultNegativeFadeOutAnim.Begin();
        }

        public bool UpdateUser(UserGridModel userGridModel)
        {
            User updatedUser = new User();
            updatedUser.id = userGridModel.id;
            updatedUser.FirstName = userGridModel.FirstName;
            updatedUser.LastName = userGridModel.LastName;
            updatedUser.UserName = userGridModel.UserName;
            updatedUser.CreateDate = DateTime.Now;
            updatedUser.IsActive = userGridModel.IsActive;
            updatedUser.GroupId = userGridModel.GroupId;
            updatedUser.Password = userGridModel.Password;
            updatedUser.LogoutTime = userGridModel.LogoutTime;

            var result = _userService.Update(updatedUser);

            if (!result)
                return false;

            Users = _userService.GetAll().Select(u => new UserGridModel()
            {
                id = u.id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,
                CreateDate = u?.CreateDate ?? DateTime.MinValue,
                GroupId = u.GroupId,
                GroupName = u.GroupId > 0 ? _userGroupService.GetById(u.GroupId)?.GroupName : string.Empty,
                IsActive = u.IsActive,
                Password = u.Password,
                LogoutTime = u.LogoutTime
            }).OrderBy(u => u.id).ToObservableCollection();

            return result;
        }

        public bool UpdateGroup(UserGroupGridModel userGroupGridModel)
        {
            UserGroup updatedGroup = new UserGroup();
            updatedGroup.id = userGroupGridModel.id;
            updatedGroup.GroupName = userGroupGridModel.GroupName;
            updatedGroup.PermissionIds = userGroupGridModel.PermissionIds;
            updatedGroup.IsActive = userGroupGridModel.IsActive;

            var result = _userGroupService.Update(updatedGroup);

            if (!result)
                return false;

            UserGroups = GetUserGroupsWithPermissionNames();

            return result;
        }

        public bool UpdateUserPassword(int userId, string hashedPass)
        {
            User updatedUser = _userService.GetById(userId);
            updatedUser.Password = hashedPass;

            var result = _userService.Update(updatedUser);

            if (!result)
                return false;

            Users = _userService.GetAll().Select(u => new UserGridModel()
            {
                id = u.id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,
                CreateDate = u?.CreateDate ?? DateTime.MinValue,
                GroupId = u.GroupId,
                GroupName = u.GroupId > 0 ? _userGroupService.GetById(u.GroupId)?.GroupName : string.Empty,
                IsActive = u.IsActive,
                Password = u.Password,
                LogoutTime = u.LogoutTime
            }).OrderBy(u => u.id).ToObservableCollection();

            return result;
        }

        public bool UpdateUserLogoutTime(int userId, string newTimeValue)
        {
            User updatedUser = _userService.GetById(userId);
            updatedUser.LogoutTime = Convert.ToInt16(newTimeValue);

            var result = _userService.Update(updatedUser);

            if (!result)
                return false;

            Users = _userService.GetAll().Select(u => new UserGridModel()
            {
                id = u.id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,
                CreateDate = u?.CreateDate ?? DateTime.MinValue,
                GroupId = u.GroupId,
                GroupName = u.GroupId > 0 ? _userGroupService.GetById(u.GroupId)?.GroupName : string.Empty,
                IsActive = u.IsActive,
                Password = u.Password,
                LogoutTime = u.LogoutTime
            }).OrderBy(u => u.id).ToObservableCollection();

            return result;
        }

        private void LoadPasswordChangePopup(object param)
        {
            int userId = (int)param;
            UserGridModel selectedUserGridModel = Users.Where(u => u.id == userId).FirstOrDefault();
            _passwordEditPopup = new Password_Edit(this, UserManagementView, userId, selectedUserGridModel);
            _passwordEditPopup.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            _passwordEditPopup.ShowDialog();
        }


        private void LoadLogoutTimeChangePopup(object param)
        {
            int userId = (int)param;
            UserGridModel selectedUserGridModel = Users.Where(u => u.id == userId).FirstOrDefault();
            _logoutTimePopup = new LogoutTime_Edit(this, UserManagementView, userId, selectedUserGridModel);
            _logoutTimePopup.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            _logoutTimePopup.ShowDialog();
        }


        private void SelectAllPermissions(object param)
        {
            bool isChecked = (bool)param;
            UpdateSelectedGroupPermissions(0, isChecked, true);
        }
    }
}
