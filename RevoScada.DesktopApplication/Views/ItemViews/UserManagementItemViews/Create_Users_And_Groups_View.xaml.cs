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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Validation;
using Revo.Core.Data;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.ViewModels;
using RevoScada.Entities;

namespace RevoScada.DesktopApplication.Views.ItemViews.UserManagementItemViews
{
    /// <summary>
    /// Interaction logic for Create_Users_And_Groups_View.xaml
    /// </summary>
    public partial class Create_Users_And_Groups_View : UserControl
    {
        #region Fields
        private UserManagementVM _viewModel;
        public Storyboard DbResultPositiveFadeOutAnim;
        public Storyboard DbResultNegativeFadeOutAnim;
        #endregion

        public Create_Users_And_Groups_View()
        {
            InitializeComponent();

            SetCreateUserBindings();
            SetCreateGroupBindings();

            DbResultPositiveFadeOutAnim = Resources["dbResultPositiveFadeOutStoryBoard"] as Storyboard;
            DbResultNegativeFadeOutAnim = Resources["dbResultNegativeFadeOutStoryBoard"] as Storyboard;
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
        private void SetCreateUserBindings()
        {
            Binding binding = new Binding() { Path = new PropertyPath(ValidationService.HasValidationErrorProperty) };
            binding.Source = createUserGenSp;
            binding.Converter = new NegationConverterExtension();
            createUserBtn.SetBinding(IsEnabledProperty, binding);
        }
        private void SetCreateGroupBindings()
        {
            Binding binding = new Binding() { Path = new PropertyPath(ValidationService.HasValidationErrorProperty) };
            binding.Source = createGroupGenSp;
            binding.Converter = new NegationConverterExtension();
            createGroupBtn.SetBinding(IsEnabledProperty, binding);
        }
        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as UserManagementVM;
        }

        private void createUserBtn_Click(object sender, RoutedEventArgs e)
        {
            User user = new User();
            user.FirstName = firstNameTb.Text;
            user.LastName = lastNameTb.Text;
            user.UserName = userNameTb.Text;
            user.Password = SecurityManager.CreateMD5Hash(passwordBox.Password);
            user.IsActive = isUserActive.IsChecked ?? false;
            user.CreateDate = DateTime.Now;
            user.LogoutTime = Convert.ToInt16(logoutTimeTb.Text);

            var result = _viewModel.AddUserToDb(user);

            if (result)
            {
                DbResultPositiveFadeOutAnim.Begin();
                firstNameTb.Text = string.Empty;
                lastNameTb.Text = string.Empty;
                userNameTb.Text = string.Empty;
                passwordBox.Password = string.Empty;
                isUserActive.IsChecked = true;
            }
            else
                DbResultNegativeFadeOutAnim.Begin();
        }

        private void createGroupBtn_Click(object sender, RoutedEventArgs e)
        {
            UserGroup userGroup = new UserGroup();
            userGroup.GroupName = groupName.Text;
            userGroup.PermissionIds = new int[0];
            userGroup.IsActive = isGroupActive.IsChecked ?? false;

            var result = _viewModel.AddUserGroupToDb(userGroup);

            if (result)
            {
                DbResultPositiveFadeOutAnim.Begin();
                groupName.Text = string.Empty;
            }
            else
                DbResultNegativeFadeOutAnim.Begin();
        }

        private void genGridTypeCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!genGridTypeCombobox.IsLoaded)
                return;

            if (genGridTypeCombobox.SelectedIndex > 0)
            {
                _viewModel.UserControlGridVisibility = Visibility.Collapsed;
                _viewModel.UserGroups = _viewModel.GetUserGroupsWithPermissionNames();
            }
            else
                _viewModel.UserControlGridVisibility = Visibility.Visible;
        }

        private bool UpdateUserControlGridColumn()
        {
            userPropertyView.CommitEditing();
            var curColumnIndex = userPropertyView.DataControl.CurrentColumn;

            switch (curColumnIndex.VisibleIndex)
            {
                case 2:
                    userPropertyView.DataControl.CurrentColumn = userControlGrid.Columns["UserName"];
                    break;
                case 3:
                    userPropertyView.DataControl.CurrentColumn = userControlGrid.Columns["FirstName"];
                    break;
                case 4:
                    userPropertyView.DataControl.CurrentColumn = userControlGrid.Columns["LastName"];
                    break;
                default:
                    return false;
            }

            return true;
        }

        private void userControlGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && userPropertyView.ActiveEditor != null)
            {
                var selectedUserGridModel = (UserGridModel)userControlGrid.GetFocusedRow();

                if (selectedUserGridModel.id == 0 || !UpdateUserControlGridColumn())
                    return;

                object firstName = userControlGrid.GetFocusedRowCellValue("FirstName");
                object lastName = userControlGrid.GetFocusedRowCellValue("LastName");
                object userName = userControlGrid.GetFocusedRowCellValue("UserName");

                selectedUserGridModel.FirstName = firstName != null ? firstName.ToString() : "";
                selectedUserGridModel.LastName = lastName != null ? lastName.ToString() : "";
                selectedUserGridModel.UserName = userName != null ? userName.ToString() : "";

                var result = _viewModel.UpdateUser(selectedUserGridModel);

                if (result)
                    DbResultPositiveFadeOutAnim.Begin();
                else
                    DbResultNegativeFadeOutAnim.Begin();
            }
        }
    }
}
