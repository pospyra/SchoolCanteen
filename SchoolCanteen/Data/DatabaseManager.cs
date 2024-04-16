using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace SchoolCanteen.Data
{
    internal class DatabaseManager
    {
        private SqlConnection connection;
        private static string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""{Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName}\DB\RestaurantCanteen.mdf"";Integrated Security=True;Connect Timeout=30"; 
        public DatabaseManager()
        {
            connection = new SqlConnection(connectionString);
        }

        public void Fill(string tableName, DataGridView grid)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand($"SELECT  * FROM [{tableName}]", conn);
            adapter.SelectCommand = cmd;
            adapter.Fill(table);
            grid.DataSource = table;
        }

        // Метод для получения данных из базы данных
        public DataTable GetData(string query, SqlParameter[] parameters = null)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }

            return dt;
        }

        // Метод для выполнения команд SQL (INSERT, UPDATE, DELETE)
        public void ExecuteCommand(string command, SqlParameter[] parameters = null)
        {
            SqlCommand cmd = new SqlCommand(command, connection);

            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }

            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    return command.ExecuteScalar();
                }
            }
        }



        public void ExecuteStoredProcedure(string procedureName, SqlParameter[] parameters = null)
        {
            SqlCommand cmd = new SqlCommand(procedureName, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    cmd.Parameters.Add(param);
                }
            }

            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }
    }
}
