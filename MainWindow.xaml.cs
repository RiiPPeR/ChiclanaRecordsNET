using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace ChiclanaRecordsNET
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Holaa");
        }

        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}