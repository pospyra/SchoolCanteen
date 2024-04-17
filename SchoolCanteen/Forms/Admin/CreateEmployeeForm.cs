using SchoolCanteen.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SchoolCanteen.Forms.Admin
{
    public partial class CreateEmployeeForm : Form
    {
        private readonly DatabaseManager databaseManager;
        private readonly AdminPanelForm adminPanelForm;
        public CreateEmployeeForm(AdminPanelForm adminPanel)
        {
            InitializeComponent();
            databaseManager = new DatabaseManager();
            adminPanelForm = adminPanel;
            FillRoles();

            dateTimePicker1.Format = DateTimePickerFormat.Time;
            dateTimePicker1.ShowUpDown = true;
            dateTimePicker2.Format = DateTimePickerFormat.Time;
            dateTimePicker2.ShowUpDown = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string fio = textBox1.Text;
            string login = textBox2.Text;
            string password = textBox3.Text;
            string phone = textBox4.Text;
            int roleId = ((KeyValuePair<int, string>)comboBox1.SelectedItem).Key;

            CreateEmployee(fio, login, password, phone, roleId);

            adminPanelForm.FillUsers();
            Hide();
        }

        public void FillRoles()
        {
            if (databaseManager != null)
            {
                string query = "SELECT RoleID, RoleName FROM Roles";

                DataTable rolesTable = databaseManager.GetData(query);
                if (rolesTable != null && rolesTable.Rows.Count > 0)
                {
                    foreach (DataRow row in rolesTable.Rows)
                    {
                        comboBox1.Items.Add(new KeyValuePair<int, string>((int)row["RoleID"], row["RoleName"].ToString()));
                    }
                    comboBox1.DisplayMember = "Value";
                    comboBox1.ValueMember = "Key";
                    comboBox1.SelectedIndex = 0;
                }
            }
        }

        public void CreateEmployee(string fio, string login, string password, string phone, int roleId)
        {
            // Проверка наличия пустых значений или неверных типов данных
            if (string.IsNullOrEmpty(fio) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            try
            {
                string insertQuery = @"INSERT INTO Users (FullName, Login, Password, Phone, RoleID)
                           VALUES (@FIO, @Login, @Password, @Phone, @RoleID);
                           SELECT SCOPE_IDENTITY();"; // Получаем идентификатор добавленного пользователя

                SqlParameter[] parameters =
                {
                    new SqlParameter("@FIO", fio),
                    new SqlParameter("@Login", login),
                    new SqlParameter("@Password", password),
                    new SqlParameter("@Phone", phone),
                    new SqlParameter("@RoleID", roleId)
                };

                object userIdObj = databaseManager.ExecuteScalar(insertQuery, parameters);
                int userId = Convert.ToInt32(userIdObj);

                // Проверяем, был ли успешно добавлен пользователь
                if (userId > 0)
                {
                    AddEmployeeShift(userId);
                    MessageBox.Show("Сотрудник успешно добавлен.");
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении сотрудника в базу данных.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении сотрудника: " + ex.Message);
            }
        }

        private void AddEmployeeShift(int userId)
        {
            try
            {
                // Извлекаем время из DateTimePicker для начала и окончания работы
                string startTime = dateTimePicker1.Value.ToString("HH:mm:ss");
                string endTime = dateTimePicker2.Value.ToString("HH:mm:ss");

                // Запрос на добавление данных о графике работы сотрудника
                string insertQuery = @"INSERT INTO EmployeeShifts (UserID, ShiftStart, ShiftEnd)
                       VALUES (@UserID, @StartTime, @EndTime)";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@StartTime", startTime),
                    new SqlParameter("@EndTime", endTime)
                };

                databaseManager.ExecuteCommand(insertQuery, parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении графика работы сотрудника: " + ex.Message);
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DateTime selectedDateTime = new DateTime(2000, 1, 1, dateTimePicker1.Value.Hour, dateTimePicker1.Value.Minute, 0);
            dateTimePicker1.Value = selectedDateTime;
        }
    }
}
