using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;

namespace SemiconductorsDBApp
{
    /// <summary>
    /// Interaction logic for TransitMaterials.xaml
    /// </summary>
    public partial class TransitMaterials : Window
    {
        private DataTable transitMaterials = new DataTable();
        public TransitMaterials()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            transitMaterials.Clear();
            FillTransferedMaterials();
        }
        private void FillTransferedMaterials()
        {
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                String query = $"SELECT quantity, materials.title FROM `transfered_materials` INNER JOIN `materials` ON transfered_materials.m_id = materials.m_id";
                MySqlCommand command = new MySqlCommand();
                command.CommandText = query;
                command.Connection = connection;
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(transitMaterials);
                TransitMaterials_DataGrid.ItemsSource = transitMaterials.DefaultView;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            transitMaterials.Clear();
            TransitMaterials_DataGrid.ItemsSource = transitMaterials.DefaultView;
        }
    }
}
