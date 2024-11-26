using System.Collections.ObjectModel;
using System.Windows.Forms;
using Caliburn.Micro;
using ChiclanaRecordsNET.Core;
using ChiclanaRecordsNET.MVVM.Model;

namespace ChiclanaRecordsNET.MVVM.ViewModel
{
    public class CollectionViewModel : Core.ViewModel
    {
        public INavigationService Navigation { get; }

        private readonly SessionViewModel SessionVM;

        public ObservableCollection<Record> _respones;
        public ObservableCollection<Record> Responses 
        {
            get => _respones; 
            set
            {
                _respones = value;
                OnPropertyChanged(nameof(Responses));
            }
        }

        public RelayCommand NavigateToRecord { get; }

        public CollectionViewModel(SessionViewModel sessionVM, INavigationService navigationService)
        {
            Navigation = navigationService;
            SessionVM = sessionVM;
            Responses = SessionVM.Records;

            NavigateToRecord = new RelayCommand(OnRecordButtonClicked, o => true);

            //InitializeAsync();
        }

        public async void InitializeAsync()
        {
            DiscogsClient discogsDAO = new DiscogsClient();

            foreach (var record in SessionVM.Records)
            {
                Responses.Add(record);
                OnPropertyChanged(nameof(Responses));
            }
        }

        private void OnRecordButtonClicked(object parameter)
        {
            if (parameter is Record selectedRecord)
            {
                System.Diagnostics.Debug.WriteLine($"Record - ID: '{selectedRecord.DiscogsId}'");

                Navigation.NavigateTo<RecordViewModel>(selectedRecord.DiscogsId);
            }
        }
    }
}