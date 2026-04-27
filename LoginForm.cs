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

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Введите логин и пароль!");
                return;
            }

            using (var conn = Database.GetConnection())
            {
                string sql = "SELECT Role FROM Users WHERE Login=@login AND Password=@pass";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@login", login);
                cmd.Parameters.AddWithValue("@pass", pass);

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
    }
}