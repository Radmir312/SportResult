using System.Data.SQLite;
using System.IO;
using System;

namespace SportResult
{
    public static class Database
    {
        public static string dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SportResult",
            "Sport.db"
        );

        public static string ConnectionString = $"Data Source={dbPath};Version=3;";

        public static SQLiteConnection GetConnection()
        {
            var conn = new SQLiteConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        public static void Initialize()
        {
            string folder = Path.GetDirectoryName(dbPath);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);

                using (var conn = GetConnection())
                {
                    string sql = @"
                        CREATE TABLE Users (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Login TEXT UNIQUE,
                            Password TEXT,
                            Role TEXT
                        );
                        
                        CREATE TABLE Sports (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Name TEXT,
                            Unit TEXT
                        );
                        
                        CREATE TABLE Sportsmen (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            FullName TEXT,
                            Team TEXT
                        );
                        
                        CREATE TABLE Competitions (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Name TEXT,
                            SportId INTEGER
                        );
                        
                        CREATE TABLE Results (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            SportsmanId INTEGER,
                            CompetitionId INTEGER,
                            Result TEXT,
                            Place INTEGER
                        );

                        INSERT INTO Users (Login, Password, Role) VALUES 
                        ('admin', '123', 'Admin'),
                        ('user', '123', 'Worker');
                        
                        INSERT INTO Sports (Name, Unit) VALUES 
                        ('Бег 100 метров', 'сек'),
                        ('Бег 200 метров', 'сек'),
                        ('Тяжёлая атлетика', 'кг'),
                        ('Прыжки в длину', 'м'),
                        ('Прыжки в высоту', 'см'),
                        ('Плавание 50 метров', 'сек'),
                        ('Гимнастика', 'очки');
                        
                        INSERT INTO Sportsmen (FullName, Team) VALUES 
                        ('Иванов Александр', 'ЦСКА'),
                        ('Петров Дмитрий', 'Динамо'),
                        ('Сидорова Анна', 'Спартак'),
                        ('Козлов Михаил', 'Локомотив'),
                        ('Смирнова Елена', 'ЦСКА'),
                        ('Новиков Артём', 'Зенит'),
                        ('Морозова Ольга', 'Динамо'),
                        ('Волков Сергей', 'Спартак'),
                        ('Зайцева Мария', 'Локомотив'),
                        ('Соколов Игорь', 'Зенит'),
                        ('Ким Виктор', 'ЦСКА'),
                        ('Ли Джейсон', 'Динамо'),
                        ('Тяжеловес Максим', 'ЦСКА'),
                        ('Прыгунов Алексей', 'Динамо'),
                        ('Пловцов Андрей', 'Спартак'),
                        ('Гимнастова Ирина', 'Локомотив');
                        
                        INSERT INTO Competitions (Name, SportId) VALUES 
                        ('Чемпионат России по бегу 100м', 1),
                        ('Кубок России по бегу 200м', 2),
                        ('Международный турнир по тяжёлой атлетике', 3),
                        ('Первенство города по прыжкам в длину', 4),
                        ('Спартакиада по бегу 100м', 1),
                        ('Открытый чемпионат по плаванию 50м', 6),
                        ('Чемпионат области по прыжкам в высоту', 5),
                        ('Кубок федерации по гимнастике', 7);
                        
                        INSERT INTO Results (SportsmanId, CompetitionId, Result, Place) VALUES 
                        (1, 1, '10.45 сек', 1),
                        (2, 1, '10.67 сек', 2),
                        (4, 1, '10.89 сек', 3),
                        (6, 1, '11.02 сек', 4),
                        (8, 1, '11.15 сек', 5),
                        (3, 1, '11.34 сек', 1),
                        (5, 1, '11.56 сек', 2),
                        (7, 1, '11.78 сек', 3),
                        (1, 2, '21.23 сек', 1),
                        (2, 2, '21.54 сек', 2),
                        (4, 2, '21.76 сек', 3),
                        (10, 2, '21.91 сек', 4),
                        (11, 2, '22.08 сек', 5),
                        (13, 3, '180 кг', 1),
                        (14, 3, '175 кг', 2),
                        (2, 3, '160 кг', 3),
                        (6, 3, '155 кг', 4),
                        (14, 4, '7.85 м', 1),
                        (8, 4, '7.42 м', 2),
                        (12, 4, '7.15 м', 3),
                        (3, 4, '6.78 м', 1),
                        (5, 4, '6.54 м', 2),
                        (9, 4, '6.32 м', 3),
                        (1, 5, '10.12 сек', 1),
                        (6, 5, '10.34 сек', 2),
                        (8, 5, '10.56 сек', 3),
                        (11, 5, '10.78 сек', 4),
                        (3, 5, '10.92 сек', 1),
                        (5, 5, '11.14 сек', 2),
                        (15, 6, '24.56 сек', 1),
                        (10, 6, '25.12 сек', 2),
                        (12, 6, '25.67 сек', 3),
                        (7, 6, '26.34 сек', 1),
                        (9, 6, '27.01 сек', 2),
                        (13, 7, '210 см', 1),
                        (14, 7, '205 см', 2),
                        (4, 7, '195 см', 3),
                        (16, 8, '9.75 очков', 1),
                        (3, 8, '9.45 очков', 2),
                        (7, 8, '9.12 очков', 3),
                        (9, 8, '8.89 очков', 4);
                    ";

                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}