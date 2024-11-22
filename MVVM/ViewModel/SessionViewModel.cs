using ChiclanaRecordsNET.MVVM.Model;
using System;
using System.Collections.Generic;
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
            Session.StaticPropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Session.User))
                {
                    CurrentUser = Session.User;
                    IsLoggedIn = true;
                }
            };
        }
    }
}