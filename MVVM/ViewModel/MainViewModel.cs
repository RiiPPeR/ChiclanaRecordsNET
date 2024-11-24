using ChiclanaRecordsNET.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using ChiclanaRecordsNET.MVVM.Model;
using System.Windows.Media;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace ChiclanaRecordsNET.MVVM.ViewModel
{
    public class MainViewModel : Core.ViewModel
    {
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

        public RelayCommand NavigateToHomeCommand { get; set; }
        public RelayCommand NavigateToSettingsViewCommand { get; set; }
        public RelayCommand NavigateToLoginCommand { get; set; }
        public RelayCommand NavigateToSearchList { get; private set; }
        public RelayCommand NavigateToAcercaDe { get; private set; }
        public RelayCommand CloseCommand { get; private set; }

        public SessionViewModel SessionVM { get; }


        public MainViewModel(INavigationService navService, SessionViewModel sessionViewModel)
        {
            Navigation = navService;
            Navigation.NavigateTo<LoginViewModel>();

            NavigateToHomeCommand = new RelayCommand(o => { Navigation.NavigateTo<HomeViewModel>(); }, o => true);
            NavigateToSettingsViewCommand = new RelayCommand(o => { Navigation.NavigateTo<SettingsViewModel>(); }, o => true);
            NavigateToLoginCommand = new RelayCommand(o => { Navigation.NavigateTo<LoginViewModel>(); }, o => true);
            NavigateToSearchList = new RelayCommand(o => { Navigation.NavigateTo<SearchListViewModel>(); }, o => true);
            NavigateToAcercaDe = new RelayCommand(o => { Navigation.NavigateTo<AcercaDeViewModel>(); }, o => true);

            SessionVM = sessionViewModel;
        }
    }
}
