using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RevoScada.DesktopApplication.Helpers
{
    public static class WindowsExtensions
    {
        public static bool IsWindowsOpen<T>()
        {
            return Application.Current.Windows.OfType<T>().ToList().Count > 0;
        }
    }
}
