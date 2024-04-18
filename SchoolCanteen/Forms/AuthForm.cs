using SchoolCanteen.Data;
using System;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
using SchoolCanteen.Forms.Cashier;
using SchoolCanteen.Forms.Cook;
using SchoolCanteen.Forms.Admin;

namespace SchoolCanteen.Forms
{
    public partial class AuthForm : Form
    {
        private DatabaseManager databaseManager;
        public AuthForm()
        {
            InitializeComponent();
            databaseManager = new DatabaseManager();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var login = textBox1.Text;
            var pass = textBox2.Text;

            var role = GetUserRole(login, pass);
            if (role != null)
            {
                switch (role)
                {
                    case "admin":
                        new AdminPanelForm().Show();
                        break;
                    case "cashier":
                        new CashierPanelForm().Show();
                        break;
                    case "cook":
                        new OrdersForm().Show();
                        break;
                    case "head_cook":
                        // Добавьте обработку для главного повара
                        break;
                    default:
                        // Добавьте обработку для остальных ролей
                        break;
                }
                Hide();
            }
            else
            {
                MessageBox.Show("Неправильный логин или пароль", "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public string GetUserRole(string username, string password)
        {
            string query = "SELECT UserID, RoleName FROM Users " +
                           "INNER JOIN Roles ON Users.RoleID = Roles.RoleID " +
                           "WHERE Login = @Username AND Password = @Password";

            SqlParameter[] parameters =
            {
                new SqlParameter("@Username", username),
                new SqlParameter("@Password", password)
            };

            DataTable result = databaseManager.GetData(query, parameters);

            // Проверяем результат выполнения запроса
            if (result != null && result.Rows.Count > 0)
            {
                DataStorage.CurrentUserId = Convert.ToInt32(result.Rows[0]["UserID"]);
                return result.Rows[0]["RoleName"].ToString();
            }
            else
            {
                return null;
            }
        }

    }
}
