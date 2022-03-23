using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Validation;
using Revo.Core.Data;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.ViewModels;
using RevoScada.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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


namespace RevoScada.DesktopApplication.Views
{
    /// <summary>
    /// Interaction logic for User_Management_View.xaml
    /// </summary>
    public partial class User_Management_View : UserControl
    {
        #region Fields
        private UserManagementVM _viewModel;
        public Storyboard DbResultPositiveFadeOutAnim;
        public Storyboard DbResultNegativeFadeOutAnim;
        #endregion

        public User_Management_View()
        {
            InitializeComponent();

            DbResultPositiveFadeOutAnim = Resources["dbResultPositiveFadeOutStoryBoard"] as Storyboard;
            DbResultNegativeFadeOutAnim = Resources["dbResultNegativeFadeOutStoryBoard"] as Storyboard;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as UserManagementVM;
            _viewModel.UserManagementView = this;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1,
                (Duration)TimeSpan.FromSeconds(1));
            this.BeginAnimation(UIElement.OpacityProperty, animation);
        }
    }
}