using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SemiconductorsDBApp
{
    /// <summary>
    /// Interaction logic for BuyMaterials.xaml
    /// </summary>
    public partial class BuyMaterials : Window
    {
        public BuyMaterials()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                List<string> names = new List<string>();
                List<string> materials = new List<string>();
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
                query = "SELECT title FROM `materials` WHERE is_product = 0;";
                command.CommandText = query;
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
                SelectedWarehouse.ItemsSource = names;
                SelectedMaterial.ItemsSource = materials;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ProductionManager window = new ProductionManager();
            window.Show();
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                string warehouse = SelectedWarehouse.Text;
                string material = SelectedMaterial.Text;
                int? quantity = null;
                int? warehouse_id = null;
                int? m_id = null;
                try
                {
                    quantity = Int32.Parse(QuantityData.Text);
                }
                catch
                {
                    MessageBox.Show("Check your data.", "Error!");
                    return;
                }
                if (quantity <= 0)
                {
                    MessageBox.Show("Check your data.", "Error!");
                    return;
                }
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
                query = $"SELECT m_id FROM `materials` WHERE title = '{material}';";
                command.CommandText = query;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        m_id = Convert.ToInt32(reader["m_id"]);
                    }
                }
                if (warehouse_id.HasValue && m_id.HasValue)
                {
                    query = $"INSERT INTO `supply`(quantity, warehouse_id, m_id) VALUES ({quantity}, {warehouse_id}, {m_id});";
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
    }
}
