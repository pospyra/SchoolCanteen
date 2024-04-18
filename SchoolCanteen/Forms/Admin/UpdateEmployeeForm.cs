using SchoolCanteen.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SchoolCanteen.Forms.Admin
{
    public partial class UpdateEmployeeForm : Form
    {
        private readonly DatabaseManager databaseManager;
        private readonly AdminPanelForm adminPanelForm;
        private readonly int userId;

        public UpdateEmployeeForm(AdminPanelForm adminPanel, int userId)
        {
            InitializeComponent();
            databaseManager = new DatabaseManager();
            adminPanelForm = adminPanel;
            this.userId = userId;
            FillRoles();

            // Заполнение данными о сотруднике для редактирования
            FillEmployeeData(userId);
            //FillShift(userId);
        }

        private void FillEmployeeData(int userId)
        {
            string query = "SELECT FullName, Login, Password, Phone, RoleID FROM Users WHERE UserID = @UserID";
            SqlParameter[] parameters =
               {
                    new SqlParameter("@UserID", userId),
                };

            DataTable userData = databaseManager.GetData(query, parameters);

            if (userData != null && userData.Rows.Count > 0)
            {
                textBox1.Text = userData.Rows[0]["FullName"].ToString();
                textBox2.Text = userData.Rows[0]["Login"].ToString();
                textBox3.Text = userData.Rows[0]["Password"].ToString();
                textBox4.Text = userData.Rows[0]["Phone"].ToString();

                // Установка выбранной роли
                int roleId = Convert.ToInt32(userData.Rows[0]["RoleID"]);
                for (int i = 0; i < comboBox1.Items.Count; i++)
                {
                    var item = (KeyValuePair<int, string>)comboBox1.Items[i];
                    if (item.Key == roleId)
                    {
                        comboBox1.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void FillShift(int userId)
        {
            
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

        public void EditEmployee(int userId, string fio, string login, string password, string phone, int roleId)
        {
            if (string.IsNullOrEmpty(fio) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            try
            {
                string updateQuery = @"UPDATE Users 
                               SET FullName = @FIO, 
                                   Login = @Login, 
                                   Password = @Password, 
                                   Phone = @Phone, 
                                   RoleID = @RoleID 
                               WHERE UserID = @UserID";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@FIO", fio),
                    new SqlParameter("@Login", login),
                    new SqlParameter("@Password", password),
                    new SqlParameter("@Phone", phone),
                    new SqlParameter("@RoleID", roleId),
                    new SqlParameter("@UserID", userId)
                };

                databaseManager.ExecuteCommand(updateQuery, parameters);
                MessageBox.Show("Данные о сотруднике успешно обновлены.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении данных о сотруднике: " + ex.Message);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string fio = textBox1.Text;
            string login = textBox2.Text;
            string password = textBox3.Text;
            string phone = textBox4.Text;
            int roleId = ((KeyValuePair<int, string>)comboBox1.SelectedItem).Key;

            // Редактирование данных сотрудника
            EditEmployee(userId, fio, login, password, phone, roleId);

            adminPanelForm.FillUsers();
            Hide();
        }
    }
}
