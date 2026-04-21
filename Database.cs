using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace SportResult
{
    public static class Database
    {
        public static string dbPath = Path.Combine(Application.StartupPath, "Sport.db");
        public static string ConnectionString = $"Data Source={dbPath};Version=3;";

        public static SQLiteConnection GetConnection()
        {
            var conn = new SQLiteConnection(ConnectionString);
            conn.Open();
            return conn;
        }
    }
}