// ===== Program.cs =====
using System;
using System.Windows.Forms;

namespace KosBuIpungApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Inisialisasi data dummy untuk simulasi
            Services.DataService.InitializeData();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Views.LoginForm());
        }
    }
}