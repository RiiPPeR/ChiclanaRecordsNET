using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiclanaRecordsNET.MVVM.ViewModel
{
    class HomeViewModel : Core.ViewModel
    {
        public SessionViewModel SessionVM { get; }

        private string _fecha;
        public string Fecha
        {
            get => _fecha;
            set
            {
                _fecha = value;
                OnPropertyChanged(nameof(Fecha));
            }
        }

        public HomeViewModel(SessionViewModel sessionVM)
        {
            SessionVM = sessionVM;
            DateTime today = DateTime.Today;
            Fecha = today.ToString().Substring(0, 10);
        }
    }
}
