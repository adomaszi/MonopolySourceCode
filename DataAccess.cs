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
using MonolpolyAnalysis;

namespace MonopolyAnalysis
{
    public static class DataAccess
    {
        static readonly string _dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "monopolyDatabase.db");
        public async static void InitializeDatabase()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("monopolyDatabase.db", CreationCollisionOption.OpenIfExists);
            using (SqliteConnection sqlConnection = new SqliteConnection($"Filename={_dbpath}"))
            {
                sqlConnection.Open();

                String gameMoveTable = "CREATE TABLE IF NOT EXISTS GameMove ( " +
                      "GameMoveID INTEGER PRIMARY KEY," +
                      "PlayerID int, " +
                      "PropertyLandedOn int," +
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
                      "PlayerID INTEGER PRIMARY KEY, " +
                      "GameID int, " +
                      "FinalTotalMoney decimal, " +

                      "CONSTRAINT FK_GameID FOREIGN KEY(GameID) " +
                      "REFERENCES Game(GameID) " +
                      "ON UPDATE CASCADE " +
                      "ON DELETE CASCADE " +
                      "); ";

                String gameTable = "CREATE TABLE IF NOT EXISTS Game ( " +
                      "GameID INTEGER PRIMARY KEY," +
                      "Name Text " +
                      ") ";


                String playerGameFieldsTable = "CREATE TABLE IF NOT EXISTS PlayerProperties ( " +
                      "PlayerPropertyID INTEGER PRIMARY KEY, " +
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
                      "PropertyID INTEGER PRIMARY KEY, " +
                      "PropertyName text UNIQUE, " +
                      "PropertyGroupID int, " +

                      "CONSTRAINT FK_FieldGroupID FOREIGN KEY(PropertyGroupID) " +
                      "REFERENCES PropertyGroup(PropertyGroupID) " +
                      "ON UPDATE CASCADE " +
                      "ON DELETE CASCADE " +
                      "); ";

                String fieldGroupTable = "CREATE TABLE IF NOT EXISTS PropertyGroup ( " +
                      "PropertyGroupID INTEGER PRIMARY KEY, " +
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
                    SqliteCommand createTable = new SqliteCommand(tableCommand, sqlConnection);

                    createTable.ExecuteReader();
                }
                sqlConnection.Close();
            }
        }

        public static void DropAllTables()
        {
            using (SqliteConnection sqlConnection = new SqliteConnection($"Filename={_dbpath}"))
            {
                sqlConnection.Open();

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
                    SqliteCommand createTable = new SqliteCommand(tableCommand, sqlConnection);

                    createTable.ExecuteReader();
                }

                sqlConnection.Close();
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
            using (SqliteConnection sqlConnection = new SqliteConnection($"Filename={_dbpath}"))
            {
                sqlConnection.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = sqlConnection;

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
                    sqlConnection.Close();
                }
            }
        }

        public static void AddField(String name, String color)
        {
            using (SqliteConnection sqlConnection = new SqliteConnection($"Filename={_dbpath}"))
            {
                sqlConnection.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = sqlConnection;

                insertCommand.CommandText = "INSERT INTO Property (PropertyName, PropertyGroupID) SELECT @Name, PropertyGroupID FROM PropertyGroup WHERE Color LIKE @Color;";
                insertCommand.Parameters.AddWithValue("@Name", name);
                insertCommand.Parameters.AddWithValue("@Color", color);

                try
                {
                    insertCommand.ExecuteReader();
                }
                catch (SqliteException ex)
                {
                    
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        public static List<String> GetPropertyGroups()
        {
            List<String> entries = new List<string>();

            using (SqliteConnection sqlConnection = new SqliteConnection($"Filename={_dbpath}"))
            {
                sqlConnection.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT Color from FieldGroup", sqlConnection);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    entries.Add(query.GetString(0));
                }

                sqlConnection.Close();
            }

            return entries;
        }

        public static void SaveGameData(List<GameResult> gameResults)
        {

            using (SqliteConnection sqlConnection = new SqliteConnection($"Filename={_dbpath}"))
            {
                sqlConnection.Open();
                
                int index = 0;
                foreach (GameResult game in gameResults)
                {
                    index++;
                    SqliteCommand command = new SqliteCommand();
                    SqliteCommand selectCommand = new SqliteCommand();
                    command.Connection = sqlConnection;
                    SqliteTransaction transaction = sqlConnection.BeginTransaction();
                    command.Transaction = transaction;
                    try
                    {
                        Board board = game.Board;
                        List<Move> moves = game.Moves;
                        int playerAmount = board.players.Count;

                        int[] players = new int[playerAmount];
                        command.CommandText = "Insert Into Game (Name) VALUES ('Game')";
                        command.ExecuteNonQuery();
                        selectCommand = new SqliteCommand("SELECT GameID FROM Game ORDER BY GameID DESC LIMIT 1;", sqlConnection);
                        selectCommand.Transaction = transaction;
                        SqliteDataReader gameResult = selectCommand.ExecuteReader();
                        int gameID = 0;

                        while (gameResult.Read())
                        {
                            gameID = gameResult.GetInt16(0);
                        }

                        gameResult.Close();

                        for (int i = 0; i < board.players.Count; i++)
                        {
                            command.CommandText = $"Insert Into Player (FinalTotalMoney, GameID) VALUES ({board.players[i].Money}, {gameID})";
                            command.ExecuteNonQuery();

                            selectCommand.CommandText = "SELECT PlayerID FROM Player ORDER BY PlayerID DESC LIMIT 1";
                            SqliteDataReader playersResult = selectCommand.ExecuteReader();

                            while (playersResult.Read())
                            {
                                players[i] = playersResult.GetInt16(0);
                            }

                            playersResult.Close();

                            foreach (BoardSpace space in board.players[i].OwnedProperties)
                            {
                                if (space is Property)
                                {
                                    command.CommandText = $"Insert Into PlayerProperties (PlayerID, PropertyID) SELECT {players[i]}, PropertyID FROM Property WHERE PropertyName Like '{space.Name}'";
                                    command.ExecuteNonQuery();
                                }
                            }
                        }

                        foreach (Move move in moves)
                        {
                            int playerIndex = board.players.FindIndex(u => u == move._player);
                            if (playerIndex > -1 && move._property is Property)
                            {
                                command.CommandText = $"INSERT INTO GameMove (PlayerID, PropertyLandedOn, DiceRoll, GameID, MoneySpend) SELECT {players[playerIndex]}, PropertyID, {move._numberRolled}, {gameID}, {move._moneyPaid} FROM Property WHERE PropertyName LIKE '{move._property.Name}'";
                                command.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }

                    catch (Exception e)
                    {
                        transaction.Rollback();
                        Debug.WriteLine("SaveGameData Inner Exception: " + e.Message);
                        Debug.WriteLine("");
                        Debug.WriteLine("Query Executed: " + command.CommandText);
                        Debug.WriteLine("");
                    }
                    finally
                    {
                        Debug.WriteLine("Done " + index);
                    }



                }
                sqlConnection.Close();
            }

        }

        public static int GetNumberOfStoredGames(int numberPlayers)
        {
            int count = 0;
            using (SqliteConnection sqlConnection = new SqliteConnection($"Filename={_dbpath}"))
            {
                sqlConnection.Open();

                SqliteCommand selectAmountCommand = new SqliteCommand();
                selectAmountCommand.Connection = sqlConnection;

                try
                {
                    selectAmountCommand.CommandText = "Select count(total) " +
                                                      "FROM " +
                                                          "(Select Count(PlayerID) as playeramount, COUNT(GameID) as total FROM Player Group By GameID) " +
                                                      "Group by playeramount " +
                                                     $"having playeramount = { numberPlayers}";

                    using (SqliteDataReader reader = selectAmountCommand.ExecuteReader())
                    {

                        int countGamesIndex = reader.GetOrdinal("count(total)");

                        while (reader.Read())
                        {
                            count = reader.GetInt32(countGamesIndex);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Inner Exception: " + e.Message);
                    Debug.WriteLine("");
                    Debug.WriteLine("Query Executed: " + selectAmountCommand.CommandText);
                    Debug.WriteLine("");
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
            return count;
        }

        public static List<int> GetRollsOfWinners(int numberPlayers)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            List<int> allWinnerRolls = new List<int>();
            List<int> allGames = GetAllGameIDs(numberPlayers);

            using (SqliteConnection sqlConnection = new SqliteConnection($"Filename={_dbpath}"))
            {
                foreach (int id in allGames)
                {
                    int winnerID = GetWinnerIDOfGame(id);
                    sqlConnection.Open();
                    SqliteCommand selectDiceRoll = new SqliteCommand();
                    selectDiceRoll.Connection = sqlConnection;

                    try {

                        selectDiceRoll.CommandText = $"Select DiceRoll From GameMove Where PlayerID = {winnerID}";
                        using (SqliteDataReader reader = selectDiceRoll.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int num = reader.GetInt32(0);
                                allWinnerRolls.Add(num);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("GetNumberOfStoredGames Inner Exception: " + e.Message);
                        Debug.WriteLine("");
                        Debug.WriteLine("Query Executed: " + selectDiceRoll.CommandText);
                        Debug.WriteLine("");
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }

                }
            }

                stopWatch.Stop();
                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;
                Debug.WriteLine(ts);
                return allWinnerRolls;
            }

            public static List<int> GetRollsOfLosers(int numberPlayers)
            {
                List<int> allLoserRolls = new List<int>();
                List<int> allGames = GetAllGameIDs(numberPlayers);
                foreach (int id in allGames)
                {
                    int loserID = GetLoserIDOfGame(id);

                    using (SqliteConnection sqlConnection = new SqliteConnection($"Filename={_dbpath}"))
                    {
                        sqlConnection.Open();

                        SqliteCommand selectDiceRoll = new SqliteCommand();

                        try
                        {
                            selectDiceRoll.CommandText = $"Select DiceRoll From GameMove Where PlayerID = {loserID}";
                            selectDiceRoll.Connection = sqlConnection;

                            using (SqliteDataReader reader = selectDiceRoll.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    allLoserRolls.Add(reader.GetInt32(0));
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("GetRollsOfLosers Inner Exception: " + e.Message);
                            Debug.WriteLine("");
                            Debug.WriteLine("Query Executed: " + selectDiceRoll.CommandText);
                            Debug.WriteLine("");
                        }
                        finally
                        {
                            sqlConnection.Close();
                        }
                    }
                }
                return allLoserRolls;
            }

            public static Dictionary<String, int> GetWinnerPropertyRevenue(int numberPlayers)
            {
                List<int> gameIDs = GetAllGameIDs(numberPlayers);
                Dictionary<String, int> revenuePerProperty = GetPropertyRevenues(gameIDs, "winner");
                return revenuePerProperty;
            }
            public static Dictionary<String, int> GetLoserPropertyRevenue(int numberPlayers)
            {
                List<int> gameIDs = GetAllGameIDs(numberPlayers);

                Dictionary<String, int> revenuePerProperty = GetPropertyRevenues(gameIDs, "looser");
                return revenuePerProperty;
            }

            public static Dictionary<String, int> GetAmountLandedOn(int numberPlayers)
            {
                List<int> gameIDs = GetAllGameIDs(numberPlayers);
                Dictionary<String, int> landedOnPerProperty = new Dictionary<string, int>();

                SqliteCommand selectRevenue = new SqliteCommand();
                using (SqliteConnection sqlConnection = new SqliteConnection($"Filename={_dbpath}"))
                {
                    try
                    {
                        List<int> playerIDs = new List<int>();
                        Dictionary<int, int> propertyValueIDs = new Dictionary<int, int>();
                        foreach (int gameID in gameIDs)
                        {
                            String playerIDsString = "";
                            playerIDs = GetAllPlayerIDsOfGame(gameID);


                            for (int i = 0; i < playerIDs.Count; i++)
                            {
                                playerIDsString += playerIDs[i].ToString();
                                if (i != playerIDs.Count - 1)
                                {
                                    playerIDsString += ", ";
                                }
                            }

                            sqlConnection.Open();
                            selectRevenue.CommandText =
                                $"Select PropertyLandedOn " +
                                $"From GameMove " +
                                $"WHERE GameID = {gameID} AND " +
                                $"PlayerID IN ( {playerIDsString} )";
                            selectRevenue.Connection = sqlConnection;

                            using (SqliteDataReader reader = selectRevenue.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int value;
                                    if (propertyValueIDs.TryGetValue(reader.GetInt32(0), out value))
                                    {
                                        propertyValueIDs[reader.GetInt32(0)] = value + 1;
                                    }
                                    else
                                    {
                                        propertyValueIDs.TryAdd(reader.GetInt32(0), 1);
                                    }
                                }

                            }
                        }

                        foreach (KeyValuePair<int, int> entry in propertyValueIDs)
                        {
                            String propertyName = GetPropertyNameByID(entry.Key);
                            landedOnPerProperty.TryAdd(propertyName, entry.Value);
                        }

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Inner Exception: " + e.Message);
                        Debug.WriteLine("");
                        Debug.WriteLine("Query Executed: " + selectRevenue.CommandText);
                        Debug.WriteLine("");
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }

            return landedOnPerProperty;
            }

            private static Dictionary<String, int> GetPropertyRevenues(List<int> gameIDs, String type)
            {
                Dictionary<String, int> revenuePerProperty = new Dictionary<string, int>();
                SqliteCommand selectRevenue = new SqliteCommand();
                using (SqliteConnection sqlConnection = new SqliteConnection($"Filename={_dbpath}"))
                {
                    try
                    {
                        List<int> playerIDs = new List<int>();
                        Dictionary<int, int> propertyValueIDs = new Dictionary<int, int>();
                        foreach (int gameID in gameIDs)
                        {
                            String playerIDsString = "";
                            switch (type)
                            {
                                case "winner":
                                    playerIDs = new List<int>();
                                    playerIDs.Add(GetWinnerIDOfGame(gameID));
                                    break;
                                case "looser":
                                    playerIDs = new List<int>();
                                    playerIDs.Add(GetLoserIDOfGame(gameID));
                                    break;
                                case "all":
                                    playerIDs = GetAllPlayerIDsOfGame(gameID);
                                    break;
                            }

                            for (int i = 0; i < playerIDs.Count; i++)
                            {
                                playerIDsString += playerIDs[i].ToString();
                                if (i != playerIDs.Count - 1)
                                {
                                    playerIDsString += ", ";
                                }
                            }

                            sqlConnection.Open();
                            selectRevenue.CommandText =
                                $"Select PropertyLandedOn, MoneySpend " +
                                $"From GameMove " +
                                $"WHERE GameID = {gameID} AND " +
                                $"PropertyLandedOn IN ( " +
                                    $"Select PropertyID From PlayerProperties " +
                                    $"WHERE PlayerID IN ({playerIDsString}))";
                            selectRevenue.Connection = sqlConnection;


                            using (SqliteDataReader reader = selectRevenue.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int value;
                                    if (propertyValueIDs.TryGetValue(reader.GetInt32(0), out value))
                                    {
                                        propertyValueIDs[reader.GetInt32(0)] = value + reader.GetInt32(1);
                                    }
                                    else
                                    {
                                        propertyValueIDs.TryAdd(reader.GetInt32(0), reader.GetInt32(1));
                                    }
                                }

                            }
                        }

                        Debug.Print(propertyValueIDs.ToString());

                        foreach (KeyValuePair<int, int> entry in propertyValueIDs)
                        {
                            String propertyName = GetPropertyNameByID(entry.Key);
                            revenuePerProperty.TryAdd(propertyName, entry.Value);
                        }

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Inner Exception: " + e.Message);
                        Debug.WriteLine("");
                        Debug.WriteLine("Query Executed: " + selectRevenue.CommandText);
                        Debug.WriteLine("");
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }

                return revenuePerProperty;
            }

            private static List<int> GetAllGameIDs(int numberPlayers)
            {
                List<int> allGameIDs = new List<int>();
                using (SqliteConnection sqlConnection = new SqliteConnection($"Filename={_dbpath}"))
                {
                    sqlConnection.Open();
                    SqliteCommand command = new SqliteCommand(
                                        "Select GameID FROM Player Group By GameID " +
                                       $"having Count(PlayerID) = { numberPlayers}");
                    command.Connection = sqlConnection;

                    try
                    {
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                allGameIDs.Add(reader.GetInt32(0));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Inner Exception: " + e.Message);
                        Debug.WriteLine("");
                        Debug.WriteLine("Query Executed: " + command.CommandText);
                        Debug.WriteLine("");
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }

                }
                return allGameIDs;
            }

            private static int GetWinnerIDOfGame(int gameID)
            {
                int winnerID = -1;

                using (SqliteConnection sqlConnection = new SqliteConnection($"Filename={_dbpath}"))
                {
                    sqlConnection.Open();
                    SqliteCommand selectWinnerIDs = new SqliteCommand(
                        $"Select PlayerID From Player Where GameID = {gameID} ORDER BY FinalTotalMoney DESC LIMIT 1");
                    selectWinnerIDs.Connection = sqlConnection;

                    try
                    {
                        using (SqliteDataReader reader = selectWinnerIDs.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                winnerID = reader.GetInt32(0);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Inner Exception: " + e.Message);
                        Debug.WriteLine("");
                        Debug.WriteLine("Query Executed: " + selectWinnerIDs.CommandText);
                        Debug.WriteLine("");
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }
                return winnerID;
            }

            private static int GetLoserIDOfGame(int gameID)
            {
                int loserID = -1;

                using (SqliteConnection sqlConnection = new SqliteConnection($"Filename={_dbpath}"))
                {
                    sqlConnection.Open();
                    SqliteCommand selectWinnerIDs = new SqliteCommand(
                        $"Select PlayerID From Player Where GameID = {gameID} ORDER BY FinalTotalMoney ASC LIMIT 1");
                    selectWinnerIDs.Connection = sqlConnection;

                    try
                    {
                        using (SqliteDataReader reader = selectWinnerIDs.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                loserID = reader.GetInt32(0);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Inner Exception: " + e.Message);
                        Debug.WriteLine("");
                        Debug.WriteLine("Query Executed: " + selectWinnerIDs.CommandText);
                        Debug.WriteLine("");
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }
                return loserID;
            }

            private static List<int> GetAllPlayerIDsOfGame(int gameID)
            {
                List<int> playerIDs = new List<int>();
                using (SqliteConnection sqlConnection = new SqliteConnection($"Filename={_dbpath}"))
                {
                    sqlConnection.Open();
                    SqliteCommand selectWinnerIDs = new SqliteCommand(
                        $"Select PlayerID From Player WHERE GameID = {gameID}");
                    selectWinnerIDs.Connection = sqlConnection;

                    try
                    {
                        using (SqliteDataReader reader = selectWinnerIDs.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                playerIDs.Add(reader.GetInt32(0));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Inner Exception: " + e.Message);
                        Debug.WriteLine("");
                        Debug.WriteLine("Query Executed: " + selectWinnerIDs.CommandText);
                        Debug.WriteLine("");
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }

                return playerIDs;
            }

            private static String GetPropertyNameByID(int propertyID)
            {
                String propertyName = "";
                using (SqliteConnection sqlConnection = new SqliteConnection($"Filename={_dbpath}"))
                {
                    sqlConnection.Open();
                    SqliteCommand command = new SqliteCommand(
                                        $"Select PropertyName FROM Property Where PropertyID = {propertyID} ");
                    command.Connection = sqlConnection;

                    try
                    {
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                propertyName = reader.GetString(0);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Inner Exception: " + e.Message);
                        Debug.WriteLine("");
                        Debug.WriteLine("Query Executed: " + command.CommandText);
                        Debug.WriteLine("");
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }

                }
                return propertyName;
            }
        }
    }
