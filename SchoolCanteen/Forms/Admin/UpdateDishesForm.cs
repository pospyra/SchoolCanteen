using SchoolCanteen.Data;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
using System;

namespace SchoolCanteen.Forms.Admin
{
    public partial class UpdateDishesForm : Form
    {
        private readonly DatabaseManager databaseManager;
        private readonly AdminPanelForm adminPanelForm;
        private readonly int dishId;
        public UpdateDishesForm(AdminPanelForm adminPanel, int dishId)
        {
            InitializeComponent();
            databaseManager = new DatabaseManager();
            adminPanelForm = adminPanel;
            this.dishId = dishId;

            FillDishDetails(dishId);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text;
            string ingredients = textBox2.Text;
            decimal price;
            int quantity;
            decimal weight;

            // Проверка корректности введенных данных
            if (!decimal.TryParse(textBox3.Text, out price))
            {
                MessageBox.Show("Пожалуйста, введите корректную цену.");
                return;
            }

            if (!int.TryParse(textBox4.Text, out quantity))
            {
                MessageBox.Show("Пожалуйста, введите корректное количество.");
                return;
            }

            if (!decimal.TryParse(textBox5.Text, out weight))
            {
                MessageBox.Show("Пожалуйста, введите корректный вес.");
                return;
            }

            // Вызов метода обновления блюда
            UpdateDish(name, ingredients, price, quantity, weight);

            adminPanelForm.FillBlydo();
            Hide();
        }


        private void FillDishDetails(int dishId)
        {
            string query = "SELECT Name, Ingredients, Price, Quantity, Weight FROM Dishes WHERE DishID = @DishID";
            SqlParameter[] parameters = { new SqlParameter("@DishID", dishId) };

            DataTable dishData = databaseManager.GetData(query, parameters);

            if (dishData != null && dishData.Rows.Count > 0)
            {
                textBox1.Text = dishData.Rows[0]["Name"].ToString();
                textBox2.Text = dishData.Rows[0]["Ingredients"].ToString();
                textBox3.Text = dishData.Rows[0]["Price"].ToString();
                textBox4.Text = dishData.Rows[0]["Quantity"].ToString();
                textBox5.Text = dishData.Rows[0]["Weight"].ToString();
            }
        }

        private void UpdateDish(string name, string ingredients, decimal price, int quantity, decimal weight)
        {
            try
            {
                string updateQuery = @"UPDATE Dishes
                                       SET Name = @Name,
                                           Ingredients = @Ingredients,
                                           Price = @Price,
                                           Quantity = @Quantity,
                                           Weight = @Weight
                                       WHERE DishID = @DishID";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@Name", name),
                    new SqlParameter("@Ingredients", ingredients),
                    new SqlParameter("@Price", price),
                    new SqlParameter("@Quantity", quantity),
                    new SqlParameter("@Weight", weight),
                    new SqlParameter("@DishID", dishId)
                };

                databaseManager.ExecuteCommand(updateQuery, parameters);
                MessageBox.Show("Блюдо успешно обновлено.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении блюда: " + ex.Message);
            }
        }
    }
}
