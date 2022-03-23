using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RevoScada.DesktopApplication.Models
{
    public class FurnaceSelectionModel : ObservableObject
    {

        private int _furnaceId;
        public int PlcDeviceId
        {
            get => _furnaceId;
            set => OnPropertyChanged(ref _furnaceId, value);
        }

        private string _furnaceName;
        public string FurnaceName
        {
            get => _furnaceName;
            set => OnPropertyChanged(ref _furnaceName, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => OnPropertyChanged(ref _description, value);
        }

        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set => OnPropertyChanged(ref _imagePath, value);
        }

        private DateTime  _lastUpTime;
        public DateTime  LastUpTime
        {
            get => _lastUpTime;
            set => OnPropertyChanged(ref _lastUpTime, value);
        }

        private bool _runEnable;
        public bool RunEnable
        {
            get => _runEnable;
            set => OnPropertyChanged(ref _runEnable, value);
        }

        private bool _syncStatus;
        public bool SyncStatus
        {
            get => _syncStatus;
            set => OnPropertyChanged(ref _syncStatus, value);
        }

        private string _plcIpAddress;
        public string PlcIpAddress
        {
            get => _plcIpAddress;
            set => OnPropertyChanged(ref _plcIpAddress, value);
        }

        private DateTime _lastCycleRunTime;
        public DateTime LastCycleRunTime
        {
            get => _lastCycleRunTime;
            set => OnPropertyChanged(ref _lastCycleRunTime, value);
        }

        private DateTime _PLCLastAccessDateFromPC;
        public DateTime PLCLastAccessDateFromPC
        {
            get => _PLCLastAccessDateFromPC;
            set => OnPropertyChanged(ref _PLCLastAccessDateFromPC, value);
        }
        
        private DateTime _PLCLastAccessDateFromServer;
        public DateTime PLCLastAccessDateFromServer
        {
            get => _PLCLastAccessDateFromServer;
            set => OnPropertyChanged(ref _PLCLastAccessDateFromServer, value);
        }

        private string _OSUptime;
        public string OSUptime
        {
            get => _OSUptime;
            set => OnPropertyChanged(ref _OSUptime, value);
        }

        private ImageSource _image;
        public ImageSource Image
        {
            get
            {
                _image = GetImage(_imagePath);
                return _image;
            }
        }

        ImageSource GetImage(string path)
        {
            return new BitmapImage(new Uri(path, UriKind.Relative));
        }
    }
}
