using RevoScada.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class VacuumPortItem : ObservableObject
    {
        public string PortName { get; set; }
        private float _portValue;
        public float PortValue
        {
            get => _portValue;
            set => OnPropertyChanged(ref _portValue, value);
        }

        private VacVentOffState _vacVentOffState;
        public VacVentOffState VacVentOffState
        {
            get => _vacVentOffState;
            set => OnPropertyChanged(ref _vacVentOffState, value);
        }
        private ManuelAutoState _manuelAutoState;
        public ManuelAutoState ManuelAutoState
        {
            get => _manuelAutoState;
            set => OnPropertyChanged(ref _manuelAutoState, value);
        }
    }

}
