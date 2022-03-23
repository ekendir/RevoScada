using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class BatchQualityModel : ObservableObject
    {

        public int id { get; set; }

        private string _cardName;
        public string CardName
        {
            get => _cardName;
            set => OnPropertyChanged(ref _cardName, value);
        }
        public string Description { get; set; }
        private DateTime _lastModified;
        public DateTime LastModified
        {
            get => _lastModified;
            set => OnPropertyChanged(ref _lastModified, value);
        }
        public short SortOrder { get; set; }
    }
}
