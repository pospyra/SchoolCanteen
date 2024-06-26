﻿using System.Data.SqlClient;
using System;
using System.Windows.Forms;
using SchoolCanteen.Data;
using System.Data;

namespace SchoolCanteen.Forms.Cashier
{
    public partial class CreateOrderForm : Form
    {
        private DatabaseManager databaseManager;
        private CashierPanelForm cashierPanelForm;
        public CreateOrderForm(CashierPanelForm panelForm)
        {
            InitializeComponent();
            databaseManager = new DatabaseManager();
            cashierPanelForm = panelForm;
            InitializeRowInDataGridView();
            FillBlydo();
        }

        private void InitializeRowInDataGridView()
        {
            dataGridView2.Columns.Add("DishID", "Номер блюда");
            dataGridView2.Columns.Add("Name", "Название блюда");
            dataGridView2.Columns.Add("Price", "Цена");
            dataGridView2.Columns.Add("Quantity", "Количество");

            textBox2.Text = 1.ToString();
            dataGridView2.Columns["Quantity"].ReadOnly = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Проверяем, есть ли строки в dataGridView2
            if (dataGridView2.Rows.Count == 1 && dataGridView2.Rows[0].IsNewRow)
            {
                MessageBox.Show("Заказ пустой.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveOrderDetailsToDatabase();

            cashierPanelForm.FillOrders();
            Hide();
        }


        public void FillBlydo()
        {
            if (databaseManager != null)
            {
                databaseManager.Fill("Dishes", dataGridView1);

                // Замена названий столбцов на русские
                dataGridView1.Columns["DishId"].HeaderText = "Номер";
                dataGridView1.Columns["Name"].HeaderText = "Название";
                dataGridView1.Columns["Ingredients"].HeaderText = "Ингредиенты";
                dataGridView1.Columns["Price"].HeaderText = "Цена";
                dataGridView1.Columns["Quantity"].HeaderText = "Количество";
                dataGridView1.Columns["Weight"].HeaderText = "Вес";
            }
        }

        private int AddOrderAndGetId()
        {
            int cashierID = DataStorage.CurrentUserId;
            DateTime orderTime = DateTime.Now; // Получаем текущее время
            bool isPaymentByCard = checkBox1.Checked;
            // Формируем SQL-запрос на добавление заказа в таблицу Orders
            string insertOrderQuery = @"
                                        INSERT INTO Orders (CashierID, OrderTime, IsPaymentByCard)
                                        VALUES (@CashierID, @OrderTime, @IsPaymentByCard);
                                        SELECT SCOPE_IDENTITY();"; // Получаем последний идентификатор (OrderID)

            // Создаем параметры для передачи значений в SQL-запрос
            SqlParameter[] parameters =
            {
                new SqlParameter("@CashierID", cashierID),
                new SqlParameter("@OrderTime", orderTime),
                new SqlParameter("@IsPaymentByCard", isPaymentByCard)
            };

            // Выполняем SQL-запрос и получаем OrderID
            object orderIdObj = databaseManager.ExecuteScalar(insertOrderQuery, parameters);
            int orderId = Convert.ToInt32(orderIdObj);

            // Возвращаем OrderID
            return orderId;
        }

        private void SaveOrderDetailsToDatabase()
        {
            // Добавляем заказ и получаем его OrderID
            int orderId = AddOrderAndGetId();

            // Проверяем, был ли успешно добавлен заказ
            if (orderId > 0)
            {
                // Добавляем детали заказа в таблицу OrderDetails, используя полученный OrderID
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (row.Cells["DishID"].Value != null)
                    {
                        // Получаем данные о блюде из DataGridView заказанных блюд
                        string dishId = row.Cells["DishID"].Value.ToString();
                        int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);

                        // Формируем SQL-запрос на добавление данных о блюде в заказ в базу данных
                        string insertOrderDetailQuery = @"
                                                      INSERT INTO OrderDetails (OrderID, DishID, Quantity)
                                                      VALUES (@OrderID, @DishID, @Quantity)";

                        // Создаем параметры для передачи значений в SQL-запрос
                        SqlParameter[] detailParameters =
                        {
                            new SqlParameter("@OrderID", orderId),
                            new SqlParameter("@DishID", dishId),
                            new SqlParameter("@Quantity", quantity)
                        };

                        // Выполняем SQL-запрос
                        databaseManager.ExecuteCommand(insertOrderDetailQuery, detailParameters);

                        UpdateDishQuantity(dishId, quantity);
                    }
                }

                // Выводим сообщение об успешном сохранении данных о блюдах в заказе
                MessageBox.Show("Заказ оформлен.");
            }
            else
            {
                // Выводим сообщение об ошибке при добавлении заказа
                MessageBox.Show("Ошибка при добавлении заказа.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (dataGridView1.SelectedRows.Count > 0)
            {
                int count = int.Parse(textBox2.Text);
                var quantity = int.Parse(dataGridView1.SelectedRows[0].Cells["Quantity"].Value.ToString());
                if (count <= 0 || count > quantity)
                {
                    label4.Text = $"Осталось {quantity}";
                    return;
                }
                else
                {
                    label4.Text = string.Empty;
                }

                // Получаем выбранное блюдо из доступных блюд
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                string dishId = selectedRow.Cells["DishID"].Value.ToString();
                string dishName = selectedRow.Cells["Name"].Value.ToString();
                decimal price = Convert.ToDecimal(selectedRow.Cells["Price"].Value);

                // Проверяем, есть ли уже такое блюдо в заказе
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (row.Cells["DishID"].Value != null && row.Cells["DishID"].Value.ToString() == dishId)
                    {
                        MessageBox.Show("Блюдо уже добавлено.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Добавляем выбранное блюдо в заказ (во второй DataGridView)
                dataGridView2.Rows.Add(dishId, dishName, price, count);

                int newQuantity = quantity - count;
                dataGridView1.SelectedRows[0].Cells["Quantity"].Value = newQuantity;

                CalculateTotalAmount();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string searchTerm = textBox1.Text;

            // Формируем запрос SQL с использованием оператора LIKE и параметра для безопасности
            string query = "SELECT * FROM Dishes WHERE Name LIKE @SearchTerm";

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
            if (dataGridView2.SelectedRows.Count > 0)
            {
                int selectedIndex = dataGridView2.SelectedRows[0].Index;

                if (selectedIndex != -1 && !dataGridView2.Rows[selectedIndex].IsNewRow)
                {
                    dataGridView2.Rows.RemoveAt(selectedIndex);
                    CalculateTotalAmount();
                }
                else
                {
                    MessageBox.Show("Выберите строку заказа для удаления.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Выберите строку заказа для удаления.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CalculateTotalAmount()
        {
            decimal totalAmount = 0;

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.Cells["Price"].Value != null && row.Cells["Quantity"].Value != null)
                {
                    decimal price = Convert.ToDecimal(row.Cells["Price"].Value);
                    int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                    totalAmount += price * quantity;
                }
            }

            label6.Text = $"Сумма к оплате: {totalAmount} руб.";
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            CalculateTotalAmount();
        }
        private void UpdateDishQuantity(string dishId, int orderedQuantity)
        {
            try
            {
                // Формируем SQL-запрос на обновление количества блюд
                string updateQuery = @"UPDATE Dishes
                               SET Quantity = Quantity - @OrderedQuantity
                               WHERE DishID = @DishID";

                // Создаем параметры для передачи значений в SQL-запрос
                SqlParameter[] parameters =
                {
                    new SqlParameter("@OrderedQuantity", orderedQuantity),
                    new SqlParameter("@DishID", dishId)
                };

                // Выполняем SQL-запрос
                databaseManager.ExecuteCommand(updateQuery, parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении количества блюд: " + ex.Message);
            }
        }

    }
}
