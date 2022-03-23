using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class AlarmLogGridModel : ObservableObject
    {
        public int Id { get; set; }

        public string AlarmKey { get; set; }

        public string AlarmName { get; set; }
        private string _status;
        public string Status
        {
            get => _status;
            set => OnPropertyChanged(ref _status, value);
        }
        public DateTime? InDateTime { get; set; }
        public DateTime? OutDateTime { get; set; }
        public DateTime? AcknowledgedDateTime { get; set; }
        public string OffsetNo { get; set; }
        public string AlarmType { get; set; }

        private bool _isSelected;
        public bool IsSelected 
        {
            get => _isSelected;
            set => OnPropertyChanged(ref _isSelected, value);
        }
        public string Source { get; set; }
        private bool _hasWarned;
        public bool HasWarned
        {
            get => _hasWarned;
            set => OnPropertyChanged(ref _hasWarned, value);
        }
    }
}
