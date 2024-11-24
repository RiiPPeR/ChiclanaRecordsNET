using ChiclanaRecordsNET.Core;
using ChiclanaRecordsNET.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChiclanaRecordsNET.MVVM.ViewModel
{
    internal class CreateUserViewModel : Core.ViewModel
    {
        private string _username;
        private string _email;
        private SecureString _password;
        private SecureString _repeatPassword;
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

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
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

        public SecureString RepeatPassword
        {
            get => _repeatPassword;
            set
            {
                _repeatPassword = value;
                OnPropertyChanged(nameof(RepeatPassword));
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

        public ICommand CreateUserCommand { get; }
        public RelayCommand NavigateToLogin { get; }

        private readonly Database _db;

        public CreateUserViewModel(INavigationService navService)
        {
            _db = new Database();
            Navigation = navService;

            CreateUserCommand = new RelayCommand(ExecuteCreateUserCommand, CanExecuteCreateUserCommand);
            NavigateToLogin = new RelayCommand(o => { Navigation.NavigateTo<LoginViewModel>(); }, o => true);

        }

        private bool CanExecuteCreateUserCommand(object obj)
        {
            IntPtr unmanagedString = IntPtr.Zero;
            string? passwordString;
            string? repeatPasswordString;

            if (Password != null && RepeatPassword != null)
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(Password);
                passwordString = Marshal.PtrToStringUni(unmanagedString);

                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(RepeatPassword);
                repeatPasswordString = Marshal.PtrToStringUni(unmanagedString);

                return !string.IsNullOrWhiteSpace(Username)
                    && Username.Length >= 3
                    && Password != null
                    && Password.Length >= 3
                    && Email.Length >= 3
                    && passwordString == repeatPasswordString
                    && !IsLoading;
            }
            return false;
        }

        private async void ExecuteCreateUserCommand(object obj)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var (success, error) = await _db.CreateUser(Username, Email, Password);

                if (success)
                {
                    IsViewVisible = false;
                    Navigation.NavigateTo<LoginViewModel>();
                }
                else
                { 
                    if (error.Contains("weak_password"))
                    {
                        error = "Contraseña muy débil.";
                    } 
                    else if (error.Contains("username"))
                    {
                        error = "Ese usuario ya existe.";
                    }
                    else if (error.Contains("user_already_exists"))
                    {
                        error = "Ese correo ya está registrado.";
                    }
                    else if (error.Contains("invalid format"))
                    {
                        error = "El email no es válido.";
                    }

                    ErrorMessage = error ?? "Fallo de inicio de sesion";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error desconocido";
                System.Diagnostics.Debug.WriteLine($"Create user error: {ex.Message}");
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

