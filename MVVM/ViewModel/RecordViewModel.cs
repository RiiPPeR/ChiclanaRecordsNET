using ChiclanaRecordsNET.Core;
using ChiclanaRecordsNET.MVVM.Model;
using Supabase.Gotrue;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Windows;


namespace ChiclanaRecordsNET.MVVM.ViewModel
{
    class RecordViewModel : Core.ViewModel
    {
        private Album _release;
        private Boolean _isDeletable;
        private Boolean _isAddable;

        public int Id { get; private set; }

        public Album Release 
        { 
            get => _release; 
            private set
            {
                _release = value;
                OnPropertyChanged(nameof(Release));
            }
        }

        public Boolean IsDeletable
        {
            get => _isDeletable;
            set
            {
                _isDeletable = value;
                OnPropertyChanged(nameof(IsDeletable));
            }
        }

        public Boolean IsAddable
        {
            get => _isAddable;
            set
            {
                _isAddable = value;
                OnPropertyChanged(nameof(IsAddable));
            }
        }

        public SessionViewModel SessionVM { get; }
        public INavigationService Navigation { get; }
        public RelayCommand AddRecord { get; }
        public RelayCommand DeleteRecord { get; }

        public override void Initialize(object parameter)
        {
            if (parameter is int idValue)
            {
                Id = idValue;
                InitializeAsync();

                IsDeletable = false;
                IsAddable = true;

                foreach (var record in SessionVM.Records)
                {
                    if (record.DiscogsId == idValue)
                    {
                        IsDeletable = true;
                        IsAddable = false;
                        break;
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Se puede eliminar: {IsDeletable}");
            }
        }

        public RecordViewModel(SessionViewModel sessionVM, INavigationService navService)
        {
            SessionVM = sessionVM;
            Navigation = navService;

            AddRecord = new RelayCommand( o => { AddRecordF(); }, o => true);
            DeleteRecord = new RelayCommand( o => { DeleteRecordF(); }, o => true);
        }

        public async void AddRecordF()
        {
            if (Release != null)
            {
                Database query = new Database();
                var results = await query.AddRecordToCollection(
                    SessionVM.CurrentUser.Id,
                    Release.id,
                    Release.title,
                    Release.images[0].resource_url,
                    Release.country,
                    Release.year,
                    Release.labels[0].name,
                    Release.labels[0].catno);

                SessionVM.Records.Add(new Record
                {
                    DiscogsId = Release.id,
                    Title = Release.title,
                    ImageUrl = Release.images[0].resource_url,
                    Country = Release.country,
                    Year = Release.year,
                    Label = Release.labels[0].name,
                    CatalogNumber = Release.labels[0].catno
                });
                OnPropertyChanged(nameof(SessionVM.Records));

                Navigation.NavigateTo<RecordViewModel>(Release.id);
            }
        } 

        public async void DeleteRecordF()
        {
            Database query = new Database();
            var results = await query.DeleteRecordFromCollection(
                    SessionVM.CurrentUser.Id,
                    Release.id);

            foreach (var record in SessionVM.Records)
            {
                if (record.DiscogsId == Release.id)
                {
                    SessionVM.Records.Remove(record);
                    break;
                }
            }

            Navigation.NavigateTo<RecordViewModel>(Release.id);
        }

        public async void InitializeAsync()
        {
            DiscogsClient recordDAO = new DiscogsClient();
            System.Diagnostics.Debug.WriteLine($"Selected Record - Id: '{Id}'");

            var (results, error) = await recordDAO.GetReleaseAsync(Id);

            Release = results;

            if (results == null)
            {
                Navigation.NavigateTo<SearchListViewModel>();
                MessageBox.Show(error);
                return;
            }

            System.Diagnostics.Debug.WriteLine($"Release Title: {Release?.title}");
            
            foreach(Image imagen in Release.images)
            {
                System.Diagnostics.Debug.WriteLine($"Release image: {imagen.uri}");
            }

        }
    }
}