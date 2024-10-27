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

        public BindableCollection<SearchResult> Responses { get; set; }
        public ICommand SearchCommand { get; }
        public ICommand RecordClicked { get; }

        private INavigationService _navigation;

        public INavigationService Navigation
        {
            get => _navigation;
            set
            {
                _navigation = value;
                OnPropertyChanged();
            }
        }

        public SearchListViewModel(INavigationService navService)
        {
            Navigation = navService;
            Responses = new BindableCollection<SearchResult>();
            SearchCommand = new RelayCommand(o =>
            {
                System.Diagnostics.Debug.WriteLine($"Artista: '{Artist}' Titulo: '{Title}'");
                InitializeAsync(Artist, Title);
            }, o => true);

            RecordClicked = new RelayCommand( OnRecordButtonClicked, o => true);
        }
        public async Task InitializeAsync(string artist, string title)
        {
            DiscogsSearch search = new DiscogsSearch();
            var results = await search.GetSearchAsync(artist, title);

            if (results?.Results != null && results.Results.Count > 0)
            {
                Responses.Clear();
                Responses.AddRange(results.Results);
                System.Diagnostics.Debug.WriteLine($"{Responses.Count} results added.");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No results found.");
            }
        }

        private void OnRecordButtonClicked(object parameter)
        {
            if (parameter is SearchResult selectedRecord)
            {
                System.Diagnostics.Debug.WriteLine($"Selected Record - ID: '{selectedRecord.Id}'");

                Navigation.NavigateTo<RecordViewModel>(selectedRecord.Id);
            }
        }

    }
}
