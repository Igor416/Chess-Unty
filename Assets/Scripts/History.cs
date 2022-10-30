using System;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class History
{
    private class Stats
    {
        public string login = "";
        public int gamesPlayed;
    }
    private const string path = "Assets/PlayerProfile.json";
    private const string dbName = "URI=file:database.db";
    private static bool isSaved = false;
    private static int gamesPlayed;
    private static readonly Stack<string> allMoves = new Stack<string>();
    private static string[] lastMove;
    public static string[] LastMove
    {
        get
        {
            if (lastMove is null)
            {
                return new string[4];
            }
            return lastMove;
        }
        private set
        {
            lastMove = value;
        }
    }

    public static void SetLastMove(params string[] values)
    {
        if (values.Length == 4)
        {
            LastMove = values;
            allMoves.Push(ToString(LastMove));
        }
    }

    public static void MoveBack()
    {
        if (allMoves.Count >= 1)
        {
            allMoves.Pop();
            if (allMoves.Count == 0)
            {
                LastMove = new string[4];
            }
            else
            {
                LastMove = ToArray(allMoves.Peek());
            }
        }
    }

    public static void DeleteLastMoves()
    {
        allMoves.Clear();
    }

    public static void DeleteSavedGames()
    {
        Stats stats = JsonUtility.FromJson<Stats>(File.ReadAllText(path));
        stats.gamesPlayed = 0;
        using (var conn = new SqliteConnection(dbName))
        {
            conn.Open();

            using (var command = conn.CreateCommand())
            {
                command.CommandText = "DELETE FROM SavedGames;";

                command.ExecuteNonQuery();
            }
            conn.Close();
        }
        File.WriteAllText(path, JsonUtility.ToJson(stats, true));
    }

    public static void ReadSavedGames()
    {
        using (var conn = new SqliteConnection(dbName))
        {
            conn.Open();

            using (var command = conn.CreateCommand())
            {
                command.CommandText = "SELECT * FROM SavedGames;";

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Debug.Log($"{reader["id"]}-{reader["mode"]}-{reader["round"]}");
                        Debug.Log(reader["figures"]);
                    }
                    reader.Close();
                }
            }
            conn.Close();
        }
    }

    public static (string figures, string mode, int round) Load(int id)
    {
        using (var conn = new SqliteConnection(dbName))
        {
            conn.Open();

            using (var command = conn.CreateCommand())
            {
                command.CommandText = "SELECT * FROM SavedGames WHERE id=" + id;

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        gamesPlayed = id;
                        isSaved = true;
                        return (reader["figures"].ToString(), reader["mode"].ToString(), Convert.ToInt32(reader["round"]));
                    }
                    reader.Close();
                }
            }
            conn.Close();
        }
        return (null, null, -1);
    }

    public static void Save(string mode, int round, string[] arr)
    {
        if (isSaved)
        {
            using (var conn = new SqliteConnection(dbName))
            {
                conn.Open();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "UPDATE SavedGames " +
                    $"SET mode='{mode}', round='{round}', figures='{ToString(arr, "")}' " +
                    "WHERE id=" + gamesPlayed;

                    command.ExecuteNonQuery();
                }
                conn.Close();
            }
        }
        else
        {
            Stats stats = JsonUtility.FromJson<Stats>(File.ReadAllText(path));
            gamesPlayed = stats.gamesPlayed++;
            using (var conn = new SqliteConnection(dbName))
            {
                conn.Open();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "INSERT INTO SavedGames (mode, round, figures)" +
                    $"VALUES('{mode}', '{round}', '{ToString(arr, "")}'); ";

                    command.ExecuteNonQuery();
                }
                conn.Close();
            }
            File.WriteAllText(path, JsonUtility.ToJson(stats, true));
            isSaved = true;
        }
    }

    private static string[] ToArray(string str)
    {
        string[] arr = new string[4];
        string value = "";
        int index = 0;
        foreach (char el in str.ToCharArray())
        {
            if (el != ';')
            {
                value += el;
            }
            else
            {
                arr[index] = value;
                value = "";
                index++;
            }
        }
        return arr;
    }

    private static string ToString(string[] arr, string separator = ";")
    {
        string str = "";
        foreach (string value in arr)
        {
            str += value;
            str += separator;
        }
        return str;
    }
}
