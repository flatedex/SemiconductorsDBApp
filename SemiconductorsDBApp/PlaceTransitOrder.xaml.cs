using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace SemiconductorsDBApp
{
    /// <summary>
    /// Interaction logic for PlaceTransitOrder.xaml
    /// </summary>
    public partial class PlaceTransitOrder : Window
    {
        private int r_id;
        private DataTable addedMaterial;
        public PlaceTransitOrder()
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
                Start.ItemsSource = names;
                addedMaterial = new DataTable();
                addedMaterial.Columns.Add("title");
                addedMaterial.Columns.Add("quantity");
                addedMaterial.Columns.Add("id");
            }
        }

        private void Start_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                int? startId = null;
                string start = Start.SelectedItem.ToString();
                List<string> names = new List<string>();
                List<String> allNames = new List<String>();

                MySqlCommand command = new MySqlCommand();
                string query = $"SELECT warehouse_id FROM `warehouse` WHERE name = '{start}';";
                command.CommandText = query;
                command.Connection = connection;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            startId = Convert.ToInt32(reader["warehouse_id"].ToString());
                        }
                    }
                }
                if (startId.HasValue)
                {
                    query = $"SELECT name FROM `warehouse` INNER JOIN `route` ON warehouse_id = destination WHERE start = {startId};";
                    command.CommandText = query;
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
                    if (startId.HasValue)
                    {
                        query = $"SELECT name FROM `warehouse`;";
                        command.CommandText = query;
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    allNames.Add(reader["name"].ToString());
                                }
                            }
                        }
                    }
                    allNames.Remove(start);
                    query = $"SELECT title FROM `available_material` INNER JOIN `materials` ON available_material.m_id = materials.m_id where warehouse_id = {startId};";
                    command.CommandText = query;
                    List<string> materials = new List<string>();
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
                    Material.ItemsSource = materials;
                    Material.SelectedIndex = 0;
                }
                Destination.ItemsSource = allNames;
                Destination.SelectedIndex = 0;
            }
        }

        private void Destination_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                int? endId = null;
                string? end = null;
                var item = Destination.SelectedItem;
                if (Destination.SelectedItem != null)
                {
                    end = Destination.SelectedItem.ToString();
                }
                List<string> names = new List<string>();
                MySqlCommand command = new MySqlCommand();
                string query = $"SELECT warehouse_id FROM `warehouse` WHERE name = '{end}';";
                command.CommandText = query;
                command.Connection = connection;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            endId = Convert.ToInt32(reader["warehouse_id"].ToString());
                        }
                    }
                }
                int? startId = null;
                query = $"SELECT warehouse_id FROM `warehouse` WHERE name = '{Start.SelectedItem}';";
                command.CommandText = query;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            startId = Convert.ToInt32(reader["warehouse_id"].ToString());
                        }
                    }
                }
                if (endId.HasValue)
                {
                    query = $"SELECT name FROM `transfer_warehouse` INNER JOIN `route` ON `transfer_warehouse`.tw_id = `route`.tw_id " +
                      $"WHERE destination = {endId} AND start = {startId};";
                    command.CommandText = query;
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
                }
                Route.ItemsSource = names;
                Route.SelectedIndex = 0;
            }
        }
        private void CompositeRoute(int? startId, int? endId)
        {
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();

                List<string> names = new List<string>();
                List<string> medium_ids = new List<string>();
                List<String> allNames = new List<String>();
                List<int?> final_ids = new List<int?>();

                MySqlCommand command = new MySqlCommand();
                String query = "";
                if (startId.HasValue)
                {
                    query = $"SELECT name, warehouse_id FROM `warehouse` INNER JOIN `route` ON warehouse_id = destination WHERE start = {startId};";// check if straight route exist
                    command.CommandText = query;
                    command.Connection = connection;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                names.Add(reader["name"].ToString());
                                medium_ids.Add(reader["warehouse_id"].ToString()); // names - affordable warehouses
                            }
                        }
                    }
                }
                int medium = 0;
                if (names.Contains("Shoreline") || names.Contains("Customs") || names.Contains("Reserve"))
                {
                    for (int i = 0; i < names.Count; i++)
                    {
                        query = $"SELECT warehouse_id FROM `warehouse` INNER JOIN `route` ON warehouse_id = destination WHERE start = {Convert.ToInt32(medium_ids[i])};";
                        command.CommandText = query;
                        command.Connection = connection;
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    final_ids.Add(Convert.ToInt32(reader["warehouse_id"]));
                                }
                            }
                        }
                        if (final_ids.Contains(endId))
                        {
                            medium = Convert.ToInt32(medium_ids[i]);
                        }
                    }
                }
                if (final_ids.Contains(endId))
                {
                    query = $"SELECT r_id FROM route WHERE start = {startId} AND destination = {medium};";
                    command.CommandText = query;
                    command.Connection = connection;
                    int? route = null;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                route = Convert.ToInt32(reader["r_id"].ToString());
                            }
                        }
                    }
                    query = $"INSERT INTO `transfer_order` (status, r_id) VALUES('Opened', {route});"; // first transfer order
                    command.CommandText = query;
                    command.Connection = connection;
                    command.ExecuteNonQuery();

                    query = $"SELECT r_id FROM route WHERE start = {medium} AND destination = {endId};";
                    command.CommandText = query;
                    command.Connection = connection;
                    route = null;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                route = Convert.ToInt32(reader["r_id"].ToString());
                            }
                        }
                    }
                    query = $"INSERT INTO `transfer_order` (status, r_id) VALUES('Opened', {route});"; // second transfer order
                    command.CommandText = query;
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                }
                int? to_id = null;
                query = "SELECT to_id FROM `transfer_order` ORDER BY to_id DESC;";
                command.CommandText = query;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        to_id = Convert.ToInt32(reader["to_id"].ToString());
                    }
                }
                foreach (DataRow row in addedMaterial.Rows)
                {
                    int m_id = Convert.ToInt32(row["id"]);
                    int quantity = Convert.ToInt32(row["quantity"]);
                    query = $"INSERT INTO `transfered_materials` (m_id, to_id, quantity) VALUES ({m_id}, {to_id}, {quantity});";
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
                foreach (DataRow row in addedMaterial.Rows)
                {
                    int m_id = Convert.ToInt32(row["id"]);
                    int quantity = Convert.ToInt32(row["quantity"]);
                    query = $"INSERT INTO `transfered_materials` (m_id, to_id, quantity) VALUES ({m_id}, {to_id - 1}, {quantity});";
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
            }
            Close();
        }
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                int? endId = null;
                string end = Destination.SelectedItem.ToString();
                MySqlCommand command = new MySqlCommand();
                string query = $"SELECT warehouse_id FROM `warehouse` WHERE name = '{end}';";
                command.CommandText = query;
                command.Connection = connection;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            endId = Convert.ToInt32(reader["warehouse_id"].ToString());
                        }
                    }
                }

                int? startId = null;
                string start = Start.SelectedItem.ToString();
                query = $"SELECT warehouse_id FROM `warehouse` WHERE name = '{start}';";
                command.CommandText = query;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            startId = Convert.ToInt32(reader["warehouse_id"].ToString());
                        }
                    }
                }

                int? twId = null;
                string? tw = null;
                if (Route.SelectedItem == null)
                {
                    tw = null;
                }
                else
                {
                    tw = Route.SelectedItem.ToString();
                }
                query = $"SELECT tw_id FROM `transfer_warehouse` WHERE name = '{tw}';";
                command.CommandText = query;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            twId = Convert.ToInt32(reader["tw_id"].ToString());
                        }
                    }
                }

                if (endId.HasValue && startId.HasValue && twId.HasValue)
                {
                    query = $"SELECT r_id FROM `route` WHERE destination = {endId} and start = {startId} and tw_id = {twId};";
                    command.CommandText = query;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                r_id = Convert.ToInt32(reader["r_id"].ToString());
                            }
                        }
                    }
                }

                string material = Material.SelectedItem.ToString();
                int? m_id = null;
                query = $"SELECT m_id FROM `materials` WHERE title = '{material}';";
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

                int? quantity = null;
                try
                {
                    quantity = Convert.ToInt32(MatQuantity.Text);
                }
                catch
                {
                    MessageBox.Show("Wrong data!", "Error!");
                    return;
                }
                if (quantity.HasValue && quantity > 0)
                {
                    DataRow[] checkRow = addedMaterial.Select($"title = '{material}'");
                    if (checkRow.Length > 0)
                    {
                        foreach (DataRow entry in checkRow)
                        {
                            int prevQuantity = Convert.ToInt32(entry["quantity"]);
                            entry["quantity"] = prevQuantity + quantity;
                        }
                    }
                    else
                    {
                        DataRow row = addedMaterial.NewRow();
                        row["title"] = material;
                        row["quantity"] = quantity;
                        row["id"] = m_id;
                        addedMaterial.Rows.Add(row);
                    }
                }
                else
                {
                    MessageBox.Show("Wrong data!", "Error!");
                    return;
                }
                AddedMaterials.ItemsSource = addedMaterial.DefaultView;
            }
            Start.IsEnabled = false;
            Destination.IsEnabled = false;
            Route.IsEnabled = false;
            PlaceBtn.IsEnabled = true;
        }
        private void AddedMaterials_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (AddedMaterials.SelectedItems.Count > 0)
            {
                RemoveBtn.IsEnabled = true;
            }
            else
            {
                RemoveBtn.IsEnabled = false;
            }
        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = AddedMaterials.SelectedIndex;
            DataRow row = addedMaterial.Rows[selectedIndex];
            int quantity = Convert.ToInt32(row["quantity"]);
            int m_id = Convert.ToInt32(row["id"]);
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                string query = $"SELECT weight FROM `materials` WHERE m_id = {m_id};";
                MySqlCommand command = new MySqlCommand();
                command.Connection = connection;
                command.CommandText = query;
            }
            row.Delete();
        }

        private void PlaceBtn_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                int? endId = null;
                string end = Destination.SelectedItem.ToString();
                MySqlCommand command = new MySqlCommand();
                string query = $"SELECT warehouse_id FROM `warehouse` WHERE name = '{end}';";
                command.CommandText = query;
                command.Connection = connection;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            endId = Convert.ToInt32(reader["warehouse_id"].ToString());
                        }
                    }
                }

                int? startId = null;
                string start = Start.SelectedItem.ToString();
                query = $"SELECT warehouse_id FROM `warehouse` WHERE name = '{start}';";
                command.CommandText = query;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            startId = Convert.ToInt32(reader["warehouse_id"].ToString());
                        }
                    }
                }
                if (Route.SelectedItem == null)
                {
                    var result = MessageBox.Show("There is no straight path to warehouse, would you prefer build composite one?", "Composite route", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        CompositeRoute(startId, endId);
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
                query = $"INSERT INTO `transfer_order` (status, r_id) VALUES ('Opened', {r_id});";
                command = new MySqlCommand();
                command.Connection = connection;
                command.CommandText = query;
                command.ExecuteNonQuery();
                int? to_id = null;
                query = "SELECT to_id FROM `transfer_order` ORDER BY to_id DESC;";
                command.CommandText = query;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        to_id = Convert.ToInt32(reader["to_id"].ToString());
                    }
                }
                foreach (DataRow row in addedMaterial.Rows)
                {
                    int m_id = Convert.ToInt32(row["id"]);
                    int quantity = Convert.ToInt32(row["quantity"]);
                    query = $"INSERT INTO `transfered_materials` (m_id, to_id, quantity) VALUES ({m_id}, {to_id}, {quantity});";
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
            }
            Close();
        }
    }
}
