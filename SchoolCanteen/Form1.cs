using SchoolCanteen.Forms;
using System;
using System.Windows.Forms;

namespace SchoolCanteen
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new MenuForm().ShowDialog();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Hide();
            new AuthForm().ShowDialog();

        }
    }
}
