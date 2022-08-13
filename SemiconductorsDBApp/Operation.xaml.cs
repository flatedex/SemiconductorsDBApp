using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SemiconductorsDBApp
{
    /// <summary>
    /// Interaction logic for Operation.xaml
    /// </summary>
    public partial class Operation : Window
    {
        private int po_id;
        public Operation(int id)
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

                List<string> prods = new List<string>();
                query = "SELECT name FROM `production_warehouse`;";
                command.CommandText = query;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            prods.Add(reader["name"].ToString());
                        }
                    }
                }
                SelectedProd.ItemsSource = prods;

                List<string> operations = new List<string>();
                query = "SELECT title FROM `operation`";
                command.CommandText = query;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            operations.Add(reader["title"].ToString());
                        }
                    }
                }
                SelectedOperation.ItemsSource = operations;
            }
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            int quantity = 0;
            int actualQuantity = 0;
            int? m_id = null;
            int? pw_id = null;
            int? warehouse_id = null;
            int? o_id = null;
            try
            {
                quantity = Convert.ToInt32(QuantityData.Text);
            }
            catch
            {
                MessageBox.Show("Check your data!", "Error!");
                return;
            }
            if (quantity <= 0)
            {
                MessageBox.Show("Check your data!", "Error!");
                return;
            }

            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand();
                string query = $"SELECT warehouse_id FROM `warehouse` WHERE name = '{SelectedWarehouse.SelectedItem}';";
                command.CommandText = query;
                command.Connection = connection;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            warehouse_id = Convert.ToInt32(reader["warehouse_id"].ToString());
                        }
                    }
                }

                query = $"SELECT pw_id FROM `production_warehouse` WHERE name = '{SelectedProd.SelectedItem}';";
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

                query = $"SELECT m_id FROM `materials` WHERE title = '{SelectedMaterial.SelectedItem}';";
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

                query = $"SELECT quantity FROM `available_material` WHERE warehouse_id = {warehouse_id} and m_id = {m_id};";
                command.CommandText = query;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            actualQuantity = Convert.ToInt32(reader["quantity"].ToString());
                        }
                    }
                }
                if (actualQuantity - quantity < 0)
                {
                    MessageBox.Show("Not enough materials!", "Error!");
                    return;
                }

                query = $"SELECT o_id FROM `operation` WHERE title = '{SelectedOperation.SelectedItem}';";
                command.CommandText = query;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            o_id = Convert.ToInt32(reader["o_id"].ToString());
                        }
                    }
                }

                query = $"INSERT INTO `wip_materials` (pw_id, o_id, po_id, m_id) VALUES ({pw_id}, {o_id}, {po_id}, {m_id});";
                command.CommandText = query;
                command.ExecuteNonQuery();

                query = $"UPDATE `available_material` SET quantity = quantity - {quantity} WHERE m_id = {m_id} AND warehouse_id = {warehouse_id};";
                command.CommandText = query;
                command.ExecuteNonQuery();

                Close();
            }
        }
        private void SelectedWarehouse_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand();
                List<string> materials = new List<string>();
                string query = "SELECT title FROM `available_material` INNER JOIN `materials` ON `available_material`.m_id = `materials`.m_id INNER JOIN" +
                  $" `warehouse` ON `warehouse`.warehouse_id = `available_material`.warehouse_id WHERE `warehouse`.name = '{SelectedWarehouse.SelectedItem}';";
                command.CommandText = query;
                command.Connection = connection;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            materials.Add(reader["title"].ToString());
                        }
                    }
                }
                SelectedMaterial.ItemsSource = materials;
                SelectedMaterial.SelectedIndex = 0;
            }
        }
    }
}
