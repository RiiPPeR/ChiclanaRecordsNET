using ChiclanaRecordsNET.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiclanaRecordsNET.MVVM.ViewModel
{
    public class SessionViewModel : Core.ViewModel
    {
        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
            }
        }

        //private List<Record> _records;

        //public List<Record> Records
        //{
        //    get => _records;
        //    set
        //    {
        //        _records = value;
        //        OnPropertyChanged(nameof(Records));
        //    }
        //}

        private ObservableCollection<Record> _records;
        public ObservableCollection<Record> Records
        {
            get => _records;
            set
            {
                _records = value;
                OnPropertyChanged(nameof(Records));
            }
        }

        private Boolean _isLoggedIn;
        public Boolean IsLoggedIn 
        {
            get => _isLoggedIn;
            set
            {
                _isLoggedIn = value;
                OnPropertyChanged(nameof(IsLoggedIn));
            }
        }

        public SessionViewModel()
        {
            CurrentUser = Session.User;
            //Records = Session.Records;
            Records = new ObservableCollection<Record>(Session.Records); 
            IsLoggedIn = Session.User != null;

            Session.StaticPropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(Session.User):
                        CurrentUser = Session.User;
                        IsLoggedIn = Session.User != null;
                        break;
                    case nameof(Session.Records):
                        //Records = Session.Records;
                        Records = new ObservableCollection<Record>(Session.Records);
                        break;
                }
            };
        }
    }
}