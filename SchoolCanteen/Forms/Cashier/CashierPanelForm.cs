using SchoolCanteen.Data;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SchoolCanteen.Forms.Cashier
{
    public partial class CashierPanelForm : Form
    {
        private DatabaseManager databaseManager;
        public CashierPanelForm()
        {
            InitializeComponent();
            databaseManager = new DatabaseManager();
            FillOrders();
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
                    dataGridView1.DataSource = ordersTable;

                    // Установка ширины столбца с временем заказа
                    dataGridView1.Columns["Время заказа"].Width = 120;
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            FillDetailsOrder();
        }

        private void FillDetailsOrder()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView1.SelectedRows[0];
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

                    dataGridView2.DataSource = detailsData;

                    dataGridView2.Columns[1].Width = 150;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new CreateOrderForm(this).ShowDialog();
        }
    }
}
