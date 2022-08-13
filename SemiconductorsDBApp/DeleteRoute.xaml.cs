using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SemiconductorsDBApp
{
    /// <summary>
    /// Interaction logic for DeleteRoute.xaml
    /// </summary>
    public partial class DeleteRoute : Window
    {
        List<int> ids;
        public DeleteRoute()
        {
            InitializeComponent();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            RoutesManager window = new RoutesManager();
            window.Show();
            Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                ids = new List<int>();
                List<int> nums = new List<int>();
                MySqlCommand command = new MySqlCommand();
                string query = "SELECT r_id FROM `route` ORDER BY r_id ASC;";
                command.CommandText = query;
                command.Connection = connection;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            ids.Add(Convert.ToInt32(reader["r_id"]));
                            nums.Add(ids.IndexOf(Convert.ToInt32(reader["r_id"])) + 1);
                        }
                    }
                }
                SelectedRoute.ItemsSource = nums;
            }
        }
        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                int r_id = ids[Convert.ToInt32(SelectedRoute.Text) - 1];
                string query = $"DELETE FROM `route` WHERE r_id = {r_id};";
                MySqlCommand command = new MySqlCommand();
                command.CommandText = query;
                command.Connection = connection;
                command.ExecuteNonQuery();
                Close();
            }
        }
    }
}
