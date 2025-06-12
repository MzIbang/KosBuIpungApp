// ===== Views/LoginForm.cs =====
using KosBuIpungApp.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace KosBuIpungApp.Views
{
    public class LoginForm : Form
    {
        private Label lblTitle, lblUsername, lblPassword;
        private TextBox txtUsername, txtPassword;
        private Button btnLogin, btnRegister;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form Properties
            this.Text = "Login - Sistem Informasi Kos Bu Ipung";
            this.Size = new Size(400, 320);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Title Label
            lblTitle = new Label();
            lblTitle.Text = "Selamat Datang!";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.Location = new Point(0, 20);
            lblTitle.Size = new Size(this.ClientSize.Width, 40);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(lblTitle);

            // Username
            lblUsername = new Label();
            lblUsername.Text = "Username";
            lblUsername.Location = new Point(40, 80);
            this.Controls.Add(lblUsername);

            txtUsername = new TextBox();
            txtUsername.Location = new Point(40, 105);
            txtUsername.Size = new Size(300, 25);
            txtUsername.Font = new Font("Segoe UI", 10);
            this.Controls.Add(txtUsername);

            // Password
            lblPassword = new Label();
            lblPassword.Text = "Password";
            lblPassword.Location = new Point(40, 145);
            this.Controls.Add(lblPassword);

            txtPassword = new TextBox();
            txtPassword.Location = new Point(40, 170);
            txtPassword.Size = new Size(300, 25);
            txtPassword.Font = new Font("Segoe UI", 10);
            txtPassword.PasswordChar = '*';
            this.Controls.Add(txtPassword);

            // Login Button
            btnLogin = new Button();
            btnLogin.Text = "Login";
            btnLogin.Location = new Point(40, 220);
            btnLogin.Size = new Size(145, 40);
            btnLogin.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnLogin.BackColor = Color.DodgerBlue;
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Click += BtnLogin_Click;
            this.Controls.Add(btnLogin);
            
            // Register Button
            btnRegister = new Button();
            btnRegister.Text = "Registrasi";
            btnRegister.Location = new Point(195, 220);
            btnRegister.Size = new Size(145, 40);
            btnRegister.Font = new Font("Segoe UI", 10);
            btnRegister.Click += BtnRegister_Click;
            this.Controls.Add(btnRegister);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Username dan password tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (AuthService.Login(username, password))
            {
                this.Hide();
                Form dashboard;
                if (AuthService.CurrentUser.Role == Enums.UserRole.Admin)
                {
                    dashboard = new AdminDashboardForm();
                }
                else
                {
                    dashboard = new UserDashboardForm();
                }
                dashboard.Show();
            }
            else
            {
                MessageBox.Show("Username atau password salah!", "Login Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
             MessageBox.Show("Fitur registrasi dapat diakses melalui admin atau hubungi pengelola kos.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}