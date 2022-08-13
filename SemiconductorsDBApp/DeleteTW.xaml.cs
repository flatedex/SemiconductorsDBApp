using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Windows;

namespace SemiconductorsDBApp
{
    /// <summary>
    /// Interaction logic for DeleteTW.xaml
    /// </summary>
    public partial class DeleteTW : Window
    {
        public DeleteTW()
        {
            InitializeComponent();
        }
        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                string wh = SelectedWarehouse.Text;
                MySqlCommand command = new MySqlCommand();
                string query = $"DELETE FROM `transfer_warehouse` WHERE name = '{wh}';";
                command.CommandText = query;
                command.Connection = connection;
                command.ExecuteNonQuery();
                Close();
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                List<string> names = new List<string>();
                MySqlCommand command = new MySqlCommand();
                string query = "SELECT name FROM `transfer_warehouse`;";
                command.CommandText = query;
                command.Connection = connection;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            names.Add(reader["name"].ToString());
                        }
                    }
                }
                SelectedWarehouse.ItemsSource = names;
            }
        }
        private void Window_Closed(object sender, System.EventArgs e)
        {
            TransitWarehouseManagement window = new TransitWarehouseManagement();
            window.Show();
        }
    }
}
