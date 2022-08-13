using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace SemiconductorsDBApp
{
    /// <summary>
    /// Interaction logic for TransitOrders.xaml
    /// </summary>
    public partial class TransitOrders : Window
    {
        private DataTable arrivals;
        private DataTable departures;
        private MySqlDataAdapter adapter;
        private int id;
        public TransitOrders()
        {
            InitializeComponent();
        }
        private void SelectedWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            arrivals = new DataTable();
            departures = new DataTable();
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                int? warehouse_id = null;
                string warehouse = SelectedWarehouse.SelectedItem.ToString();
                string query = $"SELECT warehouse_id FROM `warehouse` WHERE name = '{warehouse}';";
                MySqlCommand command = new MySqlCommand();
                command.CommandText = query;
                command.Connection = connection;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        warehouse_id = Convert.ToInt32(reader["warehouse_id"]);
                    }
                }
                if (warehouse_id.HasValue)
                {
                    query = $"SELECT to_id, arrival_time FROM transfer_order " +
                      $"INNER JOIN route on transfer_order.r_id = route.r_id " +
                      $"WHERE destination = {warehouse_id};";
                    command.CommandText = query;
                    adapter = new MySqlDataAdapter(command);
                    adapter.Fill(arrivals);
                    Arrivals.ItemsSource = arrivals.DefaultView;
                    query = $"SELECT to_id, departure_time FROM transfer_order " +
                      $"INNER JOIN route on transfer_order.r_id = route.r_id " +
                      $"WHERE start = {warehouse_id};";
                    command.CommandText = query;
                    adapter = new MySqlDataAdapter(command);
                    adapter.Fill(departures);
                    Departures.ItemsSource = departures.DefaultView;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                List<string> names = new List<string>();
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
                SelectedWarehouse.ItemsSource = names;
            }
        }

        private void PlaceBtn_Click(object sender, RoutedEventArgs e)
        {
            PlaceTransitOrder window = new PlaceTransitOrder();
            window.Show();
            Close();
        }

        private void Arrivals_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (Arrivals.SelectedItems.Count > 0)
            {
                TakeOffBtn.IsEnabled = false;
                ViewBtn.IsEnabled = true;
                DataRowView selected = (DataRowView)Arrivals.SelectedItem;
                id = Convert.ToInt32(selected[0].ToString());
                string query = $"SELECT status FROM `transfer_order` WHERE to_id = {id};";
                string status = "";
                using (MySqlConnection connection = DB.Connect())
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand();
                    command.CommandText = query;
                    command.Connection = connection;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                status = reader["status"].ToString();
                            }
                        }
                    }
                }
                if (status == "On the way")
                {
                    TakeOffBtn.IsEnabled = true;
                }
            }
            else
            {
                ViewBtn.IsEnabled = false;
                TakeOffBtn.IsEnabled = false;
            }
            Departures.UnselectAllCells();
        }

        private void Departures_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (Departures.SelectedItems.Count > 0)
            {
                ViewBtn.IsEnabled = true;
                SendBtn.IsEnabled = false;
                DataRowView selected = (DataRowView)Departures.SelectedItem;
                id = Convert.ToInt32(selected[0].ToString());
                string query = $"SELECT status FROM `transfer_order` WHERE to_id = {id};";
                string status = "";
                using (MySqlConnection connection = DB.Connect())
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand();
                    command.CommandText = query;
                    command.Connection = connection;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                status = reader["status"].ToString();
                            }
                        }
                    }
                }
                if (status == "Opened")
                {
                    SendBtn.IsEnabled = true;
                }
            }
            else
            {
                ViewBtn.IsEnabled = false;
                SendBtn.IsEnabled = false;
            }
            Arrivals.UnselectAllCells();
        }

        private void ViewBtn_Click(object sender, RoutedEventArgs e)
        {
            TransitOrder window = new TransitOrder(id);
            window.Show();
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            List<int> actualQuantites = new List<int>();
            List<int> movedQuantites = new List<int>();
            List<int> movedIds = new List<int>();
            int? warehouse_id = null;
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand();
                command.Connection = connection;
                string query = $"SELECT m_id, quantity FROM `transfered_materials` WHERE to_id = {id} ORDER BY m_id ASC;";
                command.CommandText = query;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            movedQuantites.Add(Convert.ToInt32(reader["quantity"].ToString()));
                            movedIds.Add(Convert.ToInt32(reader["m_id"].ToString()));
                        }
                    }
                }
                for (int i = 0; i < movedIds.Count; i++)
                {
                    query = "SELECT quantity, `warehouse`.warehouse_id FROM `transfer_order` INNER JOIN `route` ON `transfer_order`.r_id = `route`.r_id " +
                    "INNER JOIN `warehouse` ON start = warehouse_id INNER JOIN `available_material` ON `warehouse`.warehouse_id = `available_material`.warehouse_id " +
                    $"WHERE to_id = {id} AND m_id = {movedIds[i]} ORDER BY m_id ASC;";
                    command.CommandText = query;
                    command.Connection = connection;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                actualQuantites.Add(Convert.ToInt32(reader["quantity"].ToString()));
                                warehouse_id = Convert.ToInt32(reader["warehouse_id"].ToString());
                            }
                        }
                    }
                }
                for (int i = 0; i < movedQuantites.Count; i++)
                {
                    if (actualQuantites[i] - movedQuantites[i] < 0)
                    {
                        MessageBox.Show("Not enough materials!", "Error!");
                        return;
                    }
                }
                for (int i = 0; i < movedQuantites.Count; i++)
                {
                    query = $"UPDATE `available_material` SET quantity = quantity - {movedQuantites[i]} WHERE m_id = {movedIds[i]} AND warehouse_id = {warehouse_id};";
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
                query = $"UPDATE `transfer_order` SET departure_time = '{DateTime.Today.ToString("yyyy.MM.dd")}' WHERE to_id = {id};";
                command.CommandText = query;
                command.ExecuteNonQuery();

                int? duration = null;
                query = $"SELECT duration FROM `transfer_order` INNER JOIN `route` on `transfer_order`.r_id = `route`.r_id WHERE to_id = {id};";
                command.CommandText = query;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            duration = Convert.ToInt32(reader["duration"].ToString());
                        }
                    }
                }

                query = $"UPDATE `transfer_order` SET arrival_time = '{DateTime.Today.AddDays((double)duration).ToString("yyyy.MM.dd")}', status = 'On the way' WHERE to_id = {id};";
                command.CommandText = query;
                command.ExecuteNonQuery();

                query = $"SELECT to_id, arrival_time FROM transfer_order INNER JOIN route on transfer_order.r_id = route.r_id WHERE destination = {warehouse_id};";
                command.CommandText = query;
                adapter = new MySqlDataAdapter(command);
                arrivals.Clear();
                adapter.Fill(arrivals);
                Arrivals.ItemsSource = arrivals.DefaultView;
                query = $"SELECT to_id, departure_time FROM transfer_order " +
                  $"INNER JOIN route on transfer_order.r_id = route.r_id " +
                  $"WHERE start = {warehouse_id};";
                command.CommandText = query;
                adapter = new MySqlDataAdapter(command);
                departures.Clear();
                adapter.Fill(departures);
                Departures.ItemsSource = departures.DefaultView;
            }
        }

        private void TakeOffBtn_Click(object sender, RoutedEventArgs e)
        {
            List<int> movedQuantites = new List<int>();
            List<int> mIds = new List<int>();
            int? warehouse_id = null;
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand();
                command.Connection = connection;
                string query = $"SELECT m_id, quantity FROM `transfered_materials` WHERE to_id = {id} ORDER BY m_id ASC;";
                command.CommandText = query;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            movedQuantites.Add(Convert.ToInt32(reader["quantity"].ToString()));
                            mIds.Add(Convert.ToInt32(reader["m_id"].ToString()));
                        }
                    }
                }
                query = $"SELECT destination FROM `transfer_order` INNER JOIN `route` ON `transfer_order`.r_id = `route`.r_id WHERE to_id = {id};";
                command.CommandText = query;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            warehouse_id = Convert.ToInt32(reader["destination"].ToString());
                        }
                    }
                }
                for (int i = 0; i < movedQuantites.Count; i++)
                {
                    try
                    {
                        query = $"INSERT INTO `available_material` (m_id, warehouse_id, quantity) VALUES ({mIds[i]}, {warehouse_id}, {movedQuantites[i]});";
                        command.CommandText = query;
                        command.ExecuteNonQuery();
                    }
                    catch
                    {
                        query = $"UPDATE `available_material` SET quantity = quantity + {movedQuantites[i]} WHERE m_id = {mIds[i]} AND warehouse_id = {warehouse_id};";
                        command.CommandText = query;
                        command.ExecuteNonQuery();
                    }
                }

                query = $"UPDATE `transfer_order` SET arrival_time = '{DateTime.Today.ToString("yyyy.MM.dd")}', status = 'Closed' WHERE to_id = {id};";
                command.CommandText = query;
                command.ExecuteNonQuery();

                query = $"SELECT to_id, arrival_time FROM transfer_order " +
                  $"INNER JOIN route on transfer_order.r_id = route.r_id " +
                  $"WHERE destination = {warehouse_id};";
                command.CommandText = query;
                adapter = new MySqlDataAdapter(command);
                arrivals.Clear();
                adapter.Fill(arrivals);
                Arrivals.ItemsSource = arrivals.DefaultView;
                query = $"SELECT to_id, departure_time FROM transfer_order " +
                  $"INNER JOIN route on transfer_order.r_id = route.r_id " +
                  $"WHERE start = {warehouse_id};";
                command.CommandText = query;
                adapter = new MySqlDataAdapter(command);
                departures.Clear();
                adapter.Fill(departures);
                Departures.ItemsSource = departures.DefaultView;
            }
        }
    }
}
