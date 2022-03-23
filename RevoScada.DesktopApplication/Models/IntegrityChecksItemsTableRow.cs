using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class IntegrityChecksItemsTableRow : ObservableObject
    {
        private string _actualName;
        public string ActualName 
        {
            get => _actualName;
            set => OnPropertyChanged(ref _actualName, value);
        }
        private float _actual;
        public float Actual
        {
            get => _actual;
            set => OnPropertyChanged(ref _actual, value);
        }
        private float _start;
        public float Start
        {
            get => _start;
            set => OnPropertyChanged(ref _start, value);
        }
        private float _finish;
        public float Finish
        {
            get => _finish;
            set => OnPropertyChanged(ref _finish, value);
        }
        private float _deviation;
        public float Deviation
        {
            get => _deviation;
            set => OnPropertyChanged(ref _deviation, value);
        }
        public int BagId { get; set; }
        public int BatchId { get; set; }
        public bool IsItSelected { get; set; }
    }

}
