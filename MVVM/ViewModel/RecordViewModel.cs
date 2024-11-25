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

                IsDeletable = SessionVM.CurrentUser.Records.Contains(idValue) ? true : false;
                IsAddable = SessionVM.CurrentUser.Records.Contains(idValue) ? false : true;
                System.Diagnostics.Debug.WriteLine($"Se puede eliminar: {IsDeletable}");
            }
        }

        public RecordViewModel(SessionViewModel sessionVM, INavigationService navService)
        {
            SessionVM = sessionVM;
            Navigation = navService;

            AddRecord = new RelayCommand( o => { AddRecordF(SessionVM.CurrentUser.Id, Id); }, o => true);
            DeleteRecord = new RelayCommand( o => { DeleteRecordF(SessionVM.CurrentUser.Id, Id); }, o => true);
        }

        public async void AddRecordF(Guid id, int record_id)
        {
            Database query = new Database();
            var results = await query.AddRecordToCollection(id, record_id);

            if (!SessionVM.CurrentUser.Records.Contains(record_id))
            {
                SessionVM.CurrentUser.Records.Add(record_id);
            }

            Navigation.NavigateTo<RecordViewModel>(record_id);
        } 

        public async void DeleteRecordF(Guid id, int record_id)
        {
            Database query = new Database();
            var results = await query.DeleteRecordFromCollection(id, record_id);

            if (SessionVM.CurrentUser.Records.Contains(record_id))
            {
                SessionVM.CurrentUser.Records.Remove(record_id);
            }

            Navigation.NavigateTo<RecordViewModel>(record_id);
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