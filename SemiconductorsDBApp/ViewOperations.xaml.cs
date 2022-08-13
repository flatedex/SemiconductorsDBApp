using MySql.Data.MySqlClient;
using System.Data;
using System.Windows;

namespace SemiconductorsDBApp
{
    /// <summary>
    /// Interaction logic for ViewOperations.xaml
    /// </summary>
    public partial class ViewOperations : Window
    {
        private int po_id;
        private DataTable operations;
        public ViewOperations(int id)
        {
            InitializeComponent();
            po_id = id;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            operations = new DataTable();
            operations.Columns.Add("title");
            operations.Columns.Add("material");
            using (MySqlConnection connection = DB.Connect())
            {
                connection.Open();
                string query = "SELECT `operation`.title, `materials`.title FROM `wip_materials` INNER JOIN `materials` ON `wip_materials`.m_id = `materials`.m_id " +
                  $"INNER JOIN `operation` ON `wip_materials`.o_id = `operation`.o_id WHERE po_id = {po_id};";
                MySqlCommand command = new MySqlCommand();
                command.CommandText = query;
                command.Connection = connection;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            DataRow row = operations.NewRow();
                            row["title"] = reader[0].ToString();
                            row["material"] = reader[1].ToString();
                            operations.Rows.Add(row);
                        }
                    }
                }
                Operations.ItemsSource = operations.DefaultView;
            }
        }
    }
}
