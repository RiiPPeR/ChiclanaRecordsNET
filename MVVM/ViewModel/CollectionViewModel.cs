using System.Windows.Forms;
using Caliburn.Micro;
using ChiclanaRecordsNET.MVVM.Model;

namespace ChiclanaRecordsNET.MVVM.ViewModel
{
    public class CollectionViewModel : Core.ViewModel
    {
        private readonly SessionViewModel SessionVM;
        public BindableCollection<Album> Responses { get; set; }

        public CollectionViewModel(SessionViewModel sessionVM)
        {
            SessionVM = sessionVM;
            Responses = new BindableCollection<Album>();
            InitializeAsync();
        }

        public async void InitializeAsync()
        {
            DiscogsClient discogsDAO = new DiscogsClient();
            
            foreach (var id in SessionVM.CurrentUser.Records)
            {
                var (result, error) = await discogsDAO.GetReleaseAsync(id);

                if (result != null)
                {
                    Responses.Add(result);
                }
                else
                {
                    MessageBox.Show(error);
                }
            }
        }
    }
}