using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SemiconductorsDBApp
{
    /// <summary>
    /// Interaction logic for OpenProduction.xaml
    /// </summary>
    public partial class OpenProduction : Window
    {
        public OpenProduction()
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
                string query = "SELECT title FROM `materials` WHERE is_product = 1;";
                command.CommandText = query;
                command.Connection = connection;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            names.Add(reader["title"].ToString());
                        }
                    }
                }
                SelectedMaterial.ItemsSource = names;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Production window = new Production();
            window.Show();
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                string material = SelectedMaterial.Text;
                int? m_id = null;
                MySqlCommand command = new MySqlCommand();
                string query = $"SELECT m_id FROM `materials` WHERE title = '{material}';";
                command.CommandText = query;
                command.Connection = connection;
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
                query = $"INSERT INTO `production_order`(m_id, status) VALUES ({m_id}, 'In process');";
                command.CommandText = query;
                command.ExecuteNonQuery();
                Close();
            }
        }
    }
}
