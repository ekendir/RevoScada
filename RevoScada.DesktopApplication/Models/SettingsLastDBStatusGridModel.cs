using RevoScada.Entities.Complex.Alarm;
using System;
 
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RevoScada.DesktopApplication.Models
{

    public class SettingsLastDBStatusGridModel :ObservableObject
    {
        private LastDBStatus _lastDBStatus;
        public LastDBStatus LastDBStatus
        {
            get => _lastDBStatus;
            set => OnPropertyChanged(ref _lastDBStatus, value);
        }

        private ImageSource _image;
        public ImageSource Image
        {
            get
            {

                int diffInSeconds = Convert.ToInt32((DateTime.Now - LastDBStatus.LastUpdate).TotalMilliseconds / 1000 );

                switch (diffInSeconds)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        _image = GetImage("/Resources/settings_success.png");
                        break;
                    
                   
                    case 5:
                    case 6:
                        _image = GetImage("/Resources/settings_moderate.png");
                        break;
                    case int n when (n > 7):
                        _image = GetImage("/Resources/settings_error.png");
                        break;
                }
            
               

                return _image;
            }
        }

        ImageSource GetImage(string path)
        {

            //  return Properties.Resources.Double_Down_16px as  ImageSource;
            return new BitmapImage(new Uri(path, UriKind.Relative));
        }

    }

}
