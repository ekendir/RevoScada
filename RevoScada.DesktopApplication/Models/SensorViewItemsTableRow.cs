using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace RevoScada.DesktopApplication.Models
{
    public class SensorViewItemsTableRow : ObservableObject
    {
        private float _portValue;
        private float _rateValue;
        private int _bagNameNumber;
        private int _enableDisableCommand;
        private Visibility _enableDisableToggleViewVisibility;
        private string _enableDisableToggleStatus;
        private SolidColorBrush _rowColor;
        public int PortNumeric { get { return Convert.ToInt32(PortName.TrimStart('P', 'T', 'C', 'M', 'O', 'N')); } }
        public string PortType { get { return PortName.Remove(3, PortName.Length - 3); } }
        public string PortName { get; set; }
        public float PortValue
        {
            get => _portValue;
            set => OnPropertyChanged(ref _portValue, value);
        }
        public float RateValue
        {
            get => _rateValue;
            set => OnPropertyChanged(ref _rateValue, value);
        }
        public string BagName { get; set; }
        public int BagNameNumber
        {
            get
            {
                if (BagName != "-")
                {
                    _bagNameNumber = Convert.ToInt32(BagName.Split('-')[2].Replace("Bag", ""));
                }
                else
                {
                    _bagNameNumber = 9999;
                }
                return _bagNameNumber;
            }
            set
            {
                _bagNameNumber = value;
            }
        }


        /// <summary>
        // "0": pasif (seçili olmayan portlar)
        // "1": enable(enter parts tarafında seçilileri temsil eder)
        // "2": disable(Operator tarafından disable edilen portlar)
        // "3": otodisable(Osilasyona takılan portlar)
        // "4": operator enable(Operator tarafından enable edilen portlar)
        /// </summary>
        public int EnableDisableCommand
        {
            get => _enableDisableCommand;
            set => OnPropertyChanged(ref _enableDisableCommand, value);
        }

        /// <summary>
        // "0": Transparent 
        // "1": Yeşil 
        // "2": Gri 
        // "3": Kırmızı 
        // "4": Koyu Yeşil
        /// </summary>
        public SolidColorBrush RowColor
        {
            get => _rowColor;
            set => OnPropertyChanged(ref _rowColor, value);
        }
        
        public Visibility EnableDisableToggleViewVisibility
        {
            get => _enableDisableToggleViewVisibility;
            set => OnPropertyChanged(ref _enableDisableToggleViewVisibility, value);
        }
        public string EnableDisableStatus
        {
            get => _enableDisableToggleStatus;
            set => OnPropertyChanged(ref _enableDisableToggleStatus, value);
        }

        private string _enableDisableButtonText;
        public string EnableDisableButtonText
        {
            get => _enableDisableButtonText;
            set => OnPropertyChanged(ref _enableDisableButtonText, value);
        }

    }
}