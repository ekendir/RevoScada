using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace RevoScada.DesktopApplication.Helpers
{
    public class MenuButtonBehavior
    {
        public static readonly DependencyProperty PageTagNameProperty =
            DependencyProperty.RegisterAttached("PageTagName", typeof(string), typeof(MenuButtonBehavior),
                                                 new FrameworkPropertyMetadata(null));

        public static string GetPageTagName(DependencyObject d)
        {
            return (string)d.GetValue(PageTagNameProperty);
        }

        public static void SetPageTagName(DependencyObject d, string value)
        {
            d.SetValue(PageTagNameProperty, value);
        }
    }
}
