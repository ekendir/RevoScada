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

namespace RevoScada.DesktopApplication.Views
{
    /// <summary>
    /// Interaction logic for User_Control.xaml
    /// </summary>
    public partial class User_Control : UserControl
    {
        public User_Control()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1,
                            (Duration)TimeSpan.FromSeconds(1));

            this.BeginAnimation(UIElement.OpacityProperty, animation);
        }
    }
}
