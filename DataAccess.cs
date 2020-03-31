using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Windows.Storage;
using System.IO;
using System.Diagnostics;
using Monopoly;

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
                      "PropertyLandedOn int" +
                      "DiceRoll int, " +
                      "GameID int, " +
                      "MoneySpend double, " +

                      "CONSTRAINT FK_PlayerID FOREIGN KEY(PlayerID) " +
                      "REFERENCES Player(PlayerID) " +
                      "ON UPDATE CASCADE " +
                      "ON DELETE CASCADE, " +

                      "CONSTRAINT FK_GameFieldLandedOn FOREIGN KEY(PropertyLandedOn) " +
                      "REFERENCES Property(PropertyID) " +
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
               

                String playerGameFieldsTable = "CREATE TABLE IF NOT EXISTS PlayerProperties ( " +
                      "PlayerPropertyID int PRIMARY KEY, " +
                      "PropertyID int, " +
                      "PlayerID int, " +

                      "CONSTRAINT FK_PropertyID FOREIGN KEY(PropertyID) " +
                      "REFERENCES Property(PropertyID) " +
                      "ON UPDATE CASCADE " +
                      "ON DELETE CASCADE, " +

                      "CONSTRAINT FK_PlayerID FOREIGN KEY(PlayerID) " +
                      "REFERENCES Player(PlayerID) " +
                      "ON UPDATE CASCADE " +
                      "ON DELETE CASCADE " + 
                      "); ";

                String gameFieldTable = "CREATE TABLE IF NOT EXISTS Property ( " +
                      "PropertyID int PRIMARY KEY, " +
                      "PropertyName text UNIQUE, " +
                      "PropertyGroupID int, " +

                      "CONSTRAINT FK_FieldGroupID FOREIGN KEY(PropertyGroupID) " +
                      "REFERENCES PropertyGroup(PropertyGroupID) " +
                      "ON UPDATE CASCADE " +
                      "ON DELETE CASCADE " +
                      "); ";

                String fieldGroupTable = "CREATE TABLE IF NOT EXISTS PropertyGroup ( " +
                      "PropertyGroupID int PRIMARY KEY, " +
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
                    SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                    createTable.ExecuteReader();
                } 
            }
        }

        public static void DropAllTables()
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "monopolyDatabase.db");
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                List<String> commandStrings = new List<string>
                {
                    "DROP TABLE IF EXISTS GameMove;",
                    "DROP TABLE IF EXISTS PlayerProperties;",
                    "DROP TABLE IF EXISTS Property;",
                    "DROP TABLE IF EXISTS PropertyGroup;",
                    "DROP TABLE IF EXISTS Player;",
                    "DROP TABLE IF EXISTS Game;"
                };

                foreach (string tableCommand in commandStrings)
                {
                    SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                    createTable.ExecuteReader();
                }

                db.Close();
            }
        }

        public static void AddBoardData()
        {
            DataAccess.AddFieldGroupColor("Brown");
            DataAccess.AddFieldGroupColor("LightBlue");
            DataAccess.AddFieldGroupColor("Pink");
            DataAccess.AddFieldGroupColor("Orange");
            DataAccess.AddFieldGroupColor("Red");
            DataAccess.AddFieldGroupColor("Yellow");
            DataAccess.AddFieldGroupColor("Green");
            DataAccess.AddFieldGroupColor("DarkBlue");

            DataAccess.AddField("Vine Street", "Brown");
            DataAccess.AddField("Coventry Street", "Brown");

            DataAccess.AddField("Leicester Square", "LightBlue");
            DataAccess.AddField("Bow Street", "LightBlue");
            DataAccess.AddField("Whitechapel Road", "LightBlue");

            DataAccess.AddField("The Angel Islington", "Pink");
            DataAccess.AddField("Trafalgar Square", "Pink");
            DataAccess.AddField("Northumrld Avenue", "Pink");

            DataAccess.AddField("M'Borgough Street", "Orange");
            DataAccess.AddField("Fleet Street", "Orange");
            DataAccess.AddField("Old Knet Road", "Orange");

            DataAccess.AddField("Whitehall", "Red");
            DataAccess.AddField("Pentonville Road", "Red");
            DataAccess.AddField("Pall Mall", "Red");

            DataAccess.AddField("Bond Street", "Yellow");
            DataAccess.AddField("Strand", "Yellow");
            DataAccess.AddField("Regent Street", "Yellow");

            DataAccess.AddField("Euston Road", "Green");
            DataAccess.AddField("Picadilly", "Green");
            DataAccess.AddField("Oxford Street", "Green");

            DataAccess.AddField("Park Lane", "DarkBlue");
            DataAccess.AddField("Mayfair", "DarkBlue");
        }

        public static void AddFieldGroupColor(String color)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "monopolyDatabase.db");
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;
                
                insertCommand.CommandText = "INSERT INTO PropertyGroup VALUES (NULL, @Color);";
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

        public static void AddField(String name, String color)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "monopolyDatabase.db");
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                insertCommand.CommandText = "INSERT INTO Property (PropertyName, PropertyGroupID) SELECT @Name, PropertyGroupID FROM PropertyGroup WHERE Color LIKE @Color;";
                insertCommand.Parameters.AddWithValue("@Name", name);
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

        public static List<String> GetPropertyGroups()
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

        public static void SaveGameData(List<Move> moves, int playerAmount, Board board)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "monopolyDatabase.db");
            using (SqliteConnection db =
               new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand command = new SqliteCommand();
                command.Connection = db;
                SqliteTransaction transaction = db.BeginTransaction();
                command.Transaction = transaction;

                int[] players = new int[playerAmount];

                try
                {
                    command.CommandText = "Insert Into Game";
                    command.ExecuteNonQuery();
                    SqliteCommand selectGameIDCommand = new SqliteCommand
                    ("SELECT TOP 1 GameID from Game BY GameID DESC", db);
                    selectGameIDCommand.Transaction = transaction;
                    SqliteDataReader gameResult = selectGameIDCommand.ExecuteReader();
                    int gameID = 0;

                    while (gameResult.Read())
                    {
                        gameID = gameResult.GetInt16(0);
                    }

                    for (int i = 0; i < board.players.Count; i++)
                    {
                        command.CommandText = "Insert Into Player (FinalTotalMoney, GameID) VALUES (@Money, @GameID)";
                        command.Parameters.AddWithValue("@Money", board.players[i].Money);
                        command.Parameters.AddWithValue("@GameID", gameID);
                        command.ExecuteNonQuery();
                        SqliteCommand selectPlayerIDCommand = new SqliteCommand
                        ("SELECT TOP 1 PlayerID from Player BY PlayerID DESC", db);
                        selectGameIDCommand.Transaction = transaction;
                        SqliteDataReader playerResult = selectGameIDCommand.ExecuteReader();
                        while (playerResult.Read())
                        {
                            players[i] = playerResult.GetInt16(0);
                        }
                        foreach(Property property in board.players[i].OwnedProperties)
                        {
                            command.CommandText = "Insert Into PlayerProperties (PlayerID, PropertyID) VALUES (@PlayerID, Select PropertyID FROM Property WHERE PropertyName Like @PropertyName)";
                            command.Parameters.AddWithValue("@PlayerID", players[i]);
                            command.Parameters.AddWithValue("@PropertyName", property.Name);
                        }
                    }

                }
                catch (Exception e)
                {
                    transaction.Rollback();
                }
                finally
                {
                    db.Close();
                }
            }
        }
    }

    
}
