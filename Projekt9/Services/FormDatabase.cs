using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using Projekt9.Models;

namespace Projekt9.Services;

/// <summary>
/// Lekka warstwa dostępu do bazy SQLite (.db) — zgodnie z opisem z PDFa
/// (sekcja "Przykład .db"). Plik bazy ląduje obok pliku wykonywalnego pod
/// nazwą <c>forms.db</c>, więc projekt jest przenośny między systemami.
/// </summary>
public class FormDatabase
{
    private readonly string _connectionString;

    public FormDatabase()
    {
        var dbPath = Path.Combine(AppContext.BaseDirectory, "forms.db");
        _connectionString = $"Data Source={dbPath}";
        Initialize();
    }

    /// <summary>Tworzy tabelę przy pierwszym uruchomieniu, jeśli nie istnieje.</summary>
    private void Initialize()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        const string sql = @"
CREATE TABLE IF NOT EXISTS Forms (
    Id                 INTEGER PRIMARY KEY AUTOINCREMENT,
    Name               TEXT    NOT NULL,
    CreatedAt          TEXT    NOT NULL,

    RequestDate        TEXT,
    AlbumNumber        TEXT,
    FullName           TEXT,
    SemesterYear       TEXT,
    FieldAndDegree     TEXT,
    Subject            TEXT,
    Points             TEXT,
    Lecturer           TEXT,
    Justification      TEXT,
    StudentSignDate    TEXT,
    StudentSignature   TEXT,

    DecisionApproved   INTEGER NOT NULL DEFAULT 0,
    CommitteeMember1   TEXT,
    CommitteeMember2   TEXT,
    CommitteeMember3   TEXT,
    DecisionDate       TEXT
);";
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();

        // Migracja: jeśli baza istniała wcześniej, dodaj nowo-wprowadzoną kolumnę.
        EnsureColumn(conn, "Forms", "StudentSignature", "TEXT");
    }

    /// <summary>Dodaje kolumnę do tabeli, jeśli jeszcze nie istnieje.</summary>
    private static void EnsureColumn(SqliteConnection conn, string table, string column, string type)
    {
        using var check = conn.CreateCommand();
        check.CommandText = $"PRAGMA table_info({table});";
        using var r = check.ExecuteReader();
        while (r.Read())
        {
            if (string.Equals(r.GetString(1), column, StringComparison.OrdinalIgnoreCase))
                return;
        }
        r.Close();

        using var alter = conn.CreateCommand();
        alter.CommandText = $"ALTER TABLE {table} ADD COLUMN {column} {type};";
        alter.ExecuteNonQuery();
    }

    /// <summary>Zapisuje nowy wpis i zwraca jego Id.</summary>
    public int Save(FormEntry e)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
INSERT INTO Forms (
    Name, CreatedAt,
    RequestDate, AlbumNumber, FullName, SemesterYear, FieldAndDegree,
    Subject, Points, Lecturer, Justification, StudentSignDate, StudentSignature,
    DecisionApproved, CommitteeMember1, CommitteeMember2, CommitteeMember3, DecisionDate
) VALUES (
    $name, $createdAt,
    $requestDate, $albumNumber, $fullName, $semesterYear, $fieldAndDegree,
    $subject, $points, $lecturer, $justification, $studentSignDate, $studentSignature,
    $decisionApproved, $committee1, $committee2, $committee3, $decisionDate
);
SELECT last_insert_rowid();";

        cmd.Parameters.AddWithValue("$name", e.Name);
        cmd.Parameters.AddWithValue("$createdAt", e.CreatedAt.ToString("o"));
        cmd.Parameters.AddWithValue("$requestDate", (object?)e.RequestDate?.ToString("o") ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$albumNumber", e.AlbumNumber ?? string.Empty);
        cmd.Parameters.AddWithValue("$fullName", e.FullName ?? string.Empty);
        cmd.Parameters.AddWithValue("$semesterYear", e.SemesterYear ?? string.Empty);
        cmd.Parameters.AddWithValue("$fieldAndDegree", e.FieldAndDegree ?? string.Empty);
        cmd.Parameters.AddWithValue("$subject", e.Subject ?? string.Empty);
        cmd.Parameters.AddWithValue("$points", e.Points ?? string.Empty);
        cmd.Parameters.AddWithValue("$lecturer", e.Lecturer ?? string.Empty);
        cmd.Parameters.AddWithValue("$justification", e.Justification ?? string.Empty);
        cmd.Parameters.AddWithValue("$studentSignDate", (object?)e.StudentSignDate?.ToString("o") ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$studentSignature", e.StudentSignature ?? string.Empty);
        cmd.Parameters.AddWithValue("$decisionApproved", e.DecisionApproved ? 1 : 0);
        cmd.Parameters.AddWithValue("$committee1", e.CommitteeMember1 ?? string.Empty);
        cmd.Parameters.AddWithValue("$committee2", e.CommitteeMember2 ?? string.Empty);
        cmd.Parameters.AddWithValue("$committee3", e.CommitteeMember3 ?? string.Empty);
        cmd.Parameters.AddWithValue("$decisionDate", (object?)e.DecisionDate?.ToString("o") ?? DBNull.Value);

        var id = (long)(cmd.ExecuteScalar() ?? 0L);
        e.Id = (int)id;
        return e.Id;
    }

    /// <summary>Zwraca wszystkie zapisane formularze posortowane od najnowszego.</summary>
    public List<FormEntry> GetAll()
    {
        var result = new List<FormEntry>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM Forms ORDER BY datetime(CreatedAt) DESC, Id DESC;";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(Read(reader));
        }
        return result;
    }

    private static FormEntry Read(SqliteDataReader r)
    {
        DateTime? ParseDate(string col)
        {
            int i = r.GetOrdinal(col);
            if (r.IsDBNull(i)) return null;
            var s = r.GetString(i);
            return DateTime.TryParse(s, out var d) ? d : null;
        }

        string Str(string col)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? string.Empty : r.GetString(i);
        }

        return new FormEntry
        {
            Id = r.GetInt32(r.GetOrdinal("Id")),
            Name = Str("Name"),
            CreatedAt = ParseDate("CreatedAt") ?? DateTime.Now,

            RequestDate = ParseDate("RequestDate"),
            AlbumNumber = Str("AlbumNumber"),
            FullName = Str("FullName"),
            SemesterYear = Str("SemesterYear"),
            FieldAndDegree = Str("FieldAndDegree"),
            Subject = Str("Subject"),
            Points = Str("Points"),
            Lecturer = Str("Lecturer"),
            Justification = Str("Justification"),
            StudentSignDate = ParseDate("StudentSignDate"),
            StudentSignature = Str("StudentSignature"),

            DecisionApproved = r.GetInt32(r.GetOrdinal("DecisionApproved")) == 1,
            CommitteeMember1 = Str("CommitteeMember1"),
            CommitteeMember2 = Str("CommitteeMember2"),
            CommitteeMember3 = Str("CommitteeMember3"),
            DecisionDate = ParseDate("DecisionDate"),
        };
    }
}
