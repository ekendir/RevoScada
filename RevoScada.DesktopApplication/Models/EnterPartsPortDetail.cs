using RevoScada.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class EnterPartsPortDetail : ObservableObject
    {
        private int _selectedPortTagId;
        public int SelectedPortTagId
        {
            get
            {
                return _selectedPortTagId;
            }
            set
            {
                OnPropertyChanged(ref _selectedPortTagId, value);
            }
        }

        private int _selectedPortNumeric;
        public int SelectedPortNumeric
        {
            get
            {
                return _selectedPortNumeric;
            }
            set
            {
                OnPropertyChanged(ref _selectedPortNumeric, value);
            }
        }

        private string _selectedPortLiteral;
        public string SelectedPortLiteral
        {
            get
            {
                return _selectedPortLiteral;
            }
            set
            {
                OnPropertyChanged(ref _selectedPortLiteral, value);
            }
        }


        private   ActiveTagGroups _selectedPortType;
        public ActiveTagGroups ActiveTagGroup
        {
            get
            {
                return _selectedPortType;
            }
            set
            {
                OnPropertyChanged(ref _selectedPortType, value);
            }
        }




        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                OnPropertyChanged(ref _isSelected, value);
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                OnPropertyChanged(ref _isEnabled, value);
            }
        }

    }
}
