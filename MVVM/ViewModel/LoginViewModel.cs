using ChiclanaRecordsNET.Core;
using ChiclanaRecordsNET.MVVM.Model;
using System.Security;
using System.Windows.Input;

namespace ChiclanaRecordsNET.MVVM.ViewModel
{
    public class LoginViewModel : Core.ViewModel
    {
        private string _username;
        private SecureString _password;
        private string _errorMessage;
        private bool _isViewVisible = true;
        private bool _isLoading;

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
        private readonly Database _db;
        private User User { get; set; }

        public LoginViewModel()
        {
            _db = new Database();
            LoginCommand = new RelayCommand(ExecuteLoginCommand, CanExecuteLoginCommand);
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
                    User = user;
                    System.Diagnostics.Debug.WriteLine($"User logged in: {User.Username}, Email: {User.Email}");
                    IsViewVisible = false;  //se esconde
                                            //po despuesq
                }
                else
                {
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