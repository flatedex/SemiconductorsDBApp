using MySql.Data.MySqlClient;
using System.Data;
using System.Windows;

namespace SemiconductorsDBApp
{
    /// <summary>
    /// Interaction logic for TransitWarehouseManagement.xaml
    /// </summary>
    public partial class TransitWarehouseManagement : Window
    {
        private DataTable transits;
        private MySqlDataAdapter adapter;
        public TransitWarehouseManagement()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            transits = new DataTable();
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                string query = $"SELECT * FROM `transfer_warehouse`;";
                MySqlCommand command = new MySqlCommand();
                command.CommandText = query;
                command.Connection = connection;
                adapter = new MySqlDataAdapter(command);
                adapter.Fill(transits);
                TransitWHS.ItemsSource = transits.DefaultView;
            }
        }
        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            DeleteTW window = new DeleteTW();
            window.Show();
            Close();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            AddTW window = new AddTW();
            window.Show();
            Close();
        }
    }
}
