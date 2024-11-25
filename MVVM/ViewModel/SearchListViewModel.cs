using Caliburn.Micro;
using ChiclanaRecordsNET.Core;
using ChiclanaRecordsNET.MVVM.Model;
using System.Windows.Input;


namespace ChiclanaRecordsNET.MVVM.ViewModel
{
    public class SearchListViewModel : Core.ViewModel
    {
        private string _artist;
        private string _title;
        private string _country;
        private string _track;
        public string Artist
        {
            get => _artist;
            set
            {
                _artist = value;
                OnPropertyChanged();
            }
        }
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public string Country
        {
            get => _country;
            set
            {
                _country = value;
                OnPropertyChanged();
            }
        }

        public string Track
        {
            get => _track;
            set
            {
                _track = value;
                OnPropertyChanged();
            }
        }

        public BindableCollection<SearchResult> Responses { get; set; }
        public ICommand SearchCommand { get; }
        public ICommand RecordClicked { get; }

        private INavigationService _navigation;

        public SessionViewModel SessionVM { get; }

        public INavigationService Navigation
        {
            get => _navigation;
            set
            {
                _navigation = value;
                OnPropertyChanged();
            }
        }

        public SearchListViewModel(INavigationService navService, SessionViewModel sessionVM)
        {
            SessionVM = sessionVM;
            Navigation = navService;
            Responses = new BindableCollection<SearchResult>();

            SearchCommand = new RelayCommand(o =>
                {
                    System.Diagnostics.Debug.WriteLine($"Artista: '{Artist}' Titulo: '{Title}' Pais: '{Country}' Cancion: '{Track}'");
                    InitializeAsync(Artist, Title, Country, Track);
                }, o => true);

            RecordClicked = new RelayCommand( OnRecordButtonClicked, o => true);
        }
        public async void InitializeAsync(string artist, string title, string country, string track)
        {
            DiscogsSearch search = new DiscogsSearch();
            var (results, error) = await search.GetSearchAsync(artist, title, country, track);

            if (results?.Results != null && results.Results.Count > 0)
            {
                Responses.Clear();
                //Responses.AddRange(results.Results);

                foreach (var result in results.Results)
                {
                    result.IsInUserList = SessionVM.CurrentUser.Records.Contains(result.Id) ? true : false;
                    Responses.Add(result);
                }

                System.Diagnostics.Debug.WriteLine($"{Responses.Count} resultados.");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No se ha encontrado nada.");
            }
        }

        private void OnRecordButtonClicked(object parameter)
        {
            if (parameter is SearchResult selectedRecord)
            {
                System.Diagnostics.Debug.WriteLine($"Record - ID: '{selectedRecord.Id}'");

                Navigation.NavigateTo<RecordViewModel>(selectedRecord.Id);
            }
        }
    }
}
