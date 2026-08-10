// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Human Centric Works, Hospet

using Microsoft.Data.Sqlite;

namespace AConsulting.App.Services;

/// <summary>Practice profile notes in firm.db (single-row settings).</summary>
public sealed class LocalPracticeStore : IDisposable
{
    readonly SqliteConnection _con;

    public LocalPracticeStore(string firmDbPath)
    {
        var dir = Path.GetDirectoryName(firmDbPath);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        SQLitePCL.Batteries_V2.Init();
        _con = new SqliteConnection(new SqliteConnectionStringBuilder
        {
            DataSource = firmDbPath,
            Mode = SqliteOpenMode.ReadWriteCreate,
        }.ToString());
        _con.Open();
        EnsureSchema();
    }

    public static string DefaultFirmDbPath() => LocalEngagementsStore.DefaultFirmDbPath();

    void EnsureSchema()
    {
        using var cmd = _con.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS local_practice(
              id INTEGER PRIMARY KEY CHECK (id = 1),
              firm_name TEXT NOT NULL DEFAULT '',
              notes TEXT NOT NULL DEFAULT '',
              updated_at TEXT NOT NULL
            );
            """;
        cmd.ExecuteNonQuery();
    }

    public PracticeProfile Get()
    {
        using var cmd = _con.CreateCommand();
        cmd.CommandText = "SELECT firm_name, notes, updated_at FROM local_practice WHERE id=1";
        using var r = cmd.ExecuteReader();
        if (!r.Read())
            return new PracticeProfile("", "", "");
        return new PracticeProfile(r.GetString(0), r.GetString(1), r.GetString(2));
    }

    public void Upsert(string firmName, string notes)
    {
        using var cmd = _con.CreateCommand();
        cmd.CommandText = """
            INSERT INTO local_practice(id, firm_name, notes, updated_at)
            VALUES(1,$n,$notes,$u)
            ON CONFLICT(id) DO UPDATE SET
              firm_name=excluded.firm_name,
              notes=excluded.notes,
              updated_at=excluded.updated_at
            """;
        cmd.Parameters.AddWithValue("$n", firmName);
        cmd.Parameters.AddWithValue("$notes", notes);
        cmd.Parameters.AddWithValue("$u", DateTime.UtcNow.ToString("O"));
        cmd.ExecuteNonQuery();
    }

    public void Dispose() => _con.Dispose();
}

public sealed record PracticeProfile(string FirmName, string Notes, string UpdatedAt);
