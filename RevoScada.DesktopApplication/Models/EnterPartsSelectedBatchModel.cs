using RevoScada.DesktopApplication.Helpers;
using RevoScada.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RevoScada.DesktopApplication.Models
{
    public class EnterPartsSelectedBatchModel : ObservableObject
    {
        private Batch _selectedBatch;
        public Batch SelectedBatch
        {
            get
            {
                return _selectedBatch;
            }
            set
            {
                OnPropertyChanged(ref _selectedBatch, value);
            }
        }

        private bool _showPortsInDisabledMode;
        public bool ShowPortsInDisabledMode
        {
            get
            {
                return _showPortsInDisabledMode;
            }
            set
            {
                OnPropertyChanged(ref _showPortsInDisabledMode, value);
            }
        }

        private EnterPartsBagDetail _selectedBag;
        public EnterPartsBagDetail SelectedBag
        {
            get
            {
                return _selectedBag;
            }
            set
            {
                OnPropertyChanged(ref _selectedBag, value);
            }
        }



        private ObservableCollection<EnterPartsBagDetail> _enterPartsBagDetails;
        public ObservableCollection<EnterPartsBagDetail> EnterPartsBagDetails
        {
            get
            {
                return _enterPartsBagDetails;
            }
            set
            {
                OnPropertyChanged(ref _enterPartsBagDetails, value);
            }
        }

        private ObservableCollection<LotProperty> _selectedLotProperties;

        public ObservableCollection<LotProperty> SelectedLotProperties
        {
            get
            {
                return _selectedLotProperties;
            }
            set
            {
                OnPropertyChanged(ref _selectedLotProperties, value);
            }
        }

        private ObservableCollection<EnterPartsPortDetail> _selectedPortListPTC;
        public ObservableCollection<EnterPartsPortDetail> SelectedPortListPTC
        {
            get
            {
                return _selectedPortListPTC;
            }
            set
            {
                PtcListCountInParametricFormat = $"{PtcLanguageValue} ({value.Count})";
                OnPropertyChanged(ref _selectedPortListPTC, value);
            }
        }

        private ObservableCollection<EnterPartsPortDetail> _selectedPortListMON;
        public ObservableCollection<EnterPartsPortDetail> SelectedPortListMON
        {
            get
            {
                return _selectedPortListMON;
            }
            set
            {
                MonListCountInParametricFormat = $"{MonLanguageValue} ({value.Count})";
                OnPropertyChanged(ref _selectedPortListMON, value);
            }
        }

        private ObservableCollection<EnterPartsPortDetail> _selectedPortListVAC;
        public ObservableCollection<EnterPartsPortDetail> SelectedPortListVAC
        {
            get
            {
                return _selectedPortListVAC;
            }
            set
            {
                VacListCountInParametricFormat = $"{VacLanguageValue} ({value.Count})";
                OnPropertyChanged(ref _selectedPortListVAC, value);
            }
        }

        public string PtcLanguageValue { get; set; }

        private string _ptcListCountInParametricFormat;
        public string PtcListCountInParametricFormat
        {
            get
            {
                return _ptcListCountInParametricFormat;
            }
            set
            {
                OnPropertyChanged(ref _ptcListCountInParametricFormat, value);
            }
        }

        public string MonLanguageValue { get; set; }

        private string _monListCountInParametricFormat;
        public string MonListCountInParametricFormat
        {
            get
            {
                return _monListCountInParametricFormat;
            }
            set
            {
                OnPropertyChanged(ref _monListCountInParametricFormat, value);
            }
        }

        public string VacLanguageValue { get; set; }

        private string _vacListCountInParametricFormat;
        public string VacListCountInParametricFormat
        {
            get
            {
                return _vacListCountInParametricFormat;
            }
            set
            {
                OnPropertyChanged(ref _vacListCountInParametricFormat, value);
            }
        }

        private bool _portListIsEnabled;
        public  bool PortListIsEnabled
        {
            get
            {
                return _portListIsEnabled;
            }
            set
            {
                OnPropertyChanged(ref _portListIsEnabled, value);
            }
        }
        private bool  _portSelectionIsEnabled;
        public bool  PortSelectionIsEnabled
        {
            get
            {
                return _portSelectionIsEnabled;
            }
            set
            {
                OnPropertyChanged(ref _portSelectionIsEnabled, value);
            }
        }

    }
}
