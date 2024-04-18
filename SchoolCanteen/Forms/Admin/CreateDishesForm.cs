using SchoolCanteen.Data;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SchoolCanteen.Forms.Admin
{
    public partial class CreateDishesForm : Form
    {
        private readonly DatabaseManager databaseManager;
        private readonly AdminPanelForm adminPanel;
        public CreateDishesForm(AdminPanelForm adminPanelForm)
        {
            InitializeComponent();
            databaseManager = new DatabaseManager();
            adminPanel = adminPanelForm;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text;
            string ingredients = textBox2.Text;
            decimal price;
            int quantity;
            decimal weight;

            // Проверяем корректность введенных значений и выполняем добавление блюда
            if (decimal.TryParse(textBox3.Text, out price) && int.TryParse(textBox4.Text, out quantity) && decimal.TryParse(textBox5.Text, out weight))
            {
                // Вызываем метод AddDish, передавая ему значения из текстовых полей
                AddDish(name, ingredients, price, quantity, weight);
            }
            else
            {
                MessageBox.Show("Некорректные данные. Пожалуйста, убедитесь, что поля 'Цена', 'Количество' и 'Вес' содержат числовые значения.");
            }
        }

        private void AddDish(string name, string ingredients, decimal price, int quantity, decimal weight)
        {
            try
            {
                string insertQuery = @"INSERT INTO Dishes (Name, Ingredients, Price, Quantity, Weight)
                               VALUES (@Name, @Ingredients, @Price, @Quantity, @Weight)";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@Name", name),
                    new SqlParameter("@Ingredients", ingredients),
                    new SqlParameter("@Price", price),
                    new SqlParameter("@Quantity", quantity),
                    new SqlParameter("@Weight", weight)
                };

                databaseManager.ExecuteCommand(insertQuery, parameters);

                MessageBox.Show("Блюдо успешно добавлено.");
                adminPanel.FillBlydo();
                Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении блюда: " + ex.Message);
            }
        }

    }
}
