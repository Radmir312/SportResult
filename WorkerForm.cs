using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace SportResult
{
    public partial class WorkerForm : Form
    {
        private DataTable table;

        public WorkerForm()
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
                        s.FullName AS Спортсмен,
                        s.Team AS Команда,
                        c.Name AS Соревнование,
                        c.Date AS Дата,
                        sp.Name AS Вид_спорта,
                        r.Result AS Результат,
                        r.Place AS Место
                    FROM Results r
                    JOIN Sportsmen s ON r.SportsmanId = s.Id
                    JOIN Competitions c ON r.CompetitionId = c.Id
                    JOIN Sports sp ON c.SportId = sp.Id";

                SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql, conn);
                table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string search = txtSearch.Text;
            table.DefaultView.RowFilter = $"Спортсмен LIKE '%{search}%' OR Команда LIKE '%{search}%' OR Вид_спорта LIKE '%{search}%' OR Соревнование LIKE '%{search}%'";
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV файлы (*.csv)|*.csv";
            sfd.FileName = "Результаты.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
                {
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        sw.Write(dataGridView1.Columns[i].HeaderText + ";");
                    }
                    sw.WriteLine();

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            for (int i = 0; i < dataGridView1.Columns.Count; i++)
                            {
                                sw.Write(row.Cells[i].Value + ";");
                            }
                            sw.WriteLine();
                        }
                    }
                }
                MessageBox.Show("Файл сохранен!");
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}