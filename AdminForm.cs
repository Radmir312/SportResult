using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace SportResult
{
    public partial class AdminForm : Form
    {
        private DataTable table;
        private SQLiteDataAdapter adapter;

        public AdminForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var conn = Database.GetConnection())
            {
                string sql = @"
                    SELECT 
                        r.Id,
                        s.FullName AS Спортсмен,
                        c.Name AS Соревнование,
                        r.Result AS Результат,
                        r.Place AS Место
                    FROM Results r
                    JOIN Sportsmen s ON r.SportsmanId = s.Id
                    JOIN Competitions c ON r.CompetitionId = c.Id";

                adapter = new SQLiteDataAdapter(sql, conn);
                SQLiteCommandBuilder builder = new SQLiteCommandBuilder(adapter);

                table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                dataGridView1.Columns["Id"].Visible = false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                adapter.Update(table);
                MessageBox.Show("Изменения сохранены!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            new LoginForm().Show();
            this.Close();
        }
    }
}