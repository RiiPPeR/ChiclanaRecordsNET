using ChiclanaRecordsNET.MVVM.ViewModel;
using Microsoft.Xaml.Behaviors;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChiclanaRecordsNET.MVVM.View
{
    /// <summary>
    /// Lógica de interacción para RecordView.xaml
    /// </summary>
    public partial class RecordView : UserControl
    {
        public RecordView()
        {
            InitializeComponent();
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            var image = sender as Image;
            var parent = image.Parent as Grid;
            var popup = parent.Children.OfType<Popup>().FirstOrDefault();
            if (popup != null)
            {
                popup.IsOpen = true;
            }
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            var image = sender as Image;
            var parent = image.Parent as Grid;
            var popup = parent.Children.OfType<Popup>().FirstOrDefault();
            if (popup != null)
            {
                popup.IsOpen = false;
            }
        }
    }
}
