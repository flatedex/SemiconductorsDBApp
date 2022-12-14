using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace SemiconductorsDBApp
{
    /// <summary>
    /// Interaction logic for ProductionManager.xaml
    /// </summary>
    public partial class ProductionManager : Window
    {
        private DataTable materials;
        private MySqlDataAdapter adapter;
        public ProductionManager()
        {
            InitializeComponent();
        }
        private void FillTable()
        {
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
                    query = $"SELECT title, available_material.quantity FROM `available_material` INNER JOIN `materials` ON available_material.m_id = materials.m_id where warehouse_id = {warehouse_id} and available_material.quantity > 0;";
                    command.CommandText = query;
                    adapter = new MySqlDataAdapter(command);
                    adapter.Fill(materials);
                    AvailableMaterials.ItemsSource = materials.DefaultView;
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
        private void SelectedWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            materials = new DataTable();
            FillTable();
        }
        private void BuyBtn_Click(object sender, RoutedEventArgs e)
        {
            BuyMaterials window = new BuyMaterials();
            window.Show();
            Close();
        }
        private void SellBtn_Click(object sender, RoutedEventArgs e)
        {
            SellMaterials window = new SellMaterials();
            window.Show();
            Close();
        }
    }
}
