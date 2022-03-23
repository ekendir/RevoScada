using RevoScada.DesktopApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Helpers
{
    public class WaitIndicatorControl : ObservableObject
    {
        private bool _isWaitIndicatorVisible;
        public bool IsWaitIndicatorVisible
        {
            get { return _isWaitIndicatorVisible; }
            set
            {
                OnPropertyChanged(ref _isWaitIndicatorVisible, value);
            }
        }

        private bool _isWaitIndicatorTextActive;
        public bool IsWaitIndicatorTextActive
        {
            get { return _isWaitIndicatorTextActive; }
            set
            {
                OnPropertyChanged(ref _isWaitIndicatorTextActive, value);
            }
        }
    }
}
