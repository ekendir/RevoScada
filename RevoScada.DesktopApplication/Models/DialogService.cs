using DevExpress.Xpf.WindowsUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RevoScada.DesktopApplication.Models
{
    public class DialogService : IDialogService
    {
        public MessageBoxResult WinUIMessageBoxShowResult(string message, string title, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage)
        {
            return WinUIMessageBox.Show(message, title, messageBoxButton, messageBoxImage);
        }

    }
}
