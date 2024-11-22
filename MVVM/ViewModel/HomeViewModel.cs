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

        public HomeViewModel(SessionViewModel sessionVM)
        {
            SessionVM = sessionVM;
        }
    }
}
