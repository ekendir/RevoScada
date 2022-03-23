using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    [Serializable]
    public class EnterPartsUIElementStates : ObservableObject
    {

        private string _layoutGroupPreparePartsVisibility;
        public string LayoutGroupPreparePartsVisibility
        {
            get
            {
                return _layoutGroupPreparePartsVisibility;
            }
            set
            {

                OnPropertyChanged(ref _layoutGroupPreparePartsVisibility, value);
            }
        }

        private string _layoutGroupCompletedBatchesVisibility;
        public string LayoutGroupCompletedBatchesVisibility
        {
            get
            {
                return _layoutGroupCompletedBatchesVisibility;
            }
            set
            {

                OnPropertyChanged(ref _layoutGroupCompletedBatchesVisibility, value);
            }
        }

        private string _layoutGroupInitialCommandsVisibility;
        public string LayoutGroupInitialCommandsVisibility
        {
            get
            {
                return _layoutGroupInitialCommandsVisibility;
            }
            set
            {

                OnPropertyChanged(ref _layoutGroupInitialCommandsVisibility, value);
            }
        }


        private string _layoutGroupBagsVisibility;
        public string LayoutGroupBagsVisibility
        {
            get
            {
                return _layoutGroupBagsVisibility;
            }
            set
            {

                OnPropertyChanged(ref _layoutGroupBagsVisibility, value);
            }
        }

        private bool _skipPartsButtonIsEnabled;
        public bool SkipPartsButtonIsEnabled
        {
            get
            {
                return _skipPartsButtonIsEnabled;
            }
            set
            {

                OnPropertyChanged(ref _skipPartsButtonIsEnabled, value);
            }
        }

        private string _currentBatchDescription;
        public string CurrentBatchDescription
        {
            get
            {
                return _currentBatchDescription;
            }
            set
            {

                OnPropertyChanged(ref _currentBatchDescription, value);
            }
        }


        private bool _unloadCurrentIsEnabled;
        public bool UnloadCurrentIsEnabled
        {
            get
            {
                return _unloadCurrentIsEnabled;
            }
            set
            {

                OnPropertyChanged(ref _unloadCurrentIsEnabled, value);
            }
        }



        private bool _barbtnAddBagVisibility;
        public bool BarbtnAddBagVisibility
        {
            get
            {
                return _barbtnAddBagVisibility;
            }
            set
            {

                OnPropertyChanged(ref _barbtnAddBagVisibility, value);
            }
        }

        //private bool _barbtnRemoveBagVisibility;
        //public bool BarbtnRemoveBagVisibility
        //{
        //    get
        //    {
        //        return _barbtnRemoveBagVisibility;
        //    }
        //    set
        //    {

        //        OnPropertyChanged(ref _barbtnRemoveBagVisibility, value);
        //    }
        //}

        private bool _lotPropertyDeleteButtonIsEnabled;
        public bool LotPropertyDeleteButtonIsEnabled
        {
            get
            {
                return _lotPropertyDeleteButtonIsEnabled;
            }
            set
            {
                OnPropertyChanged(ref _lotPropertyDeleteButtonIsEnabled, value);
            }
        }
    }
}
