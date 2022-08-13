using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;

namespace SemiconductorsDBApp
{
    /// <summary>
    /// Interaction logic for RoutesManager.xaml
    /// </summary>
    public partial class RoutesManager : Window
    {
        private DataTable routes;
        public RoutesManager()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection connection = DB.Connect())
            {
                List<int> startIDs = new List<int>();
                List<int> endIDs = new List<int>();
                List<int> transitIDs = new List<int>();
                List<int> durations = new List<int>();
                List<string> starts = new List<string>();
                List<string> destinations = new List<string>();
                List<string> transits = new List<string>();
                connection.Open();
                string query = $"SELECT * FROM `route` ORDER BY r_id ASC;";
                MySqlCommand command = new MySqlCommand();
                command.CommandText = query;
                command.Connection = connection;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            startIDs.Add(Convert.ToInt32(reader["start"]));
                            endIDs.Add(Convert.ToInt32(reader["destination"]));
                            transitIDs.Add(Convert.ToInt32(reader["tw_id"]));
                            durations.Add(Convert.ToInt32(reader["duration"]));
                        }
                    }
                }
                foreach (int id in startIDs)
                {
                    query = $"SELECT name FROM `warehouse` WHERE warehouse_id = {id}";
                    command.CommandText = query;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                starts.Add(reader["name"].ToString());
                            }
                        }
                    }
                }
                foreach (int id in endIDs)
                {
                    query = $"SELECT name FROM `warehouse` WHERE warehouse_id = {id}";
                    command.CommandText = query;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                destinations.Add(reader["name"].ToString());
                            }
                        }
                    }
                }
                foreach (int id in transitIDs)
                {
                    query = $"SELECT name FROM `transfer_warehouse` WHERE tw_id = {id}";
                    command.CommandText = query;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                transits.Add(reader["name"].ToString());
                            }
                        }
                    }
                }
                routes = new DataTable();
                routes.Columns.Add("Start");
                routes.Columns.Add("Destination");
                routes.Columns.Add("Transit");
                routes.Columns.Add("Duration");
                for (int i = 0; i < starts.Count; i++)
                {
                    DataRow row = routes.NewRow();
                    row["Start"] = starts[i];
                    row["Destination"] = destinations[i];
                    row["Transit"] = transits[i];
                    row["Duration"] = durations[i];
                    routes.Rows.Add(row);
                }
                ExistingRoutes.ItemsSource = routes.DefaultView;
            }
        }
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            AddRoute window = new AddRoute();
            window.Show();
            Close();
        }
        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            DeleteRoute window = new DeleteRoute();
            window.Show();
            Close();
        }
    }
}
