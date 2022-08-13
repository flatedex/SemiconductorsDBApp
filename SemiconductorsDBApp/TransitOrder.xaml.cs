using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;

namespace SemiconductorsDBApp
{
    /// <summary>
    /// Interaction logic for TransitOrder.xaml
    /// </summary>
    public partial class TransitOrder : Window
    {
        private int to_id;
        private MySqlDataAdapter adapter;
        public TransitOrder(int to_id)
        {
            InitializeComponent();
            this.to_id = to_id;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                string query = "SELECT status, start, destination, arrival_time, departure_time FROM `transfer_order` INNER JOIN `route`" +
                  $" ON `transfer_order`.r_id = `route`.r_id WHERE to_id = {to_id};";
                MySqlCommand command = new MySqlCommand();
                command.Connection = connection;
                command.CommandText = query;
                int? startId = null;
                int? endId = null;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            startId = Convert.ToInt32(reader["start"].ToString());
                            endId = Convert.ToInt32(reader["destination"].ToString());
                            Status.Text = reader["status"].ToString();
                            DepartTime.Text = reader["departure_time"].ToString();
                            ArrivalTime.Text = reader["arrival_time"].ToString();
                        }
                    }
                }
                query = $"SELECT name FROM `warehouse` WHERE warehouse_id = {startId};";
                command.CommandText = query;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Start.Text = reader["name"].ToString();
                        }
                    }
                }
                query = $"SELECT name FROM `warehouse` WHERE warehouse_id = {endId};";
                command.CommandText = query;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            End.Text = reader["name"].ToString();
                        }
                    }
                }
                query = $"SELECT title, quantity FROM `transfered_materials` INNER JOIN `materials` ON `transfered_materials`.m_id = `materials`.m_id where to_id = {to_id};";
                command.CommandText = query;
                adapter = new MySqlDataAdapter(command);
                DataTable materials = new DataTable();
                adapter.Fill(materials);
                MovedMaterials.ItemsSource = materials.DefaultView;
            }
        }
    }
}
