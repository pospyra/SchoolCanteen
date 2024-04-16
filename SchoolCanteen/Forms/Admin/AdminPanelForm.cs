using SchoolCanteen.Data;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SchoolCanteen.Forms.Admin
{
    public partial class AdminPanelForm : Form
    {
        private DatabaseManager databaseManager;
        public AdminPanelForm()
        {
            InitializeComponent();
            databaseManager = new DatabaseManager();
            FillUsers();
        }

        void FillUsers()
        {
            if (databaseManager != null)
            {
                // Получаем данные о пользователях с информацией о ролях из базы данных
                string query = @"
                                SELECT Users.UserID, Users.FullName, Users.Login, Roles.RoleName
                                FROM Users
                                INNER JOIN Roles ON Users.RoleId = Roles.RoleId";

                DataTable dt = databaseManager.GetData(query);

                // Замена названий столбцов на русские
                dataGridView1.DataSource = dt;
                dataGridView1.Columns["UserID"].HeaderText = "Код пользователя";
                dataGridView1.Columns["FullName"].HeaderText = "Имя пользователя";
                dataGridView1.Columns["Login"].HeaderText = "Логин";
                dataGridView1.Columns["RoleName"].HeaderText = "Роль";
            }
        }

        private void FillEmployeeShifts()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView1.SelectedRows[0];
                if (selectedRow.Cells["UserID"].Value != null)
                {
                    int userID = Convert.ToInt32(selectedRow.Cells["UserID"].Value);

                    // Запрос к базе данных для получения графика работы сотрудника по его идентификатору с присоединением таблицы Users
                    string query = @"
                            SELECT es.ShiftStart, es.ShiftEnd, u.FullName
                            FROM EmployeeShifts es
                            INNER JOIN Users u ON es.UserID = u.UserID
                            WHERE es.UserID = @UserID";

                    SqlParameter[] parameters =
                    {
                        new SqlParameter("@UserID", userID)
                    };

                    DataTable scheduleTable = databaseManager.GetData(query, parameters);

                    dataGridView2.DataSource = scheduleTable;

                    dataGridView2.Columns["ShiftStart"].HeaderText = "Начало работы";
                    dataGridView2.Columns["ShiftEnd"].HeaderText = "Окончание работы";
                    dataGridView2.Columns["FullName"].HeaderText = "Имя пользователя";
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            FillEmployeeShifts();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string searchTerm = textBox1.Text;

            // Формируем запрос SQL с использованием оператора LIKE и параметра для безопасности
            string query = "SELECT * FROM Users WHERE FullName LIKE @SearchTerm";

            // Создаем параметр для передачи значения поиска
            SqlParameter parameter = new SqlParameter("@SearchTerm", SqlDbType.NVarChar);
            parameter.Value = "%" + searchTerm + "%"; // Добавляем знаки % для поиска по части имени

            // Получаем данные из базы данных
            DataTable dt = databaseManager.GetData(query, new SqlParameter[] { parameter });

            // Заполняем dataGridView блюдами
            dataGridView1.DataSource = dt;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0 && dataGridView1.SelectedRows[0].Cells[0].Value != null)
            {
                if (int.TryParse(dataGridView1.SelectedRows[0].Cells[0].Value.ToString(), out int employeeId))
                {
                    DeleteEmployee(employeeId);
                }
                else
                {
                    MessageBox.Show("Некорректный формат номера");
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника.");
            }
        }

        public void DeleteEmployee(int userId)
        {
            if (userId <= 0)
            {
                MessageBox.Show("Выберите сотрудника");
                return;
            }

            string deleteQuery = "DELETE FROM Users WHERE UserID = @UserId";

            SqlParameter parameterUserId = new SqlParameter("@UserId", userId);

            databaseManager.ExecuteCommand(deleteQuery, new SqlParameter[] { parameterUserId });
            FillUsers();

            MessageBox.Show("Сотрудник исключен из списка");
        }
    }
}
