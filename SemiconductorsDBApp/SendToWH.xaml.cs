using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SemiconductorsDBApp
{
    /// <summary>
    /// Interaction logic for SendToWH.xaml
    /// </summary>
    public partial class SendToWH : Window
    {
        private int m_id;
        private int available;
        private int pw_id;
        public SendToWH(int id, int available, int pw_id)
        {
            InitializeComponent();
            m_id = id;
            this.available = available;
            this.pw_id = pw_id;
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
        private void Window_Closed(object sender, EventArgs e)
        {
            ProductionWarehouse window = new ProductionWarehouse();
            window.Show();
            Close();
        }
        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            int quantity = 0;
            int? warehouse_id = null;
            try
            {
                quantity = Convert.ToInt32(QuantityData.Text);
            }
            catch
            {
                MessageBox.Show("Check your data!", "Error!");
                return;
            }
            if (quantity < 0)
            {
                MessageBox.Show("Check your data!", "Error!");
                return;
            }
            if (quantity > available)
            {
                MessageBox.Show("Not enough products!", "Error!");
                return;
            }

            using (MySqlConnection connection = DB.Connect())
            {
                String query = "";
                connection.Open();
                MySqlCommand command = new MySqlCommand();

                query = $"UPDATE `not_realized` SET quantity = quantity - {quantity} WHERE m_id = {m_id} AND pw_id = {pw_id};";
                command.CommandText = query;
                command.Connection = connection;
                command.ExecuteNonQuery();

                try
                {
                    query = $"INSERT INTO `available_material` (m_id, warehouse_id, quantity) VALUES ({m_id}, {warehouse_id}, {quantity});";
                    command.CommandText = query;
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                }
                catch
                {
                    query = $"UPDATE `available_material` SET quantity = quantity + {quantity} WHERE m_id = {m_id} AND warehouse_id = {warehouse_id};";
                    command.CommandText = query;
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                }

                Close();
            }
        }
    }
}