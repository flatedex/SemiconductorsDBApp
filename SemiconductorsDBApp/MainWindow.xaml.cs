using System.Windows;

namespace SemiconductorsDBApp
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
        private void WarhouseManagerBtn_Click(object sender, RoutedEventArgs e)
        {
            WarehouseManager window = new WarehouseManager();
            window.Show();
        }
        private void ProductManagerBtn_Click(object sender, RoutedEventArgs e)
        {
            ProductionManager window = new ProductionManager();
            window.Show();
        }
        private void ProductionBtn_Click(object sender, RoutedEventArgs e)
        {
            Production window = new Production();
            window.Show();
        }
        private void RoutesManagerBtn_Click(object sender, RoutedEventArgs e)
        {
            RoutesManager window = new RoutesManager();
            window.Show();
        }
    }
}
