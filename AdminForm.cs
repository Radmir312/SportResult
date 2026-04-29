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
        }

        private void AdminForm_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
                connection.Dispose();
            }

            connection = Database.GetConnection();

            string sql = @"
                SELECT 
                    r.Id,
                    s.FullName AS Спортсмен,
                    c.Name AS Соревнование,
                    c.Date AS Дата,
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
            dataGridView1.AllowUserToAddRows = true;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string search = txtSearch.Text;
            table.DefaultView.RowFilter = $"Спортсмен LIKE '%{search}%' OR Соревнование LIKE '%{search}%' OR Вид_спорта LIKE '%{search}%'";
        }

        private int GetOrCreateSportsman(string fullName)
        {
            string selectSql = "SELECT Id FROM Sportsmen WHERE FullName = @name";
            SQLiteCommand cmd = new SQLiteCommand(selectSql, connection);
            cmd.Parameters.AddWithValue("@name", fullName);
            object result = cmd.ExecuteScalar();

            if (result != null)
                return Convert.ToInt32(result);

            string insertSql = "INSERT INTO Sportsmen (FullName, Team) VALUES (@name, 'Без команды')";
            cmd = new SQLiteCommand(insertSql, connection);
            cmd.Parameters.AddWithValue("@name", fullName);
            cmd.ExecuteNonQuery();

            return (int)connection.LastInsertRowId;
        }

        private int GetOrCreateCompetition(string competitionName, string sportName, string date)
        {
            int sportId = GetOrCreateSport(sportName);

            string selectSql = "SELECT Id FROM Competitions WHERE Name = @name";
            SQLiteCommand cmd = new SQLiteCommand(selectSql, connection);
            cmd.Parameters.AddWithValue("@name", competitionName);
            object result = cmd.ExecuteScalar();

            if (result != null)
                return Convert.ToInt32(result);

            string insertSql = "INSERT INTO Competitions (Name, SportId, Date) VALUES (@name, @sportId, @date)";
            cmd = new SQLiteCommand(insertSql, connection);
            cmd.Parameters.AddWithValue("@name", competitionName);
            cmd.Parameters.AddWithValue("@sportId", sportId);
            cmd.Parameters.AddWithValue("@date", date);
            cmd.ExecuteNonQuery();

            return (int)connection.LastInsertRowId;
        }

        private int GetOrCreateSport(string sportName)
        {
            string selectSql = "SELECT Id FROM Sports WHERE Name = @name";
            SQLiteCommand cmd = new SQLiteCommand(selectSql, connection);
            cmd.Parameters.AddWithValue("@name", sportName);
            object result = cmd.ExecuteScalar();

            if (result != null)
                return Convert.ToInt32(result);

            string insertSql = "INSERT INTO Sports (Name, Unit) VALUES (@name, '')";
            cmd = new SQLiteCommand(insertSql, connection);
            cmd.Parameters.AddWithValue("@name", sportName);
            cmd.ExecuteNonQuery();

            return (int)connection.LastInsertRowId;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (connection == null || connection.State != ConnectionState.Open)
                {
                    connection = Database.GetConnection();
                }

                dataGridView1.EndEdit();

                foreach (DataRow row in table.Rows)
                {
                    if (row.RowState == DataRowState.Added)
                    {
                        string sportsmanName = row["Спортсмен"] != DBNull.Value ? row["Спортсмен"].ToString() : "";
                        string competitionName = row["Соревнование"] != DBNull.Value ? row["Соревнование"].ToString() : "";
                        string date = row["Дата"] != DBNull.Value ? row["Дата"].ToString() : DateTime.Now.ToString("dd.MM.yyyy");
                        string sportName = row["Вид_спорта"] != DBNull.Value ? row["Вид_спорта"].ToString() : "";
                        string resultValue = row["Результат"] != DBNull.Value ? row["Результат"].ToString() : "";
                        int place = row["Место"] != DBNull.Value ? Convert.ToInt32(row["Место"]) : 1;

                        if (string.IsNullOrEmpty(sportsmanName) || string.IsNullOrEmpty(competitionName))
                            continue;

                        int sportsmanId = GetOrCreateSportsman(sportsmanName);
                        int competitionId = GetOrCreateCompetition(competitionName, sportName, date);

                        string insertSql = "INSERT INTO Results (SportsmanId, CompetitionId, Result, Place) VALUES (@sid, @cid, @result, @place)";
                        SQLiteCommand cmd = new SQLiteCommand(insertSql, connection);
                        cmd.Parameters.AddWithValue("@sid", sportsmanId);
                        cmd.Parameters.AddWithValue("@cid", competitionId);
                        cmd.Parameters.AddWithValue("@result", resultValue);
                        cmd.Parameters.AddWithValue("@place", place);
                        cmd.ExecuteNonQuery();
                    }
                    else if (row.RowState == DataRowState.Modified)
                    {
                        string sportsmanName = row["Спортсмен"].ToString();
                        string competitionName = row["Соревнование"].ToString();
                        string date = row["Дата"].ToString();
                        string sportName = row["Вид_спорта"].ToString();
                        string resultValue = row["Результат"].ToString();
                        int place = Convert.ToInt32(row["Место"]);

                        int sportsmanId = GetOrCreateSportsman(sportsmanName);
                        int competitionId = GetOrCreateCompetition(competitionName, sportName, date);

                        string updateSql = "UPDATE Results SET SportsmanId = @sid, CompetitionId = @cid, Result = @result, Place = @place WHERE Id = @id";
                        SQLiteCommand cmd = new SQLiteCommand(updateSql, connection);
                        cmd.Parameters.AddWithValue("@sid", sportsmanId);
                        cmd.Parameters.AddWithValue("@cid", competitionId);
                        cmd.Parameters.AddWithValue("@result", resultValue);
                        cmd.Parameters.AddWithValue("@place", place);
                        cmd.Parameters.AddWithValue("@id", Convert.ToInt32(row["Id"]));
                        cmd.ExecuteNonQuery();
                    }
                    else if (row.RowState == DataRowState.Deleted)
                    {
                        string deleteSql = "DELETE FROM Results WHERE Id = @id";
                        SQLiteCommand cmd = new SQLiteCommand(deleteSql, connection);
                        cmd.Parameters.AddWithValue("@id", Convert.ToInt32(row["Id", DataRowVersion.Original]));
                        cmd.ExecuteNonQuery();
                    }
                }

                table.AcceptChanges();
                LoadData();
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
                dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            try
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            catch { }

            Application.Exit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            catch { }

            base.OnFormClosing(e);
        }
    }
}