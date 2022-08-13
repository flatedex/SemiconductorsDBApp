using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace SemiconductorsDBApp
{
    /// <summary>
    /// Interaction logic for Production.xaml
    /// </summary>
    public partial class Production : Window
    {
        private DataTable orders;
        private MySqlDataAdapter adapter;
        int id;
        public Production()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            orders = new DataTable();
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                string query = "SELECT po_id, title, status FROM `production_order` INNER JOIN `materials` ON `production_order`.m_id = `materials`.m_id;";
                MySqlCommand command = new MySqlCommand();
                command.CommandText = query;
                command.Connection = connection;
                adapter = new MySqlDataAdapter(command);
                adapter.Fill(orders);
                ProductionOrders.ItemsSource = orders.DefaultView;
            }
        }

        private void OpenOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenProduction window = new OpenProduction();
            window.Show();
            Close();
        }

        private void ProductionOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductionOrders.SelectedItems.Count > 0)
            {
                CloseBtn.IsEnabled = false;
                OperationBtn.IsEnabled = false;
                ViewBtn.IsEnabled = false;
                DataRowView selected = (DataRowView)ProductionOrders.SelectedItem;
                id = Convert.ToInt32(selected[0].ToString());
                ViewBtn.IsEnabled = true;
                string query = $"SELECT status FROM `production_order` WHERE po_id = {id};";
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
                if (status == "In process")
                {
                    CloseBtn.IsEnabled = true;
                    OperationBtn.IsEnabled = true;
                }
            }
            else
            {
                CloseBtn.IsEnabled = false;
                OperationBtn.IsEnabled = false;
                ViewBtn.IsEnabled = false;
            }
        }

        private void ManageBtn_Click(object sender, RoutedEventArgs e)
        {
            ProductionWarehouse window = new ProductionWarehouse();
            window.Show();
        }

        private void OperationBtn_Click(object sender, RoutedEventArgs e)
        {
            Operation window = new Operation(id);
            window.Show();
        }

        private void ViewBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewOperations window = new ViewOperations(id);
            window.Show();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseProductionOrder window = new CloseProductionOrder(id);
            window.Show();
            Close();
        }
    }
}
