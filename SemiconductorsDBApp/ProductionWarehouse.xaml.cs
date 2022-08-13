using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace SemiconductorsDBApp
{
    /// <summary>
    /// Interaction logic for ProductionWarehouse.xaml
    /// </summary>
    public partial class ProductionWarehouse : Window
    {
        private int id;
        private int pw_id;
        private int available;
        private DataTable materials;
        private MySqlDataAdapter adapter;
        public ProductionWarehouse()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                List<string> names = new List<string>();
                MySqlCommand command = new MySqlCommand();
                string query = "SELECT name FROM `production_warehouse`;";
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
        private void SelectedWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            materials = new DataTable();
            FillTable();
        }
        private void FillTable()
        {
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                int? w_id = null;
                string warehouse = SelectedWarehouse.SelectedItem.ToString();
                string query = $"SELECT pw_id FROM `production_warehouse` WHERE name = '{warehouse}';";
                MySqlCommand command = new MySqlCommand();
                command.CommandText = query;
                command.Connection = connection;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        w_id = Convert.ToInt32(reader["pw_id"]);
                    }
                }
                if (w_id.HasValue)
                {
                    query = $"SELECT title, quantity FROM `not_realized` INNER JOIN `materials` ON not_realized.m_id = materials.m_id where pw_id = {w_id} and quantity > 0;";
                    command.CommandText = query;
                    adapter = new MySqlDataAdapter(command);
                    adapter.Fill(materials);
                    AvailableMaterials.ItemsSource = materials.DefaultView;
                }
            }
        }
        private void AvailableMaterials_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AvailableMaterials.SelectedItems.Count > 0)
            {
                SendBtn.IsEnabled = true;
                DataRowView selected = (DataRowView)AvailableMaterials.SelectedItem;
                string title = selected[0].ToString();
                string query = $"SELECT m_id FROM `materials` WHERE title = '{title}';";
                available = Convert.ToInt32(selected[1].ToString());
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
                                id = Convert.ToInt32(reader["m_id"].ToString());
                            }
                        }
                    }
                    query = $"SELECT pw_id FROM `production_warehouse` WHERE name = '{SelectedWarehouse.SelectedItem}'";
                    command.CommandText = query;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                pw_id = Convert.ToInt32(reader["pw_id"].ToString());
                            }
                        }
                    }
                }
            }
            else
            {
                SendBtn.IsEnabled = false;
            }
        }
        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            SendToWH window = new SendToWH(id, available, pw_id);
            window.Show();
            Close();
        }
    }
}
