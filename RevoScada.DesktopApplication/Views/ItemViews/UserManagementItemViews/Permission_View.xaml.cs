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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RevoScada.DesktopApplication.Views.ItemViews.UserManagementItemViews
{
    /// <summary>
    /// Interaction logic for Permission_View.xaml
    /// </summary>
    public partial class Permission_View : UserControl
    {
        #region Fields
        private UserManagementVM _viewModel;
        public Storyboard DbResultPositiveFadeOutAnim;
        public Storyboard DbResultNegativeFadeOutAnim;
        #endregion

        public Permission_View()
        {
            InitializeComponent();

            DbResultPositiveFadeOutAnim = Resources["dbResultPositiveFadeOutStoryBoard"] as Storyboard;
            DbResultNegativeFadeOutAnim = Resources["dbResultNegativeFadeOutStoryBoard"] as Storyboard;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as UserManagementVM;
        }
        private void userGroupCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            if (comboBox.SelectedIndex > -1)
            {
                _viewModel.AssignedUsersVisibility = Visibility.Visible;
                _viewModel.IsSelectAllPermissionsCbEnabled = true;
            }
            else
            {
                _viewModel.AssignedUsers.Clear();
                _viewModel.PermissionGridData.Clear();
                _viewModel.IsSelectAllPermissionsCbEnabled = false;
                _viewModel.AssignedUsersVisibility = Visibility.Collapsed;
            }

            if ((comboBox.SelectedValue == null) || string.IsNullOrEmpty(comboBox.Text))
            {
                if(comboBox.HasItems)
                    comboBox.SelectedIndex = 0;
            }
        }

        private void PermissionCbOnClick(CheckBox checkBox)
        {
            if (!checkBox.IsLoaded)
                return;

            if (checkBox.Tag != null)
            {
                object[] tagVals = (object[])checkBox.Tag;
                int permissionId = (int)tagVals[0];
                short permissionGroupId = (short)tagVals[1];
                string pageName = (string)tagVals[2];

                if (permissionGroupId == 1)
                    _viewModel.ChangeSubPermissionCheckboxStates(pageName, checkBox.IsChecked ?? false);

                var result = _viewModel.UpdateSelectedGroupPermissions(permissionId, checkBox.IsChecked ?? false);

                if (!result)
                    DbResultNegativeFadeOutAnim.Begin();

                _viewModel.UpdateSelectAllCbState();
            }
        }

        private void permissionCb_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            PermissionCbOnClick(checkBox);
        }

        private void permissionCb_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            PermissionCbOnClick(checkBox);
        }
    }
}