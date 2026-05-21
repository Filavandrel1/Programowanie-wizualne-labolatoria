using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Projekt11.Models;

namespace Projekt11.Services;

/// <summary>
/// Warstwa dostępu do bazy SQLite przechowującej próbki biologiczne.
/// Plik <c>samples.db</c> zapisywany jest obok pliku wykonywalnego.
/// </summary>
public class SampleDatabase
{
    private readonly string _connectionString;

    public SampleDatabase()
    {
        var dbPath = System.IO.Path.Combine(AppContext.BaseDirectory, "samples.db");
        _connectionString = $"Data Source={dbPath}";
        Initialize();
    }

    private void Initialize()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Samples (
                RowId           INTEGER PRIMARY KEY AUTOINCREMENT,
                SampleId        TEXT NOT NULL UNIQUE,
                Name            TEXT NOT NULL,
                Type            TEXT NOT NULL,
                CollectionDate  TEXT NOT NULL,
                Description     TEXT NOT NULL
            );";
        cmd.ExecuteNonQuery();
    }

    /// <summary>Zwraca true, jeśli SampleId jest już zajęte (z opcjonalnym pominięciem RowId).</summary>
    public bool SampleIdExists(string sampleId, int ignoreRowId = 0)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM Samples WHERE SampleId = $sid AND RowId <> $rid;";
        cmd.Parameters.AddWithValue("$sid", sampleId);
        cmd.Parameters.AddWithValue("$rid", ignoreRowId);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public int Insert(Sample s)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Samples (SampleId, Name, Type, CollectionDate, Description)
            VALUES ($sid, $name, $type, $date, $desc);
            SELECT last_insert_rowid();";
        Bind(cmd, s);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public int Update(Sample s)
    {
        if (s.RowId <= 0) throw new InvalidOperationException("Update requires RowId > 0.");
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE Samples SET
                SampleId       = $sid,
                Name           = $name,
                Type           = $type,
                CollectionDate = $date,
                Description    = $desc
            WHERE RowId = $rowid;";
        Bind(cmd, s);
        cmd.Parameters.AddWithValue("$rowid", s.RowId);
        return cmd.ExecuteNonQuery();
    }

    public int Delete(int rowId)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM Samples WHERE RowId = $rid;";
        cmd.Parameters.AddWithValue("$rid", rowId);
        return cmd.ExecuteNonQuery();
    }

    public List<Sample> GetAll()
    {
        var list = new List<Sample>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT RowId, SampleId, Name, Type, CollectionDate, Description FROM Samples ORDER BY datetime(CollectionDate) DESC, RowId DESC;";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(Read(reader));
        return list;
    }

    private static void Bind(SqliteCommand cmd, Sample s)
    {
        cmd.Parameters.AddWithValue("$sid",  s.SampleId);
        cmd.Parameters.AddWithValue("$name", s.Name);
        cmd.Parameters.AddWithValue("$type", s.Type.ToString());
        cmd.Parameters.AddWithValue("$date", s.CollectionDate.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("$desc", s.Description ?? string.Empty);
    }

    private static Sample Read(SqliteDataReader r) => new()
    {
        RowId          = r.GetInt32(0),
        SampleId       = r.GetString(1),
        Name           = r.GetString(2),
        Type           = Enum.TryParse<SampleType>(r.GetString(3), out var t) ? t : SampleType.Inny,
        CollectionDate = DateTime.TryParse(r.GetString(4), out var d) ? d : DateTime.Today,
        Description    = r.GetString(5),
    };
}
