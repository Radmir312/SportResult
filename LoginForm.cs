using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace SportResult
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string pass = txtPassword.Text.Trim();

            using (var conn = Database.GetConnection())
            {
                string sql = "SELECT Role FROM Users WHERE Login = '" + login + "' AND Password = '" + pass + "'";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);

                object result = cmd.ExecuteScalar();

                if (result != null && result.ToString() == "Admin")
                {
                    new AdminForm().Show();
                    this.Hide();
                }
                else if (result != null && result.ToString() == "Worker")
                {
                    new WorkerForm().Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль!");
                }
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            txtLogin.Text = "admin";
            txtPassword.Text = "123";
        }
    }
}