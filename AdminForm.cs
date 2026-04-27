using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace SportResult
{
    public partial class AdminForm : Form
    {
        private DataTable table;
        private SQLiteConnection connection;

        public AdminForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            connection = Database.GetConnection();

            string sql = @"
                SELECT 
                    r.Id,
                    s.FullName AS Спортсмен,
                    c.Name AS Соревнование,
                    sp.Name AS Вид_спорта,
                    r.Result AS Результат,
                    r.Place AS Место
                FROM Results r
                JOIN Sportsmen s ON r.SportsmanId = s.Id
                JOIN Competitions c ON r.CompetitionId = c.Id
                JOIN Sports sp ON c.SportId = sp.Id";

            SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql, connection);
            table = new DataTable();
            adapter.Fill(table);
            dataGridView1.DataSource = table;
            dataGridView1.Columns["Id"].Visible = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.EndEdit();

                foreach (DataRow row in table.Rows)
                {
                    if (row.RowState == DataRowState.Modified)
                    {
                        string updateSql = "UPDATE Results SET Result = @result, Place = @place WHERE Id = @id";
                        SQLiteCommand cmd = new SQLiteCommand(updateSql, connection);
                        cmd.Parameters.AddWithValue("@result", row["Результат"].ToString());
                        cmd.Parameters.AddWithValue("@place", Convert.ToInt32(row["Место"]));
                        cmd.Parameters.AddWithValue("@id", Convert.ToInt32(row["Id"]));
                        cmd.ExecuteNonQuery();
                    }
                    else if (row.RowState == DataRowState.Added)
                    {
                        string insertSql = "INSERT INTO Results (SportsmanId, CompetitionId, Result, Place) VALUES (1, 1, @result, @place)";
                        SQLiteCommand cmd = new SQLiteCommand(insertSql, connection);
                        cmd.Parameters.AddWithValue("@result", row["Результат"].ToString());
                        cmd.Parameters.AddWithValue("@place", Convert.ToInt32(row["Место"]));
                        cmd.ExecuteNonQuery();
                    }
                }

                table.AcceptChanges();
                MessageBox.Show("Сохранено!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && !dataGridView1.CurrentRow.IsNewRow)
            {
                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["Id"].Value);

                string deleteSql = "DELETE FROM Results WHERE Id = @id";
                SQLiteCommand cmd = new SQLiteCommand(deleteSql, connection);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();

                dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
            }
            Application.Exit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
            }
            base.OnFormClosing(e);
        }
    }
}