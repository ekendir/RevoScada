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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RevoScada.DesktopApplication.Views.ItemViews.UserManagementItemViews
{
    /// <summary>
    /// Interaction logic for Assignment_View.xaml
    /// </summary>
    public partial class Assignment_View : UserControl
    {
        #region Fields
        private UserManagementVM _viewModel;
        public Storyboard DbResultPositiveFadeOutAnim;
        public Storyboard DbResultNegativeFadeOutAnim;
        #endregion

        public Assignment_View()
        {
            InitializeComponent();

            DbResultPositiveFadeOutAnim = Resources["dbResultPositiveFadeOutStoryBoard"] as Storyboard;
            DbResultNegativeFadeOutAnim = Resources["dbResultNegativeFadeOutStoryBoard"] as Storyboard;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as UserManagementVM;
        }

        private bool CheckIfUserAndGroupSelectedToAssign()
        {
            if (usersCombobox.SelectedIndex > -1 && groupCombobox.SelectedIndex > -1)
                return true;
            else
                return false;
        }

        private void groupCombobox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.IsAssignToUserBtnEnabled = CheckIfUserAndGroupSelectedToAssign();
        }

        private void usersCombobox_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.IsAssignToUserBtnEnabled = CheckIfUserAndGroupSelectedToAssign();
        }

        private void assignBtn_Click(object sender, RoutedEventArgs e)
        {
            List<bool> results = new List<bool>();

            List<UserGridModel> userGridModelsCasted = new List<UserGridModel>();
            var userGridModels = usersCombobox.SelectedItems;

            foreach (var item in userGridModels)
            {
                userGridModelsCasted.Add((UserGridModel)item);
            }

            foreach (var item in userGridModelsCasted)
            {
                UserGroupGridModel userGroupGridModel = (UserGroupGridModel)groupCombobox.SelectedItem;
                var result = _viewModel.AssignGroupToUser(item, userGroupGridModel);
                results.Add(result);
            }

            if (results.TrueForAll(r => r))
            {
                DbResultPositiveFadeOutAnim.Begin();
                usersCombobox.SelectedIndex = -1;
                groupCombobox.SelectedIndex = -1;
            }
            else
                DbResultNegativeFadeOutAnim.Begin();
        }
    }
}
