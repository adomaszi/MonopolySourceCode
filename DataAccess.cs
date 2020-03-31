using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Windows.Storage;
using System.IO;
using System.Diagnostics;

namespace MonopolyAnalysis
{
    public static class DataAccess
    {
        public async static void InitializeDatabase()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("monopolyDatabase.db", CreationCollisionOption.OpenIfExists);
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "monopolyDatabase.db");
            using (SqliteConnection db =
               new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                String gameMoveTable = "CREATE TABLE IF NOT EXISTS GameMove ( " +
                      "GameMoveID int PRIMARY KEY," +
                      "PlayerID int, " +
                      "GameFieldLandedOnDiceRoll int, " +
                      "GameID int, " +
                      "MoneySpend double, " +

                      "CONSTRAINT FK_PlayerID FOREIGN KEY(PlayerID) " +
                      "REFERENCES Player(PlayerID) " +
                      "ON UPDATE CASCADE " +
                      "ON DELETE CASCADE, " +

                      "CONSTRAINT FK_GameFieldLandedOnDiceRoll FOREIGN KEY(GameFieldLandedOnDiceRoll) " +
                      "REFERENCES GameField(GameFieldID) " +
                      "ON UPDATE CASCADE " +
                      "ON DELETE CASCADE, " +

                      "CONSTRAINT FK_GameID FOREIGN KEY(GameID) " +
                      "REFERENCES Game(GameID) " +
                      "ON UPDATE CASCADE " +
                      "ON DELETE CASCADE " +
                      ");";

                String playerTable = "CREATE TABLE IF NOT EXISTS Player ( " +
                      "PlayerID int PRIMARY KEY, " +
                      "GameID int, " +
                      "FinalTotalMoney decimal, " +

                      "CONSTRAINT FK_GameID FOREIGN KEY(GameID) " +
                      "REFERENCES Game(GameID) " +
                      "ON UPDATE CASCADE " +
                      "ON DELETE CASCADE " +
                      "); ";

                String gameTable = "CREATE TABLE IF NOT EXISTS Game ( " +
                      "GameID INTEGER PRIMARY KEY " +
                      ") ";
               

                String playerGameFieldsTable = "CREATE TABLE IF NOT EXISTS PlayerGameFields ( " +
                      "PlayerGameFieldsID int PRIMARY KEY, " +
                      "GameFieldID int, " +
                      "PlayerID int, " + 

                      "CONSTRAINT FK_GameFieldID FOREIGN KEY(GameFieldID) " +
                      "REFERENCES GameField(GameFieldID) " +
                      "ON UPDATE CASCADE " +
                      "ON DELETE CASCADE, " +

                      "CONSTRAINT FK_PlayerID FOREIGN KEY(PlayerID) " +
                      "REFERENCES Player(PlayerID) " +
                      "ON UPDATE CASCADE " +
                      "ON DELETE CASCADE " + 
                      "); ";

                String gameFieldTable = "CREATE TABLE IF NOT EXISTS GameField ( " +
                      "GameFieldID int PRIMARY KEY, " +
                      "FieldName text, " +
                      "IsJailed int, " +
                      "FieldGroupID int, " +
                      "PlayerID int, " +

                      "CONSTRAINT FK_PlayerID FOREIGN KEY(PlayerID) " +
                      "REFERENCES Player(PlayerID) " +
                      "ON UPDATE CASCADE " +
                      "ON DELETE CASCADE, " +

                      "CONSTRAINT FK_FieldGroupID FOREIGN KEY(FieldGroupID) " +
                      "REFERENCES FieldGroup(FieldGroupID) " +
                      "ON UPDATE CASCADE " +
                      "ON DELETE CASCADE " +
                      "); ";

                String fieldGroupTable = "CREATE TABLE IF NOT EXISTS FieldGroup ( " +
                      "FieldGroupID int PRIMARY KEY, " +
                      "Color text UNIQUE" +
                      "); ";


                List<String> commandStrings = new List<string>
                {
                    gameTable,
                    playerTable,
                    fieldGroupTable,
                    gameFieldTable,
                    playerGameFieldsTable,
                    gameMoveTable
                };
                
                foreach (string tableCommand in commandStrings)
                {
                    Debug.WriteLine(tableCommand);
                    SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                    createTable.ExecuteReader();
                } 
            }
        }

        public static void AddFieldGroupColor(string color)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "monopolyDatabase.db");
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = "INSERT INTO FieldGroup VALUES (NULL, @Color);";
                insertCommand.Parameters.AddWithValue("@Color", color);

                try
                {
                    insertCommand.ExecuteReader();
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine("Inner Exception: " + ex.Message);
                    Debug.WriteLine("");
                    Debug.WriteLine("Query Executed: " + insertCommand.CommandText);
                    Debug.WriteLine("");
                }
                finally
                {
                    db.Close();
                }
            }

        }

        public static List<String> GetData()
        {
            List<String> entries = new List<string>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "monopolyDatabase.db");
            using (SqliteConnection db =
               new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT Color from FieldGroup", db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    entries.Add(query.GetString(0));
                }

                db.Close();
            }

            return entries;
        }
    }

    
}
