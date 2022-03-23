using DevExpress.Xpf.Docking.VisualElements;
using RevoScada.Entities.PageTagConfigurations.PageTagConfigEnum;
namespace RevoScada.DesktopApplication.Models
{
    public class EmergencyGridModel : ObservableObject
    {
        private string _commandId;
        public string CommandId
        {
            get => _commandId;
            set => OnPropertyChanged(ref _commandId, value);
        }

        private int _dbNumber;
        public int DbNumber
        {
            get => _dbNumber;
            set => OnPropertyChanged(ref _dbNumber, value);
        }

        private float _offset;
        public float Offset
        {
            get => _offset;
            set => OnPropertyChanged(ref _offset, value);
        }
        private string _description;
        public string Description
        {
            get => _description;
            set => OnPropertyChanged(ref _description, value);
        }
    }
}