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
                databaseManager.Fill("Orders", dataGridView1);

                // Замена названий столбцов на русские
                dataGridView1.Columns["OrderID"].HeaderText = "Номер заказа";
                dataGridView1.Columns["CashierID"].HeaderText = "ID кассира";
                dataGridView1.Columns["OrderTime"].HeaderText = "Время заказа";
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
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new CreateOrderForm(this).ShowDialog();
        }
    }
}
