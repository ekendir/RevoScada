using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class PermissionGridModel : ObservableObject
    {
        public int id { get; set; }
        public string PageName { get; set; }
        public short PermissionGroup { get; set; }
        public string PermissionTag { get; set; }
        private bool _isSelected;
        public bool IsSelected 
        {
            get => _isSelected;
            set => OnPropertyChanged(ref _isSelected, value);
        }
        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => OnPropertyChanged(ref _isEnabled, value);
        }
        public object[] CheckboxTag { get; set; }
    }
}
