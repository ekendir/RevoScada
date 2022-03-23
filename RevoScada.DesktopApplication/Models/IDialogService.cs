using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RevoScada.DesktopApplication.Models
{
    interface IDialogService
    {
        MessageBoxResult WinUIMessageBoxShowResult(string message, string title, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage);
    }
}
