using SchoolCanteen.Data;
using System;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace SchoolCanteen.Forms
{
    public partial class MenuForm : Form
    {
        private DatabaseManager databaseManager;
        public MenuForm()
        {
            InitializeComponent();
            databaseManager = new DatabaseManager();
            FillBlydo();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Добавить проверку на null dataGridView1 SelectedRows
            var productId = int.Parse(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
            new ProductInfoForm(productId).ShowDialog();
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
    }
}

