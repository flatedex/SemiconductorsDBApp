using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SemiconductorsDBApp
{
    /// <summary>
    /// Interaction logic for AddRoute.xaml
    /// </summary>
    public partial class AddRoute : Window
    {
        public AddRoute()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                List<string> names = new List<string>();
                List<string> transits = new List<string>();
                MySqlCommand command = new MySqlCommand();
                string query = "SELECT name FROM `warehouse`;";
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
                query = "SELECT name FROM `transfer_warehouse`;";
                command.CommandText = query;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            transits.Add(reader["name"].ToString());
                        }
                    }
                }
                StartWarehouse.ItemsSource = names;
                EndWarehouse.ItemsSource = names;
                TransitWarehouse.ItemsSource = transits;
            }
        }
        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                string start = StartWarehouse.Text;
                string end = EndWarehouse.Text;
                string transit = TransitWarehouse.Text;
                int? duration = null;
                int? start_id = null;
                int? end_id = null;
                int? transit_id = null;
                try
                {
                    duration = Int32.Parse(DurationData.Text);
                }
                catch
                {
                    MessageBox.Show("Check your data.", "Error!");
                    return;
                }
                if (duration <= 0)
                {
                    MessageBox.Show("Check your data.", "Error!");
                    return;
                }
                string query = $"SELECT warehouse_id FROM `warehouse` WHERE name = '{start}';";
                MySqlCommand command = new MySqlCommand();
                command.CommandText = query;
                command.Connection = connection;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        start_id = Convert.ToInt32(reader["warehouse_id"]);
                    }
                }
                query = $"SELECT warehouse_id FROM `warehouse` WHERE name = '{end}';";
                command.CommandText = query;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        end_id = Convert.ToInt32(reader["warehouse_id"]);
                    }
                }
                query = $"SELECT tw_id FROM `transfer_warehouse` WHERE name = '{transit}';";
                command.CommandText = query;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        transit_id = Convert.ToInt32(reader["tw_id"]);
                    }
                }
                if (start_id.HasValue && end_id.HasValue && transit_id.HasValue)
                {
                    query = $"INSERT INTO `route`(start, destination, tw_id, duration) VALUES ({start_id}, {end_id}, {transit_id}, {duration});";
                    command.CommandText = query;
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message, "Error!");
                        return;
                    }
                    Close();
                }
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            RoutesManager window = new RoutesManager();
            window.Show();
        }
    }
}
