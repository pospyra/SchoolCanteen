using SchoolCanteen.Forms.Admin;
using SchoolCanteen.Forms.Cashier;
using System;
using System.Windows.Forms;

namespace SchoolCanteen
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AdminPanelForm());
        }
    }
}
