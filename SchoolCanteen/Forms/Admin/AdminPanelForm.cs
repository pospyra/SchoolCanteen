using SchoolCanteen.Data;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace SchoolCanteen.Forms.Admin
{
    public partial class AdminPanelForm : Form
    {
        private readonly DatabaseManager databaseManager;
        public AdminPanelForm()
        {
            InitializeComponent();
            databaseManager = new DatabaseManager();
            FillUsers();
            FillOrders();
            FillBlydo();
        }

        public void FillOrders()
        {
            if (databaseManager != null)
            {
                string query = @"
                                 SELECT 
                                     Orders.OrderID AS 'Номер заказа',
                                     Users.FullName AS 'Кассир',
                                     Orders.OrderTime AS 'Время заказа'
                                 FROM 
                                     Orders
                                 INNER JOIN 
                                     Users ON Orders.CashierID = Users.UserID";

                // Получаем данные заказов с именем кассира
                DataTable ordersTable = databaseManager.GetData(query);

                if (ordersTable != null)
                {
                    dataGridView4.DataSource = ordersTable;

                    // Установка ширины столбца с временем заказа
                    dataGridView4.Columns["Время заказа"].Width = 120;
                }
            }
        }

        private void FillDetailsOrder()
        {
            if (dataGridView4.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView4.SelectedRows[0];
                if (selectedRow.Cells[0].Value != null)
                {
                    var orderID = selectedRow.Cells[0].Value.ToString();

                    string queryDetailsOrder = @"
                        SELECT OD.OrderID, D.Name AS 'Name', OD.Quantity AS 'Quantity'
                        FROM OrderDetails OD
                        JOIN Dishes D ON OD.DishID = D.DishID
                        WHERE OD.OrderID = @OrderId";
                    SqlParameter parameterDetailsOrder = new SqlParameter("@OrderId", orderID);

                    DataTable detailsData = databaseManager.GetData(queryDetailsOrder, new SqlParameter[] { parameterDetailsOrder });

                    // Замена названий столбцов на русские
                    detailsData.Columns["OrderID"].ColumnName = "Номер заказа";
                    detailsData.Columns["Name"].ColumnName = "Название блюда";
                    detailsData.Columns["Quantity"].ColumnName = "Количество";

                    dataGridView5.DataSource = detailsData;

                    dataGridView5.Columns[1].Width = 150;
                }
            }
        }


        public void FillBlydo()
        {
            if (databaseManager != null)
            {
                databaseManager.Fill("Dishes", dataGridView3);

                // Замена названий столбцов на русские
                dataGridView3.Columns["DishID"].HeaderText = "Номер";
                dataGridView3.Columns["Name"].HeaderText = "Название";
                dataGridView3.Columns["Ingredients"].HeaderText = "Ингредиенты";
                dataGridView3.Columns["Price"].HeaderText = "Цена";
                dataGridView3.Columns["Quantity"].HeaderText = "Количество";
                dataGridView3.Columns["Weight"].HeaderText = "Вес";

                dataGridView3.Columns[0].Width = 50;
                dataGridView3.Columns[1].Width = 150;

                dataGridView3.Columns[2].Width = 200;
                dataGridView3.Columns[3].Width = 80;
                dataGridView3.Columns[4].Width = 50;
                dataGridView3.Columns[5].Width = 80;

            }
        }

        public void FillUsers()
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

                dataGridView1.Columns[1].Width = 200; 

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

                    dataGridView2.Columns[2].Width = 140;
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            FillEmployeeShifts();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new CreateEmployeeForm(this).ShowDialog();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string searchTerm = textBox1.Text;

            // Формируем запрос SQL с использованием оператора LIKE и параметра для безопасности
            string query = "SELECT * FROM Users WHERE FullName LIKE @SearchTerm";

            // Создаем параметр для передачи значения поиска
            SqlParameter parameter = new SqlParameter("@SearchTerm", SqlDbType.NVarChar)
            {
                Value = "%" + searchTerm + "%" // Добавляем знаки % для поиска по части имени
            };

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

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            string searchTerm = textBox2.Text;

            // Формируем запрос SQL с использованием оператора LIKE и параметра для безопасности
            string query = "SELECT * FROM Dishes WHERE Name LIKE @SearchTerm";

            // Создаем параметр для передачи значения поиска
            SqlParameter parameter = new SqlParameter("@SearchTerm", SqlDbType.NVarChar)
            {
                Value = "%" + searchTerm + "%" // Добавляем знаки % для поиска по части имени
            };

            // Получаем данные из базы данных
            DataTable dt = databaseManager.GetData(query, new SqlParameter[] { parameter });

            // Заполняем dataGridView блюдами
            dataGridView3.DataSource = dt;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView3.SelectedRows.Count > 0 && dataGridView3.SelectedRows[0].Cells[0].Value != null)
            {
                if (int.TryParse(dataGridView3.SelectedRows[0].Cells[0].Value.ToString(), out int dishId))
                {
                    new UpdateDishesForm(this, dishId).ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите блюдо для редактирования.");
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView3.SelectedRows.Count > 0 && dataGridView3.SelectedRows[0].Cells[0].Value != null)
            {
                if (int.TryParse(dataGridView3.SelectedRows[0].Cells[0].Value.ToString(), out int dishId))
                {
                    DeleteDish(dishId);
                }
                else
                {
                    MessageBox.Show("Некорректный формат номера");
                }
            }
            else
            {
                MessageBox.Show("Выберите блюдо.");
            }
        }

        public void DeleteDish(int dishId)
        {
            if (dishId <= 0)
            {
                MessageBox.Show("Выберите Блюдо");
                return;
            }

            string deleteQuery = "DELETE FROM Dishes WHERE DishID = @DishId";

            SqlParameter parameterDishId = new SqlParameter("@DishId", dishId);

            databaseManager.ExecuteCommand(deleteQuery, new SqlParameter[] { parameterDishId });
            FillBlydo();

            MessageBox.Show("Блюдо удалено из списка");
        }

        private void dataGridView4_SelectionChanged(object sender, EventArgs e)
        {
            FillDetailsOrder();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0 && dataGridView1.SelectedRows[0].Cells[0].Value != null)
            {
                if (int.TryParse(dataGridView1.SelectedRows[0].Cells[0].Value.ToString(), out int userId))
                {
                    new UpdateEmployeeForm(this, userId).ShowDialog();
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

        private void button5_Click(object sender, EventArgs e)
        {
            new CreateDishesForm(this).ShowDialog();
        }

    }
}
