using ChiclanaRecordsNET.Core;
using ChiclanaRecordsNET.MVVM.Model;
using System.ComponentModel;
using System.Security;
using System.Windows.Input;

namespace ChiclanaRecordsNET.MVVM.ViewModel
{
    public class Session 
    {
        private static User _user;
        public static User User
        {
            get => _user;
            set
            {
                _user = value;
                System.Diagnostics.Debug.WriteLine($"Session.User changed - Email: {value?.Email}");
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(User)));
            }
        }

        public static event PropertyChangedEventHandler StaticPropertyChanged;
        public event PropertyChangedEventHandler PropertyChanged;
    }
    public class LoginViewModel : Core.ViewModel
    {
        private string _username;
        private SecureString _password;
        private string _errorMessage;
        private bool _isViewVisible = true;
        private bool _isLoading;

        public event Action RequestNavigation;
        private INavigationService _navigation;

        public INavigationService Navigation
        {
            get => _navigation;
            set
            {
                _navigation = value;
                OnPropertyChanged(nameof(Navigation));
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public SecureString Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public bool IsViewVisible
        {
            get => _isViewVisible;
            set
            {
                _isViewVisible = value;
                OnPropertyChanged(nameof(IsViewVisible));
            }
        }


        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand LoginCommand { get; }
        public RelayCommand NavigateToCreateUser { get; }

        private readonly Database _db;

        public LoginViewModel(INavigationService navService)
        {
            _db = new Database();
            Navigation = navService;

            LoginCommand = new RelayCommand(ExecuteLoginCommand, CanExecuteLoginCommand);
            NavigateToCreateUser = new RelayCommand(o => { Navigation.NavigateTo<CreateUserViewModel>(); }, o => true);
        }

        private bool CanExecuteLoginCommand(object obj)
        {
            return !string.IsNullOrWhiteSpace(Username)
                && Username.Length >= 3
                && Password != null
                && Password.Length >= 3
                && !IsLoading;
        }

        private async void ExecuteLoginCommand(object obj)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var (user, error) = await _db.LogIn(Username, Password);

                if (user != null)
                {
                    Session.User = user;
                    System.Diagnostics.Debug.WriteLine($"User logged in: {user.Username}, Email: {user.Email}");
                    IsViewVisible = false;  
                    Navigation.NavigateTo<HomeViewModel>();
                    
                }
                else
                {
                    if (error.Contains("400"))
                    {
                        error = "Contraseña incorrecta";
                    }
                    ErrorMessage = error ?? "Fallo de inicio de sesion";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error desconocido";
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                if (Password != null)
                {
                    Password.Clear(); 
                }
            }
        }
    }
}