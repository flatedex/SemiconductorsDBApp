using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SemiconductorsDBApp
{
    /// <summary>
    /// Interaction logic for CloseProductionOrder.xaml
    /// </summary>
    public partial class CloseProductionOrder : Window
    {
        private int po_id;
        private int m_id;
        private int pw_id;
        public CloseProductionOrder(int id)
        {
            InitializeComponent();
            po_id = id;
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

                query = $"SELECT m_id FROM `production_order` WHERE po_id = {po_id};";
                command.CommandText = query;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            m_id = Convert.ToInt32(reader["m_id"].ToString());
                        }
                    }
                }
            }
        }

        private void SelectedWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand();
                string query = $"SELECT pw_id FROM `production_warehouse` WHERE name = '{SelectedWarehouse.SelectedItem}';";
                command.CommandText = query;
                command.Connection = connection;
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
        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand();
                string query = "";
                try
                {
                    query = $"INSERT INTO `not_realized` (m_id, pw_id, quantity) VALUES ({m_id}, {pw_id}, 1);";
                    command.CommandText = query;
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                }
                catch
                {
                    query = $"UPDATE `not_realized` quantity = quantity + 1 WHERE pw_id = {pw_id} AND m_id = {m_id};";
                    command.CommandText = query;
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                }

                query = $"UPDATE `production_order` SET status = 'Closed' WHERE po_id = {po_id};";
                command.CommandText = query;
                command.ExecuteNonQuery();

                Close();
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            Production window = new Production();
            window.Show();
        }
    }
}
