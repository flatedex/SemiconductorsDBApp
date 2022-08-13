using MySql.Data.MySqlClient;

namespace SemiconductorsDBApp
{
    public static class DB
    {
        private static string host = "localhost";
        private static string port = "3306";
        private static string username = "root";
        private static string password = "8876";
        private static string database = "semiconductors";

        public static MySqlConnection Connect()
        {
            string connectionParams = $"Server={host};Database={database};port={port};User Id={username};password={password}";
            return new MySqlConnection(connectionParams);
        }
    }
}
