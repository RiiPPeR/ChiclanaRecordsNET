using ChiclanaRecordsNET.MVVM.Model;


namespace ChiclanaRecordsNET.MVVM.ViewModel
{
    class RecordViewModel : Core.ViewModel
    {
        private Album _release;

        public int Id { get; private set; }

        public Album Release 
        { 
            get => _release; 
            private set
            {
                _release = value;
                OnPropertyChanged();
            }
        }

        public override void Initialize(object parameter)
        {
            if (parameter is int idValue)
            {
                Id = idValue;
                InitializeAsync(); 
            }
        }

        public RecordViewModel()
        {
            //InitializeAsync();
        }

        public async void InitializeAsync()
        {
            DiscogsClient recordDAO = new DiscogsClient();
            System.Diagnostics.Debug.WriteLine($"Selected Record - Id: '{Id}'");

            var results = await recordDAO.GetReleaseAsync(Id);

            Release = results;

            System.Diagnostics.Debug.WriteLine($"Release Title: {Release?.title}");
            
            foreach(Image imagen in Release.images)
            {
                System.Diagnostics.Debug.WriteLine($"Release image: {imagen.uri}");
            }

        }
    }
}