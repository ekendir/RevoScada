using RevoScada.Entities;
using System.Collections.ObjectModel;

namespace RevoScada.DesktopApplication.Models
{
    public class EnterPartsBagDetail : ObservableObject
    {
        private int _bagId;
        public int BagId
        {
            get
            {
                return _bagId;
            }
            set
            {
                OnPropertyChanged(ref _bagId, value);
            }
        }
        private int _bagNumeric;
        public int BagNumeric
        {
            get
            {
                return _bagNumeric;
            }
            set
            {
                OnPropertyChanged(ref _bagNumeric, value);
            }
        }

        private int _batchId;
        public int BatchId
        {
            get
            {
                return _batchId;
            }
            set
            {
                OnPropertyChanged(ref _batchId, value);
            }
        }
        private string _bagName;
        public string BagName
        {
            get
            {
                return _bagName;
            }
            set
            {
                OnPropertyChanged(ref _bagName, value);
            }
        }
        private string _loadNumber;
        public string LoadNumber
        {
            get
            {
                return _loadNumber;
            }
            set
            {
                OnPropertyChanged(ref _loadNumber, value);
            }
        }


        private ObservableCollection<EnterPartsPortDetail> _enterPartsPortDetails;
        public ObservableCollection<EnterPartsPortDetail> EnterPartsPortDetails
        {
            get
            {
                return _enterPartsPortDetails;
            }
            set
            {
                OnPropertyChanged(ref _enterPartsPortDetails, value);
            }
        }

        private ObservableCollection<LotProperty> _lotPropertiesData;
        public ObservableCollection<LotProperty> LotPropertiesData
        {
            get
            {
                return _lotPropertiesData;
            }
            set
            {
                OnPropertyChanged(ref _lotPropertiesData, value);
            }
        }


    }

 
}
