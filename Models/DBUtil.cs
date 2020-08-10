using MySql.Data.MySqlClient;

namespace Landtapi.Models
{
    public static class DBUtil
    {
        public static MySqlConnection conn = null;
        private static readonly string sql = "server=rmk-landt.cqh5tsnrn3ol.us-east-2.rds.amazonaws.com;userid=root;password=rmkcse104;database=land_db;Connection Timeout=180"; 

        public static MySqlConnection OpenConnection()
        {
            conn = new MySqlConnection(sql);
            conn.Open();

            return conn;
        }
    }
}