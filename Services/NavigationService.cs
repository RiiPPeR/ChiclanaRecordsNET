﻿using ChiclanaRecordsNET.Core;
using ChiclanaRecordsNET.MVVM.ViewModel;


public interface INavigationService
{
    ViewModel CurrentView { get; }
    void NavigateTo<TViewModel>() where TViewModel : ViewModel;
}

namespace ChiclanaRecordsNET.Services
{
    public class NavigationService : ObservableObject, INavigationService
    {
        private ViewModel _currentView;
        private readonly Func<Type, ViewModel> _viewModelFactory;

        public ViewModel CurrentView
        {
            get => _currentView;
            private set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public NavigationService(Func<Type, ViewModel> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public void NavigateTo<TViewModel>() where TViewModel : ViewModel
        {
            var viewModel = _viewModelFactory.Invoke(typeof(TViewModel));
            CurrentView = viewModel;
        }
    }
}