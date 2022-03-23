using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class RecipeInfo : ObservableObject
    {
        private string _recipeName;
        public string RecipeName
        {
            get => _recipeName;
            set => OnPropertyChanged(ref _recipeName, value);
        }
        private string _specification;
        public string Specification
        {
            get => _specification;
            set => OnPropertyChanged(ref _specification, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => OnPropertyChanged(ref _description, value);
        }

        private string _createdByUser;
        public string CreatedByUser
        {
            get => _createdByUser;
            set => OnPropertyChanged(ref _createdByUser, value);
        }

        private DateTime? _revisedDate;
        public DateTime? RevisedDate
        {
            get => _revisedDate;
            set => OnPropertyChanged(ref _revisedDate, value);
        }
        private DateTime? _modifiedDate;
        public DateTime? ModifiedDate
        {
            get => _modifiedDate;
            set => OnPropertyChanged(ref _modifiedDate, value);
        }
        private DateTime? _lastRunDate;
        public DateTime? LastRunDate
        {
            get => _lastRunDate;
            set => OnPropertyChanged(ref _lastRunDate, value);
        }
    }
}
