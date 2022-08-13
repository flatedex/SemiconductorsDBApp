using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SemiconductorsDBApp
{
    /// <summary>
    /// Interaction logic for AddTW.xaml
    /// </summary>
    public partial class AddTW : Window
    {
        public AddTW()
        {
            InitializeComponent();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            TransitWarehouseManagement window = new TransitWarehouseManagement();
            window.Show();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> types = new List<string> { "Plane", "Truck", "Train", "Boat" };
            TypeData.ItemsSource = types;
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            string name;
            string type = TypeData.Text;
            if (NameData.Text != "")
            {
                name = NameData.Text;
            }
            else
            {
                MessageBox.Show("Check your data!", "Error!");
                return;
            }
            if (name != null)
            {
                string query = $"INSERT INTO `transfer_warehouse` (name, type) VALUES ('{name}', '{type}');";
                using (MySqlConnection connection = DB.Connect())
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand();
                    command.CommandText = query;
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                    Close();
                }
            }
        }
    }
}
