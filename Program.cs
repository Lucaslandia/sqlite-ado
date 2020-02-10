using System;
using Microsoft.Data.Sqlite;

namespace App
{
    class Program
    {
        public const string DB_FILE = "./SqliteDB.db";

        static void Main(string[] args)
        {
            CreateDB();
            ReadDB();
        }

        public static void CreateDB()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = DB_FILE };

            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                // Create a table (drop if already exists)
                var delTableCmd = new SqliteCommand("DROP TABLE IF EXISTS Students", connection);
                delTableCmd.ExecuteNonQuery();

                var createTableCmd = new SqliteCommand(@"
                    CREATE TABLE Students (
                        Id int,
                        FirstName varchar(50),
                        LastName varchar(50),
                        Gender varchar(50)
                    )
                ", connection);
                createTableCmd.ExecuteNonQuery();

                // Seed data
                using (var transaction = connection.BeginTransaction())
                {
                    var insertCmd = new SqliteCommand(@"
                        INSERT INTO Students VALUES
                            (101, 'Steve', 'Smith', 'Male'),
                            (102, 'Sara', 'Pound', 'Female'),
                            (103, 'Ben', 'Stokes', 'Male'),
                            (104, 'Jos', 'Butler', 'Male'),
                            (105, 'Pam', 'Semi', 'Female')
                    ", connection, transaction);

                    insertCmd.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
        }

        public static void ReadDB()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = DB_FILE };

            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT * FROM Students";

                using (var reader = selectCmd.ExecuteReader())
                {
                    Console.WriteLine();

                    while (reader.Read())
                    {
                        Console.WriteLine(reader.GetString(0));
                        Console.WriteLine(reader.GetString(1));
                        Console.WriteLine(reader.GetString(2));
                        Console.WriteLine(reader.GetString(3));
                        Console.WriteLine();
                    }
                }
            }
        }  
    }     
}