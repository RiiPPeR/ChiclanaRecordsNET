using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiclanaRecordsNET.MVVM.ViewModel
{
    class RecordViewModel : Core.ViewModel
    {
        public int id { get; private set; }

        public override void Initialize(object parameter)
        {
            if(parameter is int) id = (int)parameter;
        }
    }
}
