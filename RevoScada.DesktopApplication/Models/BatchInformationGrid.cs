using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class BatchInformationGrid
    {
        public string BatchName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Equipment { get; set; }
        private string _recipeName;
        public string RecipeName
        {
            get => _recipeName;
            set
            {
                _recipeName = value;
                AbbreviatedRecipeName = value;
            }
        }

        private string _fullRecipeName;
        public string FullRecipeName
        {
            get => _fullRecipeName;
            set
            {
                _fullRecipeName = value;
            }
        }
        private string _abbreviatedRecipeName;
        public string AbbreviatedRecipeName 
        {
            get => _abbreviatedRecipeName;
            set
            {
                _abbreviatedRecipeName = value;

                if (_abbreviatedRecipeName?.Length > 33)
                {
                    var fixedVal = _abbreviatedRecipeName.Substring(0, 30) + "...";
                    _abbreviatedRecipeName = fixedVal;
                    FullRecipeName = value;
                } else
                {
                    FullRecipeName = string.Empty;
                }
            }
        }
    }
}
