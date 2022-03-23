using DevExpress.Xpf.Docking.VisualElements;
using RevoScada.Entities.PageTagConfigurations.PageTagConfigEnum;


namespace RevoScada.DesktopApplication.Models
{
    public class RunOperationProcessStartStepsModel : ObservableObject
    {
        private float _enterPartOkOpacity;
        public float EnterPartOkOpacity
        {
            get => _enterPartOkOpacity;
            set => OnPropertyChanged(ref _enterPartOkOpacity, value);
        }

        private float _recipeOkOpacity;

        public float RecipeOkOpacity
        {
            get => _recipeOkOpacity;
            set => OnPropertyChanged(ref _recipeOkOpacity, value);
        }

        private float _integrityCheckOkOpacity;
        public float IntegrityCheckOkOpacity
        {
            get => _integrityCheckOkOpacity;
            set => OnPropertyChanged(ref _integrityCheckOkOpacity, value);
        }

        private float _doorStatusOpacity;
        public float DoorStatusOpacity
        {
            get => _doorStatusOpacity;
            set => OnPropertyChanged(ref _doorStatusOpacity, value);
        }

        private float _overAllOkOpacity;
        public float OverAllOkOpacity
        {
            get
            {
                return _overAllOkOpacity;
            }
            set
            {
                OnPropertyChanged(ref _overAllOkOpacity, value);
            }
        }

        private string _enterPartOkSummary;
        public string EnterPartOkSummary
        {
            get => _enterPartOkSummary;
            set => OnPropertyChanged(ref _enterPartOkSummary, value);
        }

        private string _fullRecipeOkSummary;
        public string FullRecipeOkSummary
        {
            get => _fullRecipeOkSummary;
            set => OnPropertyChanged(ref _fullRecipeOkSummary, value);
        }

        private string _abbreviatedRecipeOkSummary;
        public string AbbreviatedRecipeOkSummary
        {
            get => _abbreviatedRecipeOkSummary;
            set 
            {
                _abbreviatedRecipeOkSummary = value;
                if (_abbreviatedRecipeOkSummary?.Length > 60)
                {
                    var fixedVal = _abbreviatedRecipeOkSummary.Substring(0, 60) + "...";
                    _abbreviatedRecipeOkSummary = fixedVal;
                    FullRecipeOkSummary = value;
                }
                else
                {
                    FullRecipeOkSummary = string.Empty;
                }
                OnPropertyChanged(ref _abbreviatedRecipeOkSummary, _abbreviatedRecipeOkSummary);
            } 
        }

        private string _integrityCheckOkSummary;

        public string IntegrityCheckOkSummary
        {
            get => _integrityCheckOkSummary;
            set => OnPropertyChanged(ref _integrityCheckOkSummary, value);
        }

        private string _doorStatusSummary;

        public string DoorStatusSummary
        {
            get => _doorStatusSummary;
            set => OnPropertyChanged(ref _doorStatusSummary, value);
        }

        private string _enterPartOkHeader;
        public string EnterPartOkHeader
        {
            get => _enterPartOkHeader;
            set => OnPropertyChanged(ref _enterPartOkHeader, value);
        }

        private string _recipeOkHeader;

        public string RecipeOkHeader
        {
            get => _recipeOkHeader;
            set => OnPropertyChanged(ref _recipeOkHeader, value);
        }

        private string _integrityCheckOkHeader;

        public string IntegrityCheckOkHeader
        {
            get => _integrityCheckOkHeader;
            set => OnPropertyChanged(ref _integrityCheckOkHeader, value);
        }

        private string _doorStatusHeader;

        public string DoorStatusHeader
        {
            get => _doorStatusHeader;
            set => OnPropertyChanged(ref _doorStatusHeader, value);
        }
        
        private string _overAllOkHeader;
        public string OverAllOkHeader
        {
            get => _overAllOkHeader;
            set => OnPropertyChanged(ref _overAllOkHeader, value);
        }
    }
}